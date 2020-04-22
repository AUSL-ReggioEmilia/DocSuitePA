import UDSRepositoryStatus = require('App/Models/UDS/UDSRepositoryStatus');
import ContainerModel = require('App/Models/Commons/ContainerModel');

class UDSRepositoryModel {
    constructor() {       
    }

    UniqueId: string;
    Name: string;
    SequenceCurrentYear: number;
    SequenceCurrentNumber: number;
    ModuleXML: string;
    Version: number;
    RegistrationDate: Date;
    ActiveDate: Date;
    ExpiredDate: Date;
    DSWEnvironment: number;
    Alias: string;
    Status: UDSRepositoryStatus;
    Container: ContainerModel;
}

export = UDSRepositoryModel;