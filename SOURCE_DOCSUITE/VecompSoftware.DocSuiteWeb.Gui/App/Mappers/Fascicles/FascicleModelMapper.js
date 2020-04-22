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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Mappers/Commons/CategoryModelMapper", "App/Mappers/Commons/ContactModelMapper", "App/Mappers/Fascicles/FascicleRoleModelMapper", "App/Mappers/Fascicles/FascicleDocumentModelMapper", "App/Mappers/Fascicles/FascicleLinkModelMapper", "App/Mappers/Fascicles/FascicleDocumentUnitModelMapper", "App/Helpers/RequireJSHelper", "App/Mappers/Commons/ContainerModelMapper", "../Dossiers/DossierFolderModelMapper"], function (require, exports, BaseMapper, CategoryModelMapper, ContactModelMapper, FascicleRoleModelMapper, FascicleDocumentModelMapper, FascicleLinkModelMapper, FascicleDocumentUnitModelMapper, RequireJSHelper, ContainerModelMapper, DossierFolderModelMapper) {
    var FascicleModelMapper = /** @class */ (function (_super) {
        __extends(FascicleModelMapper, _super);
        function FascicleModelMapper() {
            return _super.call(this) || this;
        }
        FascicleModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            var _fascicleDocumentUnitModelMapper = RequireJSHelper.getModule(FascicleDocumentUnitModelMapper, 'App/Mappers/Fascicles/FascicleDocumentUnitModelMapper');
            var _fascicleDocumentModelMapper = RequireJSHelper.getModule(FascicleDocumentModelMapper, 'App/Mappers/Fascicles/FascicleDocumentModelMapper');
            toMap.Category = source.Category ? new CategoryModelMapper().Map(source.Category) : null;
            toMap.Container = source.Container ? new ContainerModelMapper().Map(source.Container) : null;
            toMap.Conservation = source.Conservation;
            toMap.Contacts = source.Contacts ? new ContactModelMapper().MapCollection(source.Contacts) : null;
            toMap.EndDate = source.EndDate;
            toMap.FascicleDocuments = source.FascicleDocuments ? _fascicleDocumentModelMapper.MapCollection(source.FascicleDocuments) : null;
            toMap.FascicleDocumentUnits = source.FascicleDocumentUnits ? _fascicleDocumentUnitModelMapper.MapCollection(source.FascicleDocumentUnits) : null;
            toMap.FascicleLinks = source.FascicleLinks ? new FascicleLinkModelMapper().MapCollection(source.FascicleLinks) : null;
            toMap.FascicleObject = source.FascicleObject;
            toMap.FascicleRoles = source.FascicleRoles ? new FascicleRoleModelMapper().MapCollection(source.FascicleRoles) : null;
            toMap.FascicleType = source.FascicleType;
            toMap.LinkedFascicles = source.LinkedFascicles ? new FascicleLinkModelMapper().MapCollection(source.LinkedFascicles) : null;
            toMap.DossierFolders = source.DossierFolders ? new DossierFolderModelMapper().MapCollection(source.DossierFolders) : null;
            toMap.Manager = source.Manager;
            toMap.Name = source.Name;
            toMap.Note = source.Note;
            toMap.Number = source.Number;
            toMap.Rack = source.Rack;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.StartDate = source.StartDate;
            toMap.Title = source.Title;
            toMap.UniqueId = source.UniqueId;
            toMap.VisibilityType = source.VisibilityType;
            toMap.Year = source.Year;
            toMap.MetadataValues = source.MetadataValues;
            return toMap;
        };
        return FascicleModelMapper;
    }(BaseMapper));
    return FascicleModelMapper;
});
//# sourceMappingURL=FascicleModelMapper.js.map