using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowInstance : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowInstance() : base() { }

        public WorkflowInstance(string userName)
            : this()
        {
            this.WorkflowProperties = new Collection<WorkflowProperty>();
            this.WorkflowActivities = new Collection<WorkflowActivity>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }
        #endregion

        #region [ Properties ]

        public virtual WorkflowStatus Status { get; set; }

        public virtual Guid? InstanceId { get; set; }

        public virtual string RegistrationUser { get; set; }

        public virtual string Json { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowRepository WorkflowRepository { get; set; }

        public virtual ICollection<WorkflowProperty> WorkflowProperties { get; set; }

        public virtual ICollection<WorkflowActivity> WorkflowActivities { get; set; } 

        #endregion
    }
}
