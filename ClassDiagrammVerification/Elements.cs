using System.Collections.Generic;

namespace ClassDiagrammVerification
{
    public class Elements
    {
        public List<Entities.Class> Classes;
        public List<Entities.Connection> Connections;
        public List<Entities.DataType> Types;
        public List<Entities.Dependence> Dependences;
        public List<Entities.Comment> Comments; // Могут быть в роли ограничений
        public List<Entities.Enumeration> Enumerations;
        public List<Entities.Class> Interfaces;
        public List<Entities.Package> Packages;

        public Elements()
        {
            Classes = new List<Entities.Class>();
            Connections = new List<Entities.Connection>();
            Types = new List<Entities.DataType>();
            Dependences = new List<Entities.Dependence>();
            Comments = new List<Entities.Comment>();
            Enumerations = new List<Entities.Enumeration>();
            Interfaces = new List<Entities.Class>();
            Packages = new List<Entities.Package>();
        }
    }
}
