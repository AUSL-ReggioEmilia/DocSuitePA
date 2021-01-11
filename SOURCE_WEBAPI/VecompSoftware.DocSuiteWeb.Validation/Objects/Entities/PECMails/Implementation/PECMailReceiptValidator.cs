using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails
{
    public class PECMailReceiptValidator : ObjectValidator<PECMailReceipt, PECMailReceiptValidator>, IPECMailReceiptValidator
    {
        #region [ Constructor ]
        public PECMailReceiptValidator(ILogger logger, IPECMailReceiptValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity) { }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }

        public string ReceiptType { get; set; }

        public string ErrorShort { get; set; }

        public string ErrorDescription { get; set; }

        public string DateZone { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public string ReceiverType { get; set; }

        public string Subject { get; set; }

        public string Provider { get; set; }

        public string Identification { get; set; }

        public string MSGID { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public PECMail PECMail { get; set; }

        public PECMail PECMailParent { get; set; }

        #endregion
    }
}
