import WorkflowRoleMappingModel = require('App/Models/Workflows/WorkflowRoleMappingModel');
import ActivityModel = require('App/Models/Workflows/ActivityModel');

interface WorkflowActivityViewModel {
    Activity: ActivityModel;
    MappingTag: string;
    WorkflowRoleMapping: WorkflowRoleMappingModel;
}

export = WorkflowActivityViewModel;