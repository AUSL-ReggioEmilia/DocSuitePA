define(["require", "exports", "App/Models/Dossiers/DossierFolderStatus"], function (require, exports, DossierFolderStatus) {
    var DossierFolderSummaryModelMapper = /** @class */ (function () {
        function DossierFolderSummaryModelMapper() {
        }
        //questo mapper serve a mappare da un dossierFolderModel a un dossierSummayFolderViewModel
        DossierFolderSummaryModelMapper.prototype.Map = function (source) {
            if (!source) {
                return null;
            }
            var toMap = {};
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.Status = DossierFolderStatus[source.Status];
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.idFascicle = source.Fascicle ? source.Fascicle.UniqueId : null;
            toMap.idCategory = source.Category ? source.Category.EntityShortId : null;
            toMap.idRole = (source.DossierFolderRoles && source.DossierFolderRoles[0] && source.DossierFolderRoles[0].Role) ? source.DossierFolderRoles[0].Role.EntityShortId : null;
            return toMap;
        };
        return DossierFolderSummaryModelMapper;
    }());
    return DossierFolderSummaryModelMapper;
});
//# sourceMappingURL=DossierFolderSummaryModelMapper.js.map