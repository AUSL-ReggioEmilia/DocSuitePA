using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.Invoice.Models
{
    public class ModuleConfigurationModel
    {
        public string ConnectionString { get; set; }
        public Guid MetadataRepositoryId { get; set; }
        public string TopicWorkflowCompleted { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string TopicBuilderEvent { get; set; }
        public string WorkflowStartReceivableInvoiceSubscription { get; set; }
        public string WorkflowReceivableInvoiceProtocolBuildCompleteSubscription { get; set; }
        public string WorkflowReceivableInvoiceUDSBuildCompleteSubscription { get; set; }
        public string WorkflowPayableInvoiceUDSBuildCompleteSubscription { get; set; }
        public string WorkflowPayableInvoiceProtocolBuildCompleteSubscription { get; set; }
        public string WorkflowPayableInvoicePECMailBuildCompleteSubscription { get; set; }
        public string WorkflowStartUpdateReceiptMetadataInvoiceSubscription { get; set; }
        public string WorkflowStartUpdateMetadataInvoiceSubscription { get; set; }
        public Dictionary<string, Dictionary<XMLModelKind, InvoiceConfiguration>> ReceivableWorkflowConfigurations { get; set; }
        public Dictionary<string, Dictionary<XMLModelKind, InvoiceConfiguration>> PayableWorkflowConfigurations { get; set; }
    }
}
