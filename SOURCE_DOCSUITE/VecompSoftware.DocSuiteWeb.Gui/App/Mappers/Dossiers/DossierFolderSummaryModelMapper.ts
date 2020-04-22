import IMapper = require('App/Mappers/IMapper');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');

class DossierFolderSummaryModelMapper implements IMapper<DossierSummaryFolderViewModel>{
    constructor() {
    }
    //questo mapper serve a mappare da un dossierFolderModel a un dossierSummayFolderViewModel
    public Map(source: DossierFolderModel): DossierSummaryFolderViewModel {
        if (!source) {
            return null;
        }
        let toMap: DossierSummaryFolderViewModel = <DossierSummaryFolderViewModel>{};
        toMap.UniqueId = source.UniqueId
        toMap.Name = source.Name;
        toMap.Status = DossierFolderStatus[source.Status];
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.idFascicle = source.Fascicle ? source.Fascicle.UniqueId: null;
        toMap.idCategory = source.Category ? source.Category.EntityShortId : null;
        toMap.idRole = (source.DossierFolderRoles && source.DossierFolderRoles[0] && source.DossierFolderRoles[0].Role) ? source.DossierFolderRoles[0].Role.EntityShortId : null;
        return toMap;
    }
}
export = DossierFolderSummaryModelMapper;