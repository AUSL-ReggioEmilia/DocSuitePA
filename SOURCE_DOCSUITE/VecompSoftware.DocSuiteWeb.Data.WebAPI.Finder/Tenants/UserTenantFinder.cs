using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Entities.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager;
using VecompSoftware.WebAPIManager.Finder;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants
{
    public class UserTenantFinder : BaseWebAPIFinder<Tenant, TenantTableValuedModel>
    {
        public UserTenantFinder(TenantModel tenant) : base(tenant)
        {
        }

        public UserTenantFinder(IReadOnlyCollection<TenantModel> tenants) : base(tenants)
        {
        }

        #region [ Properties ] 

        public string Username { get; set; }
        public string Domain { get; set; }

        #endregion

        #region [ Method ] 
        public override IODATAQueryManager DecorateFinder(IODATAQueryManager odataQuery)
        {
            string FX_GetUserTenants = CommonDefinition.OData.TenantService.FX_GetUserTenants;
            odataQuery = odataQuery.Function(FX_GetUserTenants);
            return odataQuery;
        }

        public override void ResetDecoration()
        {
            Username = null;
            Domain = null;
        }

        #endregion
    }
}
