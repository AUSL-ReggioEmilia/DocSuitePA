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
define(["require", "exports", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Dossiers/DossierModelMapper", "App/Mappers/Fascicles/FascicleModelMapper", "App/Mappers/Dossiers/DossierFolderRoleModelMapper", "App/Mappers/BaseMapper"], function (require, exports, CategoryModelMapper, DossierModelMapper, FascicleModelMapper, DossierFolderRoleModelMapper, BaseMapper) {
    var DossierFolderModelMapper = /** @class */ (function (_super) {
        __extends(DossierFolderModelMapper, _super);
        function DossierFolderModelMapper() {
            return _super.call(this) || this;
        }
        DossierFolderModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.Status = source.Status;
            toMap.JsonMetadata = source.JsonMetadata;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.Category = source.CAtegory ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Dossier = source.Dossier ? new DossierModelMapper().Map(source.Dossier) : null;
            toMap.Fascicle = source.Fascicle ? new FascicleModelMapper().Map(source.Fascicle) : null;
            toMap.DossierFolderRoles = source.DossierFolderRoles ? new DossierFolderRoleModelMapper().MapCollection(source.DossierFolderRoles) : null;
            toMap.DossierFolders = source.DossierFolders ? new DossierFolderModelMapper().MapCollection(source.DossierFolders) : null;
            toMap.ParentInsertId = source.ParentInsertId;
            return toMap;
        };
        return DossierFolderModelMapper;
    }(BaseMapper));
    return DossierFolderModelMapper;
});
//# sourceMappingURL=DossierFolderModelMapper.js.map