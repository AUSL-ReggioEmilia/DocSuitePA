using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowRoleMappingDao : BaseWebAPIDao<WorkflowEntities.WorkflowRoleMapping, WorkflowEntities.WorkflowRoleMapping, WorkflowRoleMappingFinder>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public WorkflowRoleMappingDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowRoleMappingFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]        
        #endregion
    }
}
