define(["require", "exports"], function (require, exports) {
    var UpdateActionType;
    (function (UpdateActionType) {
        UpdateActionType[UpdateActionType["FascicleClose"] = 0] = "FascicleClose";
        UpdateActionType[UpdateActionType["TemplateCollaborationPublish"] = 1] = "TemplateCollaborationPublish";
        UpdateActionType[UpdateActionType["RoleUserTemplateCollaborationInvalid"] = 2] = "RoleUserTemplateCollaborationInvalid";
        UpdateActionType[UpdateActionType["ActivityFascicleUpdate"] = 4] = "ActivityFascicleUpdate";
        UpdateActionType[UpdateActionType["ActivityFascicleClose"] = 8] = "ActivityFascicleClose";
        UpdateActionType[UpdateActionType["ProtocolArchivedUpdate"] = 16] = "ProtocolArchivedUpdate";
        UpdateActionType[UpdateActionType["HandlingWorkflow"] = 32] = "HandlingWorkflow";
        UpdateActionType[UpdateActionType["RelaseHandlingWorkflow"] = 64] = "RelaseHandlingWorkflow";
        UpdateActionType[UpdateActionType["RemoveFascicleFromDossierFolder"] = 128] = "RemoveFascicleFromDossierFolder";
        UpdateActionType[UpdateActionType["AssociatedFascicleToDossierFolder"] = 256] = "AssociatedFascicleToDossierFolder";
        UpdateActionType[UpdateActionType["CompleteDematerialisationWorkflow"] = 512] = "CompleteDematerialisationWorkflow";
        UpdateActionType[UpdateActionType["DossierFolderAuthorizationsPropagation"] = 1024] = "DossierFolderAuthorizationsPropagation";
        UpdateActionType[UpdateActionType["CompleteSecureDocumentWorkflow"] = 2048] = "CompleteSecureDocumentWorkflow";
        UpdateActionType[UpdateActionType["UpdateCategory"] = 4096] = "UpdateCategory";
        UpdateActionType[UpdateActionType["PeriodicFascicleClose"] = 8192] = "PeriodicFascicleClose";
        UpdateActionType[UpdateActionType["ActivateProtocol"] = 16384] = "ActivateProtocol";
        UpdateActionType[UpdateActionType["OpenFascicleClosed"] = 32768] = "OpenFascicleClosed";
        UpdateActionType[UpdateActionType["CancelFascicle"] = 65536] = "CancelFascicle";
        UpdateActionType[UpdateActionType["PECMailManaged"] = 131072] = "PECMailManaged";
        UpdateActionType[UpdateActionType["PECMailInvoiceTenantCorrection"] = 262144] = "PECMailInvoiceTenantCorrection";
        UpdateActionType[UpdateActionType["FascicleMoveToFolder"] = 524288] = "FascicleMoveToFolder";
    })(UpdateActionType || (UpdateActionType = {}));
    return UpdateActionType;
});
//# sourceMappingURL=UpdateActionType.js.map