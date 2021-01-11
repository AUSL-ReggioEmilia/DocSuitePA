import FascicleFolderStatus = require("App/Models/Fascicles/FascicleFolderStatus");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascicleFolderTypology = require("App/Models/Fascicles/FascicleFolderTypology");

interface FascicleFolderModel {
    UniqueId: string;
    Name: string;
    Status: FascicleFolderStatus;
    Typology: FascicleFolderTypology;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate: Date;
    Category: CategoryModel;
    Fascicle: FascicleModel;
    ParentInsertId: string;
}

export = FascicleFolderModel;