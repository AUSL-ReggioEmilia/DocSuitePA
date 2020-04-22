using System;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.SPID.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartSPIDSubscription { get; set; }
        public string WorkflowSPIDUpdateSubscription { get; set; }
        public string WorkflowRepositoryName { get; set; }
        public string RootContact { get; set; }
        public int FascicleResponsibleContact { get; set; }
        public short CategoryId { get; set; }
        public short ContainerId { get; set; }
        public short ConservationPeriod { get; set; }
        public Guid IdMetadataRepository { get; set; }
        public Guid IdPDFTemplate { get; set; }
        public string PDFGeneratorServiceUrl { get; set; }
        public string WorkflowFascicleBuildCompleteSubscription { get; set; }
        public string TopicBuilderEvent { get; set; }
    }
}
