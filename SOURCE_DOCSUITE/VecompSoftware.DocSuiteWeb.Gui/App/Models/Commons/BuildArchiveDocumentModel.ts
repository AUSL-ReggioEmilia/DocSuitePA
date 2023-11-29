
interface BuildArchiveDocumentModel {
    IdLegacyChain?: number;
    IdChain?: string;
    IdDocument: string;
    Name: string;
    Signature: string;
    Archive: string;
    Size?: number;
    Version: number;
    ContentStream: string;
}

export = BuildArchiveDocumentModel;