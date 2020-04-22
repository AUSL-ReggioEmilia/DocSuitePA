define(["require", "exports"], function (require, exports) {
    var ProcessNodeType;
    (function (ProcessNodeType) {
        ProcessNodeType[ProcessNodeType["Root"] = 0] = "Root";
        ProcessNodeType[ProcessNodeType["Process"] = 1] = "Process";
        ProcessNodeType[ProcessNodeType["DossierFolder"] = 2] = "DossierFolder";
        ProcessNodeType[ProcessNodeType["ProcessFascicleTemplate"] = 3] = "ProcessFascicleTemplate";
    })(ProcessNodeType || (ProcessNodeType = {}));
    return ProcessNodeType;
});
//# sourceMappingURL=ProcessNodeType.js.map