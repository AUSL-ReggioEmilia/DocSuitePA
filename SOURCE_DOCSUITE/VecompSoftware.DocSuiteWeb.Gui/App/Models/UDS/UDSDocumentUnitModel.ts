import Relation = require('App/Models/UDS/UDSRelationModel');

class UDSDocumentUnitModel {
    constructor() {
    }

    IdUDS: string;
    Relation: Relation;
    UniqueId: string;
    SourceUDS: Relation;
}

export = UDSDocumentUnitModel;