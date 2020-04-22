using System;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.Entity.Workflows
{
    public class WorkflowActivityLog : DomainObject<Guid>
    {
         #region Constructors
        /// <summary>
        /// Costruttore vuoto ereditato dalla classe base
        /// </summary>
        public WorkflowActivityLog()
            : base()
        {
            RegistrationDate = DateTimeOffset.UtcNow;
        }
        #endregion

        #region Properties

        public virtual WorkflowActivity WorkflowActivity { get; set; }

        public virtual DateTimeOffset LogDate { get; set; }

        public virtual string SystemComputer { get; set; }

        public virtual string SystemUser { get; set; }

        public virtual WorkflowStatus LogType { get; set; }

        public virtual string LogDescription { get; set; }

        public virtual SeverityLog? Severity { get; set; }

        public virtual DateTimeOffset RegistrationDate { get; set; }
        #endregion
    }
}
