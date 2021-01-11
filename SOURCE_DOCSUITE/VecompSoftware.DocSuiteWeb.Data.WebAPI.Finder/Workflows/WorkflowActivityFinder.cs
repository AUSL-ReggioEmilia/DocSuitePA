using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowActivityFinder : BaseWebAPIFinder<WorkflowActivity, WorkflowActivity>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public Guid? WorkflowInstanceId { get; set; }
        public Guid? DocumentUnitReferenced { get; set; }
        public ICollection<WorkflowStatus> Statuses { get; set; }
        public WorkflowActivityType? ActivityType { get; set; }
        public string Name { get; set; }
        public bool? WorkflowInstanceInProgress { get; set; }
        public bool? IsAuthorized { get; set; }
        public string Account;
        public bool? Top1 { get; set; }
        public bool ExpandProperties { get; set; }
        public bool ExpandRepository { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowActivityFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowActivityFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
            Statuses = new List<WorkflowStatus>();
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (WorkflowInstanceId.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("WorkflowInstance/UniqueId eq {0}", WorkflowInstanceId.Value));
            }

            if (WorkflowInstanceInProgress.HasValue && WorkflowInstanceInProgress.Value)
            {
                odataQuery = odataQuery.Filter("WorkflowInstance/Status eq 'Progress'");
            }

            if (IsAuthorized.HasValue && IsAuthorized.Value && !string.IsNullOrEmpty(Account))
            {
                   odataQuery = odataQuery.Filter($"WorkflowAuthorizations/any(wa:wa/Account eq '{Account}')");
            }

            if (DocumentUnitReferenced.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("DocumentUnitReferenced/UniqueId eq {0}", DocumentUnitReferenced.Value));
            }

            if (ActivityType.HasValue)
            {
                odataQuery = odataQuery.Filter(string.Format("ActivityType eq VecompSoftware.DocSuiteWeb.Entity.Workflows.WorkflowActivityType'{0}'", (int)ActivityType.Value));
            }

            if (ExpandProperties)
            {
                odataQuery = odataQuery.Expand("WorkflowProperties");
            }

            if (ExpandRepository)
            {
                odataQuery = odataQuery.Expand("WorkflowInstance($expand=WorkflowRepository)");
            }

            if (Statuses.Count > 0)
            {
                ICollection<string> expressions = Statuses.Select(s => string.Format("Status eq VecompSoftware.DocSuiteWeb.Entity.Workflows.WorkflowStatus'{0}'", (int)s)).ToList();
                string odataExpression = $"({string.Join(" or ", expressions)})";
                odataQuery = odataQuery.Filter(odataExpression);
            }
            
            if (Top1.HasValue && Top1.Value)
            {
                EnableTopOdata = false;
                odataQuery.Top(1);
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            UniqueId = null;
            WorkflowInstanceId = null;
            DocumentUnitReferenced = null;
            Name = string.Empty;
            ActivityType = null;
            Statuses = new List<WorkflowStatus>();
            WorkflowInstanceInProgress = null;
            Top1 = null;
            ExpandProperties = false;
            ExpandRepository = false;
            EnableTopOdata = true;
        }
        #endregion
    }
}
