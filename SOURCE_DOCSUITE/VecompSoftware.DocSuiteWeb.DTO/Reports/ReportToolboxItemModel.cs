using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Reports;

namespace VecompSoftware.DocSuiteWeb.DTO.Reports
{
    public class ReportToolboxItemModel
    {
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public ICollection<IReportItem> ReportItems { get; set; }
    }
}
