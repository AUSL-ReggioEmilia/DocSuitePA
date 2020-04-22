import CategoryModel = require('App/Models/Commons/CategoryModel');

interface FascicleSummaryFolderViewModel {
    UniqueId: string;
    Status: string;
    Typology: string;
    Name: string;
    idFascicle: string;
    idCategory: number;
    hasDocuments: boolean;
    hasChildren: boolean;   
    idParent: boolean;
    FascicleFolderLevel: number;
}

export = FascicleSummaryFolderViewModel