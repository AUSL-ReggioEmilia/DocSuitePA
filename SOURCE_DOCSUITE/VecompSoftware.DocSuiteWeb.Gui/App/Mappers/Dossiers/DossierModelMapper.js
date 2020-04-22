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
define(["require", "exports", "./DossierFolderModelMapper", "App/Mappers/BaseMapper"], function (require, exports, DossierFolderModelMapper, BaseMapper) {
    var DossierModelMapper = /** @class */ (function (_super) {
        __extends(DossierModelMapper, _super);
        function DossierModelMapper() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        DossierModelMapper.prototype.construnctor = function () {
        };
        DossierModelMapper.prototype.Map = function (source) {
            var toMap = {};
            toMap.UniqueId = source.UniqueId;
            toMap.Year = source.Year;
            toMap.Number = source.Number;
            toMap.Subject = source.Subject;
            toMap.Note = source.Note;
            toMap.Container = source.Container;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.JsonMetadata = source.JsonMetadata;
            toMap.LastChangedUser = source.LastChangedUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.Contacts = source.Contacts;
            toMap.DossierRoles = source.DossierRoles;
            toMap.DossierDocuments = source.DossierDocuments;
            toMap.DossierFolders = source.DossierFolders ? new DossierFolderModelMapper().MapCollection(source.DossierFolders) : null;
            return toMap;
        };
        return DossierModelMapper;
    }(BaseMapper));
    return DossierModelMapper;
});
//# sourceMappingURL=DossierModelMapper.js.map