using System;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.UDS
{
    public class UDSUser : DSWUDSBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]
        public UDSUser() : this(Guid.NewGuid()) { }

        public UDSUser(Guid uniqueId)
            : base(uniqueId)
        {
        }
        #endregion

        #region [ Properties ]

        public string Account { get; set; }

        public AuthorizationRoleType AuthorizationType { get; set; }

        public UDSUserStatus Status { get; set; }
        #endregion

        #region [ Navigation Properties ]

        #endregion
    }
}
