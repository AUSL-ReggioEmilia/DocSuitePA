using System;
using System.IO;

namespace VecompSoftware.DocSuiteWeb.API
{
    public class DocumentDTO : IDocumentDTO
    {
        #region [ Constructors ]

        public DocumentDTO()
        {
        }

        public DocumentDTO(string fullName)
        {
            this.FullName = fullName;
            this.Name = Path.GetFileName(this.FullName);
        }

        public DocumentDTO(string name, byte[] content)
        {
            this.Content = content;
            this.Name = Path.GetFileName(name);
        }

        public DocumentDTO(string biblosArchive, int biblosId)
        {
            this.BiblosArchive = biblosArchive;
            this.BiblosId = biblosId;
        }

        public DocumentDTO(Guid biblosGuid)
        {
            this.BiblosGuid = biblosGuid.ToString();
        }

        #endregion

        #region [ Properties ]
        public string BiblosGuid { get; set; }

        public string BiblosArchive { get; set; }

        public int? BiblosId { get; set; }
        
        public string Name { get; set; }

        public string FullName { get; set; }

        public byte[] Content { get; set; }

        #endregion

        #region [ Methods ]

        public bool FileExists()
        {
            if (string.IsNullOrWhiteSpace(this.FullName))
                return false;

            return File.Exists(this.FullName);
        }

        public bool HasContent()
        {
            return this.Content != null && this.Content.Length > 0;
        }

        public bool HasBiblosGuid()
        {
            return !string.IsNullOrWhiteSpace(this.BiblosGuid);
        }

        public bool HasBiblosId()
        {
            return!string.IsNullOrWhiteSpace(this.BiblosArchive) && this.BiblosId.HasValue;
        }

        public bool HasBiblosReference()
        {
            return this.HasBiblosGuid() || this.HasBiblosId();
        }

        public Guid? GetBiblosGuid()
        {
            if (string.IsNullOrWhiteSpace(this.BiblosGuid))
                return null;

            return new Guid(this.BiblosGuid);
        }

        #endregion

        

    }
}