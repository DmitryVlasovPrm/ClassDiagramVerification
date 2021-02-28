using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassDiagrammVerification.Entities
{
    public class Literal
    {
        public string Id;
        public string Name;

        public Literal(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class Enumeration
    {
        public string Id;
        public string Name;
        public BoundingBox Box;
        public List<Literal> Elements;

        public Enumeration(string id, string name, BoundingBox box, List<Literal> elements)
        {
            Id = id;
            Name = name;
            Box = box;
            Elements = new List<Literal>(elements);
        }
    }
}
