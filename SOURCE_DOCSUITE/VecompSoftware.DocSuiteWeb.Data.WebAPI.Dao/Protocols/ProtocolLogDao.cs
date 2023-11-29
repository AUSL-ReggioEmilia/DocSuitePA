using VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.WebAPIManager.Dao;
using ProtocolEntities = VecompSoftware.DocSuiteWeb.Entity.Protocols;

namespace VecompSoftware.DocSuiteWeb.Data.WebAPI.Dao.Protocols
{
    public class ProtocolLogDao : BaseWebAPIDao<ProtocolEntities.ProtocolLog, ProtocolEntities.ProtocolLog, ProtocolLogFinder>
    {
        #region [ Constructor ]

        public ProtocolLogDao(TenantModel tenant)
            : base(tenant.WebApiClientConfig, tenant.OriginalConfiguration, new ProtocolLogFinder(tenant))
        {
        }
        #endregion
    }
}
