interface DocumentViewModel {
    CollaborationVersioningId: string;
    CollaborationId: string;
    FascicleId: string;
    DocumentId: string;
    ChainId: string;
    Name: string;
    FileName: string;
    FullName: string;
    Content: Uint8Array[];
    Mandatory: boolean;
    MandatorySelectable: boolean;
    PadesCompliant: boolean;
}

export = DocumentViewModel;