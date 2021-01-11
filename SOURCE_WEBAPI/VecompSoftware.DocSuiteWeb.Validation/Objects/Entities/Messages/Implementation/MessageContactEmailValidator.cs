using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Messages;


namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Messages
{
    public class MessageContactEmailValidator : ObjectValidator<MessageContactEmail, MessageContactEmailValidator>, IMessageContactEmailValidator
    {
        #region [ Constructor ]
        public MessageContactEmailValidator(ILogger logger, IMessageContactEmailValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity)
        { }
        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        /// <summary>
        /// Utente
        /// </summary>
        public string User { get; set; }
        /// <summary>
        /// Indirizzo mail
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Descrizione del contatto della mail
        /// </summary>
        public string Description { get; set; }

        public Guid UniqueId { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public byte[] Timestamp { get; set; }


        #endregion

        #region [ Navigation Properties ]

        /// <summary>
        /// Messaggio al quale fa riferimento la mail
        /// </summary>
        public MessageContact MessageContact { get; set; }
        #endregion
    }
}
