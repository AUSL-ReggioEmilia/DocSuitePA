using System;
using VecompSoftware.DocSuiteWeb.Entity.Dossiers;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Entity.Processes
{
    public class ProcessFascicleWorkflowRepository : DSWBaseEntity
    {
        #region [ Constructor ]

        public ProcessFascicleWorkflowRepository(Guid uniqueId) : base(uniqueId)
        {
        }

        public ProcessFascicleWorkflowRepository() : this(Guid.NewGuid())
        {

        }

        #endregion

        #region [ Properties ]

        #endregion

        #region [ Navigation Properties ]

        public virtual Process Process { get; set; }
        public virtual DossierFolder DossierFolder { get; set; }
        public virtual WorkflowRepository WorkflowRepository { get; set; }

        #endregion        
    }
}
