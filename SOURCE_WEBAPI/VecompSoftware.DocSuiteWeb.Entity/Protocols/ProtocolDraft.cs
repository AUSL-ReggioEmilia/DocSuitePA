using System;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Entity.Protocols
{
    public class ProtocolDraft : DSWBaseEntity
    {
        #region [ Constructor ]
        public ProtocolDraft() : this(Guid.NewGuid()) { }

        public ProtocolDraft(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public virtual bool IsActive { get; set; }

        public virtual string Description { get; set; }

        public virtual string Data { get; set; }

        public virtual int DraftType { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual Collaboration Collaboration { get; set; }
        public virtual Protocol Protocol { get; set; }
        #endregion
    }
}
