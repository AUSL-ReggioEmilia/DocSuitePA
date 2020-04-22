define(["require", "exports"], function (require, exports) {
    var WorkflowPropertyType;
    (function (WorkflowPropertyType) {
        WorkflowPropertyType[WorkflowPropertyType["Json"] = 1] = "Json";
        WorkflowPropertyType[WorkflowPropertyType["RelationGuid"] = 2] = "RelationGuid";
        WorkflowPropertyType[WorkflowPropertyType["RelationInt"] = 4] = "RelationInt";
        WorkflowPropertyType[WorkflowPropertyType["PropertyString"] = 8] = "PropertyString";
        WorkflowPropertyType[WorkflowPropertyType["PropertyInt"] = 16] = "PropertyInt";
        WorkflowPropertyType[WorkflowPropertyType["PropertyDate"] = 32] = "PropertyDate";
        WorkflowPropertyType[WorkflowPropertyType["PropertyDouble"] = 64] = "PropertyDouble";
        WorkflowPropertyType[WorkflowPropertyType["PropertyBoolean"] = 128] = "PropertyBoolean";
        WorkflowPropertyType[WorkflowPropertyType["PropertyGuid"] = 256] = "PropertyGuid";
    })(WorkflowPropertyType || (WorkflowPropertyType = {}));
    return WorkflowPropertyType;
});
//# sourceMappingURL=WorkflowPropertyType.js.map