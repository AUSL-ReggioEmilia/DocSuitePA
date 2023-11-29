using System;

namespace VecompSoftware.DocSuiteWeb.DTO.UDS
{
    public class UDSRepositoryDetailsSearchModel
    {
        public double? Year { get; set; }
        public double? Number { get; set; }
        public DateTimeOffset? DateFrom { get; set; }
        public DateTimeOffset? DateTo { get; set; }
        public string Subject { get; set; }
        public int CategoryId { get; set; }
        public string IsCancelledArchive { get; set; }
        public string DocumentName { get; set; }
        public string GenericDocument { get; set; }
    }
}
