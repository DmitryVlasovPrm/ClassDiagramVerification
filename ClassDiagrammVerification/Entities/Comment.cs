namespace ClassDiagrammVerification.Entities
{
    public class Comment
    {
        public string Id;
        public string Body;
        public BoundingBox Box;
        public string AnnotatedElementId;

        public Comment(string id, string body, BoundingBox box, string annotatedElementId)
        {
            Id = id;
            Body = body;
            Box = box;
            AnnotatedElementId = annotatedElementId;
        }
    }
}
