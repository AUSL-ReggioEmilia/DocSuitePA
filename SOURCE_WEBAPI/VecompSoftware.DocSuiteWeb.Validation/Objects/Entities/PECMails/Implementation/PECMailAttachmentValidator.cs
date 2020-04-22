using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails
{
    public class PECMailAttachmentValidator : ObjectValidator<PECMailAttachment, PECMailAttachmentValidator>, IPECMailAttachmentValidator
    {
        #region [ Constructor ]
        public PECMailAttachmentValidator(ILogger logger, IPECMailAttachmentValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]
        public int EntityId { get; set; }
        public string AttachmentName { get; set; }
        public bool IsMain { get; set; }
        public Guid? IDDocument { get; set; }
        public long? Size { get; set; }
        public string RegistrationUser { get; set; }
        public DateTimeOffset RegistrationDate { get; set; }
        public string LastChangedUser { get; set; }
        public DateTimeOffset? LastChangedDate { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public PECMailAttachment Parent { get; set; }

        public PECMail PECMail { get; set; }

        #endregion
    }
}
