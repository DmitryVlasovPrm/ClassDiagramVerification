namespace ClassDiagrammVerification.Entities
{
    public class Parameter
    {
        public string Id;
        public string Name;
        public string DataTypeId;

        public Parameter(string id, string name, string dataTypeId)
        {
            Id = id;
            Name = name;
            DataTypeId = dataTypeId;
        }
    }
}