import DocumentUnitModel = require("./DocumentUnitModel");
import ChainType = require("./ChainType");

class DocumentUnitChainModel {
    UniqueId: string;
    DocumentName: string;
    IdArchiveChain: string;
    ArchiveName: string;
    ChainType: ChainType;
    DocumentLabel: string;
    DocumentUnit: DocumentUnitModel;
}

export = DocumentUnitChainModel;