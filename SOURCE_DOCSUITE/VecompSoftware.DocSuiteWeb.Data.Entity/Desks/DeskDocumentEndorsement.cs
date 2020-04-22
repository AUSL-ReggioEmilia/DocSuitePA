using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskDocumentEndorsement : DomainObject<Guid>
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        public DeskDocumentEndorsement()
            : base()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Relazione tra la tabella Endorsement e la tabella DeskRoleUser
        /// </summary>
        public virtual DeskRoleUser DeskRoleUser { get; set; }
        /// <summary>
        /// Relazione tra la tabella Endorsement e la versione del DeskDocument
        /// </summary>
        public virtual DeskDocumentVersion DeskDocumentVersion { get; set; }
        /// <summary>
        /// Endorsement: identifica l'accettazione di una versione di un documento da parte dell'utente.
        /// </summary>
        public virtual bool Endorsement { get; set; }

        /// <summary>
        /// Primo inserimento delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }
        #endregion
    }
}
