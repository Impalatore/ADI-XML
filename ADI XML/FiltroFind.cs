using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApp1;

public class FiltroFind

{
    /// Función para llamar a un servicio web SOAP
    public static void Main2()
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
            Console.WriteLine("Por favor, ingrese el DNI para realizar la búsqueda:");
            string jugadorDNI = Console.ReadLine();

            //Constructor de XML usando la librería LinQ
            XElement requestXML =
                new XElement("CRMAcresMessage",
                    new XElement("Header",
                        //new XElement("TimeStamp", "${dformat}"), //falta declarar el dformat
                        new XElement("Operation", new XAttribute("Data", "PlayerFind"),
                            new XAttribute("Operand", "Request"))
                    ),
                    new XElement("Body",
                        new XElement("PlayerFind",
                            new XElement("Filter",
                                new XElement("SearchSSN",
                                    new XElement("SSN", jugadorDNI)  //{ Value = jugadorDNI}
                                )
                            )
                        )
                    )
                );

            // Convert the xml into a stream that we write to our request
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
                IEnumerable<XElement> search =
                    from el in root.Descendants("PlayerFound")
                    select el;
                Console.WriteLine(( "Resultados encontrados: " + root.Descendants("PlayerFound").Count()));
                foreach (XElement el in search)
                {
                    Console.WriteLine(" ");
                    Console.WriteLine("##### JUGADOR ENCONTRADO #####");
                    Console.WriteLine(el.Element("PlayerID").Value);
                    Console.WriteLine(el.Element("FirstName").Value);
                    Console.WriteLine(el.Element("LastName").Value);
                    Console.WriteLine(el.Element("SSN").Value);
                    Console.WriteLine(el.Element("AddressLine1").Value);
                }
            }
        }
        catch (Exception ex)
        {
            // Write exception to console & wait for key press
            Console.WriteLine(ex.Message + ex.StackTrace);
            Console.ReadKey();
        }
    }
}