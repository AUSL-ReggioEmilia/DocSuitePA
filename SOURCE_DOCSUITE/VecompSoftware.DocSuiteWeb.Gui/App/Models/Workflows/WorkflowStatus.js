define(["require", "exports"], function (require, exports) {
    var WorkflowStatus;
    (function (WorkflowStatus) {
        WorkflowStatus[WorkflowStatus["Todo"] = 1] = "Todo";
        WorkflowStatus[WorkflowStatus["Progress"] = 2] = "Progress";
        WorkflowStatus[WorkflowStatus["Suspended"] = 4] = "Suspended";
        WorkflowStatus[WorkflowStatus["Done"] = 8] = "Done";
        WorkflowStatus[WorkflowStatus["Error"] = 16] = "Error";
    })(WorkflowStatus || (WorkflowStatus = {}));
    return WorkflowStatus;
});
//# sourceMappingURL=WorkflowStatus.js.map