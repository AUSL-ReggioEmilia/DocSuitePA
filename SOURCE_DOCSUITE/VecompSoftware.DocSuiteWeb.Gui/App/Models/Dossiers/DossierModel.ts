import DossierRoleModel = require('App/Models/Dossiers/DossierRoleModel');
import DossierDocumentModel = require('App/Models/Dossiers/DossierDocumentModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import DossierType = require('App/Models/Dossiers/DossierType');
import DossierStatus = require('App/Models/Dossiers/DossierStatus');

interface DossierModel {
    UniqueId: string;
    Year: number;
    Number: string;
    Subject: string;
    Note: string;
    Container: ContainerModel;
    StartDate: Date;
    EndDate?: Date;
    MetadataDesigner: string;
    MetadataValues: string;
    RegistrationDate: Date;
    RegistrationUser: string;
    LastChangedUser: string;
    LastChangedDate: Date;
    MetadataRepository: MetadataRepositoryModel;
    DossierRoles: DossierRoleModel[];
    Contacts: ContactModel[];
    DossierDocuments: DossierDocumentModel[];
    DossierFolders: DossierFolderModel[];
    Category: CategoryModel;
    DossierType: DossierType;
    Status: DossierStatus;
}

export = DossierModel;