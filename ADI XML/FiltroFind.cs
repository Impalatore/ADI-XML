using System.Net;
using System.Text;
using System.Xml.Linq;

namespace ConsoleApp1;

public class FiltroFind

{
    /// Función para llamar a un servicio web SOAP
    public static void Main3()
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

            //Configurando variables de entorno
            var jugadorDNI = "09958319-0000000";
            var jugadorID = "2033970";

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
                                new XElement("SearchPlayerID",
                                    new XElement("PlayerID", jugadorID)  //{ Value = jugadorDNI}
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
                    from el in root.Descendants("PlayersFound")
                    select el;
                foreach (XElement el in search)
                {
                    Console.WriteLine(( "Resultados encontrados: " + el.Elements("PlayerFound").Count()));
                    Console.WriteLine("##############################");
                    Console.Write(el);
                    // Console.WriteLine(el.Element("PlayerFound").Elements("PlayerID"));
                    // Console.WriteLine(el.Element("PlayerFound").Elements("FirstName"));
                    // Console.WriteLine(el.Element("PlayerFound").Elements("LastName"));
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