define(["require", "exports"], function (require, exports) {
    var WorkflowAuthorizationType;
    (function (WorkflowAuthorizationType) {
        WorkflowAuthorizationType[WorkflowAuthorizationType["None"] = 0] = "None";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllRoleUser"] = 1] = "AllRoleUser";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllSecretary"] = 2] = "AllSecretary";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllSigner"] = 4] = "AllSigner";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllManager"] = 8] = "AllManager";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllOChartRoleUser"] = 16] = "AllOChartRoleUser";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllOChartManager"] = 32] = "AllOChartManager";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllOChartHierarchyManager"] = 64] = "AllOChartHierarchyManager";
        WorkflowAuthorizationType[WorkflowAuthorizationType["UserName"] = 128] = "UserName";
        WorkflowAuthorizationType[WorkflowAuthorizationType["ADGroup"] = 256] = "ADGroup";
        WorkflowAuthorizationType[WorkflowAuthorizationType["MappingTags"] = 512] = "MappingTags";
        WorkflowAuthorizationType[WorkflowAuthorizationType["AllDematerialisationManager"] = 1024] = "AllDematerialisationManager";
    })(WorkflowAuthorizationType || (WorkflowAuthorizationType = {}));
    return WorkflowAuthorizationType;
});
//# sourceMappingURL=WorkflowAuthorizationType.js.map