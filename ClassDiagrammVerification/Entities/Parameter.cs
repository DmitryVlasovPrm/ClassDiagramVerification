namespace ClassDiagrammVerification.Entities
{
    public class Parameter
    {
        public string Id;
        public string Name;
        public string TypeId;

        public Parameter(string id, string name, string typeId)
        {
            Id = id;
            Name = name;
            TypeId = typeId;
        }
    }
}