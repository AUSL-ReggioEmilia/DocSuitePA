using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.TokenSecurityGenerator.Models
{
    public class ModuleConfigurationModel
    {
        public string Topic_Token_Event { get; set; }
        public string Subscription_ExternalViewerSecurityToken { get; set; }
        public double MillisecondExpiryToken { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid? AuthenticationId { get; set; }
        public string WorkflowName { get; set; }
    }
}
