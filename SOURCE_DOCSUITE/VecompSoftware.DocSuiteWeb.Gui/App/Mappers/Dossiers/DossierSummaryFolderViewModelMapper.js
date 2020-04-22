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
    var DossierSummaryFolderViewModelMapper = /** @class */ (function (_super) {
        __extends(DossierSummaryFolderViewModelMapper, _super);
        function DossierSummaryFolderViewModelMapper() {
            return _super.call(this) || this;
        }
        //questo mapper serve a mappare il modello di dossierFolder che arriva dalle API in un 
        //dossierSummaryFolderViewModel
        DossierSummaryFolderViewModelMapper.prototype.Map = function (source) {
            if (!source) {
                return null;
            }
            var toMap = {};
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.Status = source.Status;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.idFascicle = source.IdFascicle;
            toMap.idCategory = source.Category ? source.Category.EntityShortId : source.IdCategory;
            toMap.idRole = (source.DossierFolderRoles && source.DossierFolderRoles[0] && source.DossierFolderRoles[0].Role) ? source.DossierFolderRoles[0].Role.EntityShortId : source.IdRole;
            return toMap;
        };
        return DossierSummaryFolderViewModelMapper;
    }(BaseMapper));
    return DossierSummaryFolderViewModelMapper;
});
//# sourceMappingURL=DossierSummaryFolderViewModelMapper.js.map