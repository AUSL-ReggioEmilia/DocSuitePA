using System;

namespace VecompSoftware.BPM.Integrations.Modules.TECMARKET.EventViewerAnalyzer.Models
{
    public class ModuleConfigurationModel
    {
        public string PathQueryXpath { get; set; }
        public string WorkflowName { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
    }
}
