define(["require", "exports"], function (require, exports) {
    var WorkflowActivityStatus;
    (function (WorkflowActivityStatus) {
        WorkflowActivityStatus[WorkflowActivityStatus["Todo"] = 1] = "Todo";
        WorkflowActivityStatus[WorkflowActivityStatus["Handling"] = 2] = "Handling";
        WorkflowActivityStatus[WorkflowActivityStatus["Rejected"] = 4] = "Rejected";
        WorkflowActivityStatus[WorkflowActivityStatus["Done"] = 8] = "Done";
    })(WorkflowActivityStatus || (WorkflowActivityStatus = {}));
    return WorkflowActivityStatus;
});
//# sourceMappingURL=WorkflowActivityStatus.js.map