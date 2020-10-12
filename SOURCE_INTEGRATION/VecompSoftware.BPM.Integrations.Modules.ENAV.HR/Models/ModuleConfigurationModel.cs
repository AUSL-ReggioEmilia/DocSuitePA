using System;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.HR.Models
{
    public class ModuleConfigurationModel
    {
        public string ConnectionString { get; set; }
        public string TopicBuilderEvent { get; set; }
        public string DossierSubjectFormat { get; set; }
        public Guid DossierMetadataRepositoryId { get; set; }
        public string AutomaticProtocolWorkflowRepositoryName { get; set; }
        public string FullProtocolWorkflowRepositoryName { get; set; }
        public string FascicleProtocolWorkflowRepositoryName { get; set; }
        public string ManualProtocolWorkflowRepositoryName { get; set; }
        public string DigitalSignatureWorkflowRepositoryName { get; set; }
        public string WorkflowStartEHCDossierCreatedSubscription { get; set; }
        public string WorkflowStartEHCFascicleCreatedSubscription { get; set; }
        public string WorkflowStartEHCProtocolCanceledSubscription { get; set; }
        public string WorkflowStartEHCProtocolCreatedSubscription { get; set; }
        public int CategoryId { get; set; }
    }
}
