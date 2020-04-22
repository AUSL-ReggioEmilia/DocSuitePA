define(["require", "exports"], function (require, exports) {
    var WorkflowSeverityLogType;
    (function (WorkflowSeverityLogType) {
        WorkflowSeverityLogType[WorkflowSeverityLogType["Off"] = 1] = "Off";
        WorkflowSeverityLogType[WorkflowSeverityLogType["Fatal"] = 2] = "Fatal";
        WorkflowSeverityLogType[WorkflowSeverityLogType["Error"] = 4] = "Error";
        WorkflowSeverityLogType[WorkflowSeverityLogType["Warning"] = 8] = "Warning";
        WorkflowSeverityLogType[WorkflowSeverityLogType["Info"] = 16] = "Info";
        WorkflowSeverityLogType[WorkflowSeverityLogType["Debug"] = 32] = "Debug";
        WorkflowSeverityLogType[WorkflowSeverityLogType["Trace"] = 64] = "Trace";
    })(WorkflowSeverityLogType || (WorkflowSeverityLogType = {}));
    return WorkflowSeverityLogType;
});
//# sourceMappingURL=WorkflowSeverityLogType.js.map