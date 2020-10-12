import DossierFolderStatus = require("App/Models/Dossiers/DossierFolderStatus");
import DossierModel = require("App/Models/Dossiers/DossierModel");
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import DossierFolderRoleModel = require("App/Models/Dossiers/DossierFolderRoleModel");
import ProcessFascicleTemplateModel = require("App/Models/Processes/ProcessFascicleTemplateModel");

interface DossierFolderModel {
    UniqueId: string;
    Name: string;
    Status: DossierFolderStatus;
    JsonMetadata: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate: Date;
    Category: CategoryModel;
    Dossier: DossierModel;
    Fascicle: FascicleModel;
    DossierFolderRoles: DossierFolderRoleModel[];
    ParentInsertId: string;
    DossierFolders: DossierFolderModel[];
    FascicleTemplates: ProcessFascicleTemplateModel[];
    DossierFolderPath: string;
    DossierFolderLevel: number;
}

export = DossierFolderModel;