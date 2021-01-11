using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Desks
{
    public class DeskLog : DomainObject<Guid>
    {
        #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        public DeskLog()
            : base()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Riferimento del log al tavolo
        /// </summary>
        public virtual Desk Desk { get; set; }
        /// <summary>
        /// Data di creazione del Log
        /// </summary>
        public virtual DateTime LogDate { get; set; }
        /// <summary>
        /// Computer che crea il Log
        /// </summary>
        public virtual string SystemComputer { get; set; }
        /// <summary>
        /// Utente che crea il log
        /// </summary>
        public virtual string SystemUser { get; set; }
        /// <summary>
        /// Tipologia di Log
        /// </summary>
        public virtual DeskLogType LogType { get; set; }
        /// <summary>
        /// Descrizione presente nel Log
        /// </summary>
        public virtual string LogDescription { get; set; }
        /// <summary>
        /// Importanza del Log
        /// </summary>
        public virtual SeverityLog? Severity { get; set; }

        /// <summary>
        /// Primo inserimento delle informazioni
        /// </summary>
        public virtual DateTimeOffset RegistrationDate { get; set; }
        #endregion
    }
}
