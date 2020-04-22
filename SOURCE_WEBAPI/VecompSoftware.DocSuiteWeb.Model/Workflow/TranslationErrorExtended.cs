using System;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow
{
    public class TranslationErrorExtended
    {
        public TranslationErrorExtended(TranslationError te)
        {
            ActivityId = te.ActivityId;
            EndColumn = te.EndColumn;
            EndLine = te.EndColumn;
            ExpressionText = te.ExpressionText;
            Message = te.Message;
            StartColumn = te.StartColumn;
            StartLine = te.StartLine;
        }

        public TranslationErrorExtended(Exception ex)
        {
            Message = string.Concat(ex.Message, Environment.NewLine, ex.InnerException);
        }

        public string Message { get; set; }
        public string ActivityId { get; set; }
        public int EndColumn { get; set; }
        public int EndLine { get; set; }
        public string ExpressionText { get; set; }
        public int StartColumn { get; set; }
        public int StartLine { get; set; }
    }
}
