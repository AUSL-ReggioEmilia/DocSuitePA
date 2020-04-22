using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskDocumentVersion : DSWBaseEntity
    {
        #region [ Constructor ]

        public DeskDocumentVersion() : this(Guid.NewGuid()) { }

        public DeskDocumentVersion(Guid uniqueId)
            : base(uniqueId)
        {
            DeskDocumentEndorsements = new HashSet<DeskDocumentEndorsement>();
            DeskStoryBoards = new HashSet<DeskStoryBoard>();
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Versione incrementale del documento
        /// </summary>
        public decimal? Version { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra la "versione" e il "documento".
        /// </summary>
        public virtual DeskDocument DeskDocument { get; set; }
        /// <summary>
        /// Relazione tra la versione corrente del documento e le accetazioni.
        /// </summary>
        public virtual ICollection<DeskDocumentEndorsement> DeskDocumentEndorsements { get; set; }
        /// <summary>
        /// Relazione tra la "lavagna" e la versione attuale del documento.
        /// </summary>
        public virtual ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }
        #endregion
    }
}
