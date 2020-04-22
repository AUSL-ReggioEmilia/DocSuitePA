using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using System;
using System.Linq;
using System.Linq.Expressions;
using VecompSoftware.WebAPIManager;
using System.Text;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowAuthorizationFinder : BaseWebAPIFinder<WorkflowAuthorization, WorkflowAuthorization>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public  string Account;

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
            StringBuilder stringBuilder = new StringBuilder();
            if (WorkflowActivityId.HasValue)
            {
                stringBuilder.Append($"WorkflowActivity/UniqueId eq {WorkflowActivityId.Value}");
            }
            if (!string.IsNullOrEmpty(Account))
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(" and ");
                }
                stringBuilder.Append($"Account eq '{Account}'");
            }
            if (stringBuilder.Length > 0)
            {
                odataQuery = odataQuery.Filter(stringBuilder.ToString());
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
