using System.Collections.Generic;

namespace ClassDiagrammVerification.Entities
{
    public class Operation
    {
        public string Id;
        public string Name;
        public List<Parameter> Parameters;
        public Visibility Visibility;
        public string ReturnDataTypeId;

        public Operation(string id, string name, List<Parameter> parameters, Visibility visibility, string returnDataTypeId)
        {
            Id = id;
            Name = name;
            Parameters = new List<Parameter>(parameters);
            Visibility = visibility;
            ReturnDataTypeId = returnDataTypeId;
        }
    }
}