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
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var DocumentUnitRoleMapper = /** @class */ (function (_super) {
        __extends(DocumentUnitRoleMapper, _super);
        function DocumentUnitRoleMapper() {
            return _super.call(this) || this;
        }
        DocumentUnitRoleMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.AssignUser = source.AssignUser;
            toMap.AuthorizationRoleType = source.AuthorizationRoleType;
            toMap.RoleLabel = source.RoleLabel;
            toMap.UniqueIdRole = source.UniqueIdRole;
            toMap.EntityShortId = source.EntityShortId;
            return toMap;
        };
        return DocumentUnitRoleMapper;
    }(BaseMapper));
    return DocumentUnitRoleMapper;
});
//# sourceMappingURL=DocumentUnitRoleMapper.js.map