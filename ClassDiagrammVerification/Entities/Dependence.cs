namespace ClassDiagrammVerification.Entities
{
    public enum DependenceType
    {
        Usage = 0,
        Dependency = 1,
    }

    public class Dependence
    {
        public string Id;
        public string Name;
        public string ClientElementId;
        public string SupplierElementId;
        public DependenceType DependenceType;

        public Dependence(string id, string name, string clientElementId, string supplierElementId, DependenceType dependenceType)
        {
            Id = id;
            Name = name;
            ClientElementId = clientElementId;
            SupplierElementId = supplierElementId;
            DependenceType = dependenceType;
        }
    }
}
