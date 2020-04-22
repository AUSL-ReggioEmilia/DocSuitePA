using System;

namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{

    public class PECMailReceipt : DSWBaseEntity
    {
        #region [ Constructor ]

        public PECMailReceipt() : this(Guid.NewGuid()) { }

        public PECMailReceipt(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion

        #region [ Properties ]
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

        #endregion

        #region[ Navigation Properties ]

        public virtual PECMail PECMail { get; set; }

        public virtual PECMail PECMailParent { get; set; }

        #endregion
    }
}
