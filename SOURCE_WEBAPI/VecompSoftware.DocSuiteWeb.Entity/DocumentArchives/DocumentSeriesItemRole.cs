using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.DocumentArchives
{
    public class DocumentSeriesItemRole : DSWBaseEntity
    {
        #region [ Constructor ]

        public DocumentSeriesItemRole() : this(Guid.NewGuid()) { }
        public DocumentSeriesItemRole(Guid uniqueId)
            : base(uniqueId)
        {

        }
        #endregion

        #region [ Properties ]        
        public DocumentSeriesItemRoleLinkType LinkType { get; set; }
        #endregion

        #region [ Navigation Properties ]
        public virtual DocumentSeriesItem DocumentSeriesItem { get; set; }
        public virtual Role Role { get; set; }
        #endregion
    }
}
