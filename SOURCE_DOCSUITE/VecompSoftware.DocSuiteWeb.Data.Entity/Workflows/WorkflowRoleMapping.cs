using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowRoleMapping : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]

        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowRoleMapping() : base() { }

        public WorkflowRoleMapping(string userName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }

        #endregion

        #region  [ Properties ]
        public virtual string MappingTag { get; set; }

        public virtual WorkflowAuthorizationType AuthorizationType { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string RegistrationUser { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual string IdInternalActivity { get; set; }

        public virtual string AccountName { get; set; }
        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowRepository WorkflowRepository { get; set; }

        public virtual Role Role { get; set; }
        #endregion
    }
}
