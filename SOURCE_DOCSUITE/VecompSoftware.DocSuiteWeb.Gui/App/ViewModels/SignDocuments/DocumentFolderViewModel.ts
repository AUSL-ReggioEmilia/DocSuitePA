import DocumentViewModel = require("App/ViewModels/SignDocuments/DocumentViewModel");
import ChainType = require("App/Models/DocumentUnits/ChainType");


interface DocumentFolderViewModel {
    Name: string;
    ChainType: ChainType;
    Documents: DocumentViewModel[];
}

export = DocumentFolderViewModel;