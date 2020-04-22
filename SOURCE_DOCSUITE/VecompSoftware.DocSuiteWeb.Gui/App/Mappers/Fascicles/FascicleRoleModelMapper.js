var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Mappers/Commons/RoleModelMapper", "App/Mappers/BaseMapper", "App/Mappers/Fascicles/FascicleModelMapper", "App/Helpers/RequireJSHelper"], function (require, exports, RoleModelMapper, BaseMapper, FascicleModelMapper, RequireJSHelper) {
    var FascicleRoleModelMapper = /** @class */ (function (_super) {
        __extends(FascicleRoleModelMapper, _super);
        function FascicleRoleModelMapper() {
            return _super.call(this) || this;
        }
        FascicleRoleModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.AuthorizationRoleType = source.AuthorizationRoleType;
            var roleMapper = new RoleModelMapper();
            toMap.Role = source.Role ? roleMapper.Map(source.Role) : null;
            var _fascicleModelMapper;
            _fascicleModelMapper = RequireJSHelper.getModule(FascicleModelMapper, 'App/Mappers/Fascicles/FascicleModelMapper');
            toMap.Fascicle = source.Fascicle ? _fascicleModelMapper.Map(source.Fascicle) : null;
            toMap.IsMaster = source.IsMaster;
            return toMap;
        };
        return FascicleRoleModelMapper;
    }(BaseMapper));
    return FascicleRoleModelMapper;
});
//# sourceMappingURL=FascicleRoleModelMapper.js.map