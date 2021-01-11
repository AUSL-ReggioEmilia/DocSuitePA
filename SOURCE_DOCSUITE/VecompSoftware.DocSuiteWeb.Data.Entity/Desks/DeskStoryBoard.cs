using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskStoryBoard : DomainObject<Guid>, IAuditable
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base. 
        /// L'entità mappa la funzione di "LAVAGNA".
        /// </summary>
        protected DeskStoryBoard() : base()
        {
        }

        public DeskStoryBoard(string UserName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = UserName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Relazione tra la "Lavagna" e "Tavoli"
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Relazione tra un commento in lavagna e l'utente che l'ha scritto.
        /// </summary>
        public virtual DeskRoleUser DeskRoleUser { get; set; }
        /// <summary>
        /// Relazione tra un commento in lavagna e una specifica versione di un documento.
        /// </summary>
        public virtual DeskDocumentVersion DeskDocumentVersion { get; set; }
        /// <summary>
        /// Commento lasciato sulla lavagna
        /// </summary>
        public virtual string Comment { get; set; }
        /// <summary>
        /// Data e ora d'inserimento del commento
        /// </summary>
        public virtual DateTime? DateBoard { get; set; }
        /// <summary>
        /// Nome completo dell'autore del commento
        /// </summary>
        public virtual string Author { get; set; }
        /// <summary>
        /// Tipologia di commento lasciato in lavagna
        /// </summary>
        public virtual DeskStoryBoardType BoardType { get; set; }
        /// <summary>
        /// Ultima modifica del commento
        /// </summary>
        public virtual DateTimeOffset? LastChangedDate { get; set; }
        /// <summary>
        /// Ultimo utente che ha modificato il commento
        /// </summary>
        public virtual string LastChangedUser { get; set; }
        /// <summary>
        /// Primo utente che inserisce l'informazione del commento
        /// </summary>
        public virtual string RegistrationUser { get; set; }
        /// <summary>
        /// Prima data di registrazione dell'informazione commento
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }
        #endregion
    }
}
