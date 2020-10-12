using System;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities.ERP
{
    public class Command
    {
        public decimal RequestId { get; set; }
        public decimal? FascicleId { get; set; }
        public decimal? ContractId { get; set; }
        public decimal? VendorId { get; set; }
        public decimal? ProtId { get; set; }
        public decimal? ODAId { get; set; }
        public decimal? FascicleYear { get; set; }
        public string Action { get; set; }
        public string DocumentType { get; set; }
        public string Owner { get; set; }
        public string ODA { get; set; }
        public string Contract { get; set; }
        public string Contact { get; set; }
        public string ContactPEC { get; set; }
        public string CIG { get; set; }
        public DateTime InsertTime { get; set; }
        public DateTime? ProcessedTime { get; set; }
    }
}
