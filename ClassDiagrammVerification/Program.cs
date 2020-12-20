using System;
using System.Xml;
using System.Collections.Generic;
using ClassDiagrammVerification.Entities;

namespace ClassDiagrammVerification
{
    class Program
    {
        public static List<Class> Classes;
        public static List<Association> Associations;
        public static List<Entities.Type> Types;
        
        static void Main(string[] args)
        {
            Classes = new List<Class>();
            Associations = new List<Association>();
            Types = new List<Entities.Type>();
            
            try
            {
                var doc = new XmlDocument();
                doc.Load("/Users/dmitry/Desktop/Example2.xml");
                var root = doc.DocumentElement;
                
                // Для отрисовки на png
                var graphics = root.GetElementsByTagName("contents");
                // Сами элементы диаграммы
                var elements = root.GetElementsByTagName("packagedElement");
                ExtractElements.Extract(elements, ref Classes, ref Associations, ref Types);
                Console.WriteLine("Все элементы считаны");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}