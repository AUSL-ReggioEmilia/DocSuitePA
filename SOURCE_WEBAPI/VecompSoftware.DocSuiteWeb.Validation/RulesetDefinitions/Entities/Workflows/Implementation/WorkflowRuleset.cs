﻿
namespace VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Workflows
{
    public class WorkflowRuleset : IWorkflowRuleset
    {
        public string READ => "WorkflowRead";

        public string INSERT => "WorkflowInsert";

        public string UPDATE => "WorkflowUpdate";

        public string DELETE => "WorkflowDelete";
    }
}
