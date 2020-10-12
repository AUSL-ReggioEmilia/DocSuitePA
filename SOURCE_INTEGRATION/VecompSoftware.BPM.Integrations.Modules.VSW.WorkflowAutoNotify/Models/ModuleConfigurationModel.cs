namespace VecompSoftware.BPM.Integrations.Modules.VSW.WorkflowAutoNotify.Models
{
    public class ModuleConfigurationModel
    {
        public string TopicBuilderEvent { get; set; }
        public string TopicWorkflowActivityCompleted { get; set; }
        public string WorkflowAutoCompleteBuildCompleteSubscription { get; set; }
        public string WorkflowActivityAutoCompleteSubscription { get; set; }
}
}
