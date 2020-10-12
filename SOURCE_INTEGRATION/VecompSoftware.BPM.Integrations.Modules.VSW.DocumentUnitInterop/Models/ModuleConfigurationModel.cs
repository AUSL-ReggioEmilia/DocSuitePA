using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.DocumentUnitInterop.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string TopicWorkflowActivityCompleted { get; set; }
        public string WorkflowStartInteropShareDocumentUnitSubscription { get; set; }
        public string WorkflowActivityInteropShareDocumentUnitCompleteSubscription { get; set; }
        public string WorkflowName { get; set; }
        public IDictionary<string, TenantConfiguration> TenantProxies { get; set; }
    }
}