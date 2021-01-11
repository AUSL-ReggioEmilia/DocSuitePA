import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import ProcessFascicleTemplateModelMapper = require('App/Mappers/Processes/ProcessFascicleTemplateModelMapper');

class DossierSummaryFolderViewModelMapper extends BaseMapper<DossierSummaryFolderViewModel>{
    constructor() {
        super();
    }
    //questo mapper serve a mappare il modello di dossierFolder che arriva dalle API in un 
    //dossierSummaryFolderViewModel
    public Map(source: any): DossierSummaryFolderViewModel {
        if (!source) {
            return null;
        }
        let toMap: DossierSummaryFolderViewModel = <DossierSummaryFolderViewModel>{};
        toMap.UniqueId = source.UniqueId
        toMap.Name = source.Name;
        toMap.Status = source.Status;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.LastChangedDate = source.LastChangedDate;
        toMap.idFascicle = source.IdFascicle;
        toMap.idCategory = source.Category? source.Category.EntityShortId : source.IdCategory;
        toMap.idRole = (source.DossierFolderRoles && source.DossierFolderRoles[0] && source.DossierFolderRoles[0].Role) ? source.DossierFolderRoles[0].Role.EntityShortId : source.IdRole;
        toMap.DossierFolders = source.DossierFolders ? this.MapCollection(source.DossierFolders) : null;
        toMap.FascicleTemplates = source.FascicleTemplates ? new ProcessFascicleTemplateModelMapper().MapCollection(source.FascicleTemplates) : null;
        toMap.JsonMetadata = source.JsonMetadata;
        return toMap;
    }


}
    export = DossierSummaryFolderViewModelMapper;