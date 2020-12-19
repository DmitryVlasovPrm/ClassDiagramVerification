using System;
using System.Xml;

namespace ClassDiagrammVerification
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load("/Users/dmitry/Desktop/Example1.xml");
                var root = doc.DocumentElement;
                
                // Для отрисовки на png
                var graphics = root.GetElementsByTagName("contents")[0];
                // Сами элементы диаграммы
                var elements = root.GetElementsByTagName("packagedElement");
                Console.WriteLine(elements.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}