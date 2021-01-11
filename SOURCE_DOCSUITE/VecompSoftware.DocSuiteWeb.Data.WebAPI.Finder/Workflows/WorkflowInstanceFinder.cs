using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using System;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowInstanceFinder : BaseWebAPIFinder<WorkflowInstance, WorkflowInstance>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public bool ExpandRepository { get; set; }
        public bool ExpandProperties { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowInstanceFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowInstanceFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandRepository)
            {
                odataQuery = odataQuery.Expand("WorkflowRepository");
            }

            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("WorkflowProperties");
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            UniqueId = null;
            ExpandRepository = false;
            ExpandProperties = false;
        }
        #endregion
    }
}

