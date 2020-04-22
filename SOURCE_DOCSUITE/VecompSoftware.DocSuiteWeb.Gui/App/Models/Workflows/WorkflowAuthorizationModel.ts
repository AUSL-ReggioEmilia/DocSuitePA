import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');

interface WorkflowAuthorizationModel{
    UniqueId: string;   
    Account: string;
    IsHandler: boolean;
    WorkflowActivity: WorkflowActivityModel;
}

export = WorkflowAuthorizationModel;