import IMapper = require("App/Mappers/IMapper");
import ProcessFascicleTemplateModel = require("App/Models/Processes/ProcessFascicleTemplateModel");
import ProcessModelMapper = require("App/Mappers/Processes/ProcessModelMapper");
import DossierFolderModelMapper = require("App/Mappers/Dossiers/DossierFolderModelMapper");
import BaseMapper = require("App/Mappers/BaseMapper");

class ProcessFascicleTemplateModelMapper extends BaseMapper<ProcessFascicleTemplateModel> implements IMapper<ProcessFascicleTemplateModel>{
    constructor() {
        super();
    }
    public Map(source: any): ProcessFascicleTemplateModel {
        let toMap: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Process = source.Process ? new ProcessModelMapper().Map(source.Process) : null;
        toMap.DossierFolder = source.DossierFolder ? new DossierFolderModelMapper().Map(source.DossierFolder) : null;
        toMap.Name = source.Name;
        toMap.JsonModel = source.JsonModel;
        toMap.StartDate = source.StartDate;
        toMap.EndDate = source.EndDate;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        return toMap;
    }
}
export = ProcessFascicleTemplateModelMapper;