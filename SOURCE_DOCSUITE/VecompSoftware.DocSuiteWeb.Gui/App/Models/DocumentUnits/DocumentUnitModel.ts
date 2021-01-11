import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel')
import DocumentUnitRoleModel = require('App/Models/DocumentUnits/DocumentUnitRoleModel');
import DocumentUnitChainModel = require('App/Models/DocumentUnits/DocumentUnitChainModel');
import RoleModel = require('../Commons/RoleModel');

class DocumentUnitModel {
    Environment: number;
    UniqueId: string;
    EntityId: number;
    DocumentUnitName: string;
    Year: number;
    Number: string;
    Title: string;
    ReferenceType: FascicleReferenceType;
    RegistrationDate: Date;
    RegistrationUser: string;
    Subject: string;
    Category: CategoryModel;
    Container: ContainerModel;
    UDSRepository: UDSRepositoryModel;
    Roles: RoleModel[];
    DocumentUnitRoles: DocumentUnitRoleModel[] = new Array<DocumentUnitRoleModel>();
    DocumentUnitChains: DocumentUnitChainModel[] = new Array<DocumentUnitChainModel>();
    IdFascicle: string;
    IsFascicolable?: boolean;

    constructor() {     
    }
}

export = DocumentUnitModel;