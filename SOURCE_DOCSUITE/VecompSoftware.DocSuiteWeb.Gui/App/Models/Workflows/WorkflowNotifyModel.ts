import WorkflowArgument = require('App/Models/Workflows/WorkflowArgumentModel');

interface WorkflowNotifyModel {
    WorkflowName: string;
    WorkflowActivityId: string;
    ModuleName: string;
    OutputArguments: { string?: WorkflowArgument };
    InstanceId: string;
}

export = WorkflowNotifyModel;