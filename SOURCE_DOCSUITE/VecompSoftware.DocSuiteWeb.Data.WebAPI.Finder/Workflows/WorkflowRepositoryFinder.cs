using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowRepositoryFinder : BaseWebAPIFinder<WorkflowRepository, WorkflowRepository>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public bool? OnlyUserWorkflow { get; set; }

        #endregion

        #region [ Constructor ]
        public WorkflowRepositoryFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowRepositoryFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (OnlyUserWorkflow.HasValue && OnlyUserWorkflow.Value)
            {
                odataQuery = odataQuery.Filter($"DSWEnvironment ne 'Any'").Sorting("Name");
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {

        }
        #endregion
    }
}
