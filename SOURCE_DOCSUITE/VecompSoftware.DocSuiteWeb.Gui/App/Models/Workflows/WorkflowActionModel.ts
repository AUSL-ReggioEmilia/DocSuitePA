import ContentBase = require("App/Models/ContentBase");

abstract class WorkflowActionModel {
    $type: string;
    UniqueId: string;
    CorrelationId: string;
    IdWorkflowActivity?: string;
    WorkflowName: string;
    Referenced: ContentBase;
    MessageTypeDependency: string;
}

export = WorkflowActionModel;