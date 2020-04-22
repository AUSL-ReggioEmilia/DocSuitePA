import BaseMapper = require('App/Mappers/BaseMapper');
import DocumentUnitChainModel = require('App/Models/DocumentUnits/DocumentUnitChainModel');

class DocumentUnitChainMapper extends BaseMapper<DocumentUnitChainModel> {
    constructor() {
        super();
    }

    public Map(source: any): DocumentUnitChainModel {
        let toMap: DocumentUnitChainModel = <DocumentUnitChainModel>{};

        if (!source) {
            return null;
        }

        toMap.ArchiveName = source.ArchiveName;
        toMap.ChainType = source.ChainType;
        toMap.DocumentLabel = source.DocumentLabel;
        toMap.DocumentName = source.DocumentName;
        toMap.IdArchiveChain = source.IdArchiveChain;
        toMap.UniqueId = source.UniqueId;

        return toMap;
    }
}

export = DocumentUnitChainMapper