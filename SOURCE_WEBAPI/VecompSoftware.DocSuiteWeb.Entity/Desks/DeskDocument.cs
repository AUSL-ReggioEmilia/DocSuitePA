using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskDocument : DSWBaseEntity
    {
        #region [ Constructor ]
        public DeskDocument() : this(Guid.NewGuid()) { }

        public DeskDocument(Guid uniqueId)
            : base(uniqueId)
        {
            DeskDocumentVersions = new HashSet<DeskDocumentVersion>();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Cancellazione logica documento
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Tipologia di documento memorizzato
        /// 1) Documento
        /// 2) Allegato
        /// 3) Annesso
        /// </summary>
        public virtual DeskDocumentType DocumentType { get; set; }
        /// <summary>
        /// Identificativo del documento derivante da Biblos
        /// </summary>
        public Guid? IdDocument { get; set; }
        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra il documento e il "Tavolo"
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Collezione di versioni del documento
        /// </summary>
        public virtual ICollection<DeskDocumentVersion> DeskDocumentVersions { get; set; }
        #endregion
    }
}
