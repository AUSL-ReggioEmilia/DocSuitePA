import ProcessModel = require("App/Models/Processes/ProcessModel");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import WorkflowRepositoryModel = require("App/Models/Workflows/WorkflowRepositoryModel");

interface ProcessFascicleWorkflowRepositoryModel {
    UniqueId: string;
    Process: ProcessModel;
    DossierFolder: DossierFolderModel;
    WorkflowRepository: WorkflowRepositoryModel;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;
}

export = ProcessFascicleWorkflowRepositoryModel;