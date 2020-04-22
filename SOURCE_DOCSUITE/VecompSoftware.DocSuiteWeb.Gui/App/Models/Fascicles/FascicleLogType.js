define(["require", "exports"], function (require, exports) {
    var FascicleLogType;
    (function (FascicleLogType) {
        FascicleLogType[FascicleLogType["Insert"] = 0] = "Insert";
        FascicleLogType[FascicleLogType["Modify"] = 1] = "Modify";
        FascicleLogType[FascicleLogType["View"] = 2] = "View";
        FascicleLogType[FascicleLogType["Delete"] = 4] = "Delete";
        FascicleLogType[FascicleLogType["Close"] = 8] = "Close";
        FascicleLogType[FascicleLogType["UDInsert"] = 16] = "UDInsert";
        FascicleLogType[FascicleLogType["UDReferenceInsert"] = 32] = "UDReferenceInsert";
        FascicleLogType[FascicleLogType["DocumentView"] = 64] = "DocumentView";
        FascicleLogType[FascicleLogType["UDDelete"] = 128] = "UDDelete";
        FascicleLogType[FascicleLogType["Error"] = 256] = "Error";
        FascicleLogType[FascicleLogType["DocumentInsert"] = 512] = "DocumentInsert";
        FascicleLogType[FascicleLogType["DocumentDelete"] = 1024] = "DocumentDelete";
        FascicleLogType[FascicleLogType["Workflow"] = 1024] = "Workflow";
        FascicleLogType[FascicleLogType["Authorize"] = 2048] = "Authorize";
        FascicleLogType[FascicleLogType["FolderInsert"] = 4096] = "FolderInsert";
        FascicleLogType[FascicleLogType["FolderUpdate"] = 8192] = "FolderUpdate";
        FascicleLogType[FascicleLogType["FolderDelete"] = 16384] = "FolderDelete";
    })(FascicleLogType || (FascicleLogType = {}));
    return FascicleLogType;
});
//# sourceMappingURL=FascicleLogType.js.map