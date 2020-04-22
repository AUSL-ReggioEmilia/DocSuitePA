using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.UDS
{
    public class UDSMessagesController : BaseODataController<UDSMessage, IUDSMessageService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public UDSMessagesController(IUDSMessageService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region [ Methods ]

        #endregion
    }
}