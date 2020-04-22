define(["require", "exports"], function (require, exports) {
    var TenantMapper = /** @class */ (function () {
        function TenantMapper() {
        }
        TenantMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.TenantName = source.TenantName;
            toMap.CompanyName = source.CompanyName;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.Note = source.Note;
            return toMap;
        };
        return TenantMapper;
    }());
    return TenantMapper;
});
//# sourceMappingURL=TenantMapper.js.map