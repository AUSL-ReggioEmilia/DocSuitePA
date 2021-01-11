class WorkflowInstanceSearchFilterDTO {
    name: string;
    workflowRepositoryId: string;
    activeFrom: string;
    activeTo: string;
    status: string;
    skip?: number;
    top?: number;
}

export = WorkflowInstanceSearchFilterDTO;