namespace ClassDiagrammVerification.Entities
{
    public class Association
    {
        public string Id;
        public string Name;

        public string OwnedElementId1;
        public string Role1;
        public string Multiplicity1;
        public bool Navigalable1;
        
        public string OwnedElementId2;
        public string Role2;
        public string Multiplicity2;
        public bool Navigalable2;

        public Association(string id, string name, string ownedElementId1, string role1, string multiplicity1, bool navigalable1,
            string ownedElementId2, string role2, string multiplicity2, bool navigalable2)
        {
            Id = id;
            Name = name;
            
            OwnedElementId1 = ownedElementId1;
            Role1 = role1;
            Multiplicity1 = multiplicity1;
            Navigalable1 = navigalable1;
            
            OwnedElementId2 = ownedElementId2;
            Role2 = role2;
            Multiplicity2 = multiplicity2;
            Navigalable2 = navigalable2;
        }
    }
}