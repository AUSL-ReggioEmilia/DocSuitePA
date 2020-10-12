using System;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.ERP.Contract.Models
{
    public class ModuleConfigurationModel
    {
        public string ConnectionString { get; set; }
        public Guid WorkflowRepositoryId { get; set; }
        public Guid MetadataRepositoryId { get; set; }
        public short CategoryId { get; set; }
        public short FascicleConservation { get; set; }
        public int FascicleContactId { get; set; }
        public string UDSRepositoryName { get; set; }
        public int ContactFatherId { get; set; }
        public string WorkflowIntegrationTopic { get; set; }
        public string SecurityTopicSubscription { get; set; }
        public string WorkflowCompletedTopic { get; set; }
        public string WorkflowERPContrattiSubscription { get; set; }
        public string TenantName { get; set; }
        public Guid TenantId { get; set; }
        public Guid TenantAOOId { get; set; }
    }
}
