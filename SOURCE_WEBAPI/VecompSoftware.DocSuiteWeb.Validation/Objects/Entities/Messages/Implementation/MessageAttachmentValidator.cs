using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages
{
    public class MessageAttachmentValidator : ObjectValidator<MessageAttachment, MessageAttachmentValidator>, IMessageAttachmentValidator
    {
        #region [ Constructor ]
        public MessageAttachmentValidator(ILogger logger, IMessageAttachmentValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        /// <summary>
        /// Server in cui è archiviato l'attachment
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Archive in cui è archiviato l'attacment
        /// </summary>
        public string Archive { get; set; }
        /// <summary>
        /// Id della catena in cui è archiviato l'attacment
        /// </summary>
        public int ChainId { get; set; }
        /// <summary>
        /// Indice posizionale del documento
        /// </summary>
        public int? DocumentEnum { get; set; }
        /// <summary>
        /// Estensione dell'attachment
        /// </summary>
        public string Extension { get; set; }

        public Guid UniqueId { get; set; }
        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }

        #endregion

        #region [ Navigation Properties ]
        /// <summary>
        /// Messaggio in cui è contenuto l'attachment
        /// </summary>
        public Message Message { get; set; }
        #endregion
    }
}