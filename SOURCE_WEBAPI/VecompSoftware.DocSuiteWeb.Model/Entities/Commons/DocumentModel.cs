using System;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.Commons
{
    public class DocumentModel
    {
        #region [ Constructors ]

        public DocumentModel()
        {
            UniqueId = Guid.NewGuid();
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }

        public ChainType ChainType { get; set; }

        public string ArchiveSection { get; set; }

        public string Segnature { get; set; }

        public string FileName { get; set; }

        public Guid? ChainId { get; set; }

        public Guid? DocumentId { get; set; }

        public int? LegacyDocumentId { get; set; }

        public byte[] ContentStream { get; set; }

        #endregion

        #region [ Methods ]


        #endregion
    }
}
