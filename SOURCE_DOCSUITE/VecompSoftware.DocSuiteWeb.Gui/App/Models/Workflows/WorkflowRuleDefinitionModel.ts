import WorkflowRule = require("App/Models/Workflows/WorkflowRule");
import DSWEnvironmentType = require("App/Models/Workflows/WorkflowDSWEnvironmentType");

class WorkflowRuleDefinitionModel {
    Environment: DSWEnvironmentType;
    Rules: WorkflowRule[];
}

export = WorkflowRuleDefinitionModel;