using System;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Monitors;
using VecompSoftware.DocSuiteWeb.Finder.Monitors;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Model.Monitors;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Monitors;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Monitors
{
    public class TransparentAdministrationMonitorLogsController : BaseODataController<TransparentAdministrationMonitorLog, ITransparentAdministrationMonitorLogService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfwork;
        private readonly ITransparentAdministrationMonitorLogService _service;
        private readonly ITransparentAdministrationMonitorLogTableValuedModelMapper _mapperTableValue;
        #endregion

        #region [ Constructor ]
        public TransparentAdministrationMonitorLogsController(ITransparentAdministrationMonitorLogService service, IDataUnitOfWork unitOfWork, ILogger logger,
            ITransparentAdministrationMonitorLogTableValuedModelMapper mapperTableValue, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _service = service;
            _mapperUnitOfwork = mapperUnitOfWork;
            _mapperTableValue = mapperTableValue;
        }
        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetByDateInterval(string dateFrom, string dateTo, string userName, short? idContainer, int? environment)
        {
            DateTimeOffset dateFromOffset = DateTimeOffset.ParseExact(dateFrom, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            DateTimeOffset dateToOffset = DateTimeOffset.ParseExact(dateTo, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                IQueryable<TransparentAdministrationMonitorLog> transparentAdministrationMonitorResults = _unitOfWork.Repository<TransparentAdministrationMonitorLog>()
                .GetByDateInterval(dateFromOffset, dateToOffset, userName, idContainer, environment);
                IQueryable<TransparentAdministrationMonitorLogModel> results = _mapperUnitOfwork.Repository<IDomainMapper<TransparentAdministrationMonitorLog, TransparentAdministrationMonitorLogModel>>().MapCollection(transparentAdministrationMonitorResults).AsQueryable();
                return Ok(results);
            }, _logger, LogCategories);
        }
        #endregion
    }
}