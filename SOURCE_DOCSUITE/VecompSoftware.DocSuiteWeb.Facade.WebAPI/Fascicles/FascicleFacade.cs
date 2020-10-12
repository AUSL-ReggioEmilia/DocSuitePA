using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.Fascicles
{
    public class FascicleFacade : FacadeWebAPIBase<Fascicle, FascicleDao>
    {
        public FascicleFacade(ICollection<TenantModel> model, Tenant currentTenant)
            :base(model.Select(s => new WebAPITenantConfiguration<Fascicle, FascicleDao>(s)).ToList(), currentTenant)
        {
        }
    }
}
