using Microsoft.AspNet.OData;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document.TelerikReport.Reportings;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Reports;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;


namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.OData.Reportings
{
    [LogCategory(LogCategoryDefinition.ODATAAPI)]
    [EnableQuery]
    public class ReportServicesController : ODataController
    {
        #region [ Fields ]

        private readonly ITelerikReporting _telerikReporting;
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories = null;

        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ReportServicesController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructors ]
        public ReportServicesController(ITelerikReporting telerikReporting, ILogger logger)
        {
            _telerikReporting = telerikReporting;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        [HttpGet]
        public async Task<IHttpActionResult> GetCategories()
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
                {
                    ICollection<ReportCategoryModel> model = await _telerikReporting.GetCategories();
                    return Ok(model);
                }, _logger, LogCategories);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetReports(string documentId)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
                {
                    ICollection<ReportModel> model = await _telerikReporting.GetReports(documentId);
                    return Ok(model);
                }, _logger, LogCategories);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetParameters(string reportId)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
                {
                    ICollection<ReportParameterModel> model = await _telerikReporting.GetReportParameters(reportId);
                    return Ok(model);
                }, _logger, LogCategories);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetOutputTypes()
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerGeneric(async () =>
                 {
                     ICollection<ReportOutputFormatModel> model = await _telerikReporting.GetFormats();
                     return Ok(model);
                 }, _logger, LogCategories);
        }

        #endregion
    }
}