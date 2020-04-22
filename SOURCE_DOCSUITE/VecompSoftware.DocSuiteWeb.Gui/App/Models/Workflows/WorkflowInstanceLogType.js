define(["require", "exports"], function (require, exports) {
    var WorkflowInstanceLogType;
    (function (WorkflowInstanceLogType) {
        WorkflowInstanceLogType[WorkflowInstanceLogType["DocumentApproved"] = 0] = "DocumentApproved";
        WorkflowInstanceLogType[WorkflowInstanceLogType["DocumentRefused"] = 1] = "DocumentRefused";
        WorkflowInstanceLogType[WorkflowInstanceLogType["DocumentRegistered"] = 3] = "DocumentRegistered";
        WorkflowInstanceLogType[WorkflowInstanceLogType["UDSCreated"] = 4] = "UDSCreated";
        WorkflowInstanceLogType[WorkflowInstanceLogType["UDSRegistered"] = 5] = "UDSRegistered";
        WorkflowInstanceLogType[WorkflowInstanceLogType["PECSended"] = 6] = "PECSended";
        WorkflowInstanceLogType[WorkflowInstanceLogType["MailSended"] = 7] = "MailSended";
        WorkflowInstanceLogType[WorkflowInstanceLogType["Information"] = 8] = "Information";
        WorkflowInstanceLogType[WorkflowInstanceLogType["WFStarted"] = 9] = "WFStarted";
        WorkflowInstanceLogType[WorkflowInstanceLogType["WFRoleAssigned"] = 10] = "WFRoleAssigned";
        WorkflowInstanceLogType[WorkflowInstanceLogType["WFTakeCharge"] = 11] = "WFTakeCharge";
        WorkflowInstanceLogType[WorkflowInstanceLogType["WFRelease"] = 12] = "WFRelease";
        WorkflowInstanceLogType[WorkflowInstanceLogType["WFRefused"] = 13] = "WFRefused";
        WorkflowInstanceLogType[WorkflowInstanceLogType["WFCompleted"] = 14] = "WFCompleted";
    })(WorkflowInstanceLogType || (WorkflowInstanceLogType = {}));
    return WorkflowInstanceLogType;
});
//# sourceMappingURL=WorkflowInstanceLogType.js.map