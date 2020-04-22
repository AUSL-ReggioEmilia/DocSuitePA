using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{

    public class DeskRoleUser : DSWBaseEntity
    {
        #region [ Constructor ]

        public DeskRoleUser() : this(Guid.NewGuid()) { }

        public DeskRoleUser(Guid uniqueId)
            : base(uniqueId)
        {
            DeskDocumentEndorsements = new HashSet<DeskDocumentEndorsement>();
            DeskStoryBoards = new HashSet<DeskStoryBoard>();
        }
        #endregion

        #region [ Properties  ]        
        /// <summary>
        /// Mapping dell'utente sugli utenti di Active Directory
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Ruolo dell'utente all'interno del tavolo.
        /// </summary>
        public DeskPermissionType PermissionType { get; set; }

        #endregion

        #region [ Navigation Properties  ]
        /// <summary>
        /// Riferimento alle approvazioni date dall'utente
        /// </summary>
        public virtual ICollection<DeskDocumentEndorsement> DeskDocumentEndorsements { get; set; }
        /// <summary>
        /// Riferimento dell'utente con il tavolo
        /// </summary>
        public virtual Desk Desk { get; set; }

        /// <summary>
        /// Riferimento alla lavagna
        /// </summary>
        public virtual ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }
        #endregion
    }
}
