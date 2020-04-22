using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenants;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Tenants
{
    public class TenantConfigurationController : BaseWebApiController<TenantConfiguration, ITenantConfigurationService>
    {
        #region [ Fields ]
        #endregion

        #region [ Constructor ]

        public TenantConfigurationController(ITenantConfigurationService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]
        #endregion

    }
}