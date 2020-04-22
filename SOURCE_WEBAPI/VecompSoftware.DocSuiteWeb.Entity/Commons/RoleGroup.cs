using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Commons
{
    public class RoleGroup : DSWBaseEntity
    {
        #region [ Constructor ]

        public RoleGroup() : this(Guid.NewGuid()) { }
        public RoleGroup(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region[ Properties ]

        public string GroupName { get; set; }
        public string ProtocolRights { get; set; }
        public string ResolutionRights { get; set; }
        public string DocumentRights { get; set; }
        public string DocumentSeriesRights { get; set; }
        public string FascicleRights { get; set; }

        #endregion

        #region [ Navigation Properties ]
        public virtual Role Role { get; set; }

        public virtual SecurityGroup SecurityGroup { get; set; }


        #endregion
    }
}
