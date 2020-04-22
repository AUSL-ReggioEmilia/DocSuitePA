
export enum WorkflowRequestState {
    requested = 0,
    evaluating = 1,
    processing = 2,
    rejected = processing * 2,
    confirmed = rejected * 2
}