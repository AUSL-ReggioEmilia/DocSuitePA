import DocumentFolderViewModel = require("App/ViewModels/SignDocuments/DocumentFolderViewModel");
import DocumentSignBehaviour = require("App/ViewModels/SignDocuments/DocumentSignBehaviour");

interface DocumentRootFolderViewModel {
    Id: number;
    Name: string;
    UniqueId: string;
    SignBehaviour: DocumentSignBehaviour;
    DocumentFolders: DocumentFolderViewModel[]
    PageUrl: string;
    CommentVisibile: boolean;
    CommentChecked: boolean;
    Comment: string;
    MainDocumentIsPadesCompliant: boolean;
}

export = DocumentRootFolderViewModel;