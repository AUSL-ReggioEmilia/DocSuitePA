namespace VecompSoftware.DocSuiteWeb.Model.Reports
{
    public class ReportParameterModel
    {

        #region [ Properties ]

        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Mergeable { get; set; }
        public string Text { get; set; }
        public bool Visible { get; set; }
        public bool MultiValue { get; set; }
        public bool AllowNull { get; set; }
        public bool AllowBlank { get; set; }
        public bool AutoRefresh { get; set; }

        #endregion
    }
}
