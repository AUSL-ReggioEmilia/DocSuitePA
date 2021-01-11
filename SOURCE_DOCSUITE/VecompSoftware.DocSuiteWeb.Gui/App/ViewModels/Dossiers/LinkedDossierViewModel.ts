import CategoryModel = require("App/Models/Commons/CategoryModel");


interface LinkedDossierViewModel {
    UniqueId: string;
    DossierFolderName: string;
    DossierName: string;
    StartDate: string;
    Contenitori: string;
    Subject: string;
    Category: string;
}

export = LinkedDossierViewModel;