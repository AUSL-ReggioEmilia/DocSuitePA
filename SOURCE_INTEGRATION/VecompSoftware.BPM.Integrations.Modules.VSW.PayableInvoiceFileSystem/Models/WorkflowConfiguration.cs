using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Models
{
    public class WorkflowConfiguration
    {
        public Guid TenantAOOId { get; set; }
        public string FolderLookingInvoice { get; set; }
        public string FolderRejectedInvoice { get; set; }
        public string FolderWorkingInvoice { get; set; }
        public string FolderBackupInvoice { get; set; }
        public string FolderLookingMetadata { get; set; }
        public string FolderRejectedMetadata { get; set; }
        public string FolderWorkingMetadata { get; set; }
        public string FolderBackupMetadata { get; set; }
        public Dictionary<XMLModelKind, InvoiceConfiguration> InvoiceTypes { get; set; }
    }
}
