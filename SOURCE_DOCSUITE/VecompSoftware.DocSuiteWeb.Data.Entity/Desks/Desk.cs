using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class Desk : DomainObject<Guid>, IAuditable
    {

        #region Constructors

        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected Desk() : base()
        {

        }

        public Desk(string UserName)
            : this()
        {
            DeskDocuments = new Collection<DeskDocument>();
            DeskLogs = new Collection<DeskLog>();
            DeskMessages = new Collection<DeskMessage>();
            DeskRoleUsers = new Collection<DeskRoleUser>();
            DeskStoryBoards = new Collection<DeskStoryBoard>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = UserName;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Oggetto esterno riferito ad un contenitore
        /// </summary>
        public virtual Container Container { get; set; }
                       
        /// <summary>
        /// Nome del tavolo
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Descrizione del tavolo
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Data di scadenza del tavolo
        /// </summary>
        public virtual DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Stato del tavolo.
        /// 1) aperto
        /// 2) chiuso
        /// 3) approvazione
        /// </summary>
        public virtual DeskState? Status { get; set; }

        /// <summary>
        /// Ultimo cambiamento delle informazioni
        /// </summary>
        public virtual DateTimeOffset? LastChangedDate { get; set; }

        /// <summary>
        /// Ultimo utente che ha effettuato un cambiamento delle informazioni
        /// </summary>
        public virtual string LastChangedUser { get; set; }

        /// <summary>
        /// Utente che ha creato le informazioni la prima volta
        /// </summary>
        public virtual string RegistrationUser { get; set; }

        /// <summary>
        /// Primo inserimento delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }

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
        #endregion
    }
}
