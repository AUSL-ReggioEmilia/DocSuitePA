using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.UDS;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.UDS
{
    public class UDSPECMailsController : BaseODataController<UDSPECMail, IUDSPECMailService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        private readonly IUDSPECMailService _service;
        #endregion

        #region [ Constructor ]
        public UDSPECMailsController(IUDSPECMailService service, IDataUnitOfWork unitOfWork, ILogger logger,
            IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _service = service;
            _mapperUnitOfwork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}