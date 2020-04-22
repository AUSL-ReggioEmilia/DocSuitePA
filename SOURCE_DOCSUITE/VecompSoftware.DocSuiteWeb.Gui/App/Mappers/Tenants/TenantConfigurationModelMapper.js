define(["require", "exports"], function (require, exports) {
    var TenantConfigurationModelMapper = /** @class */ (function () {
        function TenantConfigurationModelMapper() {
        }
        TenantConfigurationModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Tenant = source.Tenant;
            toMap.ConfigurationType = source.ConfigurationType;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.Note = source.Note;
            toMap.JsonValue = source.JsonValue;
            return toMap;
        };
        return TenantConfigurationModelMapper;
    }());
    return TenantConfigurationModelMapper;
});
//# sourceMappingURL=TenantConfigurationModelMapper.js.map