namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Models
{
    public class ModuleConfigurationModel
    {
        public string WorkflowIntegrationTopic { get; set; }
        public string ProtocolCreatedSubscription { get; set; }
        public string ProtocolCancelSubscription { get; set; }
        public string PECMailCreatedSubscription { get; set; }
        public string PECMailReceiptReceivedSubscription { get; set; }
        public string WorkflowStartZENShareDocumentUnitSubscription { get; set; }
        public string ZENWebApiUrl { get; set; }
        public string ZENUsername { get; set; }
        public string ZENPassword { get; set; }
    }
}
