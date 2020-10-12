namespace VecompSoftware.DocSuiteWeb.API
{
    public class WorkflowActionDTO : IWorkflowActionDTO
    {
        #region [ Constructor ]
        public WorkflowActionDTO(string workflowName, WorkflowActionType workflowActionType)
        {
            WorkflowName = workflowName;
            WorkflowActionType = workflowActionType;
        }
        #endregion

        #region [ Properties ]
        public string WorkflowName { get; private set; }   
        public WorkflowActionType WorkflowActionType { get; private set; }   
        #endregion
    }
}
