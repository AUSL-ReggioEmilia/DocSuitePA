
import FascicleType = require('App/Models/Fascicles/FascicleType');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import FascicleLinkModel = require('App/Models/Fascicles/FascicleLinkModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import ClassMapping = require('App/Core/Serialization/ClassMapping');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import ProcessModel = require('../Processes/ProcessModel');
import ProcessFascicleTemplateModel = require('../Processes/ProcessFascicleTemplateModel');

class FascicleModel {
    UniqueId: string;
    Year: number;
    Number: number;
    Conservation: number;
    Manager: string;
    StartDate: Date;
    EndDate: Date;
    Rack: string;
    Name: string;
    FascicleObject: string;
    Note: string;
    Title: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    LastChangedUser: string;
    LastChangedDate: Date;
    FascicleType: FascicleType;
    VisibilityType: VisibilityType;
    DSWEnvironment: number;
    MetadataValues: string;
    Category: CategoryModel;
    Container: ContainerModel;
    MetadataRepository: MetadataRepositoryModel;
    Contacts: ContactModel[];
    FascicleDocumentUnits: FascicleDocumentUnitModel[];
    FascicleLinks: FascicleLinkModel[];
    LinkedFascicles: FascicleLinkModel[];
    FascicleDocuments: FascicleDocumentModel[];
    FascicleRoles: FascicleRoleModel[];
    FascicleFolders: FascicleFolderModel[];
    DossierFolders: DossierFolderModel[];
    FascicleTemplate: ProcessFascicleTemplateModel;

    /**
     * Costruttore
     */
    constructor() {
        this.Contacts = new Array<ContactModel>();
        this.FascicleDocumentUnits = new Array<FascicleDocumentUnitModel>();
        this.FascicleLinks = new Array<FascicleLinkModel>();
        this.LinkedFascicles = new Array<FascicleLinkModel>();
        this.FascicleDocuments = new Array<FascicleDocumentModel>();
        this.FascicleRoles = new Array<FascicleRoleModel>();
        this.FascicleFolders = new Array<FascicleFolderModel>();
    }
}

export = FascicleModel;
