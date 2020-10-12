class QueryParametersWorkflowEvaluationProperty {
    Action: string;
    WorkflowRepositoryId: string;
    WorkflowEvaluationPropertyId: string;
    StartProposer?: number;
    StartReceiver?: number;

    //key names
    public static QUERY_PARAM_ACTION = "Action";
    public static QUERY_PARAM_WORKFLOW_REPOSITORY_ID = "WorkflowRepositoryId";
    public static QUERY_PARAM_WORKFLOW_EVALUATION_PROPERTY_ID = "WorkflowEvaluationPropertyId";
    public static QUERY_PARAM_START_PROPOSER = "StartProposer";
    public static QUERY_PARAM_START_RECEIVER = "StartReceiver";
}

export = QueryParametersWorkflowEvaluationProperty