using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationAggregate : DSWBaseEntity
    {
        #region [ Constructor ]

        public CollaborationAggregate() : this(Guid.NewGuid()) { }

        public CollaborationAggregate(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        public string CollaborationDocumentType { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Collaboration CollaborationFather { get; set; }
        public virtual Collaboration CollaborationChild { get; set; }
        #endregion
    }
}
