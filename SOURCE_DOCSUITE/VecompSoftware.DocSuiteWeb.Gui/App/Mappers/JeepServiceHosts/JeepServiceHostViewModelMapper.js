define(["require", "exports"], function (require, exports) {
    var JeepServiceHostViewModelMapper = /** @class */ (function () {
        function JeepServiceHostViewModelMapper() {
        }
        JeepServiceHostViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Hostname = source.Hostname;
            toMap.IsActive = source.IsActive;
            toMap.IsDefault = source.IsDefault;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            return toMap;
        };
        return JeepServiceHostViewModelMapper;
    }());
    return JeepServiceHostViewModelMapper;
});
//# sourceMappingURL=JeepServiceHostViewModelMapper.js.map