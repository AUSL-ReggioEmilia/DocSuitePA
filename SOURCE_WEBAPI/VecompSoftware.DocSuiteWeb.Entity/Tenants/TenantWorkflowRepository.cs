using System;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{
    public class TenantWorkflowRepository : DSWTenantBaseEntity
    {
        #region [ Constructor ]

        public TenantWorkflowRepository() : this(Guid.NewGuid()) { }
        public TenantWorkflowRepository(Guid uniqueId)
            : base(uniqueId)
        {

        }

        #endregion

        #region [ Properties ]
        public TenantWorkflowRepositoryType ConfigurationType { get; set; }
        public string JsonValue { get; set; }
        public string IntegrationModuleName { get; set; }
        public string Conditions { get; set; }
        #endregion

        #region[ Navigation Properties ]
        public virtual WorkflowRepository WorkflowRepository { get; set; }
        public virtual Tenant Tenant { get; set; }
        #endregion
    }
}
