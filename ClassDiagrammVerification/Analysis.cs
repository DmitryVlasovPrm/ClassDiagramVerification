using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClassDiagrammVerification.Entities;
using Type = System.Type;

namespace ClassDiagrammVerification
{
    public static class Analysis
    {
        private static string[] AllTypes =
        {
            "int", "string", "float", "double", "bool", "List<T>", "List<string>",
            "List<Variable>", "List<Domain>"
        };

        public static void LexicalAnalysis(ref List<Class> classes, ref List<Connection> connections,  ref List<Entities.Type> types)
        {
            try
            {
                var classesCount = classes.Count;
                for (var i = 0; i < classesCount; i++)
                {
                    var curClass = classes[i];
                    var curClassName = curClass.Name;
                    if (!char.IsUpper(curClassName[0]))
                        Console.WriteLine($"Ошибка. Имя класса начинается с маленькой буквы: \"{curClassName}\"");
                    if (curClassName.Contains(" "))
                        Console.WriteLine($"Ошибка. Имя класса содержит пробелы: \"{curClassName}\"");

                    var attributes = curClass.Attributes;
                    var attributesCount = attributes.Count;
                    for (var j = 0; j < attributesCount; j++)
                    {
                        var curAttribute = attributes[j];
                        var curAttributeName = curAttribute.Name;
                        if (char.IsUpper(curAttributeName[0]))
                            Console.WriteLine($"Ошибка. Имя атрибута начинается с большой буквы (класс \"{curClassName}\"): \"{curAttributeName}\"");
                        if (curAttributeName.Contains(" "))
                            Console.WriteLine($"Ошибка. Имя атрибута содержит пробелы (класс \"{curClassName}\"): \"{curAttributeName}\"");
                    
                        var curType = types.Find(a => a.Id == curAttribute.TypeId);
                        if (curType != null)
                            if (!AllTypes.Contains(curType.TypeValue) && classes.FindIndex(a => a.Id == curAttribute.TypeId) == -1)
                                Console.WriteLine($"Ошибка. Неверное имя типа (класс \"{curClassName}\"): \"{curType.TypeValue}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in LexicalAnalysis: " + ex.Message);
            }
        }

        public static void SemanticAnalysis(ref List<Class> classes, ref List<Connection> connections, ref List<Entities.Type> types)
        {
            try
            {
                var connectionsCount = connections.Count;
                for (var i = 0; i < connectionsCount; i++)
                {
                    var curConnection = connections[i];

                    // Проверка кратности у композиции (не должна быть больше 1)
                    if (curConnection.ConnectionType1 == ConnectionType.compositeAggregation)
                    {
                        var maxValue =
                            curConnection.Multiplicity2.Split(new string[] {".."}, StringSplitOptions.None)[1];
                        if (maxValue == "*" || int.Parse(maxValue) > 1)
                        {
                            var className = classes.Find(a => a.Id == curConnection.OwnedElementId2);
                            Console.WriteLine(
                                $"Ошибка. Кратность указана неверно (класс \"{className.Name}\"): \"{curConnection.Multiplicity2}\"");
                        }
                    }

                    if (curConnection.ConnectionType2 == ConnectionType.compositeAggregation)
                    {
                        var maxValue =
                            curConnection.Multiplicity1.Split(new string[] {".."}, StringSplitOptions.None)[1];
                        if (maxValue == "*" || int.Parse(maxValue) > 1)
                        {
                            var className = classes.Find(a => a.Id == curConnection.OwnedElementId1);
                            Console.WriteLine(
                                $"Ошибка. Кратность указана неверно (класс \"{className.Name}\"): \"{curConnection.Multiplicity1}\"");
                        }
                    }

                    // Проверка композиции или агрегации в главном элементе
                    if (curConnection.ConnectionType1 == ConnectionType.compositeAggregation ||
                        curConnection.ConnectionType1 == ConnectionType.sharedAggregation)
                    {
                        var mainClass = classes.Find(a => a.Id == curConnection.OwnedElementId2);
                        var mainAttributes = mainClass.Attributes;
                        var mainAttributesCount = mainAttributes.Count;
                        var subordinateClass = classes.Find(a => a.Id == curConnection.OwnedElementId1);

                        var isExist = false;
                        for (var j = 0; j < mainAttributesCount; j++)
                        {
                            var curAttribute = mainAttributes[j];
                            if (curAttribute.TypeId == subordinateClass.Id)
                            {
                                isExist = true;
                                break;
                            }
                        }

                        if (!isExist)
                            Console.WriteLine(
                                $"Ошибка. Класс \"{subordinateClass.Name}\" не является частью класса \"{mainClass.Name}\"");
                    }

                    if (curConnection.ConnectionType2 == ConnectionType.compositeAggregation ||
                        curConnection.ConnectionType2 == ConnectionType.sharedAggregation)
                    {
                        var mainClass = classes.Find(a => a.Id == curConnection.OwnedElementId1);
                        var mainAttributes = mainClass.Attributes;
                        var mainAttributesCount = mainAttributes.Count;
                        var subordinateClass = classes.Find(a => a.Id == curConnection.OwnedElementId2);

                        var isExist = false;
                        for (var j = 0; j < mainAttributesCount; j++)
                        {
                            var curAttribute = mainAttributes[j];
                            if (curAttribute.TypeId == subordinateClass.Id)
                            {
                                isExist = true;
                                break;
                            }
                        }

                        if (!isExist)
                            Console.WriteLine(
                                $"Ошибка. Класс \"{subordinateClass.Name}\" не является частью класса \"{mainClass.Name}\"");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in SemanticAnalysis: " + ex.Message);
            }
        }
    }
}