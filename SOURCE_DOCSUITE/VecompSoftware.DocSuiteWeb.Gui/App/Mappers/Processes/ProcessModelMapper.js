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
define(["require", "exports", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Dossiers/DossierModelMapper", "App/Mappers/BaseMapper", "../Commons/RoleModelMapper"], function (require, exports, CategoryModelMapper, DossierModelMapper, BaseMapper, RoleModelMapper) {
    var ProcessModelMapper = /** @class */ (function (_super) {
        __extends(ProcessModelMapper, _super);
        function ProcessModelMapper() {
            return _super.call(this) || this;
        }
        ProcessModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Dossier = source.Dossier ? new DossierModelMapper().Map(source.Dossier) : null;
            toMap.Name = source.Name;
            toMap.FascicleType = source.FascicleType;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.Note = source.Note;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.Roles = source.Roles ? new RoleModelMapper().MapCollection(source.Roles) : null;
            toMap.ProcessType = source.ProcessType;
            return toMap;
        };
        return ProcessModelMapper;
    }(BaseMapper));
    return ProcessModelMapper;
});
//# sourceMappingURL=ProcessModelMapper.js.map