using System;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Data.Entities
{
    public class PayableInvoice
    {
        public PayableInvoice()
        {
            WorkflowId = null;
        }

        public int RequestId { get; set; }
        public string Customer { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SectionalVAT { get; set; }
        public DateTime? DateVAT { get; set; }
        public int? ProtocolNumberVAT { get; set; }
        public short? YearVAT { get; set; }
        public string InvoiceFilename { get; set; }
        public string Invoice { get; set; }
        public DateTime? InsertDate { get; set; }
        public DateTime? WorkflowStarted { get; set; }
        public Guid? WorkflowId { get; set; }
        public WorkflowStatus? WorkflowStatus { get; set; }
        public string SDIIdentification { get; set; }
        public string SDIResult { get; set; }
        public DateTime? SDIDate { get; set; }
        public string SDIResultDescription { get; set; }
        public short? DocSuiteProtocolYear { get; set; }
        public string DocSuiteProtocolNumber { get; set; }
    }
}
