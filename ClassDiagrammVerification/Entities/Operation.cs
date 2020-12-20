using System.Collections.Generic;

namespace ClassDiagrammVerification.Entities
{
    public class Operation
    {
        public string Id;
        public string Name;
        public List<Parameter> Parameters;
        public Visibility Visibility;
        public string ReturnTypeId;

        public Operation(string id, string name, List<Parameter> parameters, Visibility visibility, string returnTypeId)
        {
            Id = id;
            Name = name;
            Parameters = new List<Parameter>(parameters);
            Visibility = visibility;
            ReturnTypeId = returnTypeId;
        }
    }
}