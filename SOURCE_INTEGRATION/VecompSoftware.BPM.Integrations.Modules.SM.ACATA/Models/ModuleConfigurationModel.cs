using System;

namespace VecompSoftware.BPM.Integrations.Modules.SM.ACATA.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartActaShareProtocolSubscription { get; set; }
        public Endpoints Endpoints { get; set; }
        public AcarisParameters AcarisParameters { get; set; }
    }
}
