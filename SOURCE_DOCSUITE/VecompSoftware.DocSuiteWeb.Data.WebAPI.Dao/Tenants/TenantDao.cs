using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using APITenants = VecompSoftware.DocSuiteWeb.Entity.Tenants;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Tenants
{
    public class TenantDao : BaseWebAPIDao<APITenants.Tenant, APITenants.Tenant, TenantFinder>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public TenantDao(TenantModel tenant)
            :base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new TenantFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
