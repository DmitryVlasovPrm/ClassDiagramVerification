using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using ClassDiagrammVerification.Entities;
using System.Windows.Forms;

namespace ClassDiagrammVerification
{
    class Program
    {
        public static List<Class> Classes;
        public static List<Connection> Connections;
        public static List<Entities.Type> Types;

        [STAThread]
        static void Main(string[] args)
        {
            Classes = new List<Class>();
            Connections = new List<Connection>();
            Types = new List<Entities.Type>();
            
            try
            {
                Stopwatch sw;
                XmlElement root = null;
                var filePath = string.Empty;
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
				{
                    openFileDialog.Title = "Выбери файл с метаданными";
                    openFileDialog.InitialDirectory = "C://";
                    openFileDialog.Filter = "xml files|*.xml|xmi files|*.xmi";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
					{
                        filePath = openFileDialog.FileName;
					}
                    else
					{
                        return;
					}

                    sw = new Stopwatch();
                    sw.Start();

                    var doc = new XmlDocument();
                    doc.Load(filePath);
                    root = doc.DocumentElement;
                }
                
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
                Console.WriteLine("Синтаксический анализ завершен\n--------------------");

                sw.Stop();
                Console.WriteLine($"Время работы программы составило: {sw.ElapsedMilliseconds} мс");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Program.cs: " + ex.Message);
            }
        }
    }
}