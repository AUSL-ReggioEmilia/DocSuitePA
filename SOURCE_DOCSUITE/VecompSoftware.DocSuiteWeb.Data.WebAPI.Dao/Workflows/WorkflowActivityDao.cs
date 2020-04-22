using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowActivityDao : BaseWebAPIDao<WorkflowEntities.WorkflowActivity, WorkflowEntities.WorkflowActivity, WorkflowActivityFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public WorkflowActivityDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowActivityFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]        
        #endregion
    }
}
