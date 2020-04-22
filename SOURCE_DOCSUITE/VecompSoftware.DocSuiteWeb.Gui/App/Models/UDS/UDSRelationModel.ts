import Repository = require('App/Models/UDS/UDSRepositoryModel');

interface UDSRelationModel {
    UniqueId: string;
    Year: number;
    Number: number;
    Title: string;
    Environment: number;
    DocumentUnitName: string;
    Subject: string;
    Status: string;
    RegistrationDate: Date;
    UDSRepository: Repository;
}

export = UDSRelationModel;