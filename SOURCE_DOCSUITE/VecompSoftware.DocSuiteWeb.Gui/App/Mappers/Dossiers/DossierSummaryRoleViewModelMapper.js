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
define(["require", "exports", "App/Mappers/BaseMapper", "App/ViewModels/BaseEntityRoleViewModel"], function (require, exports, BaseMapper, BaseEntityRoleViewModel) {
    var DossierSummaryRoleViewModelMapper = /** @class */ (function (_super) {
        __extends(DossierSummaryRoleViewModelMapper, _super);
        function DossierSummaryRoleViewModelMapper() {
            return _super.call(this) || this;
        }
        DossierSummaryRoleViewModelMapper.prototype.Map = function (source) {
            var toMap = new BaseEntityRoleViewModel();
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.EntityShortId = source.Role.IdRole ? source.Role.IdRole : null;
            toMap.Name = source.Role.Name;
            toMap.Type = source.Type;
            toMap.IsMaster = source.IsMaster;
            toMap.IsActive = source.IsActive;
            return toMap;
        };
        return DossierSummaryRoleViewModelMapper;
    }(BaseMapper));
    return DossierSummaryRoleViewModelMapper;
});
//# sourceMappingURL=DossierSummaryRoleViewModelMapper.js.map