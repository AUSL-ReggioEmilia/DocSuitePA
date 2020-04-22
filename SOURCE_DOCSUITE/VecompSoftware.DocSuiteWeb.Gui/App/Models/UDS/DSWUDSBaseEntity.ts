import UDSRepositoryModel = require("App/Models/UDS/UDSRepositoryModel");

class DSWUDSBaseEntity {
    IdUDS: string;
    Environment: number;
    Repository: UDSRepositoryModel;
}

export = DSWUDSBaseEntity;