using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskDocumentEndorsement : DSWBaseEntity
    {
        #region [ Constructor ]

        public DeskDocumentEndorsement() : this(Guid.NewGuid()) { }

        public DeskDocumentEndorsement(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Endorsement: identifica l'accettazione di una versione di un documento da parte dell'utente.
        /// </summary>
        public bool Endorsement { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Relazione tra la tabella Endorsement e la tabella DeskRoleUser
        /// </summary>
        public virtual DeskRoleUser DeskRoleUser { get; set; }
        /// <summary>
        /// Relazione tra la tabella Endorsement e la versione del DeskDocument
        /// </summary>
        public virtual DeskDocumentVersion DeskDocumentVersion { get; set; }
        #endregion
    }
}
