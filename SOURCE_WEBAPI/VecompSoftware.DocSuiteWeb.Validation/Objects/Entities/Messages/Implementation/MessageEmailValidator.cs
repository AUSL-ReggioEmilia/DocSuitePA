using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages
{
    public class MessageEmailValidator : ObjectValidator<MessageEmail, MessageEmailValidator>, IMessageEmailValidator
    {
        #region [ Constructor ]
        public MessageEmailValidator(ILogger logger, IMessageEmailValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }

        #endregion

        #region [ Properties ]       
        public int EntityId { get; set; }
        /// <summary>
        /// Data di invio
        /// </summary>
        public DateTime? SentDate { get; set; }
        /// <summary>
        /// Oggetto del messaggio
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// Corpo del messaggio
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// Priorità
        /// </summary>
        public string Priority { get; set; }
        /// <summary>
        /// Guid del documento EML
        /// </summary>
        public Guid? EmlDocumentId { get; set; }
        /// <summary>
        /// E' un messaggio di notifica
        /// </summary>
        public bool? IsDispositionNotification { get; set; }

        public Guid UniqueId { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string RegistrationUser { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]

        public ICollection<DeskMessage> DeskMessages { get; set; }

        /// <summary>
        /// Messaggio al quale fa riferimento la mail
        /// </summary>
        public Message Message { get; set; }
        #endregion
    }
}