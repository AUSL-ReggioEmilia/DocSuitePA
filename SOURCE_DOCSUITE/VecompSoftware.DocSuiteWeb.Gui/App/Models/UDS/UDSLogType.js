define(["require", "exports"], function (require, exports) {
    var UDSLogType;
    (function (UDSLogType) {
        UDSLogType[UDSLogType["Insert"] = 0] = "Insert";
        UDSLogType[UDSLogType["Modify"] = 1] = "Modify";
        UDSLogType[UDSLogType["AuthorizationModify"] = 2] = "AuthorizationModify";
        UDSLogType[UDSLogType["DocumentModify"] = 4] = "DocumentModify";
        UDSLogType[UDSLogType["ObjectModify"] = 8] = "ObjectModify";
        UDSLogType[UDSLogType["Delete"] = 16] = "Delete";
        UDSLogType[UDSLogType["DocumentView"] = 32] = "DocumentView";
        UDSLogType[UDSLogType["SummaryView"] = 64] = "SummaryView";
        UDSLogType[UDSLogType["AuthorizationInsert"] = 128] = "AuthorizationInsert";
        UDSLogType[UDSLogType["AuthorizationDelete"] = 256] = "AuthorizationDelete";
    })(UDSLogType || (UDSLogType = {}));
    return UDSLogType;
});
//# sourceMappingURL=UDSLogType.js.map