namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Invoice.Models
{
    public class InvoiceFilePersistance
    {
        public string UDSRepositoryName { get; set; }
        public string FolderReceivedInvoice { get; set; }
        public string FolderMetadataInvoiceLooking { get; set; }
        public string FolderMetadataInvoiceWorking { get; set; }
        public string FolderMetadataInvoiceError { get; set; }
        public string FolderMetadataInvoiceBackup { get; set; }
    }
}
