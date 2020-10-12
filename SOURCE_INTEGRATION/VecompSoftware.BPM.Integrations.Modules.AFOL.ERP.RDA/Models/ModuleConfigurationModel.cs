using System;
using System.Collections.Generic;

namespace VecompSoftware.BPM.Integrations.Modules.AFOL.ERP.RDA.Models
{
    public class ModuleConfigurationModel
    {
        public string ConnectionString { get; set; }
        public int FascicleResponsibleContact { get; set; }
        public string FascicleObject { get; set; }
        public short CategoryId { get; set; }
        public int RootContact { get; set; }
        public Guid IdMetadataRepository { get; set; }
        public Guid RDAUdsRepositoryId { get; set; }
        public Guid ODAUdsRepositoryId { get; set; }
        public Guid PreventivoUdsRepositoryId { get; set; }
        public short ConservationPeriod { get; set; }
        public string RDAWorkflowRepositoryName { get; set; }
        public string ODAWorkflowRepositoryName { get; set; }
        public string PreventivoWorkflowRepositoryName { get; set; }
        public List<short> AuthorizationRoles { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
        public string TopicWorkflowIntegration { get; set; }
        public string WorkflowStartResolutionRDASubscription { get; set; }

    }
}
