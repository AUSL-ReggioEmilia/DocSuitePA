using System;
using System.Collections.Generic;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.PECMails;

namespace VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.PECMails
{
    public class PECMailValidator : ObjectValidator<PECMail, PECMailValidator>, IPECMailValidator
    {
        #region [ Constructor ]
        public PECMailValidator(ILogger logger, IPECMailValidatorMapper mapper, IDataUnitOfWork unitOfWork, ISecurity currentSecurity, IDecryptedParameterEnvService parameterEnvSecurity)
            : base(logger, mapper, unitOfWork, currentSecurity, parameterEnvSecurity) { }

        #endregion

        #region [ Properties ]

        public int EntityId { get; set; }

        public PECMailDirection Direction { get; set; }

        public short? Year { get; set; }

        public int? Number { get; set; }

        public string MailUID { get; set; }

        public string MailContent { get; set; }

        public string MailSubject { get; set; }

        public string MailSenders { get; set; }

        public string MailRecipients { get; set; }

        public DateTime? MailDate { get; set; }

        public string MailType { get; set; }

        public string MailError { get; set; }

        public PECMailPriority? MailPriority { get; set; }

        public string XTrasporto { get; set; }

        public string MessageID { get; set; }

        public string XRiferimentoMessageID { get; set; }

        public string Segnatura { get; set; }

        public string MessaggioRitornoName { get; set; }

        public string MessaggioRitornoStream { get; set; }

        public string MailBody { get; set; }

        public byte? RecordedInDocSuite { get; set; }

        public int? ContentLength { get; set; }

        public bool IsToForward { get; set; }

        public bool IsValidForInterop { get; set; }

        public PECMailActiveType IsActive { get; set; }

        public byte? MailStatus { get; set; }

        public bool? IsDestinated { get; set; }

        public string DestinationNote { get; set; }

        public string Handler { get; set; }

        public Guid? IDAttachments { get; set; }

        public Guid? IDDaticert { get; set; }

        public Guid? IDEnvelope { get; set; }

        public Guid? IDMailContent { get; set; }

        public Guid? IDPostacert { get; set; }

        public Guid? IDSegnatura { get; set; }

        public Guid? IDSmime { get; set; }

        public PECType? PECType { get; set; }

        public string Checksum { get; set; }

        public bool Multiple { get; set; }

        public int? SplittedFrom { get; set; }

        public string OriginalRecipient { get; set; }

        public string HeaderChecksum { get; set; }

        public PECMailProcessStatus? ProcessStatus { get; set; }

        public string MailRecipientsCc { get; set; }

        public bool? ReceivedAsCc { get; set; }

        public long? Size { get; set; }

        public PECMailMultipleType? MultipleType { get; set; }

        public string RegistrationUser { get; set; }

        public DateTimeOffset RegistrationDate { get; set; }

        public string LastChangedUser { get; set; }

        public DateTimeOffset? LastChangedDate { get; set; }

        public Guid UniqueId { get; set; }

        public InvoiceStatus? InvoiceStatus { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public Location Location { get; set; }
        public PECMailBox PECMailBox { get; set; }
        public DocumentUnit DocumentUnit { get; set; }
        public ICollection<PECMailReceipt> PECMailChildrenReceipts { get; set; }
        public ICollection<PECMailReceipt> PECMailReceipts { get; set; }
        // public PECMailBox PECMailBox { get; set; }
        // public ICollection<SMSNotification> SMSNotifications { get; set; }
        // public ICollection<TaskHeaderPECMail> TaskHeaderPECMails { get; set; }
        public ICollection<PECMailAttachment> Attachments { get; set; }

        #endregion
    }
}
