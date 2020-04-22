using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
{
    public class TenantFinder : BaseWebAPIFinder<Tenant, Tenant>
    {
        public TenantFinder(TenantModel tenant) : base(tenant)
        {
        }

        public TenantFinder(IReadOnlyCollection<TenantModel> tenants) : base(tenants)
        {
        }

        #region [ Properties ] 
        public bool IncludeContainers { get; set; }
        #endregion

        #region [ Method ] 
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            if (IncludeContainers)
            {
                odataQuery = odataQuery.Expand("Containers");
            }
            return base.DecorateFinder(odataQuery);
        }

        public override void ResetDecoration()
        {
            IncludeContainers = false;   
        }
        #endregion
    }
}
