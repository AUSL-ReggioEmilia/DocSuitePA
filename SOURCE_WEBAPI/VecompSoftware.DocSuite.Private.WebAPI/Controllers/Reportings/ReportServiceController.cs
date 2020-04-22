using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using VecompSoftware.DocSuite.Document.TelerikReport.Reportings;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Reports;
using CommonHelpers = VecompSoftware.DocSuite.WebAPI.Common.Helpers;

namespace VecompSoftware.DocSuite.Private.WebAPI.Controllers.Reportings
{
    [LogCategory(LogCategoryDefinition.WEBAPISERVICEBUS)]
    public class ReportServiceController : ApiController
    {
        #region [ Fields ]
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ILogger _logger;
        private readonly ITelerikReporting _telerikReporting;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ReportServiceController));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]

        public ReportServiceController(ILogger logger, ITelerikReporting telerikReporting)
        {
            _logger = logger;
            _telerikReporting = telerikReporting;
        }

        #endregion

        #region [ Methods ]

        [HttpPost]
        public async Task<IHttpActionResult> PostAsync([FromBody]ReportBuildModel reportBuildModel)
        {
            return await CommonHelpers.ActionHelper.TryCatchWithLoggerAsync(
                async () =>
                {
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
                    byte[] generateReport = await _telerikReporting.GenerateReport(reportBuildModel);
                    httpResponseMessage.Content = new ByteArrayContent(generateReport);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = $"report{reportBuildModel.ReportParameterValueModel.ReportId}.{reportBuildModel.ReportParameterValueModel.Format}"
                    };
                    return ResponseMessage(httpResponseMessage);
                }, BadRequest, Content, InternalServerError, _logger, LogCategories);
        }


        #endregion
    }
}