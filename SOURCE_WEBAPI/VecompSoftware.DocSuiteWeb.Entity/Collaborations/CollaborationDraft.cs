using System;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationDraft : DSWBaseEntity
    {
        #region [ Constructor ]
        public CollaborationDraft() : this(Guid.NewGuid()) { }

        public CollaborationDraft(Guid uniqueId)
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
        public virtual DocumentUnit DocumentUnit { get; set; }
        #endregion
    }
}
