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
define(["require", "exports", "App/Mappers/Commons/RoleModelMapper", "App/Mappers/BaseMapper"], function (require, exports, RoleModelMapper, BaseMapper) {
    var DossierFolderRoleModelMapper = /** @class */ (function (_super) {
        __extends(DossierFolderRoleModelMapper, _super);
        function DossierFolderRoleModelMapper() {
            return _super.call(this) || this;
        }
        DossierFolderRoleModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.AuthorizationRoleType = source.AuthorizationRoleType;
            toMap.IsMaster = source.IsMaster;
            toMap.Status = source.Status;
            toMap.Role = source.Role ? new RoleModelMapper().Map(source.Role) : null;
            return toMap;
        };
        return DossierFolderRoleModelMapper;
    }(BaseMapper));
    return DossierFolderRoleModelMapper;
});
//# sourceMappingURL=DossierFolderRoleModelMapper.js.map