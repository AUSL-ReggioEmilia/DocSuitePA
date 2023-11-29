using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.Helpers.Workflow;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowInstanceFinder : BaseWebAPIFinder<WorkflowInstance, WorkflowInstance>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public bool ExpandRepository { get; set; }
        public bool ExpandProperties { get; set; }
        public Guid? InstanceId { get; set; }
        public int? CollaborationId { get; set; }
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

            if (CollaborationId.HasValue)
            {
                odataQuery = odataQuery.Filter($"WorkflowActivities/any(wa:wa/WorkflowProperties/any(wp:wp/ValueInt eq {CollaborationId.Value} and wp/Name eq '{WorkflowPropertyHelper.DSW_FIELD_COLLABORATION_ID}'))");
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

