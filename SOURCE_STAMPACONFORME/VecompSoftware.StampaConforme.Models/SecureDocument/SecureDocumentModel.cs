namespace VecompSoftware.StampConforme.Models.SecureDocument
{
    public class SecureDocumentModel
    {
        public string IdDocument { get; set; }
        public byte[] DocumentContent { get; set; }
        public string DocumentName { get; set; }
    }
}
