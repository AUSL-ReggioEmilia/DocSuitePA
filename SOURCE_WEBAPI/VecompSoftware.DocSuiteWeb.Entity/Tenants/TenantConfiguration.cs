using System;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{

    public class TenantConfiguration : DSWTenantBaseEntity
    {
        #region [ Constructor ]

        public TenantConfiguration() : this(Guid.NewGuid()) { }
        public TenantConfiguration(Guid uniqueId)
            : base(uniqueId)
        {
        }

        #endregion

        #region [ Properties ]
        public TenantConfigurationType ConfigurationType { get; set; }
        public string JsonValue { get; set; }

        #endregion

        #region[ Navigation Properties ]
        public virtual Tenant Tenant { get; set; }
        #endregion
    }
}
