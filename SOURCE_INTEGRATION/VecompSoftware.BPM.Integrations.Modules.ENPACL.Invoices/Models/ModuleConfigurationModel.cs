using System;

namespace VecompSoftware.BPM.Integrations.Modules.ENPACL.Invoices.Models
{
    public class ModuleConfigurationModel
    {
        public string SFTPAddress { get; set; }
        public int SFTPPort { get; set; }
        public string SFTPUsername { get; set; }
        public string SFTPPassword { get; set; }
        public string SFTPPath { get; set; }
        public string FolderInvoiceWorking { get; set; }
        public string FolderInvoiceError { get; set; }
        public string FolderInvoiceBackup { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
        public string TopicName { get; set; }
        public string EventName { get; set; }
    }
}
