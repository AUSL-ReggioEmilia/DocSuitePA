using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Service.Entity.Tenants;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Tenants
{
    public class TenantWorkflowRepositoryController : BaseWebApiController<TenantWorkflowRepository, ITenantWorkflowRepositoryService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public TenantWorkflowRepositoryController(ITenantWorkflowRepositoryService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}