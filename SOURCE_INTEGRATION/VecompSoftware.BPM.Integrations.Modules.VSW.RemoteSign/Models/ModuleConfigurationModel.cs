using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.RemoteSign.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartRemoteSignSubscription { get; set; }
        public string WorkflowStartOTPRequestSubscription { get; set; }
    }
}
