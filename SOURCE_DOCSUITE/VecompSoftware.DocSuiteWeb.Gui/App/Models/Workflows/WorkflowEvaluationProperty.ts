import WorkflowType = require('App/Models/Workflows/WorkflowType');
import WorkflowPropertyType = require('App/Models/Workflows/WorkflowPropertyType');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');

interface WorkflowEvaluationProperty {
    UniqueId: string;
    Name: string;
    WorkflowType: WorkflowType;
    PropertyType: WorkflowPropertyType;
    ValueInt?: number;
    ValueDate?: Date;
    ValueDouble?: number;
    ValueBoolean?: boolean;
    ValueGuid?: string;
    ValueString: string;
    WorkflowRepository: WorkflowRepositoryModel;
}
export = WorkflowEvaluationProperty;

