using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Workflows
{
    public class WorkflowInstanceFacade : FacadeWebAPIBase<WorkflowInstance, WorkflowInstanceDao>
    {
        #region [ Fields ]        
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WorkflowInstanceFacade(ICollection<TenantModel> model)
            : base(model.Select(s => new WebAPITenantConfiguration<WorkflowInstance, WorkflowInstanceDao>(s)).ToList())
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}

