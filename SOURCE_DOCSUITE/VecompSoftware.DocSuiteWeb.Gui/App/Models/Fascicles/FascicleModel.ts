
import FascicleType = require('App/Models/Fascicles/FascicleType');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import FascicleLinkModel = require('App/Models/Fascicles/FascicleLinkModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import ProcessFascicleTemplateModel = require('App/Models/Processes/ProcessFascicleTemplateModel');
import WorkflowActionModel = require('App/Models/Workflows/WorkflowActionModel');
import TenantAOOModel = require('App/Models/Tenants/TenantAOOModel');

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
    MetadataDesigner: string;
    MetadataValues: string;
    Category: CategoryModel;
    Container: ContainerModel;
    MetadataRepository: MetadataRepositoryModel;
    FascicleTemplate: ProcessFascicleTemplateModel;
    Contacts: ContactModel[];
    FascicleDocumentUnits: FascicleDocumentUnitModel[];
    FascicleLinks: FascicleLinkModel[];
    LinkedFascicles: FascicleLinkModel[];
    FascicleDocuments: FascicleDocumentModel[];
    FascicleRoles: FascicleRoleModel[];
    FascicleFolders: FascicleFolderModel[];
    DossierFolders: DossierFolderModel[];
    WorkflowActions: WorkflowActionModel[];
    CustomActions: string;
    TenantAOO: TenantAOOModel

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
        this.WorkflowActions = new Array<WorkflowActionModel>();
    }
}

export = FascicleModel;
