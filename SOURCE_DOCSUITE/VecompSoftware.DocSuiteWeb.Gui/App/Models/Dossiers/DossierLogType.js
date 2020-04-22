define(["require", "exports"], function (require, exports) {
    var DossierLogType;
    (function (DossierLogType) {
        DossierLogType[DossierLogType["Insert"] = 0] = "Insert";
        DossierLogType[DossierLogType["Modify"] = 1] = "Modify";
        DossierLogType[DossierLogType["View"] = 2] = "View";
        DossierLogType[DossierLogType["Authorize"] = 4] = "Authorize";
        DossierLogType[DossierLogType["Delete"] = 8] = "Delete";
        DossierLogType[DossierLogType["Close"] = 16] = "Close";
        DossierLogType[DossierLogType["Workflow"] = 32] = "Workflow";
        DossierLogType[DossierLogType["FolderInsert"] = 64] = "FolderInsert";
        DossierLogType[DossierLogType["FolderModify"] = 128] = "FolderModify";
        DossierLogType[DossierLogType["FolderAuthorize"] = 256] = "FolderAuthorize";
        DossierLogType[DossierLogType["FolderFascicleRemove"] = 512] = "FolderFascicleRemove";
        DossierLogType[DossierLogType["FolderClose"] = 1024] = "FolderClose";
        DossierLogType[DossierLogType["FolderDelete"] = 2048] = "FolderDelete";
        DossierLogType[DossierLogType["FolderHystory"] = 4096] = "FolderHystory";
        DossierLogType[DossierLogType["FolderResponsibleChange"] = 8192] = "FolderResponsibleChange";
        DossierLogType[DossierLogType["ResponsibleChange"] = 16384] = "ResponsibleChange";
        DossierLogType[DossierLogType["FascicleInsert"] = 32768] = "FascicleInsert";
        DossierLogType[DossierLogType["FascicleView"] = 65536] = "FascicleView";
        DossierLogType[DossierLogType["DocumentInsert"] = 131072] = "DocumentInsert";
        DossierLogType[DossierLogType["DocumentView"] = 262144] = "DocumentView";
        DossierLogType[DossierLogType["DocumentDelete"] = 524288] = "DocumentDelete";
        DossierLogType[DossierLogType["Error"] = 1048576] = "Error";
    })(DossierLogType || (DossierLogType = {}));
    return DossierLogType;
});
//# sourceMappingURL=DossierLogType.js.map