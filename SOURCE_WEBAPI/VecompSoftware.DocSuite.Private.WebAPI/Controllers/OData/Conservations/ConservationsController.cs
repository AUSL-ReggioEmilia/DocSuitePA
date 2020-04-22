using Microsoft.AspNet.OData.Query;
using System.Collections.Generic;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Finder.Conservations;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Conservations;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Conservations
{
    public class ConservationsController : BaseODataController<Conservation, IConservationService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]

        public ConservationsController(ILogger logger, IConservationService service, IDataUnitOfWork unitOfWork, ISecurity security, IMapperUnitOfWork mapperUnitOfWork)
            : base(service, unitOfWork, logger, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetAvailableProtocolLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailableProtocolLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailableProtocolLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailableProtocolLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableDocumentSeriesItemLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailableDocumentSeriesItemLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailableDocumentSeriesItemLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailableDocumentSeriesItemLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableUDSLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailableUDSLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailableUDSLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailableUDSLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableDossierLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailableDossierLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailableDossierLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailableDossierLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableFascicleLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailableFascicleLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailableFascicleLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailableFascicleLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailablePECMailLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailablePECMailLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailablePECMailLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailablePECMailLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetAvailableTableLogs(ODataQueryOptions<Conservation> options, int skip, int top)
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                ICollection<ConservationLogModel> logResults = _unitOfWork.Repository<Conservation>().AvailableTableLogs(skip, top);
                return Ok(logResults);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult CountAvailableTableLogs()
        {
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric<IHttpActionResult>(() =>
            {
                int logs = _unitOfWork.Repository<Conservation>().CountAvailableTableLogs();
                return Ok(logs);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
