using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
{
    public class PECMailModel
    {
        #region [ Constructor ]

        public PECMailModel()
        {
            Attachments = new List<DocumentModel>();
        }

        #endregion

        #region [ Properties ]
        public Guid UniqueId { get; set; }
        public PECMailDirection Direction { get; set; }

        public string MailContent { get; set; }

        public string MailSubject { get; set; }

        public string MailSenders { get; set; }

        public string MailRecipients { get; set; }

        public string MailRecipientsCc { get; set; }

        public DateTime? MailDate { get; set; }

        public string MailBody { get; set; }

        public PECMailPriority? MailPriority { get; set; }

        public PECMailActiveType IsActive { get; set; }

        public byte? MailStatus { get; set; }

        public Guid? IDAttachments { get; set; }

        public Guid? IDDaticert { get; set; }

        public Guid? IDEnvelope { get; set; }

        public Guid? IDMailContent { get; set; }

        public Guid? IDPostacert { get; set; }

        public Guid? IDSegnatura { get; set; }

        public Guid? IDSmime { get; set; }

        public bool Multiple { get; set; }

        public PECMailProcessStatus? ProcessStatus { get; set; }

        public long? Size { get; set; }

        public PECMailMultipleType? MultipleType { get; set; }

        public InvoiceStatus? InvoiceStatus { get; set; }

        #endregion

        #region[ Navigation Properties ]

        public PECMailBoxModel PECMailBox { get; set; }

        public DocumentModel MainAttachment { get; set; }

        public ICollection<DocumentModel> Attachments { get; set; }
        #endregion
    }
}
