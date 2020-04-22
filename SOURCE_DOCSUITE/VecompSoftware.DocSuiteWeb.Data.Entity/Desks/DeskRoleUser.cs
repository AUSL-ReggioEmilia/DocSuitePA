using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskRoleUser : DomainObject<Guid>
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        public DeskRoleUser() : base() 
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            DeskStoryBoards = new Collection<DeskStoryBoard>();
            DeskDocumentEndorsements = new Collection<DeskDocumentEndorsement>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Riferimento dell'utente con il tavolo
        /// </summary>
        public virtual Desk Desk { get; set; }
        
        /// <summary>
        /// Mapping dell'utente sugli utenti di Active Directory
        /// </summary>
        public virtual string AccountName { get; set; }
        /// <summary>
        /// Primo inserimento delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }
        /// <summary>
        /// Ruolo dell'utente all'interno del tavolo.
        /// </summary>
        public virtual DeskPermissionType PermissionType { get; set; }
        /// <summary>
        /// Riferimento alla lavagna
        /// </summary>
        public virtual ICollection<DeskStoryBoard> DeskStoryBoards { get; set; }
        /// <summary>
        /// Riferimento alle approvazioni date dall'utente
        /// </summary>
        public virtual ICollection<DeskDocumentEndorsement> DeskDocumentEndorsements { get; set; }
        #endregion
    }
}
