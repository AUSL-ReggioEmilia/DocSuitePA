using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Workflows;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Finder;
using VecompSoftware.WebAPIManager;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Workflows
{
    public class WorkflowRoleMappingFinder : BaseWebAPIFinder<WorkflowRoleMapping, WorkflowRoleMapping>
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        public bool ExpandRole { get; set; }
        #endregion

        #region [ Constructor ]
        public WorkflowRoleMappingFinder(TenantModel tenant)
           : this(new List<TenantModel>() { tenant })
        {
        }

        public WorkflowRoleMappingFinder(IReadOnlyCollection<TenantModel> tenants)
            : base(tenants)
        {
        }
        #endregion

        #region [ Methods ]
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (ExpandRole)
            {
                odataQuery = odataQuery.Expand("Role");
            }
            return base.DecorateFinder(odataQuery);
        }
        public override void ResetDecoration()
        {
            UniqueId = null;
            ExpandRole = false;
        }
        #endregion
    }
}
