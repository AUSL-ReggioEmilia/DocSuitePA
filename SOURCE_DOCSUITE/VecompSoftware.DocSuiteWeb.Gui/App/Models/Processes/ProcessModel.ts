import CategoryModel = require("App/Models/Commons/CategoryModel");
import DossierModel = require("App/Models/Dossiers/DossierModel");
import FascicleType = require("App/Models/Fascicles/FascicleType");
import RoleModel = require("App/Models/Commons/RoleModel");
import ProcessType = require("App/Models/Processes/ProcessType");

interface ProcessModel {
    UniqueId: string;
    Category: CategoryModel;
    Dossier: DossierModel;
    Name: string;
    FascicleType: FascicleType;
    StartDate: Date;
    EndDate: Date;
    Note: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;
    Roles: RoleModel[];
    ProcessType: string;
}

export = ProcessModel;