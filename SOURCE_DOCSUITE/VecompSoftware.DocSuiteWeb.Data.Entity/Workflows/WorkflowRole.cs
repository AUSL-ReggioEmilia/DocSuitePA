using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowRole : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]

        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowRole() : base() { }

        public WorkflowRole(string userName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }

        #endregion

        #region  [ Properties ]

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string RegistrationUser { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowRepository WorkflowRepository { get; set; }

        public virtual Role Role { get; set; }

        #endregion
    }
}
