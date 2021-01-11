using iTextSharp.text;

namespace VecompSoftware.Helpers.iTextSharp
{
    public class DocumentWrapper : Document
    {

        public byte[] Content { get; set; }
        public bool? IsEncrypted { get; set; }

    }
}
