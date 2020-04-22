import WorkflowAuthorizationType = require('App/Models/Workflows/WorkflowAuthorizationType');
import RoleModel = require('App/Models/Commons/RoleModel');

interface WorkflowRoleMappingModel {
    UniqueId: string;
    MappingTag: string;
    AuthorizationType: WorkflowAuthorizationType;
    Role: RoleModel;
    IdInternalActivity: string;
    AccountName: string;
}

export = WorkflowRoleMappingModel;