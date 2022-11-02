using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApp1;

public class Program

{
    /// Función para llamar a un servicio web SOAP
    public static void Main()
    {
        try
        {
            // Http POST request, se puede trabajar con HttpWebRequest ó WebRequest
            // Falta declarar conexión al ADI
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create(
                    new Uri("http://10.27.66.3:8085"));
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.Accept = "application/xml";

            //Configurando variables de entorno
            Console.WriteLine("Ingrese el PlayerID:");
            string jugadorID = Console.ReadLine();
            
            Console.WriteLine("Ingrese el tipo de saldo a modificar:");
            Console.WriteLine("XtraCredit | Points");
            string tipoSaldo = Console.ReadLine();  
            
            Console.WriteLine("Ingrese la operación a realizar");
            Console.WriteLine("C - Acreditar");
            Console.WriteLine("D - Debitar");
            string tipoOperacion = Console.ReadLine();
            
            Console.WriteLine("Ingrese el monto:");
            string monto = Console.ReadLine();
            
            Console.WriteLine("Ingrese el origen:");
            string origen = Console.ReadLine();

            var siteID = "1";

            //Constructor de XML usando la librería LinQ
            XElement requestXML =
                new XElement("CRMAcresMessage",
                    new XElement("Header",
                        //new XElement("TimeStamp", "${dformat}"), //falta declarar el dformat
                        new XElement("Operation", new XAttribute("Data", "PlayerBalanceAdjustment"),
                            new XAttribute("Operand", "Request"))
                    ),
                    new XElement("PlayerID", jugadorID),
                    new XElement("SiteID", siteID),
                    new XElement("Body",
                        new XElement("PlayerBalanceAdjustment",
                            new XElement("BalanceType", tipoSaldo),
                            new XElement("LocalOrGlobal", "L"),
                            new XElement("CreditOrDebit", tipoOperacion),
                            new XElement("Amount", monto),
                            new XElement("UserLogin", origen)
                        )
                    )
                );
            // Convertir el xml a un stream con el que se manejará el requestXML
            byte[] bytes = Encoding.UTF8.GetBytes(requestXML.ToString());
            request.ContentLength = bytes.Length;
            
            using (Stream putStream = request.GetRequestStream())
            {
                putStream.Write(bytes, 0, bytes.Length);
            }

            // Ejecutar el request y obtener un response "reader"
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var streamData = reader;
                XElement root = XElement.Load(streamData);
                IEnumerable<XElement> reporteBalance =
                    from el in root.Descendants("PlayerBalanceAdjustment")
                    select el;
                foreach (XElement el in reporteBalance)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("##### REPORTE BALANCES #####");
                    Console.WriteLine("ORIGEN: " + (el.Element("UserLogin").Value));
                    Console.WriteLine("TIPO: " + el.Element("BalanceType").Value);
                    Console.WriteLine("CANTIDAD: " + el.Element("Amount").Value);
                    Console.WriteLine("SALDO ANTERIOR: " + el.Element("OldXtraCreditBalance").Value);
                    Console.WriteLine("SALDO ACTUAL: " + el.Element("NewXtraCreditBalance").Value);
                }
            }
        }
        catch (Exception ex)
        {
            // Escribir excepción en la Consola
            Console.WriteLine(ex.Message + ex.StackTrace);
            Console.ReadKey();
        }
    }
}