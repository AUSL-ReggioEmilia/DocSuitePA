using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.UDS
{
    public class UDSLogDao :BaseWebAPIDao<UDSLog, UDSLog, UDSLogFinder>
    {
        #region [ Constructor ]

        public UDSLogDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new UDSLogFinder(tenant))
        {
        }
        #endregion
    }
}
