import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import WorkflowRoleMappingModel = require('App/Models/Workflows/WorkflowRoleMappingModel');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import RoleModel = require('../Commons/RoleModel');

interface WorkflowRepositoryModel {
    UniqueId: string;
    Name: string;
    Version: string;
    ActiveFrom: Date;
    ActiveTo?: Date;
    Xaml: string;
    Json: string;
    CustomActivities: string;
    Status: WorkflowStatus;
    DSWEnvironment: number;

    WorkflowRoleMappings: WorkflowRoleMappingModel[]; 
    WorkflowEvaluationProperties: WorkflowEvaluationProperty[];
    Roles: RoleModel[];
}

export = WorkflowRepositoryModel;