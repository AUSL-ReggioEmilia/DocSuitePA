import WorkflowType = require('App/Models/Workflows/WorkflowType');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');

interface WorkflowProperty {
    Name: string;
    WorkflowType: WorkflowType;
    PropertyType: ArgumentType;
    ValueInt ?: number;
    ValueDate ?: Date;
    ValueDouble ?: number;
    ValueBoolean ?: boolean;
    ValueGuid: string;
    ValueString: string;
    RegistrationDate: Date;

    WorkflowActivity: WorkflowActivityModel;
}
export = WorkflowProperty;