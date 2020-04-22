define(["require", "exports"], function (require, exports) {
    var LocationTypeEnum;
    (function (LocationTypeEnum) {
        LocationTypeEnum[LocationTypeEnum["ProtLocation"] = 0] = "ProtLocation";
        LocationTypeEnum[LocationTypeEnum["ReslLocation"] = 1] = "ReslLocation";
        LocationTypeEnum[LocationTypeEnum["DocmLocation"] = 2] = "DocmLocation";
        LocationTypeEnum[LocationTypeEnum["DocumentSeriesLocation"] = 3] = "DocumentSeriesLocation";
        LocationTypeEnum[LocationTypeEnum["UDSLocation"] = 4] = "UDSLocation";
        LocationTypeEnum[LocationTypeEnum["DeskLocation"] = 5] = "DeskLocation";
    })(LocationTypeEnum || (LocationTypeEnum = {}));
    return LocationTypeEnum;
});
//# sourceMappingURL=LocationTypeEnum.js.map