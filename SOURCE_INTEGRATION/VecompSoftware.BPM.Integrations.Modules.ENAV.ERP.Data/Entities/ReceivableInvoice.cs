using System;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Data.Entities
{
    public class ReceivableInvoice
    {

        #region [ Fields ]
        public const string DOCSUITE_OWNER = "DocSuite";
        public const string ERP_OWNER = "ERP";

        #endregion
        public ReceivableInvoice()
        {
            RecordOwner = DOCSUITE_OWNER;
        }

        public decimal? ProtocolNumberVAT { get; set; }
        public string Supplier { get; set; }
        public string PIVACF { get; set; }
        public string InvoiceNumber { get; set; }
        public string CIG { get; set; }
        public string ODA { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string SectionalVAT { get; set; }
        public DateTime? DateVAT { get; set; }
        public decimal? YearVAT { get; set; }
        public string InvoiceFilename { get; set; }
        public string Invoice { get; set; }
        public string AutoInvoiceFilename { get; set; }
        public string AutoInvoice { get; set; }
        public DateTime? ERPUpdatedDate { get; set; }
        public DateTime? WorkflowProcessed { get; set; }
        public string WorkflowId { get; set; }
        public decimal? WorkflowStatus { get; set; }
        public string SDIIdentification { get; set; }
        public string SDIResult { get; set; }
        public DateTime? SDIDate { get; set; }
        public string SDIResultDescription { get; set; }
        public decimal? DocSuiteProtocolYear { get; set; }
        public string DocSuiteProtocolNumber { get; set; }
        public DateTime? DocSuiteProtocolDate { get; set; }
        public string RecordOwner { get; set; }
        public decimal TenantId { get; set; }
    }
}
