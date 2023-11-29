import UDSFieldListStatus = require("App/Models/UDS/UDSFieldListStatus");
import UDSRepositoryModel = require("App/Models/UDS/UDSRepositoryModel");

interface UDSFieldListModel {
    UniqueId: string;
    FieldName: string;
    Name: string;
    Status: UDSFieldListStatus;
    Environment: number;
    UDSFieldListPath: string;
    UDSFieldListLevel: number;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate?: Date;
    Repository: UDSRepositoryModel;
}

export = UDSFieldListModel;