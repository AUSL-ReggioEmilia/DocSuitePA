using System;

namespace VecompSoftware.BPM.Integrations.Modules.TEAFLEX.ArchiveInvoice.Models
{
    public class ModuleConfigurationModel
    {
        public string WorkflowIntegrationTopic { get; set; }
        public string WorkflowStartArchiveInvoiceSubscription { get; set; }
        public string ReceivableInvoiceUDSName { get; set; }
        public string FolderLookingAttachmentInvoice { get; set; }
        public string FolderWorkingAttachmentInvoice { get; set; }
        public string FolderBackupAttachmentInvoice { get; set; }
        public string FolderRejectedAttachmentInvoice { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
    }
}
