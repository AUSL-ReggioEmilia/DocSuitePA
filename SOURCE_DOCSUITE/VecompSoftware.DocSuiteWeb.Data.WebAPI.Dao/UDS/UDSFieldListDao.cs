using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.UDS
{
    public class UDSFieldListDao : BaseWebAPIDao<UDSFieldList, UDSFieldList, UDSFieldListFinder>
    {
        #region [ Constructor ]

        public UDSFieldListDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new UDSFieldListFinder(tenant))
        { }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
