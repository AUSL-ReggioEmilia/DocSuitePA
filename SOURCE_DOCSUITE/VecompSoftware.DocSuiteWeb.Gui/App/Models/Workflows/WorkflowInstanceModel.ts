import WorkflowRepository = require('App/Models/Workflows/WorkflowRepositoryModel');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');

interface WorkflowInstanceModel {
    UniqueId: string;
    RegistrationDate: string;
    RegistrationUser: string;
    Name: string;
    Status: string;
    WorkflowActivitiesDoneCount: number;
    HasActivitiesInError: boolean;
    HasActivitiesInErrorLabel: string;
    WorkflowActivitiesCount: number;
    WorkflowRepository: WorkflowRepository;
    WorkflowActivities: WorkflowActivityModel[];
}

export = WorkflowInstanceModel;