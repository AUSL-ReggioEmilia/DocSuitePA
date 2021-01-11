using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowRepository : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowRepository() : base() { }

        public WorkflowRepository(string userName)
            : this()
        {
            this.WorkflowInstances = new HashSet<WorkflowInstance>();
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }
        #endregion

        #region [ Properties ]

        public virtual string Name { get; set; }

        public virtual string Version { get; set; }

        public virtual DateTimeOffset ActiveFrom { get; set; }

        public virtual DateTimeOffset? ActiveTo { get; set; }

        public virtual WorkflowStatus Status { get; set; }

        public virtual string RegistrationUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }


        #endregion
    }
}
