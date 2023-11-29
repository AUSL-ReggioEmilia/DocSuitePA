using System;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowProperty : DomainObject<Guid>, IAuditable
    {
        #region [ Constructor ]
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base.
        /// Inizializza le Collezioni presenti nell'oggetto.
        /// </summary>
        protected WorkflowProperty() : base() { }

        public WorkflowProperty(string userName)
            : this()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
            RegistrationUser = userName;
        }
        #endregion

        #region [ Properties ]

        public virtual string Name
        {
            get; set;
        }

        public virtual long? ValueInt
        {
            get; set;
        }

        public virtual DateTime? ValueDate
        {
            get; set;
        }

        public virtual double? ValueDouble
        {
            get; set;
        }

        public virtual bool? ValueBoolean
        {
            get; set;
        }

        public virtual string ValueString
        {
            get; set;
        }

        public virtual Guid? ValueGuid
        {
            get; set;
        }

        public virtual string RegistrationUser
        {
            get; set;
        }

        public virtual DateTimeOffset RegistrationDate
        {
            get; set;
        }

        public virtual string LastChangedUser
        {
            get; set;
        }

        public virtual DateTimeOffset? LastChangedDate
        {
            get; set;
        }

        #endregion

        #region [ Navigation Properties ]

        public virtual WorkflowInstance WorkflowInstance
        {
            get; set;
        }

        public virtual WorkflowActivity WorkflowActivity
        {
            get; set;
        }

        #endregion
    }
}
