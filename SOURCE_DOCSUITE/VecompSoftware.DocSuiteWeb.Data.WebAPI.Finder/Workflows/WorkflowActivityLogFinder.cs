using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using System;
using System.Linq;
using System.Linq.Expressions;
using VecompSoftware.WebAPIManager;
namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowActivityLogFinder : BaseWebAPIFinder<WorkflowActivityLog, WorkflowActivityLog>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
 
        #endregion

        #region [ Constructor ]
        public WorkflowActivityLogFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowActivityLogFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

 

        public override void ResetDecoration()
        {
 
        }
 
    }
}
