import ProcessModel = require("App/Models/Processes/ProcessModel");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");

interface ProcessFascicleTemplateModel {
    UniqueId: string;
    Process: ProcessModel;
    DossierFolder: DossierFolderModel;
    Name: string;
    JsonModel: string;
    StartDate: Date;
    EndDate: Date;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;
}

export = ProcessFascicleTemplateModel;