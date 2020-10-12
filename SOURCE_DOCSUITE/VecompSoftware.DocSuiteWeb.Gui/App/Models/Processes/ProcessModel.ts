import CategoryModel = require("App/Models/Commons/CategoryModel");
import DossierModel = require("App/Models/Dossiers/DossierModel");
import RoleModel = require("App/Models/Commons/RoleModel");
import ProcessType = require("App/Models/Processes/ProcessType");

interface ProcessModel {
    UniqueId: string;
    Category: CategoryModel;
    Dossier: DossierModel;
    Name: string;
    StartDate: Date;
    EndDate: Date;
    Note: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;
    Roles: RoleModel[];
    ProcessType: ProcessType;
}

export = ProcessModel;