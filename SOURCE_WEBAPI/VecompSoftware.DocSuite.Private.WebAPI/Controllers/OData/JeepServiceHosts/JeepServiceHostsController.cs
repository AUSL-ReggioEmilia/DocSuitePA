using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.JeepServiceHosts;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.JeepServiceHosts;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.JeepServiceHosts
{
    public class JeepServiceHostsController : BaseODataController<JeepServiceHost, IJeepServiceHostService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private readonly IJeepServiceHostService _service;
        #endregion

        #region [ Constructor ]
        public JeepServiceHostsController(IJeepServiceHostService service, IDataUnitOfWork unitOfWork, ILogger logger,
            IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _service = service;
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        #endregion

        #region [ Methods ]

        #endregion
    }
}