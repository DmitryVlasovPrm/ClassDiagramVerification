using System;
using System.Collections.Generic;
using System.Linq;
using ClassDiagrammVerification.Entities;
using System.Xml;
using Attribute = ClassDiagrammVerification.Entities.Attribute;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace ClassDiagrammVerification
{
    public static class ExtractElements
    {
        private static readonly string[] connectionTypes = { "association", "composite", "shared" };

        public static void Extract(XmlElement root, ref Elements allElements, ref List<Error> errors)
        {
            var image = new Image<Bgra, byte>("C://Users//Dmitry//Downloads//Test1.png");
            var (realMinX, realMinY) = MinCoordinates.Compute(image);
            var curMinX = 10000;
            var curMinY = 10000;

            // Для отрисовки на png
            var graphics = root.GetElementsByTagName("plane");
            var graphicInfo = new List<Tuple<string, BoundingBox>>();

            var graphicsCount = graphics.Count;
            for (var i = 0; i < graphicsCount; i++)
            {
                try
                {
                    var curContent = graphics[i];
                    var graphicElements = curContent.SelectNodes("ownedDiagramElements");
                    var graphicElementsCount = graphicElements.Count;
                    for (var j = 0; j < graphicElementsCount; j++)
                    {
                        var curElement = graphicElements[j];
                        var elementId = curElement.Attributes["modelElement"].Value;
                        var x = int.Parse(curElement.Attributes["x"].Value);
                        var y = int.Parse(curElement.Attributes["y"].Value);

                        // Для нормировки
                        if (x < curMinX)
                            curMinX = x;
                        if (y < curMinY)
                            curMinY = y;

                        var w = 0;
                        if (curElement.Attributes["width"] != null)
                            w = int.Parse(curElement.Attributes["width"].Value);
                        var h = 0;
                        if (curElement.Attributes["height"] != null)
                            h = int.Parse(curElement.Attributes["height"].Value);
                        
                        var bbox = new BoundingBox(x, y, w, h);
                        var item = new Tuple<string, BoundingBox>(elementId, bbox);
                        graphicInfo.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in extracting \"contents\": {ex.Message}");
                }
            }

            // Нормировка
            for (var i = 0; i < graphicInfo.Count; i++)
            {
                var offsetX = realMinX - curMinX;
                var offsetY = realMinY - curMinY;
                graphicInfo[i].Item2.X += offsetX;
                graphicInfo[i].Item2.Y += offsetY;

                //var curBb = graphicInfo[i].Item2;
                //if (curBb.W != 0 && curBb.H != 0)
                    //CvInvoke.Rectangle(image, new Rectangle(curBb.X + offsetX, curBb.Y + offsetY, curBb.W, curBb.H),
                        //new MCvScalar(0, 0, 255, 255), 2);
            }
            //image.Save("C://Users//Dmitry//Учеба//1.png");

            // Сами элементы диаграммы
            var xmlElements = root.GetElementsByTagName("packagedElement");

            var elementsCount = xmlElements.Count;
            for (var i = 0; i < elementsCount; i++)
            {
                try
                {
                    var curElement = xmlElements[i];
                    var elementType = curElement.Attributes["xsi:type"].Value;
                    var elementId = curElement.Attributes["xmi:id"].Value;

                    var nameAttribute = curElement.Attributes["name"];
                    var elementName = nameAttribute != null ? nameAttribute.Value : "";

                    switch (elementType)
                    {
                        case "uml:Package":
                            var box = graphicInfo.Find(a => a.Item1 == elementId).Item2;
                            allElements.Packages.Add(new Package(elementId, elementName, box));
                            break;

                        case "uml:Class":
                        case "uml:Interface":
                            var attributes = new List<Attribute>();
                            var operations = new List<Operation>();

                            // Считываем атрибуты
                            var attributeElements = curElement.SelectNodes("ownedAttribute");
                            var attribElementsCount = attributeElements.Count;
                            for (var k = 0; k < attribElementsCount; k++)
                            {
                                var curAttrib = attributeElements[k];
                                var attribId = curAttrib.Attributes["xmi:id"].Value;
                                var attribName = curAttrib.Attributes["name"].Value;
                                var attribVisibility = (Visibility)Enum.Parse(typeof(Visibility), curAttrib.Attributes["visibility"].Value, true);
                                var attribDataTypeId = GetDataType(curAttrib);

                                attributes.Add(new Attribute(attribId, attribName, attribVisibility, attribDataTypeId));
                            }

                            // Считываем операции
                            var operationElements = curElement.SelectNodes("ownedOperation");
                            var operationElementsCount = operationElements.Count;
                            for (var k = 0; k < operationElementsCount; k++)
                            {
                                var curOperation = operationElements[k];
                                var operationId = curOperation.Attributes["xmi:id"].Value;
                                var operationName = curOperation.Attributes["name"].Value;

                                var returnDataTypeId = "";
                                var parameters = new List<Parameter>();
                                var parametersElements = curOperation.SelectNodes("ownedParameter");
                                var parametersCount = parametersElements.Count;
                                for (var t = 0; t < parametersCount; t++)
                                {
                                    var curParameter = parametersElements[t];
                                    var paramId = curParameter.Attributes["xmi:id"].Value;
                                    var paramName = curParameter.Attributes["name"].Value;

                                    // Для возвращаемого значения операции
                                    if (curParameter.Attributes["direction"] != null &&
                                        curParameter.Attributes["direction"].Value == "return")
                                    {
                                        returnDataTypeId = GetDataType(curParameter);
                                    }
                                    else
                                    {
                                        // Обычный параметр
                                        var paramDataType = GetDataType(curParameter);
                                        parameters.Add(new Parameter(paramId, paramName, paramDataType));
                                    }
                                }

                                var operationVisibility = Visibility.@public;
                                if (curOperation.Attributes["visibility"] != null)
                                    operationVisibility = (Visibility)Enum.Parse(typeof(Visibility),
                                        curOperation.Attributes["visibility"].Value, true);

                                operations.Add(new Operation(operationId, operationName, parameters,
                                    operationVisibility, returnDataTypeId));
                            }

                            // Смотрим обобщение (наследование)
                            var generalClassesIdxs = new List<string>();
                            var generalizationElements = curElement.SelectNodes("generalization");
                            var genElementsCount = generalizationElements.Count;
                            for (var k = 0; k < genElementsCount; k++)
                            {
                                var curGeneralization = generalizationElements[k];
                                generalClassesIdxs.Add(curGeneralization.Attributes["general"].Value);
                            }

                            // Смотрим интерфейсы
                            var interfaceSuppliersIdxs = new List<string>();
                            var interfaceElements = curElement.SelectNodes("interfaceRealization");
                            var interfaceElementsCount = interfaceElements.Count;
                            for (var k = 0; k < interfaceElementsCount; k++)
                            {
                                var curRealization = interfaceElements[k];
                                interfaceSuppliersIdxs.Add(curRealization.Attributes["supplier"].Value);
                            }

                            box = graphicInfo.Find(a => a.Item1 == elementId).Item2;
                            var newClass = new Class(elementId, elementName, box, attributes, operations,
                                generalClassesIdxs, interfaceSuppliersIdxs);
                            if (elementType == "uml:Class") allElements.Classes.Add(newClass);
                            else allElements.Interfaces.Add(newClass);
                            break;

                        case "uml:Association":
                            string[] navigableEndIdxs = null;
                            if (curElement.Attributes["navigableOwnedEnd"] != null)
                                navigableEndIdxs = curElement.Attributes["navigableOwnedEnd"].Value.Split(' ');

                            var ownedEnds = curElement.SelectNodes("ownedEnd");
                            string ownedElementId1 = "",
                                role1 = "",
                                mult1 = "",
                                ownedElementId2 = "",
                                role2 = "",
                                mult2 = "";
                            bool navigalable1 = false, navigalable2 = false;
                            ConnectionType connType1 = ConnectionType.Association,
                                connType2 = ConnectionType.Association;

                            // Смотрим каждый из концов связи
                            for (var j = 0; j < 2; j++)
                            {
                                var curOwnedEnd = ownedEnds[j];
                                var curOwnedEndId = curOwnedEnd.Attributes["xmi:id"].Value;
                                string curRole = "";
                                if (curOwnedEnd.Attributes["name"] != null)
                                    curRole = curOwnedEnd.Attributes["name"].Value;
                                // id элемента, к которому идет данная связь
                                var curOwnedElementId = curOwnedEnd.Attributes["type"].Value;

                                var curConnectionType = ConnectionType.Association;
                                if (curOwnedEnd.Attributes["aggregation"] != null)
                                    curConnectionType = (ConnectionType)Array.IndexOf(connectionTypes,
                                        curOwnedEnd.Attributes["aggregation"].Value);

                                var curNavigation = false;
                                if (navigableEndIdxs != null)
                                    curNavigation = navigableEndIdxs.Contains(curOwnedEndId);

                                var curLowerValue = "0";
                                if (curOwnedEnd.SelectSingleNode("lowerValue").Attributes["value"] != null)
                                    curLowerValue = curOwnedEnd.SelectSingleNode("lowerValue").Attributes["value"].Value;
                                var curUpperValue = curOwnedEnd.SelectSingleNode("upperValue").Attributes["value"].Value;
                                var curMult = curLowerValue + ".." + curUpperValue;

                                if (j == 0)
                                {
                                    ownedElementId1 = curOwnedElementId;
                                    role1 = curRole;
                                    mult1 = curMult;
                                    navigalable1 = curNavigation;
                                    connType1 = curConnectionType;
                                }
                                else
                                {
                                    ownedElementId2 = curOwnedElementId;
                                    role2 = curRole;
                                    mult2 = curMult;
                                    navigalable2 = curNavigation;
                                    connType2 = curConnectionType;
                                }
                            }

                            allElements.Connections.Add(new Connection(elementId, elementName,
                                ownedElementId1, role1, mult1, navigalable1, connType1,
                                ownedElementId2, role2, mult2, navigalable2, connType2));
                            break;

                        case "uml:DataType":
                            allElements.Types.Add(new DataType(elementId, elementName));
                            break;

                        case "uml:Usage":
                            var clientId = curElement.Attributes["client"].Value;
                            var supplierId = curElement.Attributes["supplier"].Value;
                            allElements.Dependences.Add(new Dependence(elementId, "use", clientId, supplierId, DependenceType.Usage));
                            break;

                        case "uml:Dependency":
                            clientId = curElement.Attributes["client"].Value;
                            supplierId = curElement.Attributes["supplier"].Value;
                            allElements.Dependences.Add(new Dependence(elementId, elementName, clientId, supplierId, DependenceType.Dependency));
                            break;

                        case "uml:Enumeration":
                            var literals = new List<Literal>();
                            var literalElements = curElement.SelectNodes("ownedLiteral");
                            var literalElementsCount = literalElements.Count;
                            for (var k = 0; k < literalElementsCount; k++)
                            {
                                var curLiteral = literalElements[k];
                                var literalId = curLiteral.Attributes["xmi:id"].Value;
                                var literalName = curLiteral.Attributes["name"].Value;
                                literals.Add(new Literal(literalId, literalName));
                            }

                            box = graphicInfo.Find(a => a.Item1 == elementId).Item2;
                            allElements.Enumerations.Add(new Enumeration(elementId, elementName, box, literals));
                            break;

                        default:
                            // Добавить ошибки о сторонних элементах
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in extracting \"packageElements\": {ex.Message}");
                }
            }

            // Проверяем комментарии (ограничения)
            var xmlComments = root.GetElementsByTagName("ownedComment");

            var commentsCount = xmlComments.Count;
            for (var i = 0; i < commentsCount; i++)
            {
                try
                {
                    var curComment = xmlElements[i];
                    var commentId = curComment.Attributes["xmi:id"].Value;
                    var annotatedElementId = "";
                    if (curComment.Attributes["annotatedElement"] != null)
                        annotatedElementId = curComment.Attributes["annotatedElement"].Value;
                    var body = "";
                    if (curComment.Attributes["body"] != null)
                        body = curComment.Attributes["body"].Value;

                    var box = graphicInfo.Find(a => a.Item1 == commentId).Item2;
                    allElements.Comments.Add(new Comment(commentId, body, box, annotatedElementId));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in extracting \"ownedComments\": {ex.Message}");
                }
            }
        }

        private static string GetDataType(XmlNode node)
        {
            var dataTypeId = "";
            if (node.Attributes["type"] != null)
            {
                dataTypeId = node.Attributes["type"].Value;
            }
            else
            {
                var typeNode = node.SelectSingleNode("type");
                if (typeNode != null)
                    dataTypeId = "primitiveType";
            }
            return dataTypeId;
        }
    }
}