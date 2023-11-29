using System;
using System.IO;
using VecompSoftware.Services.Biblos.Models;

namespace VecompSoftware.DocSuiteWeb.DTO.SignDocuments
{
    public class Document
    {
        public Document() { }

        public Document(TempFileDocumentInfo docInfo)
        {
            Content = docInfo.Stream;
            Name = docInfo.Name;
            FullName = docInfo.FileInfo.FullName;
            FileName = docInfo.FileInfo.Name;
            DocumentId = Guid.NewGuid();
        }

        public Document(BiblosDocumentInfo docInfo)
        {
            DocumentId = docInfo.DocumentId;
            ChainId = docInfo.ChainId;
            Name = docInfo.Name;
        }

        public Guid DocumentId { get; set; }
        public Guid ChainId { get; set; }
        public Guid CollaborationVersioningId { get; set; }
        public Guid CollaborationId { get; set; }
        public Guid FascicleId { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public bool Mandatory { get; set; }
        public bool MandatorySelectable { get; set; }
        public bool PadesCompliant
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return false;
                }

                string fileExtension = Path.GetExtension(Name);

                if (string.IsNullOrEmpty(fileExtension))
                {
                    return false;
                }

                return fileExtension.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
