define(["require", "exports"], function (require, exports) {
    var DossierFolderStatus;
    (function (DossierFolderStatus) {
        DossierFolderStatus[DossierFolderStatus["InProgress"] = 1] = "InProgress";
        DossierFolderStatus[DossierFolderStatus["Fascicle"] = 2] = "Fascicle";
        DossierFolderStatus[DossierFolderStatus["FascicleClose"] = 4] = "FascicleClose";
        DossierFolderStatus[DossierFolderStatus["Folder"] = 8] = "Folder";
        DossierFolderStatus[DossierFolderStatus["DoAction"] = 16] = "DoAction";
    })(DossierFolderStatus || (DossierFolderStatus = {}));
    return DossierFolderStatus;
});
//# sourceMappingURL=DossierFolderStatus.js.map