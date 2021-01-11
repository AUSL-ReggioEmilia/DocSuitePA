interface DocumentModel {
    Segnature: string;
    FileName: string;
    ChainId?: string;
    DocumentId?: string;
    LegacyDocumentId?: number;
    ContentStream: string;
}

export = DocumentModel;