using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowRepositoryDao : BaseWebAPIDao<WorkflowEntities.WorkflowRepository, WorkflowEntities.WorkflowRepository, WorkflowRepositoryFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public WorkflowRepositoryDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowRepositoryFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
