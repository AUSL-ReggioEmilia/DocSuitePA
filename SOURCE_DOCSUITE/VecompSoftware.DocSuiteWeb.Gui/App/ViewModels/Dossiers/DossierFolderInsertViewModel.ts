import FascicleModel = require("App/Models/Fascicles/FascicleModel");

class DossierFolderInsertViewModel {
    constructor() {
        this.Roles = [];
        this.Children = [];
    }

    Name: string;
    Children: DossierFolderInsertViewModel[];
    Roles: number[];
    Fascicle: FascicleModel;
}

export = DossierFolderInsertViewModel;