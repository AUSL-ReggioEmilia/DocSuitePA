define(["require", "exports"], function (require, exports) {
    var TreeNodeType;
    (function (TreeNodeType) {
        TreeNodeType[TreeNodeType["Category"] = 0] = "Category";
        TreeNodeType[TreeNodeType["DocumentUnit"] = 1] = "DocumentUnit";
        TreeNodeType[TreeNodeType["Process"] = 2] = "Process";
        TreeNodeType[TreeNodeType["DocumentUnitChain"] = 3] = "DocumentUnitChain";
        TreeNodeType[TreeNodeType["DossierFolder"] = 4] = "DossierFolder";
        TreeNodeType[TreeNodeType["FascicleFolder"] = 5] = "FascicleFolder";
        TreeNodeType[TreeNodeType["Fascicle"] = 6] = "Fascicle";
        TreeNodeType[TreeNodeType["File"] = 7] = "File";
        TreeNodeType[TreeNodeType["MiscellaneousFolder"] = 8] = "MiscellaneousFolder";
        TreeNodeType[TreeNodeType["Root"] = 9] = "Root";
    })(TreeNodeType || (TreeNodeType = {}));
    return TreeNodeType;
});
//# sourceMappingURL=TreeNodeType.js.map