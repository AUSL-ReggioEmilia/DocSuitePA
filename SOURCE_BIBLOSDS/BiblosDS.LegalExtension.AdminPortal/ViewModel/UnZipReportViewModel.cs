using System;

namespace BiblosDS.LegalExtension.AdminPortal.ViewModel
{
    public class UnZipReportViewModel
    {
        public const string TYPE_INFORMATION = "Info";
        public const string TYPE_ERROR = "Error";
        public const string TYPE_WARN = "Warn";
        public const string TYPE_SUCCESS = "Success";

        public string Description { get; set; }
        public string LogType { get; set; }
        public Guid? ReferenceId { get; set; }
    }
}