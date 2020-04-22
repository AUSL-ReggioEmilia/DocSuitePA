define(["require", "exports"], function (require, exports) {
    var BuildActionType;
    (function (BuildActionType) {
        BuildActionType[BuildActionType["None"] = 0] = "None";
        BuildActionType[BuildActionType["Build"] = 1] = "Build";
        BuildActionType[BuildActionType["Director"] = 2] = "Director";
        BuildActionType[BuildActionType["Evaluate"] = 4] = "Evaluate";
        BuildActionType[BuildActionType["Synchronize"] = 8] = "Synchronize";
        BuildActionType[BuildActionType["Destroy"] = 16] = "Destroy";
    })(BuildActionType || (BuildActionType = {}));
    return BuildActionType;
});
//# sourceMappingURL=BuildActionType.js.map