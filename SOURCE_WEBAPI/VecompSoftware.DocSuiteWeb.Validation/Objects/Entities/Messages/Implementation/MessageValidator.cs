using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages
{
    public class MessageValidator : ObjectValidator<Message, MessageValidator>, IMessageValidator
    {
        #region [ Constructor ]
        public MessageValidator(ILogger logger, IMessageValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public MessageStatus Status { get; set; }

        public MessageType MessageType { get; set; }

        public Guid UniqueId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public Location Location { get; set; }

        public ICollection<MessageEmail> MessageEmails { get; set; }

        public ICollection<MessageAttachment> MessageAttachments { get; set; }

        public ICollection<MessageContact> MessageContacts { get; set; }

        public ICollection<MessageLog> MessageLogs { get; set; }

        public ICollection<Protocol> Protocols { get; set; }
        public ICollection<Dossier> Dossiers { get; set; }

        #endregion
    }
}
