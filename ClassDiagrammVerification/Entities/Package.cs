namespace ClassDiagrammVerification.Entities
{
    public class Package
    {
        public string Id;
        public string Name;
        public BoundingBox Box;

        public Package(string id, string name, BoundingBox box)
        {
            Id = id;
            Name = name;
            Box = box;
        }
    }
}
