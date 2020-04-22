using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.UDS;

namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public class PECMail : DSWBaseEntity, IWorkflowContentBase
    {
        #region [ Constructor ]

        public PECMail() : this(Guid.NewGuid()) { }

        public PECMail(Guid uniqueId)
            : base(uniqueId)
        {
            PECMailLogs = new HashSet<PECMailLog>();
            PECMailReceipts = new HashSet<PECMailReceipt>();
            PECMailChildrenReceipts = new HashSet<PECMailReceipt>();
            Attachments = new HashSet<PECMailAttachment>();
            UDSPECMails = new HashSet<UDSPECMail>();
            WorkflowActions = new List<IWorkflowAction>();
        }

        #endregion

        #region [ Properties ]
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

        public Guid? IdUDS { get; set; }

        public InvoiceStatus? InvoiceStatus { get; set; }

        public DSWEnvironmentType? DocumentUnitType { get; set; }
        #endregion

        #region[ Navigation Properties ]

        public virtual Location Location { get; set; }
        public virtual PECMailBox PECMailBox { get; set; }
        public virtual UDSRepository UDSRepository { get; set; }
        public virtual ICollection<PECMailLog> PECMailLogs { get; set; }
        public virtual ICollection<PECMailReceipt> PECMailChildrenReceipts { get; set; }
        public virtual ICollection<PECMailReceipt> PECMailReceipts { get; set; }
        // public virtual ICollection<SMSNotification> SMSNotifications { get; set; }
        // public virtual ICollection<TaskHeaderPECMail> TaskHeaderPECMails { get; set; }
        public virtual ICollection<PECMailAttachment> Attachments { get; set; }
        public virtual ICollection<UDSPECMail> UDSPECMails { get; set; }

        #endregion

        #region [ Not Mapping Properties ]
        public Guid? IdWorkflowActivity { get; set; }
        public string WorkflowName { get; set; }

        public ICollection<IWorkflowAction> WorkflowActions { get; set; }

        #endregion
    }
}
