using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Model.Workflow.Client
{
    public class WorkflowClientConfiguration
    {
        public string RootScope { get; set; }
        public string UriToSendServiceBus { get; set; }
        public string UriToSendWorkflowManager { get; set; }
        public string UriToSendEntity { get; set; }
        public ICollection<string> FolderCustomActivity { get; set; }
        public string FolderCodeActivity { get; set; }
        public string WorkflowScopeValidation { get; set; }
    }
}
