using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowActivityLogDao : BaseWebAPIDao<WorkflowEntities.WorkflowActivityLog, WorkflowEntities.WorkflowActivityLog, WorkflowActivityLogFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public WorkflowActivityLogDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowActivityLogFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]        
        #endregion
    }
}
