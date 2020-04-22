define(["require", "exports"], function (require, exports) {
    var InsertActionType;
    (function (InsertActionType) {
        InsertActionType[InsertActionType["InsertActivityFascicle"] = 0] = "InsertActivityFascicle";
        InsertActionType[InsertActionType["ViewProtocolDocument"] = 1] = "ViewProtocolDocument";
        InsertActionType[InsertActionType["InsertContainer"] = 2] = "InsertContainer";
        InsertActionType[InsertActionType["DematerialisationLogInsert"] = 4] = "DematerialisationLogInsert";
        InsertActionType[InsertActionType["InsertPeriodicFascicle"] = 8] = "InsertPeriodicFascicle";
        InsertActionType[InsertActionType["SecureDocumentLogInsert"] = 16] = "SecureDocumentLogInsert";
        InsertActionType[InsertActionType["InsertCategory"] = 32] = "InsertCategory";
        InsertActionType[InsertActionType["InsertDossierFolderAssociatedToFascicle"] = 64] = "InsertDossierFolderAssociatedToFascicle";
        InsertActionType[InsertActionType["BuildDossierFolders"] = 128] = "BuildDossierFolders";
        InsertActionType[InsertActionType["AutomaticIntoFascicleDetection"] = 256] = "AutomaticIntoFascicleDetection";
        InsertActionType[InsertActionType["CreateProtocol"] = 512] = "CreateProtocol";
        InsertActionType[InsertActionType["DocumentUnitArchived"] = 1024] = "DocumentUnitArchived";
        InsertActionType[InsertActionType["ProtocolShared"] = 2048] = "ProtocolShared";
    })(InsertActionType || (InsertActionType = {}));
    return InsertActionType;
});
//# sourceMappingURL=InsertActionType.js.map