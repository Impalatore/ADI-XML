using System.Xml.Linq;

namespace ConsoleApp1;

public class PruebaXML
{
    static void Main(string[] args)
    {
        var streamData = @"C:\Users\Naim\Desktop\XMLADI.xml";
        
        XElement root = XElement.Load(streamData);
        IEnumerable<XElement> search =
            from el in root.Descendants("PlayerFound")
            select el;
        Console.WriteLine(( "Resultados encontrados: " + root.Descendants("PlayerFound").Count()));
        foreach (XElement el in search)
        {
            Console.WriteLine("##### JUGADOR ENCONTRADO #####");
            //Console.Write(el);
            
            //Falta parsear
            Console.WriteLine(el.Elements("PlayerID"));
            Console.WriteLine(el.Elements("FirstName"));
            Console.WriteLine(el.Elements("LastName"));
            Console.WriteLine(el.Elements("AddressLine1"));
        }
    }
}