define(["require", "exports"], function (require, exports) {
    var WorkflowTreeNodeType;
    (function (WorkflowTreeNodeType) {
        WorkflowTreeNodeType[WorkflowTreeNodeType["Step"] = 0] = "Step";
        WorkflowTreeNodeType[WorkflowTreeNodeType["Workflow"] = 1] = "Workflow";
        WorkflowTreeNodeType[WorkflowTreeNodeType["Root"] = 2] = "Root";
    })(WorkflowTreeNodeType || (WorkflowTreeNodeType = {}));
    return WorkflowTreeNodeType;
});
//# sourceMappingURL=WorkflowTreeNodeType.js.map