using System;

namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public class PECMailAttachment : DSWBaseEntity
    {
        #region [ Constructor ]

        public PECMailAttachment() : this(Guid.NewGuid()) { }

        public PECMailAttachment(Guid uniqueId)
            : base(uniqueId)
        { }

        #endregion

        #region [ Properties ]
        public string AttachmentName { get; set; }
        public bool IsMain { get; set; }
        public Guid? IDDocument { get; set; }
        public long? Size { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public PECMailAttachment Parent { get; set; }
        public PECMailAttachment Child { get; set; }
        public PECMail PECMail { get; set; }

        #endregion
    }
}
