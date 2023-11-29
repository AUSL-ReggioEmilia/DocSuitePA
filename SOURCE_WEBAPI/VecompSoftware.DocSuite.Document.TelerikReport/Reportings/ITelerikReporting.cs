using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.Reports;

namespace VecompSoftware.DocSuite.Document.TelerikReport.Reportings
{
    public interface ITelerikReporting
    {
        Task<ICollection<ReportCategoryModel>> GetCategories();
        Task<ICollection<ReportModel>> GetReports(string documentId);
        Task<ICollection<ReportParameterModel>> GetReportParameters(string reportId);
        Task<ICollection<ReportOutputFormatModel>> GetFormats();
        Task<byte[]> GenerateReport(ReportBuildModel data);
    }
}
