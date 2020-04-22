using System;

namespace VecompSoftware.BPM.Integrations.Modules.ENAV.ERP.Data.Entities
{
    public class PayableInvoice
    {
        public PayableInvoice()
        {
            WorkflowId = null;
        }

        public decimal RequestId { get; set; }
        public decimal TenantId { get; set; }
        public string Customer { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SectionalVAT { get; set; }
        public DateTime? DateVAT { get; set; }
        public decimal? ProtocolNumberVAT { get; set; }
        public decimal? YearVAT { get; set; }
        public string InvoiceFilename { get; set; }
        public string Invoice { get; set; }
        public DateTime? WorkflowStarted { get; set; }
        public string WorkflowId { get; set; }
        public decimal? WorkflowStatus { get; set; }
        public string SDIIdentification { get; set; }
        public string SDIResult { get; set; }
        public DateTime? SDIDate { get; set; }
        public string SDIResultDescription { get; set; }
        public decimal? DocSuiteProtocolYear { get; set; }
        public string DocSuiteProtocolNumber { get; set; }
    }
}
