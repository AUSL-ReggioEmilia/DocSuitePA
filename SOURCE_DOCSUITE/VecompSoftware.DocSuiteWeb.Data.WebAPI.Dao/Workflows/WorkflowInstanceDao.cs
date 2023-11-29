using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowInstanceDao : BaseWebAPIDao<WorkflowEntities.WorkflowInstance, WorkflowEntities.WorkflowInstance, WorkflowInstanceFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public WorkflowInstanceDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowInstanceFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]
        public WorkflowEntities.WorkflowInstance GetByCollaborationId(int collaborationId)
        {
            Finder.ResetDecoration();
            Finder.ExpandRepository = true;
            Finder.CollaborationId = collaborationId;
            return Finder.DoSearch().Select(s => s.Entity).SingleOrDefault();
        }
        #endregion
    }
}

