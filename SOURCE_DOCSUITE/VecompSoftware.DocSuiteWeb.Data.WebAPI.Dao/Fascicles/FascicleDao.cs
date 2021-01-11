using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using FascicleEntities = VecompSoftware.DocSuiteWeb.Entity.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Fascicles
{
    public class FascicleDao : BaseWebAPIDao<FascicleEntities.Fascicle, FascicleModel, FascicleFinder>
    {
        #region [ Constructor ]

        public FascicleDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new FascicleFinder(tenant))
        { }

        #endregion

        #region [ Methods ]


        #endregion
    }
}
