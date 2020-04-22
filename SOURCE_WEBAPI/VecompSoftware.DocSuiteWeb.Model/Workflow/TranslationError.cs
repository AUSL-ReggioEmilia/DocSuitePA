namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class TranslationError
    {
        #region [ Properties ]
        public string Message { get; set; }
        public string ActivityId { get; set; }
        public int EndColumn { get; set; }
        public int EndLine { get; set; }
        public string ExpressionText { get; set; }
        public int StartColumn { get; set; }
        public int StartLine { get; set; }
        #endregion

        #region [ Constructor ]
        public TranslationError() { }
        #endregion
    }
}