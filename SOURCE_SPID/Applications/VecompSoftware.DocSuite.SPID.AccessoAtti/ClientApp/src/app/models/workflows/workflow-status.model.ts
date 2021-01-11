import { WorkflowRequestState } from './workflow-request-state.enum';
import { FascicleModel } from '@app-models/fascicles/fascicle.model';

export class WorkflowStatusModel {
    Id: string;
    Name: string;
    Status: string;
    Date: Date;
    Model: string;    
}