define(["require", "exports"], function (require, exports) {
    var WorkflowEvaluationPropertyType;
    (function (WorkflowEvaluationPropertyType) {
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["Boolean"] = 1] = "Boolean";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["Double"] = 2] = "Double";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["Integer"] = 4] = "Integer";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["String"] = 8] = "String";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["Json"] = 16] = "Json";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["Guid"] = 32] = "Guid";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["Date"] = 64] = "Date";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["RelationGuid"] = 128] = "RelationGuid";
        WorkflowEvaluationPropertyType[WorkflowEvaluationPropertyType["RelationInt"] = 256] = "RelationInt";
    })(WorkflowEvaluationPropertyType || (WorkflowEvaluationPropertyType = {}));
    return WorkflowEvaluationPropertyType;
});
//# sourceMappingURL=WorkflowEvaluationPropertyType.js.map