import UDSRelationType = require('App/Models/UDS/UDSRelationType');
import DSWUDSBaseEntity = require('App/Models/UDS/DSWUDSBaseEntity');

class DSWUDSRelationBaseEntity<T> extends DSWUDSBaseEntity {
    RelationType: UDSRelationType;
    Relation: T;
}

export = DSWUDSRelationBaseEntity;