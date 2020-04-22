import DossierFolderInsertViewModel = require('App/ViewModels/Dossiers/DossierFolderInsertViewModel');

class DossierInsertViewModel {
    constructor() {
        this.Folders = [];
        this.MetadataRestrictions = [];
    }

    Subject: string;
    Note: string;
    Folders: DossierFolderInsertViewModel[];
    MetadataRestrictions: string[];
    SetMetadataReadOnly: boolean;
}

export = DossierInsertViewModel;