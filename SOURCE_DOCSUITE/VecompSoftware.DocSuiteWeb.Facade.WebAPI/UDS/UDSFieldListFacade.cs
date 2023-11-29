using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.UDS;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuiteWeb.Facade.WebAPI.UDS
{
    public class UDSFieldListFacade : FacadeWebAPIBase<UDSFieldList, UDSFieldListDao>
    {
        #region [ Fields ]
        #endregion [ Fields ]

        #region [ Properties ]
        
        #endregion [ Properties ]

        #region [ Constructor ]
        public UDSFieldListFacade(ICollection<TenantModel> model, Tenant currentTenant)
            :base(model.Select(s => new WebAPITenantConfiguration<UDSFieldList, UDSFieldListDao>(s)).ToList(), currentTenant)
        {

        }
        #endregion [ Constructor ]

        #region [ Methods ]
        #endregion
    }
}
