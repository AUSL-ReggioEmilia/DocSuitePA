import UDSTypologyStatus = require('App/Models/UDS/UDSTypologyStatus');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');

class UDSTypologyModel {
    constructor() {       
    }

    UniqueId: string;
    Name: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedDate: Date;
    LastChangedUser: string;
    Alias: string;
    Status: UDSTypologyStatus;

    UDSRepositories: UDSRepositoryModel[];
}

export = UDSTypologyModel;