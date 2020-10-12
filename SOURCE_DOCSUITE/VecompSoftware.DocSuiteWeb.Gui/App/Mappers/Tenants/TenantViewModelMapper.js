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
define(["require", "exports", "../BaseMapper", "./TenantAOOModelMapper"], function (require, exports, BaseMapper, TenantAOOModelMapper) {
    var TenantViewModelMapper = /** @class */ (function (_super) {
        __extends(TenantViewModelMapper, _super);
        function TenantViewModelMapper() {
            return _super.call(this) || this;
        }
        TenantViewModelMapper.prototype.Map = function (source) {
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
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.TenantAOO = source.TenantAOO ? new TenantAOOModelMapper().Map(source.TenantAOO) : null;
            toMap.TenantTypology = source.TenantTypology;
            return toMap;
        };
        return TenantViewModelMapper;
    }(BaseMapper));
    return TenantViewModelMapper;
});
//# sourceMappingURL=TenantViewModelMapper.js.map