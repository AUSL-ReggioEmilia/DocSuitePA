namespace VecompSoftware.DocSuiteWeb.Model.Reports
{
    public class ReportBuilderSortModel : IReportItem
    {
        #region [ Constructor ]

        #endregion

        #region [ Properties ]
        public ReportBuilderPropertyModel ReportProperty { get; set; }
        public ReportSortType Direction { get; set; }
        public int SortPriorityIndex { get; set; }
        #endregion
    }
}
