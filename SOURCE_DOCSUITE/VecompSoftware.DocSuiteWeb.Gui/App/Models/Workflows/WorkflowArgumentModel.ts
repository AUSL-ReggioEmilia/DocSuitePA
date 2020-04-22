import ArgumentType = require("./ArgumentType");

interface WorkflowArgumentModel {
    Name: string;
    PropertyType: ArgumentType;
    ValueInt?: number;
    ValueDate?: Date;
    ValueDouble?: number;
    ValueBoolean?: boolean;
    ValueGuid?: string;
    ValueString?: string;
    ValueJson?: string;
}

export = WorkflowArgumentModel;