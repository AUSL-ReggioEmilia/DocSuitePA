define(["require", "exports"], function (require, exports) {
    var ProcessNodeType;
    (function (ProcessNodeType) {
        ProcessNodeType[ProcessNodeType["Root"] = 0] = "Root";
        ProcessNodeType[ProcessNodeType["Category"] = 1] = "Category";
        ProcessNodeType[ProcessNodeType["Process"] = 2] = "Process";
        ProcessNodeType[ProcessNodeType["DossierFolder"] = 3] = "DossierFolder";
        ProcessNodeType[ProcessNodeType["ProcessFascicleTemplate"] = 4] = "ProcessFascicleTemplate";
        ProcessNodeType[ProcessNodeType["TreeRootNode"] = 5] = "TreeRootNode";
    })(ProcessNodeType || (ProcessNodeType = {}));
    return ProcessNodeType;
});
//# sourceMappingURL=ProcessNodeType.js.map