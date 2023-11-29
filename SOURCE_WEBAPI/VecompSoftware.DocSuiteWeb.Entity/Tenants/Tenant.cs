using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Entity.Tenants
{
    public class Tenant : DSWTenantBaseEntity
    {
        #region [ Constructor ]

        public Tenant() : this(Guid.NewGuid()) { }

        public Tenant(Guid uniqueId)
            : base(uniqueId)
        {
            Configurations = new HashSet<TenantConfiguration>();
            Containers = new HashSet<Container>();
            PECMailBoxes = new HashSet<PECMailBox>();
            TenantWorkflowRepositories = new HashSet<TenantWorkflowRepository>();
            WorkflowActivities = new HashSet<WorkflowActivity>();
        }

        #endregion

        #region [ Properties ]
        public string TenantName { get; set; }
        public string CompanyName { get; set; }
        public TenantTypologyType TenantTypology { get; set; }
        #endregion

        #region[ Navigation Properties ]

        public virtual ICollection<TenantConfiguration> Configurations { get; set; }
        public virtual ICollection<Container> Containers { get; set; }
        public virtual ICollection<PECMailBox> PECMailBoxes { get; set; }
        public virtual ICollection<TenantWorkflowRepository> TenantWorkflowRepositories { get; set; }
        public virtual ICollection<WorkflowActivity> WorkflowActivities { get; set; }
        public virtual TenantAOO TenantAOO { get; set; }

        #endregion
    }
}
