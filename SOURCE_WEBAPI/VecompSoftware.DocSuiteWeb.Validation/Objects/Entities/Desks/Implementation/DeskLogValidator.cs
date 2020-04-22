using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Desks
{
    [HasSelfValidation]
    public class DeskLogValidator : ObjectValidator<DeskLog, DeskLogValidator>, IDeskLogValidator
    {
        #region [ Constructor ]
        public DeskLogValidator(ILogger logger, IDeskLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        /// <summary>
        /// Data di creazione del Log
        /// </summary>
        public DateTime LogDate { get; set; }
        /// <summary>
        /// Computer che crea il Log
        /// </summary>
        public string SystemComputer { get; set; }
        /// <summary>
        /// Utente che crea il log
        /// </summary>
        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }
        /// <summary>
        /// Tipologia di Log
        /// </summary>
        public DeskLogType LogType { get; set; }
        /// <summary>
        /// Descrizione presente nel Log
        /// </summary>
        public string LogDescription { get; set; }
        /// <summary>
        /// Importanza del Log
        /// </summary>
        public SeverityLog? Severity { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Riferimento del log al tavolo
        /// </summary>
        public Desk Desk { get; set; }
        #endregion
    }
}
