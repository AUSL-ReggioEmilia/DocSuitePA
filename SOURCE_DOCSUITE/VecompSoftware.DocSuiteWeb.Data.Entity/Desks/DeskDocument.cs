using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskDocument : DomainObject<Guid>, IAuditable, ISupportLogicDelete
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        protected DeskDocument() : base()
        {

        }
        public DeskDocument(string UserName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = UserName;
            DeskDocumentVersions = new Collection<DeskDocumentVersion>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Relazione tra il documento e il "Tavolo"
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Identificativo del documento derivante da Biblos
        /// </summary>
        public virtual Nullable<Guid> IdDocument { get; set; }
        /// <summary>
        /// Tipologia di documento memorizzato
        /// 1) Documento
        /// 2) Allegato
        /// 3) Annesso
        /// </summary>
        public virtual DeskDocumentType DocumentType { get; set; }
        /// <summary>
        /// Ultima data in cui è stato modificato il documento
        /// </summary>
        public virtual DateTimeOffset? LastChangedDate { get; set; }
        /// <summary>
        /// Ultimo utente che ha modificato il documento
        /// </summary>
        public virtual string LastChangedUser { get; set; }
        /// <summary>
        /// Primo utente che ha creato il documento
        /// </summary>
        public virtual string RegistrationUser { get; set; }
        /// <summary>
        /// Prima data di creazione del documento
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }
        /// <summary>
        /// Cancellazione logica documento
        /// </summary>
        public virtual short IsActive { get; set; } 
        /// <summary>
        /// Collezione di versioni del documento
        /// </summary>
        public virtual ICollection<DeskDocumentVersion> DeskDocumentVersions { get; set; }
      
        #endregion
    }
}
