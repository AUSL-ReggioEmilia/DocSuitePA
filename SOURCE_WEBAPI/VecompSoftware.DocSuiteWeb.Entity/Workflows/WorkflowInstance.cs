using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Entity.Workflows
{

    public class WorkflowInstance : DSWBaseEntity
    {
        #region [ Constructor ]

        public WorkflowInstance() : this(Guid.NewGuid()) { }

        public WorkflowInstance(Guid uniqueId)
            : base(uniqueId)
        {
            WorkflowProperties = new HashSet<WorkflowProperty>();
            WorkflowActivities = new HashSet<WorkflowActivity>();
            Collaborations = new HashSet<Collaboration>();
            WorkflowInstanceLogs = new HashSet<WorkflowInstanceLog>();
            Dossiers = new HashSet<Dossier>();
            Fascicles = new HashSet<Fascicle>();
            WorkflowInstanceRoles = new HashSet<WorkflowInstanceRole>();
        }
        #endregion

        #region [ Properties ]

        public WorkflowStatus Status { get; set; }

        public Guid? InstanceId { get; set; }

        public string Json { get; set; }
        public string Subject { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowRepository WorkflowRepository { get; set; }

        public virtual ICollection<WorkflowProperty> WorkflowProperties { get; set; }

        public virtual ICollection<WorkflowActivity> WorkflowActivities { get; set; }

        public virtual ICollection<Collaboration> Collaborations { get; set; }

        public virtual ICollection<WorkflowInstanceLog> WorkflowInstanceLogs { get; set; }

        public virtual ICollection<WorkflowInstanceRole> WorkflowInstanceRoles { get; set; }

        public virtual ICollection<Dossier> Dossiers { get; set; }

        public virtual ICollection<Fascicle> Fascicles { get; set; }
        #endregion
    }
}
