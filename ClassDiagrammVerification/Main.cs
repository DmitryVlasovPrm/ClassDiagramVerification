using System;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ClassDiagrammVerification
{
    class Program
    {
        public static Elements AllElements;
        public static List<Entities.Error> Errors;
        public static XmlElement Root;

        [STAThread]
        static void Main(string[] args)
        {
            AllElements = new Elements();
            Errors = new List<Entities.Error>();

            Stopwatch sw;
            Root = null;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Выбери файл с метаданными";
                openFileDialog.InitialDirectory = "C://";
                openFileDialog.Filter = "xmi files|*.xmi";
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
                Root = doc.DocumentElement;
            }

            ExtractElements.Extract(Root, ref AllElements, ref Errors);
            Console.WriteLine("Все элементы считаны\n--------------------");

            // Лексический анализ
            Analysis.LexicalAnalysis(AllElements);
            Console.WriteLine("Лексический анализ завершен\n--------------------");

            // Синтаксический анализ
            Analysis.SemanticAnalysis(AllElements);
            Console.WriteLine("Синтаксический анализ завершен\n--------------------");

            sw.Stop();
            Console.WriteLine($"Время работы программы составило: {sw.ElapsedMilliseconds} мс");
        }
    }
}