using System.Xml.Linq;

namespace ConsoleApp1;

public class PruebaXML
{
    static void Test(string[] args)
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
            
            Console.WriteLine(el.Element("PlayerID"));
            Console.WriteLine(el.Element("FirstName"));
            Console.WriteLine(el.Element("LastName"));
            Console.WriteLine(el.Element("AddressLine1"));
        }
    }
}