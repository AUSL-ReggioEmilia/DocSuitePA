import BaseMapper = require("App/Mappers/BaseMapper");
import UDSDocumentModel = require("App/Models/UDS/UDSDocumentModel");

class UDSDocumentModelMapper extends BaseMapper<UDSDocumentModel>{
    constructor() {
        super();
    }

    public Map(source: any): UDSDocumentModel {
        let toMap: UDSDocumentModel = <UDSDocumentModel>{};

        if (!source) {
            return null;
        }

        toMap.UDSDocumentId = source.UDSDocumentId;
        toMap.IdDocument = source.IdDocument;
        toMap.DocumentType = source.DocumentType;
        toMap.RegistrationDate = source.RegistrationDate;
        toMap.DocumentName = source.DocumentName;
        toMap.UDSId = source.UDSId;
        toMap.DocumentLabel = source.DocumentLabel;

        return toMap;
    }
}

export = UDSDocumentModelMapper;