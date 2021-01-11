using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
{
    class WorkflowAuthorizationFacade : FacadeWebAPIBase<WorkflowAuthorization, WorkflowAuthorizationDao>
    {
        #region [ Fields ]        
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowAuthorizationFacade(ICollection<TenantModel> model, Tenant currentTenant)
            : base(model.Select(s => new WebAPITenantConfiguration<WorkflowAuthorization, WorkflowAuthorizationDao>(s)).ToList(), currentTenant)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
