using Microsoft.AspNet.OData;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenants;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Tenants
{
    [EnableQuery]
    public class TenantConfigurationsController : BaseODataController<TenantConfiguration, ITenantConfigurationService>
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public TenantConfigurationsController(ITenantConfigurationService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion
    }
}
