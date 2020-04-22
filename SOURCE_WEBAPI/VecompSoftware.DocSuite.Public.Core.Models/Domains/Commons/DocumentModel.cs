using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    public class DocumentModel : DomainModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        public DocumentModel(Guid documentId, string name) : base(documentId)
        {
            Name = name;
            DocumentName = name;
        }

        #endregion

        #region [ Properties ]

        public Guid ChainId { get; set; }


        public string DocumentName { get; set; }

        /// <summary>
        /// Tipo del documento <see cref="DocumentType"/>
        /// </summary>
        public DocumentType DocumentType { get; set; }

        /// <summary>
        /// Se l'unità documentaria è un archivio, è necessario specificare anche l'identificativo o Label del documento
        /// </summary>
        public string ArchiveSection { get; set; }

        #endregion
    }
}
