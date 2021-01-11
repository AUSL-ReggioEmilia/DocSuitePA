using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Entity.Fascicles
{
    public class FascicleRole : DSWBaseEntity
    {
        #region [ Constructor ]

        public FascicleRole() : this(Guid.NewGuid()) { }

        public FascicleRole(Guid uniqueId)
            : base(uniqueId)
        { }
        #endregion

        #region [ Properties ]

        public AuthorizationRoleType AuthorizationRoleType { get; set; }

        public bool IsMaster { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual Fascicle Fascicle { get; set; }

        public virtual Role Role { get; set; }

        #endregion
    }
}
