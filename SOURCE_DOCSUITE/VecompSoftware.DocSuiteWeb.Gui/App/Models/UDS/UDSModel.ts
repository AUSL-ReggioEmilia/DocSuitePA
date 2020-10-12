import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import UDSDocumentModel = require('App/Models/UDS/UDSDocumentModel');

interface UDSModel{
    UniqueId: string;
    Year: number;
    Number: number;
    Subject: string;
    RegistrationDate: Date;
    Category: CategoryModel;
    Container: ContainerModel;
    Documents: UDSDocumentModel[];
}

export = UDSModel;