using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Processes;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{

    public class WorkflowRepository : DSWBaseEntity
    {
        #region [ Constructor ]
        public WorkflowRepository() : this(Guid.NewGuid()) { }

        public WorkflowRepository(Guid uniqueId)
            : base(uniqueId)
        {
            WorkflowInstances = new HashSet<WorkflowInstance>();
            Roles = new HashSet<Role>();
            WorkflowRoleMappings = new HashSet<WorkflowRoleMapping>();
            TenantWorkflowRepositories = new HashSet<TenantWorkflowRepository>();
            WorkflowEvaluationProperties = new HashSet<WorkflowEvaluationProperty>();
            FascicleWorkflowRepositories = new HashSet<ProcessFascicleWorkflowRepository>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public string Version { get; set; }

        public DateTimeOffset ActiveFrom { get; set; }

        public DateTimeOffset? ActiveTo { get; set; }

        public string Xaml { get; set; }

        public string Json { get; set; }

        public WorkflowRepositoryStatus Status { get; set; }

        public string CustomActivities { get; set; }

        public DSWEnvironmentType DSWEnvironment { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<WorkflowRoleMapping> WorkflowRoleMappings { get; set; }

        public virtual ICollection<TenantWorkflowRepository> TenantWorkflowRepositories { get; set; }

        public virtual ICollection<WorkflowEvaluationProperty> WorkflowEvaluationProperties { get; set; }

        public virtual ICollection<ProcessFascicleWorkflowRepository> FascicleWorkflowRepositories { get; set; }

        #endregion
    }
}
