using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowActivity : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowActivity() : base() { }

        public WorkflowActivity(string userName)
            : this()
        {
            this.WorkflowProperties = new HashSet<WorkflowProperty>();
            this.WorkflowActivityLogs = new HashSet<WorkflowActivityLog>();
            this.WorkflowAuthorizations = new HashSet<WorkflowAuthorization>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }
        #endregion

        #region [ Properties ]

        public virtual string Name { get; set; }

        public virtual ActivityType ActivityType { get; set; }

        public virtual WorkflowStatus Status { get; set; }

        public virtual DateTimeOffset? DueDate { get; set; }

        public virtual short? Priority { get; set; }

        public virtual string Subject { get; set; }

        public virtual string RegistrationUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        public virtual Guid? DocumentUnitReferencedId { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<WorkflowProperty> WorkflowProperties { get; set; }

        public virtual ICollection<WorkflowActivityLog> WorkflowActivityLogs { get; set; }

        public virtual ICollection<WorkflowAuthorization> WorkflowAuthorizations { get; set; }

        public virtual WorkflowInstance WorkflowInstance { get; set; }
        #endregion
    }
}
