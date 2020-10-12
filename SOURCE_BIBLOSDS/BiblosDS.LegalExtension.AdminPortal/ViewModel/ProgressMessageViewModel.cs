using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel
{
    public class ProgressMessageViewModel
    {
        public string TaskReferenceId { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public ProgressMessageLevel Level { get; set; }
        public bool HasReport { get; set; }
    }
}