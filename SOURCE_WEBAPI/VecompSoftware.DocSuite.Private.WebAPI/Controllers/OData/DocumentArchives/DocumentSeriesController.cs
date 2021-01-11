using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Model.Entities.Monitors;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.DocumentArchives
{
    public class DocumentSeriesController : BaseODataController<DocumentSeries, IDocumentSeriesService>
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public DocumentSeriesController(IDocumentSeriesService service, IDataUnitOfWork unitOfWork, ILogger logger, ISecurity security)
            : base(service, unitOfWork, logger, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        [HttpGet]
        public IHttpActionResult GetMonitoringSeriesBySection(string dateFrom, string dateTo)
        {
            var dateFromFormat = DateTimeOffset.ParseExact(dateFrom, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            var dateToFormat = DateTimeOffset.ParseExact(dateTo, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<MonitoringSeriesSectionModel> monitoringSeriesSectionResult = _unitOfWork.Repository<DocumentSeries>().GetMonitoringSeriesBySection(dateFromFormat, dateToFormat);
                return Ok(monitoringSeriesSectionResult);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMonitoringQualitySummary(string dateFrom, string dateTo, int idDocumentSeries = 0)
        {
            var dateFromFormat = DateTimeOffset.ParseExact(dateFrom, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            var dateToFormat = DateTimeOffset.ParseExact(dateTo, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<MonitoringQualitySummaryModel> monitoringQualitySummaryResult = _unitOfWork.Repository<DocumentSeries>().GetMonitoringQualitySummary(dateFromFormat, dateToFormat, idDocumentSeries);
                return Ok(monitoringQualitySummaryResult);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMonitoringQualityDetails(int idDocumentSeries, int? idRole, string dateFrom, string dateTo)
        {
            var dateFromFormat = DateTimeOffset.ParseExact(dateFrom, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            var dateToFormat = DateTimeOffset.ParseExact(dateTo, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<MonitoringQualityDetailsModel> monitoringQualityDetailsResult = _unitOfWork.Repository<DocumentSeries>().GetMonitoringQualityDetails(idDocumentSeries, idRole, dateFromFormat, dateToFormat);
                return Ok(monitoringQualityDetailsResult);
            }, _logger, LogCategories);
        }

        [HttpGet]
        public IHttpActionResult GetMonitoringSeriesByRole(string dateFrom, string dateTo)
        {
            var dateFromFormat = DateTimeOffset.ParseExact(dateFrom, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            var dateToFormat = DateTimeOffset.ParseExact(dateTo, "yyyyMMddhhmmss", CultureInfo.InvariantCulture);
            return CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(() =>
            {
                ICollection<MonitoringSeriesRoleModel> monitoringSeriesRoleResult = _unitOfWork.Repository<DocumentSeries>().GetMonitoringSeriesByRole(dateFromFormat, dateToFormat);
                return Ok(monitoringSeriesRoleResult);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
