using System;
using System.Collections.Generic;

namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Models.Documents
{
    public class SearchDocumentsRequestModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IDictionary<string, string> DynamicFilters { get; set; }
        public Guid IdArchive { get; set; }
        public int? Skip { get; set; }
        public int? Top { get; set; }
    }
}