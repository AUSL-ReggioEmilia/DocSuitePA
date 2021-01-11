import DocumentModel = require("./DocumentModel");

class WorkflowSignModel {
    workflowName: string;
    idWorkflowActivity: string;
    documents: DocumentModel[];
}

export =WorkflowSignModel;