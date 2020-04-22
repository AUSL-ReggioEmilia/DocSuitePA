using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Protocols
{
    public class ProtocolLogController : BaseWebApiController<ProtocolLog, IProtocolLogService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]

        public ProtocolLogController(IProtocolLogService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion

    }
}