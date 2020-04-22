using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskDocumentVersion : DomainObject<Guid>, IAuditable
    {
        #region Constructors
        
        protected DeskDocumentVersion() : base()
        {
        }

        public DeskDocumentVersion(string UserName)
            : this()
        {
            DeskDocumentEndorsements = new List<DeskDocumentEndorsement>();
            DeskStoryBoards = new List<DeskStoryBoard>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = UserName;
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Relazione tra la "versione" e il "documento".
        /// </summary>
        public virtual DeskDocument DeskDocument { get; set; }
        /// <summary>
        /// Versione incrementale del documento
        /// </summary>
        public virtual decimal? Version { get; set; }
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
