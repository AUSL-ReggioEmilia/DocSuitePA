using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages
{
    public class MessageContactValidator : ObjectValidator<MessageContact, MessageContactValidator>, IMessageContactValidator
    {
        #region [ Constructor ]
        public MessageContactValidator(ILogger logger, IMessageContactValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity)
        { }
        #endregion

        #region [ Properties ]      
        public int EntityId { get; set; }
        /// <summary>
        /// Tipo di contatto
        /// </summary>
        public MessageContactType ContactType { get; set; }
        /// <summary>
        /// Contatto di posizione
        /// <example>Sender, Recipient, RecipientCc, RecipientBcc</example>
        /// </summary>
        public MessageContactPosition ContactPosition { get; set; }
        /// <summary>
        /// Descrizione della mail
        /// </summary>
        public string Description { get; set; }

        public Guid UniqueId { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Messaggio in cui è contenuto l'attachment
        /// </summary>
        public Message Message { get; set; }

        public ICollection<MessageContactEmail> MessageContactEmail { get; set; }
        #endregion
    }
}
