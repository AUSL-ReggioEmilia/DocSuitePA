import WorkflowArgument = require('App/Models/Workflows/WorkflowArgumentModel');

interface WorkflowStart {
    WorkflowName: string;
    Arguments: { string?:WorkflowArgument };
    StartParameters: { string?: Object };
}

export = WorkflowStart;