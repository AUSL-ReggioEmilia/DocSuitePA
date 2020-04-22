using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using APICommon = VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Commons
{
    public class RoleUserDao : BaseWebAPIDao<APICommon.RoleUser, APICommon.RoleUser, RoleUserFinder>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]
        public RoleUserDao(TenantModel tenant)
            :base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new RoleUserFinder(tenant))
        {
        }
        #endregion

        #region [ Methods ]
        #endregion
    }
}
