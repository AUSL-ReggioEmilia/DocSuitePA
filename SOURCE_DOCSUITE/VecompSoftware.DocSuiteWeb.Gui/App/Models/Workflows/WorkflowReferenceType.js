define(["require", "exports"], function (require, exports) {
    var WorkflowReferenceType;
    (function (WorkflowReferenceType) {
        WorkflowReferenceType[WorkflowReferenceType["Json"] = 1] = "Json";
        WorkflowReferenceType[WorkflowReferenceType["RelationGuid"] = 2] = "RelationGuid";
        WorkflowReferenceType[WorkflowReferenceType["RelationInt"] = 4] = "RelationInt";
        WorkflowReferenceType[WorkflowReferenceType["PropertyString"] = 8] = "PropertyString";
        WorkflowReferenceType[WorkflowReferenceType["PropertyInt"] = 16] = "PropertyInt";
        WorkflowReferenceType[WorkflowReferenceType["PropertyDate"] = 32] = "PropertyDate";
        WorkflowReferenceType[WorkflowReferenceType["PropertyDouble"] = 64] = "PropertyDouble";
        WorkflowReferenceType[WorkflowReferenceType["PropertyBoolean"] = 128] = "PropertyBoolean";
        WorkflowReferenceType[WorkflowReferenceType["PropertyGuid"] = 256] = "PropertyGuid";
    })(WorkflowReferenceType || (WorkflowReferenceType = {}));
    return WorkflowReferenceType;
});
//# sourceMappingURL=WorkflowReferenceType.js.map