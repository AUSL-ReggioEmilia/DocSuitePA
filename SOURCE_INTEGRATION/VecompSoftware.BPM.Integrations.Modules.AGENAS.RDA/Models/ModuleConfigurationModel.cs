using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.AGENAS.RDA.Models
{
    public class ModuleConfigurationModel
    {
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string TopicBuilderEvent { get; set; }
        public string TopicWorkflowActivityCompleted { get; set; }
        public string WorkflowRDAFascicleBuildCompleteSubscription { get; set; }
        public string WorkflowActivityRDAAssignmentCompleteSubscription { get; set; }
        public string WorkflowStartRDASubscription { get; set; }
        public string WorkflowStartUpdateFascicleRDASubscription { get; set; }
        public string WorkflowRepositoryName { get; set; }
        public int FascicleResponsibleContact { get; set; }
        public short RDACategoryId { get; set; }
        public short ContractCategoryId { get; set; }
        public short ConservationPeriod { get; set; }
        public Dictionary<string, Guid> MetadataRepositoryMappings { get; set; }
        public short ContainerDossierId { get; set; }
        public short RoleDossierId { get; set; }
    }
}
