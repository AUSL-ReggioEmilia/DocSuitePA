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
    var RoleModelMapper = /** @class */ (function (_super) {
        __extends(RoleModelMapper, _super);
        function RoleModelMapper() {
            return _super.call(this) || this;
        }
        RoleModelMapper.prototype.Map = function (source) {
            var _this = this;
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityShortId = source.EntityShortId;
            toMap.Name = source.Name;
            toMap.IdRoleTenant = source.IdRoleTenant;
            toMap.TenantId = source.TenantId;
            toMap.IdRole = source.IdRole ? source.IdRole : source.EntityShortId;
            toMap.FullIncrementalPath = source.FullIncrementalPath;
            toMap.IsActive = source.IsActive;
            toMap.IdRoleFather = source.Father ? source.Father.EntityShortId : source.IdRoleFather ? source.IdRoleFather : null;
            toMap.UniqueId = source.UniqueId;
            toMap.Children = source.Children;
            toMap.ServiceCode = source.ServiceCode;
            toMap.ActiveFrom = source.ActiveFrom ? moment(source.ActiveFrom).format("DD/MM/YYYY") : "";
            toMap.Children = source.Children && source.Children.length ? source.Children.map(function (childRole) { return _this.Map(childRole); }) : [];
            return toMap;
        };
        return RoleModelMapper;
    }(BaseMapper));
    return RoleModelMapper;
});
//# sourceMappingURL=RoleModelMapper.js.map