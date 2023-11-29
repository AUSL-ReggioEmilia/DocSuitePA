using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Repository.Entity;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{
    public class WorkflowActivity : DSWBaseEntity, IUnauditableEntity
    {
        #region [ Constructor ]

        public WorkflowActivity() : this(Guid.NewGuid()) { }

        public WorkflowActivity(Guid uniqueId)
            : base(uniqueId)
        {
            WorkflowProperties = new HashSet<WorkflowProperty>();
            WorkflowActivityLogs = new HashSet<WorkflowActivityLog>();
            WorkflowAuthorizations = new HashSet<WorkflowAuthorization>();
        }
        #endregion

        #region [ Properties ]

        public string Name { get; set; }

        public WorkflowActivityType ActivityType { get; set; }

        public WorkflowActivityAction ActivityAction { get; set; }

        public WorkflowActivityArea ActivityArea { get; set; }

        public WorkflowStatus Status { get; set; }

        public DateTimeOffset? DueDate { get; set; }

        public string Subject { get; set; }

        public Guid? IdArchiveChain { get; set; }

        public WorkflowPriorityType? Priority { get; set; }

        public string Note { get; set; }

        public bool IsVisible { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<WorkflowProperty> WorkflowProperties { get; set; }

        public virtual ICollection<WorkflowActivityLog> WorkflowActivityLogs { get; set; }

        public virtual ICollection<WorkflowAuthorization> WorkflowAuthorizations { get; set; }

        public virtual WorkflowInstance WorkflowInstance { get; set; }

        public virtual DocumentUnit DocumentUnitReferenced { get; set; }

        public virtual Tenant Tenant { get; set; }
        #endregion
    }
}
