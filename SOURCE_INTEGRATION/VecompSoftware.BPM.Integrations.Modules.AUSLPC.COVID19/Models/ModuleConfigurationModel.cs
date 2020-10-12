namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models
{
    public class ModuleConfigurationModel
    {
        public string WorkflowIntegrationTopic { get; set; }
        public string WorkflowStartCovidPortalNotificationSubscription { get; set; }
        public string PortaleCovidAPIUrl { get; set; }
        public string PortaleCovidAPIAuthUrl { get; set; }
        public string PortaleCovidAPIUsername { get; set; }
        public string PortaleCovidAPIPassword { get; set; }
    }
}
