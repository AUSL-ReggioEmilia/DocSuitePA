namespace VecompSoftware.BPM.Integrations.Modules.VSW.SecureDocument.Models
{
    public class ModuleConfigurationModel
    {
        public string Topic_Workflow_Integration { get; set; }
        public string Subscription_SecureDocument { get; set; }
        public string ProtocolSignature { get; set; }
        public bool ProtocolAttachLocationEnabled { get; set; }
    }
}
