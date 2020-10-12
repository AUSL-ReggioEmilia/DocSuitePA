import MetadataRepositoryStatus = require('App/Models/Commons/MetadataRepositoryStatus');

class MetadataRepositoryModel {
    constructor()
    {       
    }

    Name: string;
    Version: number;
    Status: MetadataRepositoryStatus;
    DateFrom: Date;
    DateTo?: Date;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate?: Date;
    JsonMetadata: string;
    UniqueId: string;
    Id: string;
}

export = MetadataRepositoryModel;