using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages
{
    public class MessageLogValidator : ObjectValidator<MessageLog, MessageLogValidator>, IMessageLogValidator
    {
        #region [ Constructor ]
        public MessageLogValidator(ILogger logger, IMessageLogValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        /// <summary>
        /// Descrizione
        /// </summary>
        public string LogDescription { get; set; }
        /// <summary>
        /// Tipologia di log
        /// </summary>
        public MessageLogType LogType { get; set; }
        /// <summary>
        /// Data del log
        /// </summary>
        public DateTime LogDate { get; set; }
        /// <summary>
        /// Computer
        /// </summary>
        public string SystemComputer { get; set; }
        /// <summary>
        /// Utente
        /// </summary>
        public string RegistrationUser { get; set; }
        /// <summary>
        /// Severity
        /// </summary>
        public SeverityLog? Severity { get; set; }

        public Guid UniqueId { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Messaggio al quale fa riferimento la mail
        /// </summary>
        public Message Message { get; set; }
        #endregion
    }
}
