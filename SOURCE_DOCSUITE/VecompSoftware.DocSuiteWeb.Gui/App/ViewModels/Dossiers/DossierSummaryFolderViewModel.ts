import CategoryModel = require('App/Models/Commons/CategoryModel');

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
}

export = DossierSummaryFolderViewModel