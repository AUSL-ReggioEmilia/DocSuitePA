using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowAuthorizationDao : BaseWebAPIDao<WorkflowEntities.WorkflowAuthorization, WorkflowEntities.WorkflowAuthorization, WorkflowAuthorizationFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public WorkflowAuthorizationDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowAuthorizationFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
