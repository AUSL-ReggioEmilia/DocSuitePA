import BaseMapper = require('App/Mappers/BaseMapper');
import UDSDocumentUnitModel = require('App/Models/UDS/UDSDocumentUnitModel');

class UDSDocumentUnitModelMapper extends BaseMapper<UDSDocumentUnitModel> {
    constructor() {
        super();
    }

    public Map(source: any): UDSDocumentUnitModel {
        let toMap: UDSDocumentUnitModel = <UDSDocumentUnitModel>{};

        if (!source) {
            return null;
        }

        toMap.IdUDS = source.IdUDS;
        toMap.Relation = source.Relation;

        return toMap;
    }
}

export = UDSDocumentUnitModelMapper;