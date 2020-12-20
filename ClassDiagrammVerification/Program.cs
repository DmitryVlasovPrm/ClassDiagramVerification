using System;
using System.Xml;
using System.Collections.Generic;
using ClassDiagrammVerification.Entities;

namespace ClassDiagrammVerification
{
    class Program
    {
        public static List<Class> Classes;
        public static List<Connection> Connections;
        public static List<Entities.Type> Types;
        
        static void Main(string[] args)
        {
            Classes = new List<Class>();
            Connections = new List<Connection>();
            Types = new List<Entities.Type>();
            
            try
            {
                var doc = new XmlDocument();
                doc.Load("/Users/dmitry/Desktop/example.xml");
                var root = doc.DocumentElement;
                
                // Для отрисовки на png
                var graphics = root.GetElementsByTagName("contents");
                // Сами элементы диаграммы
                var elements = root.GetElementsByTagName("packagedElement");
                ExtractElements.Extract(elements, ref Classes, ref Connections, ref Types);
                Console.WriteLine("Все элементы считаны\n--------------------");
                
                // Лексический анализ
                Analysis.LexicalAnalysis(ref Classes, ref Connections, ref Types);
                Console.WriteLine("Лексический анализ завершен\n--------------------");
                
                // Семантический анализ
                Analysis.SemanticAnalysis(ref Classes, ref Connections, ref Types);
                Console.WriteLine("Семантический анализ завершен\n--------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Program.cs: " + ex.Message);
            }
        }
    }
}