using System;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.RequestOpinon.Models
{
    //public class ModuleConfigurationModel
    //{
    //    public string TenantName { get; set; }​
    //    public Guid TenantId { get; set; }
    //    public string TopicWorkflowIntegration { get; set; }​
    //    public string WorkflowStartProtocolUserAuthorizationSubscription { get; set; }
    //    public string ExternalViewerUrl { get; set; }​
    //    public string WorkflowRepositoryName { get; set; }​
    //}
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartProtocolUserAuthorizationSubscription { get; set; }
        public string WorkflowStartProtocolRemoveUserAuthorizationSubscription { get; set; }
        public string ExternalViewerUrl { get; set; }
        public string WorkflowRepositoryName { get; set; }
    }
}
