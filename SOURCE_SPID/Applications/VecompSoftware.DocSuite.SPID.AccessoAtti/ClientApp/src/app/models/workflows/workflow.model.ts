import { WorkflowParameterModel } from './parameters/workflow-parameter.model';

export interface WorkflowModel {
    ActivityTitlePrefix: string;
    Name: string;
    WorkflowParameters: WorkflowParameterModel[];
}