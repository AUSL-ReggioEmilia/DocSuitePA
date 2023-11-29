namespace VecompSoftware.DocSuiteWeb.Model.Documents
{
    public class DocumentSuiteInfo
    {
        public string ReferenceId { get; }
        public string ReferenceType { get; }
        public string FileExtension { get; }
        public int Size { get; }
        public string DocumentId { get; }
        public string FileName { get; }
        public string VirtualPath { get; }
        public bool IsOriginal { get; }


        public DocumentSuiteInfo(
            string referenceId, 
            string referenceType, 
            string fileExtension, 
            int size, 
            string documentId, 
            string fileName, 
            string virtualPath,
            bool isOriginal = false)
        {
            ReferenceId = referenceId;
            ReferenceType = referenceType;
            FileExtension = fileExtension;
            Size = size;
            DocumentId = documentId;
            FileName = fileName;
            VirtualPath = virtualPath;
            IsOriginal = isOriginal;
        }
    }
}
