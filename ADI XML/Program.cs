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
                    new Uri("http://10.20.66.3:8085"));
            request.Method = "POST";
            request.ContentType = "application/xml";
            request.Accept = "application/xml";

            // Configurando variables de entorno
            // XNamespace jugadorID = "2057809";

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
                                    new XElement("SSN", "70398427")
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

            // Ejecutar el request y obetener un response "reader"
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                var streamData = reader.ReadToEnd();
                Console.WriteLine(streamData);
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