using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using FascicleEntities = VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles
{
    public class FascicleLogDao : BaseWebAPIDao<FascicleEntities.FascicleLog, FascicleEntities.FascicleLog, FascicleLogFinder>
    {
        #region [ Constructor ]

        public FascicleLogDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new FascicleLogFinder(tenant))
        {
        }
        #endregion
    }
}
