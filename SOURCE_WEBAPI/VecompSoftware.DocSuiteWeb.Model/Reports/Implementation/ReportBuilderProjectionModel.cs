using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Reports
{
    public class ReportBuilderProjectionModel
    {
        #region [ Constructor ]
        public ReportBuilderProjectionModel()
        {
            ReportProperties = new List<ReportBuilderPropertyModel>();
        }
        #endregion

        #region [ Properties ]
        public ICollection<ReportBuilderPropertyModel> ReportProperties { get; set; }
        public ReportBuilderProjectionType ProjectionType { get; set; }
        public string Alias { get; set; }
        public string TagName { get; set; }
        #endregion
    }
}
