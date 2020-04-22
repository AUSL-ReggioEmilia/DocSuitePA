using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuite.Document.TelerikReport.Clients;
using VecompSoftware.DocSuite.Document.TelerikReport.Verbs;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Reports;

namespace VecompSoftware.DocSuite.Document.TelerikReport.Reportings
{
    [LogCategory(LogCategoryDefinition.DOCUMENTCONTEX)]
    public class TelerikReporting : ITelerikReporting
    {
        #region [ Fields ] 

        private readonly ITelerikReportClient _report;
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;

        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(TelerikReporting));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public TelerikReporting(ITelerikReportClient report, ILogger logger)
        {
            _report = report;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        public async Task<ICollection<ReportCategoryModel>> GetCategories()
        {
            return await ReportHelper.TryCatchWithLogger(async () =>
            {
                return await _report.GetAsync<ReportCategoryModel>(GetAction.Categories, string.Empty);
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ReportModel>> GetReports(string documentId)
        {
            return await ReportHelper.TryCatchWithLogger(async () =>
            {
                return await _report.GetAsync<ReportModel>(GetAction.Reports, documentId);
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ReportParameterModel>> GetReportParameters(string reportId)
        {
            return await ReportHelper.TryCatchWithLogger(async () =>
            {
                return await _report.GetAsync<ReportParameterModel>(GetAction.Parameters, reportId);
            }, _logger, LogCategories);
        }

        public async Task<ICollection<ReportOutputFormatModel>> GetFormats()
        {
            return await ReportHelper.TryCatchWithLogger(async () =>
            {
                return await _report.GetAsync<ReportOutputFormatModel>(GetAction.Formats, string.Empty);
            }, _logger, LogCategories);
        }

        public async Task<byte[]> GenerateReport(ReportBuildModel reportBuildModel)
        {
            return await ReportHelper.TryCatchWithLogger(async () =>
            {
                return await _report.PostAsync(PostAction.GenerateDocument, reportBuildModel.ReportParameterValueModel);
            }, _logger, LogCategories);
        }
        #endregion

    }
}
