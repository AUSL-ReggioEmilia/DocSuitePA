using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowPropertyFinder : BaseWebAPIFinder<WorkflowProperty, WorkflowProperty>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public Guid? WorkflowActivityId { get; set; }

        public Guid? WorkflowInstanceId { get; set; }

        public string Name { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowPropertyFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowPropertyFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (WorkflowActivityId.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("WorkflowActivity/UniqueId eq {0}",WorkflowActivityId.Value));
            }

            if (WorkflowInstanceId.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("WorkflowInstance/UniqueId eq {0}", WorkflowInstanceId.Value));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                odataQuery = odataQuery.Filter(string.Format("contains(Name, '{0}')", Name));
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            UniqueId = null;
            WorkflowInstanceId = null;
            WorkflowActivityId = null;
            Name = string.Empty;
        }
        #endregion
    }
}
