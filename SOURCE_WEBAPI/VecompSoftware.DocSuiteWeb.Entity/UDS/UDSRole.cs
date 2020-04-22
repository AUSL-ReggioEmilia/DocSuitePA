using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSRole : DSWUDSRelationBaseEntity<Role>, IUnauditableEntity
    {
        #region [ Constructor ]
        public UDSRole() : this(Guid.NewGuid()) { }

        public UDSRole(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]
        public AuthorizationRoleType AuthorizationType { get; set; }
        public string AuthorizationLabel { get; set; }
        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
