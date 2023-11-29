using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using WorkflowEntities = VecompSoftware.DocSuiteWeb.Entity.Workflows;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Workflows
{
    public class WorkflowPropertyDao : BaseWebAPIDao<WorkflowEntities.WorkflowProperty, WorkflowEntities.WorkflowProperty, WorkflowPropertyFinder>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public WorkflowPropertyDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new WorkflowPropertyFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]
        public WorkflowEntities.WorkflowProperty FindPropertyByActivityIdAndName(Guid idWorkflowActivity, string propertyName)
        {
            Finder.ResetDecoration();
            Finder.WorkflowActivityId = idWorkflowActivity;
            Finder.Name = propertyName;
            return Finder.DoSearch().Select(s => s.Entity).SingleOrDefault();
        }
        #endregion
    }
}
