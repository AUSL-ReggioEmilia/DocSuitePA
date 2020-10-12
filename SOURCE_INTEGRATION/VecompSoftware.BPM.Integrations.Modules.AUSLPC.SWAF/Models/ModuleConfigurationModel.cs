namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Models
{
    public class ModuleConfigurationModel
    {
        public string WorkflowIntegrationTopic { get; set; }
        public string WorkflowStartSWAFNotificationSubscription { get; set; }
        public string WorkflowStartShareToSWAFNotificationSubscription { get; set; }
        public string SWAFAPIUrl { get; set; }
        public string ArchiveContactSectionLabel { get; set; }
        public string ArchiveDocumentTypeMetadata { get; set; }
    }
}
