
const UDSConstants = {
    HubMessageEvents: {
        WorkflowStatusDone: "workflowStatusDone",
        WorkflowStatusError: "workflowStatusError",
        WorkflowNotificationInfo: "workflowNotificationInfo",
        WorkflowNotificationInfoAsModel: "workflowNotificationInfoAsModel",
        WorkflowNotificationWarning: "workflowNotificationWarning",
        WorkflowNotificationError: "workflowNotificationError",
        WorkflowResumeStatus: "workflowResumeStatus"
    },

     HubMethods: {
        SubscribeResumeWorkflow: "SubscribeResumeWorkflow",
         SubscribeStartWorkflow: "SubscribeStartWorkflow"
    }
}

export = UDSConstants