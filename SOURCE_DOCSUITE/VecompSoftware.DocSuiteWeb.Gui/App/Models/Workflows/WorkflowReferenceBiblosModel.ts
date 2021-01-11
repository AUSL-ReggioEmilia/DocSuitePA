import ChainType = require("../DocumentUnits/ChainType");

interface WorkflowReferenceBiblosModel {
    DocumentName: string;
    ArchiveName: string;
    ChainType: ChainType;
    ArchiveChainId: string;
    ArchiveDocumentId: string;
    ReferenceDocument: WorkflowReferenceBiblosModel;
}

export = WorkflowReferenceBiblosModel;