namespace ClassDiagrammVerification.Entities
{
    public class Attribute
    {
        public string Id;
        public string Name;
        public Visibility Visibility;
        public string TypeId;

        public Attribute(string id, string name, Visibility visibility, string typeId)
        {
            Id = id;
            Name = name;
            Visibility = visibility;
            TypeId = typeId;
        }
    }
}