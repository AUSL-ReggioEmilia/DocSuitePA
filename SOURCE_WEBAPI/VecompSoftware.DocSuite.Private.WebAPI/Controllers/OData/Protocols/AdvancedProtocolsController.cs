using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Protocols
{
    public class AdvancedProtocolsController : BaseODataController<AdvancedProtocol, IAdvancedProtocolService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public AdvancedProtocolsController(ILogger logger, IAdvancedProtocolService service, IDataUnitOfWork unitOfWork, ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

    }
}