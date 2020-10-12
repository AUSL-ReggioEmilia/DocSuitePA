import ProcessFascicleTemplateModel = require('App/Models/Processes/ProcessFascicleTemplateModel');

interface DossierSummaryFolderViewModel {
    UniqueId: string;
    Status: string;
    Name: string;
    idFascicle: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate: Date;
    idCategory: number;
    idRole: number;
    idParent: string;
    FascicleTemplates: ProcessFascicleTemplateModel[];
    DossierFolders: DossierSummaryFolderViewModel[];
    JsonMetadata: string;
}

export = DossierSummaryFolderViewModel