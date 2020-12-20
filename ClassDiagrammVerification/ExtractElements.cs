using System;
using System.Collections.Generic;
using System.Linq;
using ClassDiagrammVerification.Entities;
using System.Xml;
using Attribute = ClassDiagrammVerification.Entities.Attribute;

namespace ClassDiagrammVerification
{
    public static class ExtractElements
    {
        private static readonly string[] connectionTypes = { "association", "composite", "shared" };
        
        public static void Extract(XmlNodeList elements, ref List<Class> classes, ref List<Connection> connections,
            ref List<Entities.Type> types)
        {
            try
            {
                var elementsCount = elements.Count;
                for (var i = 0; i < elementsCount; i++)
                {
                    var curElement = elements[i];
                    var elementType = curElement.Attributes["xsi:type"].Value;
                    var elementId = curElement.Attributes["xmi:id"].Value;
                    var elementName = curElement.Attributes["name"].Value;

                    switch (elementType)
                    {
                        case "uml:Class":
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
                                var attribVisibility = (Visibility)Enum.Parse(typeof(Visibility),
                                    curAttrib.Attributes["visibility"].Value, true);
                                var attribTypeId = curAttrib.Attributes["type"].Value;
                                attributes.Add(new Attribute(attribId, attribName, attribVisibility, attribTypeId));
                            }
                            
                            // Считываем операции
                            var operationElements = curElement.SelectNodes("ownedOperation");
                            var operationElementsCount = operationElements.Count;
                            for (var k = 0; k < operationElementsCount; k++)
                            {
                                var curOperation = operationElements[k];
                                var operationId = curOperation.Attributes["xmi:id"].Value;
                                var operationName = curOperation.Attributes["name"].Value;

                                var returnTypeId = "";
                                var parameters = new List<Parameter>();
                                var parametersElements = curOperation.SelectNodes("ownedParameter");
                                var parametersCount = parametersElements.Count;
                                for (var t = 0; t < parametersCount; t++)
                                {
                                    var curParameter = parametersElements[t];
                                    if (curParameter.Attributes["direction"] != null &&
                                        curParameter.Attributes["direction"].Value == "return")
                                        returnTypeId = curParameter.Attributes["type"].Value;
                                    else
                                        parameters.Add(new Parameter(curParameter.Attributes["xmi:id"].Value,
                                        curParameter.Attributes["name"].Value,
                                        curParameter.Attributes["type"].Value));
                                }
                                
                                var operationVisibility = Visibility.@public;
                                if (curOperation.Attributes["visibility"] != null)
                                    operationVisibility = (Visibility)Enum.Parse(typeof(Visibility),
                                        curOperation.Attributes["visibility"].Value, true);
                                
                                operations.Add(new Operation(operationId, operationName, parameters,
                                    operationVisibility, returnTypeId));
                            }

                            classes.Add(new Class(elementId, elementName, attributes, operations));
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
                            ConnectionType connType1 = ConnectionType.association,
                                connType2 = ConnectionType.association;
                            
                            // Смотрим каждый из концов связи
                            for (var j = 0; j < 2; j++)
                            {
                                var curOwnedEnd = ownedEnds[j];
                                var curOwnedEndId = curOwnedEnd.Attributes["xmi:id"].Value;
                                var curRole = curOwnedEnd.Attributes["name"].Value;
                                var curOwnedElementId = curOwnedEnd.Attributes["type"].Value;

                                var curConnectionType = ConnectionType.association;
                                if (curOwnedEnd.Attributes["aggregation"] != null)
                                    curConnectionType = (ConnectionType) Array.IndexOf(connectionTypes,
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

                            connections.Add(new Connection(elementId, elementName,
                                ownedElementId1, role1, mult1, navigalable1, connType1,
                                ownedElementId2, role2, mult2, navigalable2, connType2));
                            break;
                        
                        case "uml:DataType":
                            types.Add(new Entities.Type(elementId, elementName));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}