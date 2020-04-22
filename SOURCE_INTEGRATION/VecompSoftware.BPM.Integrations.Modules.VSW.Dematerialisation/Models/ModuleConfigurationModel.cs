using System;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.Dematerialisation
{
    public class ModuleConfigurationModel
    {
        public string Topic_Workflow_Integration { get; set; }
        public string Topic_UDS { get; set; }
        public string Subscription_Dematerialisation { get; set; }
        public string ProtocolSignature { get; set; }
        public string ResolutionSignature { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string CorporateAcronym { get; set; }
    }
}
