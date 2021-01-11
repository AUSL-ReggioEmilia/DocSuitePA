using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.EntityMapper.WebAPI;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Tenants
{
    public class TenantFacade : FacadeWebAPIBase<Tenant, TenantDao>
    {
        #region [ Fields ]
        private readonly WebAPIDtoMapper<Tenant> _mapper;
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public TenantFacade(ICollection<TenantModel> model, Tenant currentTenant)
            :base(model.Select(s=>new WebAPITenantConfiguration<Tenant, TenantDao>(s)).ToList(), currentTenant)
        {
            this._mapper = new WebAPIDtoMapper<Tenant>();
        }
        #endregion
    }
}
