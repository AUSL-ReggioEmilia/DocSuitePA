using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskCollaboration : DomainObject<Guid>
    {

        #region Constructors

        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        public  DeskCollaboration() : base()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Oggetto esterno riferito ad una collaborazione
        /// </summary>
        public virtual Collaboration Collaboration { get; set; }

        /// <summary>
        /// Oggetto esterno riferito ad un tavolo
        /// </summary>
        public virtual Desk Desk { get; set; }

        /// <summary>
        /// Primo inserimento delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }

        #endregion
    }
}
