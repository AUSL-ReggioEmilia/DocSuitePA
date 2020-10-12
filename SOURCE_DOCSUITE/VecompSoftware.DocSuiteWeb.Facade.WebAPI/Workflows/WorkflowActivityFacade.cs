using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
{
    public class WorkflowActivityFacade : FacadeWebAPIBase<WorkflowActivity, WorkflowActivityDao>
    {
        #region [ Fields ]  
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowActivityFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<WorkflowActivity, WorkflowActivityDao>(s)).ToList(), currentTenant)
        {
            
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
