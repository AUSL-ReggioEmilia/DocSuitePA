import WorkflowRoleMappingModel = require('App/Models/Workflows/WorkflowRoleMappingModel');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import RoleModel = require('../Commons/RoleModel');
import WorkflowRepositoryStatus = require('./WorkflowRepositoryStatus');

interface WorkflowRepositoryModel {
    UniqueId: string;
    Name: string;
    Version: string;
    ActiveFrom: Date;
    ActiveTo?: Date;
    Xaml: string;
    Json: string;
    CustomActivities: string;
    Status: WorkflowRepositoryStatus;
    DSWEnvironment: number;

    WorkflowRoleMappings: WorkflowRoleMappingModel[]; 
    WorkflowEvaluationProperties: WorkflowEvaluationProperty[];
    Roles: RoleModel[];
}

export = WorkflowRepositoryModel;