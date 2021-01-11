using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Service.Entity.Monitors;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Monitors
{
    public class TransparentAdministrationMonitorLogController : BaseWebApiController<TransparentAdministrationMonitorLog, ITransparentAdministrationMonitorLogService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public TransparentAdministrationMonitorLogController(ITransparentAdministrationMonitorLogService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {

        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}