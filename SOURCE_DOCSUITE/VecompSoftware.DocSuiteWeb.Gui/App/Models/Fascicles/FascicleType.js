define(["require", "exports"], function (require, exports) {
    var FascicleType;
    (function (FascicleType) {
        FascicleType[FascicleType["Legacy"] = -1] = "Legacy";
        FascicleType[FascicleType["SubFascicle"] = 0] = "SubFascicle";
        FascicleType[FascicleType["Procedure"] = 1] = "Procedure";
        FascicleType[FascicleType["Period"] = 2] = "Period";
        FascicleType[FascicleType["Activity"] = 4] = "Activity";
    })(FascicleType || (FascicleType = {}));
    return FascicleType;
});
//# sourceMappingURL=FascicleType.js.map