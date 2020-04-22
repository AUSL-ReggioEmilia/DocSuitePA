using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string TopicBuilderEvent { get; set; }
        public string WorkflowStartReceivableInvoiceSubscription { get; set; }
        public string WorkflowStartReceivableInvoiceAdESubscription { get; set; }
        public string WorkflowStartInvoiceDeleteSubscription { get; set; }
        public string WorkflowStartInvoiceMoveSubscription { get; set; }
        public string WorkflowReceivableInvoiceProtocolBuildCompleteSubscription { get; set; }
        public string WorkflowReceivableInvoiceUDSBuildCompleteSubscription { get; set; }
        public string WorkflowInvoiceDeleteProtocolDeleteCompleteSubscription { get; set; }
        public string WorkflowInvoiceDeleteUDSDeleteCompleteSubscription { get; set; }
        public bool PersistInvoiceFilesystemEnabled { get; set; }
        public bool PersistSDIMetadataEnabled { get; set; }
        public bool InvoiceTenantValidationEnabled { get; set; }
        public bool InvoiceAdEEnabled { get; set; }
        public bool AdEProtocolNotificationEnabled { get; set; }
        public string WorkflowInvoiceDeleteRepositoryName { get; set; }
        public Dictionary<string, Dictionary<XMLModelKind, InvoiceConfiguration>> WorkflowConfigurations { get; set; }
    }
}
