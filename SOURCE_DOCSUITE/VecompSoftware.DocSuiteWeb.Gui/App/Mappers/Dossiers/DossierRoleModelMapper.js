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
            toMap.IsMaster = source.IsMaster;
            toMap.EntityShortId = source.EntityShortId;
            toMap.Dossier = source.Dossier;
            toMap.Status = source.Status;
            return toMap;
        };
        return DossierRoleModelMapper;
    }());
    return DossierRoleModelMapper;
});
//# sourceMappingURL=DossierRoleModelMapper.js.map