using System;

namespace VecompSoftware.BPM.Integrations.Modules.ENEA.Wide.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartWideEneaProtocolSubscription { get; set; }
        public string WideWebServiceUrl { get; set; }
        public string WideUsername { get; set; }
        public string WideRelativePath { get; set; }
        public string FTPUrl { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }

    }
}
