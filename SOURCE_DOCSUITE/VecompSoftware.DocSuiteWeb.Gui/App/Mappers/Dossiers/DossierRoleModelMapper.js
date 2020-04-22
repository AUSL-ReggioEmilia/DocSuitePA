define(["require", "exports"], function (require, exports) {
    var DossierRoleModelMapper = /** @class */ (function () {
        function DossierRoleModelMapper() {
        }
        DossierRoleModelMapper.prototype.construnctor = function () {
        };
        DossierRoleModelMapper.prototype.Map = function (source) {
            var toMap = {};
            toMap.UniqueId = source.UniqueId;
            toMap.AuthorizationRoleType = source.AuthorizationRoleType;
            return toMap;
        };
        return DossierRoleModelMapper;
    }());
    return DossierRoleModelMapper;
});
//# sourceMappingURL=DossierRoleModelMapper.js.map