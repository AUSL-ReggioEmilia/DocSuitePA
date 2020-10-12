using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicBuilderEvent { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartUpdateReceiptMetadataInvoiceSubscription { get; set; }
        public string WorkflowStartUpdateMetadataInvoiceSubscription { get; set; }
        public bool DeleteLookingDirectoryEnabled { get; set; }
        public bool WaitingEnabled { get; set; }
        public uint WaitingSecondDuration { get; set; }
        public string WorkflowInvoiceDeleteRepositoryName { get; set; }
        public string WorkflowPayableInvoicePECMailBuildCompleteSubscription { get; set; }
        public Dictionary<string, WorkflowConfiguration> WorkflowConfigurations { get; set; }
        public MessageConfiguration MessageConfiguration { get; set; }
    }
}
