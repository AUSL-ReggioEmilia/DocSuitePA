using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowAuthorization : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]

        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowAuthorization() : base() { }

        public WorkflowAuthorization(string userName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }

        #endregion

        #region [ Properties ]

        public virtual string Account { get; set; }

        public virtual DateTimeOffset? LastChangedDate { get; set; }

        public virtual string LastChangedUser { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }

        public virtual string RegistrationUser { get; set; }

        public virtual string IsHandler { get; set; }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowActivity WorkflowActivity { get; set; }

        #endregion
    }
}
