import { WorkflowModel } from './workflow.model';

export interface StartWorkflowContentType {
    ExecutorUser: string;
    Content: WorkflowModel;
}