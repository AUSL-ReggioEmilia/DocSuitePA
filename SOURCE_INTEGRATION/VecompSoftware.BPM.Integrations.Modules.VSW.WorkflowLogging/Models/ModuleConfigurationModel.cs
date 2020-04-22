namespace VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowLogging.Models
{
    public class ModuleConfigurationModel
    {
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowLoggingNotificationErrorSubscription { get; set; }
        public string WorkflowLoggingNotificationInfoSubscription { get; set; }
        public string WorkflowLoggingNotificationWarningSubscription { get; set; }
        public string WorkflowLoggingStartRequestDoneSubscription { get; set; }
        public string WorkflowLoggingStartRequestErrorSubscription { get; set; }
    }
}
