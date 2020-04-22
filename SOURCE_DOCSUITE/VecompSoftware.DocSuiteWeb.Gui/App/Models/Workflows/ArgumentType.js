define(["require", "exports"], function (require, exports) {
    var ArgumentType;
    (function (ArgumentType) {
        ArgumentType[ArgumentType["Json"] = 1] = "Json";
        ArgumentType[ArgumentType["RelationGuid"] = 2] = "RelationGuid";
        ArgumentType[ArgumentType["RelationInt"] = 4] = "RelationInt";
        ArgumentType[ArgumentType["PropertyString"] = 8] = "PropertyString";
        ArgumentType[ArgumentType["PropertyInt"] = 16] = "PropertyInt";
        ArgumentType[ArgumentType["PropertyDate"] = 32] = "PropertyDate";
        ArgumentType[ArgumentType["PropertyDouble"] = 64] = "PropertyDouble";
        ArgumentType[ArgumentType["PropertyBoolean"] = 128] = "PropertyBoolean";
        ArgumentType[ArgumentType["PropertyGuid"] = 256] = "PropertyGuid";
    })(ArgumentType || (ArgumentType = {}));
    return ArgumentType;
});
//# sourceMappingURL=ArgumentType.js.map