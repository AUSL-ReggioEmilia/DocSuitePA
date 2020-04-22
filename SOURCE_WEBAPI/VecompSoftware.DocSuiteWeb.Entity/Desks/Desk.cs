using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{
    [Serializable]
    public class Desk : DSWBaseEntity
    {
        #region [ Constructor ]

        public Desk() : this(Guid.NewGuid()) { }

        public Desk(Guid uniqueId)
            : base(uniqueId)
        {
            DeskDocuments = new HashSet<DeskDocument>();
            DeskLogs = new HashSet<DeskLog>();
            DeskMessages = new HashSet<DeskMessage>();
            DeskRoleUsers = new HashSet<DeskRoleUser>();
            DeskStoryBoards = new HashSet<DeskStoryBoard>();
            DeskCollaborations = new HashSet<DeskCollaboration>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// <summary>
        /// Nome del tavolo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descrizione del tavolo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Data di scadenza del tavolo
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Stato del tavolo.
        /// 1) aperto
        /// 2) chiuso
        /// 3) approvazione
        /// </summary>
        public DeskState? Status { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual ICollection<Collaboration> Collaborations { get; set; }
        /// <summary>
        /// Collezione di Documenti presenti nel Tavolo
        /// </summary>
        public virtual ICollection<DeskDocument> DeskDocuments { get; set; }
        /// <summary>
        /// Collezione di Log che riferiscono al tavolo
        /// </summary>
        public virtual ICollection<DeskLog> DeskLogs { get; set; }
        /// <summary>
        /// Collezione di Messaggi che riferiscono al tavolo
        /// </summary>
        public virtual ICollection<DeskMessage> DeskMessages { get; set; }
        /// <summary>
        /// Collezione di RoleUser che riferiscono al tavolo
        /// </summary>
        public virtual ICollection<DeskRoleUser> DeskRoleUsers { get; set; }
        /// <summary>
        /// Collezione di Story Board che riferiscono al tavolo
        /// </summary>
        public virtual ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }

        /// <summary>
        /// Collezione contenente i riferimenti tra Desk e Collaboration
        /// </summary>
        public virtual ICollection<DeskCollaboration> DeskCollaborations { get; set; }

        ///// <summary>
        ///// Collezione contenente i riferimenti tra Desk e Container
        ///// </summary>
        //public virtual ICollection<Container> Containers { get; set; }

        #endregion
    }
}
