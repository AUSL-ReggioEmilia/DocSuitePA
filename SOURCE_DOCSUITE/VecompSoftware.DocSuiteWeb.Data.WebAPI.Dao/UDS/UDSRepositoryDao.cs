using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Entities.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.UDS
{
    public class UDSRepositoryDao : BaseWebAPIDao<UDSRepository, UDSRepositoryModel, UDSRepositoryFinder>
    {
        #region [ Constructor ]

        public UDSRepositoryDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new UDSRepositoryFinder(tenant))
        { }

        #endregion

        #region [ Methods ]

        #endregion
    }
}
