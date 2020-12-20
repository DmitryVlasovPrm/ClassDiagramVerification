using System.Collections.Generic;

namespace ClassDiagrammVerification.Entities
{
    public class Class
    {
        public string Id;
        public string Name;
        public List<Attribute> Attributes;
        public List<Operation> Operations;

        public Class(string id, string name, List<Attribute> attributes, List<Operation> operations)
        {
            Id = id;
            Name = name;
            Attributes = new List<Attribute>(attributes);
            Operations = new List<Operation>(operations);
        }
    }
}