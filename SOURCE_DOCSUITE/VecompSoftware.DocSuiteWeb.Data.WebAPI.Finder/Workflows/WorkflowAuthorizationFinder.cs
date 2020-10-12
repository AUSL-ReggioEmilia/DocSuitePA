using System;
using System.Collections.Generic;
using System.Text;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowAuthorizationFinder : BaseWebAPIFinder<WorkflowAuthorization, WorkflowAuthorization>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public string Account { get; set; }
        public bool? IsHandler { get; set; }
        public Guid? WorkflowActivityId { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowAuthorizationFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowAuthorizationFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (WorkflowActivityId.HasValue)
            {
                odataQuery = odataQuery.Filter($"WorkflowActivity/UniqueId eq {WorkflowActivityId.Value}");
            }
            if (IsHandler.HasValue)
            {
                odataQuery = odataQuery.Filter($"IsHandler eq {IsHandler.ToString().ToLower()}");
            }
            if (!string.IsNullOrEmpty(Account))
            {
                odataQuery = odataQuery.Filter($"Account eq '{Account}'");
            }
            
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            Account = null;
            WorkflowActivityId = null;
        }
        #endregion
    }
}
