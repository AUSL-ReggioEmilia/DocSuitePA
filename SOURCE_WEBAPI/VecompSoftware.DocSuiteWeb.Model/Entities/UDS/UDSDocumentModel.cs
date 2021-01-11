using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.UDS
{
    public class UDSDocumentModel
    {
        #region [ Constructor ]

        public UDSDocumentModel()
        {
        }

        #endregion

        #region [ Properties ]

        public Guid IdChain { get; set; }

        public Guid BiblosArchive { get; set; }

        public string DocumentLabel { get; set; }

        public string DocumentName { get; set; }

        public UDSDocumentType DocumentType { get; set; }

        #endregion

    }
}
