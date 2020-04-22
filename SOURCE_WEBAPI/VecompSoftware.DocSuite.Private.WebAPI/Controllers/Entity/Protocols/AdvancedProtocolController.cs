using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Entity.Protocols
{
    public class AdvancedProtocolController : BaseWebApiController<AdvancedProtocol, IAdvancedProtocolService>
    {
        #region [ Fields ]

        #endregion

        #region [ Constructor ]
        public AdvancedProtocolController(IAdvancedProtocolService service, IDataUnitOfWork unitOfWork, ILogger logger)
            : base(service, unitOfWork, logger)
        {
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}