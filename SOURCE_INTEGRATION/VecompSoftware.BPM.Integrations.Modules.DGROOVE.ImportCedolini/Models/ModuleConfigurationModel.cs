using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.DGROOVE.ImportCedolini.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowUDSCedoliniCompleteSubscription { get; set; }
        public string WorkflowRepositoryName { get; set; }
        public IDictionary<string, string> MatricolaUtenti { get; set; }
    }
}
