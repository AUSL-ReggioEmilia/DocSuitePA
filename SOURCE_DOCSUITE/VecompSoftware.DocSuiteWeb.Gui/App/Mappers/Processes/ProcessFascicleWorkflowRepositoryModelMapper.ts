import IMapper = require("App/Mappers/IMapper");
import ProcessFascicleWorkflowRepositoryModel = require("App/Models/Processes/ProcessFascicleWorkflowRepositoryModel");
import ProcessModelMapper = require("App/Mappers/Processes/ProcessModelMapper");
import DossierFolderModelMapper = require("App/Mappers/Dossiers/DossierFolderModelMapper");
import WorkflowRepositoryModelMapper = require("App/Mappers/Workflows/WorkflowRepositoryModelMapper");

class ProcessFascicleWorkflowRepositoryModelMapper implements IMapper<ProcessFascicleWorkflowRepositoryModel>{
    constructor() {
    }
    public Map(source: any): ProcessFascicleWorkflowRepositoryModel {
        let toMap: ProcessFascicleWorkflowRepositoryModel = <ProcessFascicleWorkflowRepositoryModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.Process = source.Process ? new ProcessModelMapper().Map(source.Process) : null;
        toMap.DossierFolder = source.DossierFolder ? new DossierFolderModelMapper().Map(source.DossierFolder) : null;
        toMap.WorkflowRepository = source.WorkflowRepository ? new WorkflowRepositoryModelMapper().Map(source.WorkflowRepository) : null;
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.LastChangedUser = source.LastChangedUser;
        toMap.LastChangedDate = source.LastChangedDate;
        return toMap;
    }
}

export = ProcessFascicleWorkflowRepositoryModelMapper;