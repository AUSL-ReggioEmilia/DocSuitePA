import IMapper = require('App/Mappers/IMapper');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');

class FascicleSummaryFolderViewModelMapper extends BaseMapper<FascicleSummaryFolderViewModel>{
    constructor() {
        super();
    }
    //questo mapper serve a mappare il modello di dossierFolder che arriva dalle API in un 
    //dossierSummaryFolderViewModel
    public Map(source: any): FascicleSummaryFolderViewModel {
        if (!source) {
            return null;
        }
        let toMap: FascicleSummaryFolderViewModel = <FascicleSummaryFolderViewModel>{};
        toMap.UniqueId = source.UniqueId
        toMap.Name = source.Name;
        toMap.Status = source.Status;
        toMap.Typology = source.Typology;
        toMap.idFascicle = source.IdFascicle;
        toMap.idCategory = source.Category? source.Category.EntityShortId : source.IdCategory;
        toMap.hasChildren = source.HasChildren;
        toMap.hasDocuments = source.HasDocuments;
        toMap.FascicleFolderLevel = source.FascicleFolderLevel;
        toMap.FascicleDocuments = source.FascicleDocuments;
        return toMap;
    }


}
    export = FascicleSummaryFolderViewModelMapper;