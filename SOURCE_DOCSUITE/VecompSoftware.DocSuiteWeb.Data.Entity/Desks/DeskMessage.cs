using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskMessage : DomainObject<Guid>
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        public DeskMessage()
            : base()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Riferimento al "Tavolo"
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Riferimento al "Message"
        /// </summary>
        public virtual MessageEmail Message { get; set; }

        /// <summary>
        /// Primo inserimento delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }
        #endregion
    }
}
