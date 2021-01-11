import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import ProcessService = require('App/Services/Processes/ProcessService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ProcessModel = require('App/Models/Processes/ProcessModel');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import ProcessFascicleWorkflowRepositoryModel = require('App/Models/Processes/ProcessFascicleWorkflowRepositoryModel');
import ProcessFascicleWorkflowRepositoryService = require('App/Services/Processes/ProcessFascicleWorkflowRepositoryService');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import MetadataRepositoryViewModel = require('App/ViewModels/Commons/MetadataRepositoryViewModel');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
import UscFascicleFolders = require('UserControl/uscFascicleFolders');
import AjaxModel = require('App/Models/AjaxModel');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import FascicleFolderStatus = require('App/Models/Fascicles/FascicleFolderStatus');
import Guid = require('App/Helpers/GuidHelper');
import FascicleFolderTypology = require('App/Models/Fascicles/FascicleFolderTypology');
import ContactModel = require('App/Models/Commons/ContactModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import ProcessFascicleTemplateModel = require('App/Models/Processes/ProcessFascicleTemplateModel');
import UscErrorNotification = require('./uscErrorNotification');
import uscRoleRest = require('./uscRoleRest');
import RoleModel = require('App/Models/Commons/RoleModel');
import ProcessNodeType = require('App/Models/Processes/ProcessNodeType');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import uscContattiSelRest = require('./uscContattiSelRest');
import ProcessFascicleTemplateService = require('App/Services/Processes/ProcessFascicleTemplateService');
import TbltProcess = require('Tblt/TbltProcess');
import ExternalSourceActionEnum = require('App/Helpers/ExternalSourceActionEnum');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import CategoryService = require('App/Services/Commons/CategoryService');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import FascicleCustomActionModel = require('App/Models/Commons/FascicleCustomActionModel');
import uscCustomActionsRest = require('./uscCustomActionsRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class uscProcessDetails {

    processesModel: ProcessModel[];
    static processFascicleWorkflowRepositories: ProcessFascicleWorkflowRepositoryModel[];
    workflowRepositories: WorkflowRepositoryModel[];
    metadataRepositories: MetadataRepositoryViewModel[];
    fascicleFolders: FascicleFolderModel[];
    static responsibleRole: RoleModel;
    static authorizedRoles: RoleModel[];
    static contacts: ContactModel[];
    static raciRoles: RoleModel[] = <RoleModel[]>[];

    static selectedCategoryId: number;
    static selectedProcessId: string;
    static selectedDossierFolderId: string;
    static selectedProcessFascicleTemplateId: string;
    static selectedEntityType: ProcessNodeType;
    static selectedFascicleTemplate: string;

    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    pnlDetailsId: string;
    lblNameId: string;
    lblFolderNameId: string;
    divFolderNameId: string;
    lblClasificationNameId: string;
    rcbWorkflowRepositoryId: string;
    toolbarWorkflowRepositoryId: string;
    rtvWorkflowRepositoryId: string;
    rcbMetadataRepositoryId: string;
    rbAddFascicleId: string;
    rtbFascicleSubjectId: string;
    rbFascicleVisibilityTypeId: string;
    uscFascicleFoldersId: string;
    uscContactRestId: string;
    uscRoleRestId: string;
    uscResponsibleRolesId: string;
    uscAuthorizedRolesId: string;
    lblActivationDateId: string;
    rcbFascicleTypeId: string;
    rpbDetailsId: string;
    lblCategoryCodeId: string;
    lblCategoryNameId: string;
    lblStartDateId: string;
    lblEndDateId: string;
    lblMetadataId: string;
    lblMassimarioNameId: string;
    lblRegistrationDateId: string;
    lblNoteId: string;
    uscCustomActionsRestId: string;

    public static InformationDetails_PanelName: string = "informationDetails";
    public static CategoryInformationDetails_PanelName: string = "categoryInformationDetails";
    public static RoleDetails_PanelName: string = "roleDetails";
    public static WorkflowDetails_PanelName: string = "workflowDetails";
    public static FascicleDetails_PanelName: string = "fascicleDetails";
    public TYPOLOGY_ATTRIBUTE: string = "Typology";

    private _lblActivationDate: HTMLLabelElement;
    private _lblProcessName: HTMLLabelElement;
    private _lblFolderName: HTMLLabelElement;
    private _divFolderName: HTMLTableRowElement;
    private _lblClasificationName: HTMLLabelElement;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rcbWorkflowRepository: Telerik.Web.UI.RadComboBox;
    private _toolbarWorkflowRepository: Telerik.Web.UI.RadToolBar;
    private _rtvWorkflowRepository: Telerik.Web.UI.RadTreeView;
    private _rcbMetadataRepository: Telerik.Web.UI.RadComboBox;
    private _rbAddFascicle: Telerik.Web.UI.RadButton;
    private _rtbFascicleSubject: Telerik.Web.UI.RadTextBox;
    private _rbFascicleVisibilityType: Telerik.Web.UI.RadButton;
    private _uscFascicleFolders: UscFascicleFolders;
    private _uscContactRest: uscContattiSelRest;
    private _uscRoleRest: uscRoleRest;
    private _uscResponsibleRoles: uscRoleRest;
    private _uscAuthorizedRoles: uscRoleRest;
    private _rcbFascicleType: Telerik.Web.UI.RadComboBox;
    private _rpbDetails: Telerik.Web.UI.RadPanelBar;
    private _pnlInformations: Telerik.Web.UI.RadPanelItem;
    private _pnlCategoryInformations: Telerik.Web.UI.RadPanelItem;
    private _pnlRoleDetails: Telerik.Web.UI.RadPanelItem;
    private _pnlWorkflowDetails: Telerik.Web.UI.RadPanelItem;
    private _pnlFascicleDetails: Telerik.Web.UI.RadPanelItem;
    private _lblCategoryCode: HTMLLabelElement;
    private _lblCategoryName: HTMLLabelElement;
    private _lblStartDate: HTMLLabelElement;
    private _lblEndDate: HTMLLabelElement;
    private _lblMetadata: HTMLLabelElement;
    private _lblMassimarioName: HTMLLabelElement;
    private _lblRegistrationDate: HTMLLabelElement;
    private _lblNote: HTMLLabelElement;

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    private _categoryService: CategoryService;
    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _processFascicleWorkflowRepositoryService: ProcessFascicleWorkflowRepositoryService;
    private _workflowRepositoryService: WorkflowRepositoryService;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _processFascicleTemplateService: ProcessFascicleTemplateService;

    private needRolesFromExternalSource_eventArgs: string[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        this.initializeServices();
        this.initializeControls();
        this.initializeUserControls();
        $(`#${this.pnlDetailsId}`).hide();
        this._ajaxLoadingPanel.show(this.rcbWorkflowRepositoryId);
        this._ajaxLoadingPanel.show(this.rcbMetadataRepositoryId);
        this.loadWorkflowRepositories();
        this.loadMetadataRepositories();
        this.loadCustomActions(<FascicleCustomActionModel>{
            AutoClose: false,
            AutoCloseAndClone: false
        });
        this.bindLoaded();
        this.loadFascicleTypes();
    }

    initializeServices(): void {
        let categoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Category");
        this._categoryService = new CategoryService(categoryConfiguration);
        let processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Process");
        this._processService = new ProcessService(processConfiguration);
        let dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
        let processFascicleWorkflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ProcessFascicleWorkflowRepository");
        this._processFascicleWorkflowRepositoryService = new ProcessFascicleWorkflowRepositoryService(processFascicleWorkflowRepositoryConfiguration);
        let workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
        this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
        let metadataRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository");
        this._metadataRepositoryService = new MetadataRepositoryService(metadataRepositoryConfiguration);
        let processFascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ProcessFascicleTemplate");
        this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
    }

    initializeControls(): void {
        this._lblProcessName = <HTMLLabelElement>document.getElementById(this.lblNameId);
        this._lblFolderName = <HTMLLabelElement>document.getElementById(this.lblFolderNameId);
        this._divFolderName = <HTMLTableRowElement>document.getElementById(this.divFolderNameId);
        this._lblActivationDate = <HTMLLabelElement>document.getElementById(this.lblActivationDateId);
        this._lblNote = <HTMLLabelElement>document.getElementById(this.lblNoteId);

        this._lblClasificationName = <HTMLLabelElement>document.getElementById(this.lblClasificationNameId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rcbWorkflowRepository = <Telerik.Web.UI.RadComboBox>$find(this.rcbWorkflowRepositoryId);
        this._toolbarWorkflowRepository = <Telerik.Web.UI.RadToolBar>$find(this.toolbarWorkflowRepositoryId);
        this._toolbarWorkflowRepository.add_buttonClicked(this.toolbarWorkflowRepository_buttonClick);
        this._rtvWorkflowRepository = <Telerik.Web.UI.RadTreeView>$find(this.rtvWorkflowRepositoryId);
        this._rcbMetadataRepository = <Telerik.Web.UI.RadComboBox>$find(this.rcbMetadataRepositoryId);
        this._rbAddFascicle = <Telerik.Web.UI.RadButton>$find(this.rbAddFascicleId);
        this._rbAddFascicle.add_clicked(this.rbAddFascicle_onClick);
        this._rtbFascicleSubject = <Telerik.Web.UI.RadTextBox>$find(this.rtbFascicleSubjectId);
        this._rbFascicleVisibilityType = <Telerik.Web.UI.RadButton>$find(this.rbFascicleVisibilityTypeId);
        this._rcbFascicleType = <Telerik.Web.UI.RadComboBox>$find(this.rcbFascicleTypeId);
        this._rcbFascicleType.add_selectedIndexChanged(this.rcbFascicleType_selectedIndexChanged);
        this._rpbDetails = <Telerik.Web.UI.RadPanelBar>$find(this.rpbDetailsId);
        this._pnlInformations = this._rpbDetails.findItemByValue("pnlInformations");
        this._pnlCategoryInformations = this._rpbDetails.findItemByValue("pnlCategoryInformations");
        this._pnlRoleDetails = this._rpbDetails.findItemByValue("pnlRoleDetails");
        this._pnlWorkflowDetails = this._rpbDetails.findItemByValue("pnlWorkflowDetails");
        this._pnlFascicleDetails = this._rpbDetails.findItemByValue("pnlFascicleDetails");
        this._lblCategoryCode = <HTMLLabelElement>document.getElementById(this.lblCategoryCodeId);
        this._lblCategoryName = <HTMLLabelElement>document.getElementById(this.lblCategoryNameId);
        this._lblStartDate = <HTMLLabelElement>document.getElementById(this.lblStartDateId);
        this._lblEndDate = <HTMLLabelElement>document.getElementById(this.lblEndDateId);
        this._lblMetadata = <HTMLLabelElement>document.getElementById(this.lblMetadataId);
        this._lblMassimarioName = <HTMLLabelElement>document.getElementById(this.lblMassimarioNameId);
        this._lblRegistrationDate = <HTMLLabelElement>document.getElementById(this.lblRegistrationDateId);
    }

    initializeUserControls(): void {
        $("#".concat(this.uscFascicleFoldersId)).bind(UscFascicleFolders.LOADED_EVENT, (args) => {
        });
        this._uscFascicleFolders = <UscFascicleFolders>$(`#${this.uscFascicleFoldersId}`).data();
        if (!jQuery.isEmptyObject(this._uscFascicleFolders)) {
            this._uscFascicleFolders.hideLoadingPanel();
            this._uscFascicleFolders.setRootNode("");
            this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().clear();

            let fascicleFolder = <FascicleFolderModel>{};
            fascicleFolder.Status = FascicleFolderStatus.Active;
            fascicleFolder.Name = "Fascicolo";
            let model = <AjaxModel>{};
            model.ActionName = "ManageParent";
            model.Value = [];
            fascicleFolder.UniqueId = Guid.newGuid();
            fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
            model.Value.push(JSON.stringify(fascicleFolder));

            this._uscFascicleFolders.addNewFolder(model);
            this._uscFascicleFolders.setManageFascicleFolderVisibility(true);
        }

        this._uscContactRest = <uscContattiSelRest>$(`#${this.uscContactRestId}`).data();
        this._uscRoleRest = <uscRoleRest>$(`#${this.uscRoleRestId}`).data();
        this._uscResponsibleRoles = <uscRoleRest>$(`#${this.uscResponsibleRolesId}`).data();
        this._uscAuthorizedRoles = <uscRoleRest>$(`#${this.uscAuthorizedRolesId}`).data();
        this.registerUscRoleRestEventHandlers();
        uscProcessDetails.contacts = [];
        this._uscContactRest.renderContactsTree([]);
        this.registerUscContattiRestEventHandlers();
    }

    private registerUscRoleRestEventHandlers(): void {
        let uscRoleRestEventsDictionary = this._uscRoleRest.uscRoleRestEvents;
        this._uscRoleRest.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscRoleRestId);
        this._uscRoleRest.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscRoleRestId);
        this._uscResponsibleRoles.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscResponsibleRolesId);
        this._uscResponsibleRoles.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscResponsibleRolesId);
        this._uscAuthorizedRoles.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscAuthorizedRolesId);
        this._uscAuthorizedRoles.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscAuthorizedRolesId);
    }

    private registerUscContattiRestEventHandlers(): void {
        let uscContattiSelRestEvents = this._uscContactRest.uscContattiSelRestEvents;

        this._uscContactRest.registerEventHandler(uscContattiSelRestEvents.ContactDeleted, this.deleteContactPromise);
        this._uscContactRest.registerEventHandler(uscContattiSelRestEvents.NewContactsAdded, this.updateContactPromise);
    }

    private deleteRolePromise = (roleIdToDelete: number, senderId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!roleIdToDelete)
            return promise.promise();
        switch (uscProcessDetails.selectedEntityType) {
            case ProcessNodeType.Category:
            case ProcessNodeType.Process: {
                this._processService.getById(uscProcessDetails.selectedProcessId, (data) => {
                    let process: ProcessModel = data;
                    process.Roles = process.Roles
                        .filter(role => role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
                    this._processService.update(process, (data) => {
                        promise.resolve(data);
                    },
                        (error: ExceptionDTO) => {
                            this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
                            this.showNotificationException(error);
                        });
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
                    this.showNotificationException(error);
                });
                break;
            }
            case ProcessNodeType.DossierFolder: {
                this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, (data) => {
                    let dossierFolder: DossierFolderModel = data[0];
                    dossierFolder.DossierFolderRoles = dossierFolder.DossierFolderRoles
                        .filter(role => role.Role.IdRole !== roleIdToDelete || role.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
                    this._dossierFolderService.updateDossierFolder(dossierFolder, null, (data) => {
                        promise.resolve(data);
                    }, (error) => {
                        this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
                        this.showNotificationException(error);
                    });
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                    this.showNotificationException(error);
                });
                break;
            }
            case ProcessNodeType.ProcessFascicleTemplate: {
                switch (senderId) {
                    case this.uscResponsibleRolesId: {
                        promise.resolve(uscProcessDetails.responsibleRole ? [uscProcessDetails.responsibleRole] : []);
                        uscProcessDetails.responsibleRole = null;
                        break;
                    }
                    case this.uscAuthorizedRolesId: {
                        uscProcessDetails.authorizedRoles = uscProcessDetails.authorizedRoles
                            .filter(role => role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
                        promise.resolve(uscProcessDetails.authorizedRoles);
                        break;
                    }
                }
                break;
            }
        }
        return promise.promise();
    }

    private updateRolesPromise = (newAddedRoles: RoleModel[], senderId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!newAddedRoles.length)
            return promise.promise();

        switch (uscProcessDetails.selectedEntityType) {
            case ProcessNodeType.Category:
            case ProcessNodeType.Process: {
                this._processService.getById(uscProcessDetails.selectedProcessId, (data) => {
                    let process: ProcessModel = data;
                    process.Roles = [...process.Roles, ...newAddedRoles];
                    this._processService.update(process, (data) => {
                        promise.resolve(data);
                    },
                        (error: ExceptionDTO) => {
                            this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
                            this.showNotificationException(error);
                        });
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
                    this.showNotificationException(error);
                });
                break;
            }
            case ProcessNodeType.DossierFolder: {
                this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, (data) => {
                    let dossierFolder: DossierFolderModel = data[0];
                    for (let newRole of newAddedRoles) {
                        if (dossierFolder.DossierFolderRoles.map(x => x.Role).indexOf(newRole) === -1) {
                            let dossierFolderRole: DossierFolderRoleModel = <DossierFolderRoleModel>{};
                            dossierFolderRole.Role = newRole;
                            dossierFolderRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                            dossierFolderRole.IsMaster = true;
                            dossierFolder.DossierFolderRoles.push(dossierFolderRole);
                        }
                    }
                    this._dossierFolderService.updateDossierFolder(dossierFolder, null, (data) => {
                        promise.resolve(data);
                    }, (error) => {
                        this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
                        this.showNotificationException(error);
                    });
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                    this.showNotificationException(error);
                });
                break;
            }
            case ProcessNodeType.ProcessFascicleTemplate: {
                switch (senderId) {
                    case this.uscResponsibleRolesId: {
                        for (let authorizedRole of uscProcessDetails.authorizedRoles) {
                            if (newAddedRoles.filter(x => x.IdRole === authorizedRole.IdRole).length > 0) {
                                let existedRole: RoleModel = newAddedRoles.filter(x => x.IdRole === authorizedRole.IdRole)[0];
                                alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore autorizzato del modello di fascicolo`);
                                newAddedRoles = newAddedRoles.filter(x => x.IdRole !== authorizedRole.IdRole);
                                promise.resolve(existedRole, true);
                            }
                        }
                        if (newAddedRoles.length > 0) {
                            uscProcessDetails.responsibleRole = newAddedRoles[0];
                            promise.resolve([uscProcessDetails.responsibleRole]);
                        }
                        else {
                            promise.resolve([]);
                        }
                        break;
                    }
                    case this.uscAuthorizedRolesId: {
                        if (uscProcessDetails.responsibleRole && (newAddedRoles.filter(x => x.IdRole === uscProcessDetails.responsibleRole.IdRole).length > 0)) {
                            let existedRole: RoleModel = newAddedRoles.filter(x => x.IdRole === uscProcessDetails.responsibleRole.IdRole)[0];
                            alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore responsabile del modello di fascicolo`);
                            newAddedRoles = newAddedRoles.filter(x => x.IdRole !== uscProcessDetails.responsibleRole.IdRole);
                            promise.resolve(existedRole);
                        }
                        uscProcessDetails.authorizedRoles = [...uscProcessDetails.authorizedRoles, ...newAddedRoles];
                        promise.resolve(uscProcessDetails.authorizedRoles);
                        break;
                    }
                }
                break;
            }
        }
        return promise.promise();
    }

    private deleteContactPromise = (contactIdToDelete: number, senderId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!contactIdToDelete)
            return promise.promise();
        uscProcessDetails.contacts = uscProcessDetails.contacts
            .filter(contact => contact.EntityId !== contactIdToDelete || contact.FullIncrementalPath.indexOf(contactIdToDelete.toString()) === -1);
        promise.resolve(uscProcessDetails.contacts);
        return promise.promise();
    }

    private updateContactPromise = (newAddedContact: ContactModel, senderId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!newAddedContact)
            return promise.promise();
        uscProcessDetails.contacts.push(newAddedContact);
        promise.resolve(uscProcessDetails.contacts);
        return promise.promise();
    }

    private bindLoaded(): void {
        $(`#${this.pnlDetailsId}`).data(this);
    }

    loadWorkflowRepositories(): void {
        this._workflowRepositoryService.getWorkflowRepositories((data) => {
            this.workflowRepositories = data;
            this._rcbWorkflowRepository.get_items().clear();
            let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text("");
            item.set_value("");
            this._rcbWorkflowRepository.get_items().add(item);
            for (let workflowRepository of this.workflowRepositories) {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(workflowRepository.Name);
                item.set_value(workflowRepository.UniqueId);
                this._rcbWorkflowRepository.get_items().add(item);
            }
            this._ajaxLoadingPanel.hide(this.rcbWorkflowRepositoryId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rcbWorkflowRepositoryId);
            this.showNotificationException(error);
        });
    }

    loadMetadataRepositories(): void {
        this._metadataRepositoryService.findMetadataRepositories("", (data) => {
            this.metadataRepositories = data;
            this._rcbMetadataRepository.get_items().clear();
            let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text("");
            this._rcbMetadataRepository.get_items().add(item);
            for (let metadataRepository of this.metadataRepositories) {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(metadataRepository.Name);
                item.set_value(metadataRepository.UniqueId);
                this._rcbMetadataRepository.get_items().add(item);
            }
            this._ajaxLoadingPanel.hide(this.rcbMetadataRepositoryId);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this.rcbMetadataRepositoryId);
            this.showNotificationException(error);
        });
    }

    setCategoryDetails(): void {
        this._uscRoleRest.disableButtons();
        this._categoryService.getRolesByCategoryId(uscProcessDetails.selectedCategoryId, (data) => {
            let category: CategoryModel = data;
            this._lblCategoryCode.innerText = category.getFullCodeDotted();
            this._lblCategoryName.innerText = `${category.Name} (${category.EntityShortId})`;
            this._lblStartDate.innerText = category.StartDate ? moment(new Date(category.StartDate)).format("DD/MM/YYYY") : "";
            this._lblEndDate.innerText = category.EndDate ? moment(new Date(category.EndDate)).format("DD/MM/YYYY") : "";
            if (this._lblMetadata) {
                this._lblMetadata.innerText = category.MetadataRepository ? category.MetadataRepository.Name : "";
            }
            this._lblMassimarioName.innerText = category.MassimarioScarto
                ? `${category.MassimarioScarto.FullCode.replace("|", ".")}.${category.MassimarioScarto.Name}(${category.MassimarioScarto.ConservationPeriod} Anni)`
                : "";
            this._lblRegistrationDate.innerText = moment(new Date(category.RegistrationDate)).format("DD/MM/YYYY");

            if (category.CategoryFascicles.length > 0) {
                let categoryFascicleRightsModel = category.CategoryFascicles.map(x => x.CategoryFascicleRights);
                let roleArray: RoleModel[] = [];
                for (let cfrm of categoryFascicleRightsModel) {
                    roleArray = cfrm.map(x => x.Role);
                }
                this._uscRoleRest.renderRolesTree(roleArray);
            } else {
                this._uscRoleRest.renderRolesTree([]);
            }
        }, (error) => {
            this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
            this.showNotificationException(error);
        });
    }

    setProcessDetails(dossierFolderName: string, populateRoles: boolean): void {
        this._uscRoleRest.enableButtons();
        let workflowRepositories: Telerik.Web.UI.RadComboBoxItemCollection = this._rcbWorkflowRepository.get_items();
        let metadataRepositories: Telerik.Web.UI.RadComboBoxItemCollection = this._rcbMetadataRepository.get_items();
        if (workflowRepositories.get_count() > 0) {
            workflowRepositories.getItem(0).select();
        }
        if (metadataRepositories.get_count() > 0) {
            metadataRepositories.getItem(0).select();
        }
        this._uscResponsibleRoles.renderRolesTree([]);
        this._uscAuthorizedRoles.renderRolesTree([]);
        this._uscAuthorizedRoles.disableRaciRoleButton();

        this._uscFascicleFolders.fileManagementButtonsVisibility(false);
        if (uscProcessDetails.selectedEntityType === ProcessNodeType.ProcessFascicleTemplate) {
            uscProcessDetails.responsibleRole = null;
            uscProcessDetails.authorizedRoles = [];
            uscProcessDetails.raciRoles = [];
        }

        this._processService.getById(uscProcessDetails.selectedProcessId, (data) => {
            let process: ProcessModel = data;
            this._divFolderName.style.display = "none";
            this._lblProcessName.innerText = process.Name;

            if (process.StartDate) {
                this._lblActivationDate.innerText = moment(process.StartDate).format("DD/MM/YYYY");
            }

            if (dossierFolderName && dossierFolderName !== '') {
                this._divFolderName.style.display = "";
                this._lblFolderName.innerText = dossierFolderName;
            }
            this._lblClasificationName.innerText = `${process.Category.getFullCodeDotted()} - ${process.Category.Name}`;
            this._lblNote.innerText = process.Note;

            if (populateRoles) {
                //set popup roles source
                if (uscProcessDetails.selectedEntityType === ProcessNodeType.Process) {
                    this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Process.toString(), uscProcessDetails.selectedProcessId];
                }
                else if (uscProcessDetails.selectedEntityType === ProcessNodeType.Category) {
                    this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Category.toString(), uscProcessDetails.selectedCategoryId.toString()];
                }
                $(`#${this.uscRoleRestId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);

                this._uscRoleRest.renderRolesTree(process.Roles);

            }
            this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this._pnlInformations.get_element().id);
            this.showNotificationException(error);
        });
    }

    clearProcessDetails(): void {
        this._lblProcessName.innerText = "";
        this._lblClasificationName.innerText = "";
    }

    setDossierFolderWorkflowRepositories(): void {
        this._uscRoleRest.enableButtons();
        this._processFascicleWorkflowRepositoryService.getByDossierFolderId(uscProcessDetails.selectedDossierFolderId, (data) => {
            uscProcessDetails.processFascicleWorkflowRepositories = data;
            this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().clear();
            for (let processFascicleWorkflowRepository of uscProcessDetails.processFascicleWorkflowRepositories) {
                let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                node.set_text(processFascicleWorkflowRepository.WorkflowRepository.Name);
                node.set_value(processFascicleWorkflowRepository.UniqueId);
                this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().add(node);
            }
            this._rtvWorkflowRepository.get_nodes().getNode(0).expand();
            this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
        }, (error) => {
            this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
            this.showNotificationException(error);
        });
    }

    setDossierFolderRoles(): void {
        this._uscRoleRest.enableButtons();
        this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, (data) => {
            let dossierFolder: DossierFolderModel = data[0];

            //set popup roles source
            this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Process.toString(), uscProcessDetails.selectedProcessId];
            $(`#${this.uscRoleRestId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);

            if (dossierFolder.DossierFolderRoles && dossierFolder.DossierFolderRoles.length > 0) {
                this._uscRoleRest.renderRolesTree(dossierFolder.DossierFolderRoles.map(x => x.Role));
            } else {
                this._uscRoleRest.clearRoleTreeView();
            }
        }, (error) => {
            this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
            this.showNotificationException(error);
        });
    }

    setFascicle(): void {
        this._processFascicleTemplateService.getById(uscProcessDetails.selectedProcessFascicleTemplateId, (data) => {
            this.populateDetails(data);
        }, (error) => {
            this.showNotificationException(error);
        });
    }

    toolbarWorkflowRepository_buttonClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let toolbarButton: Telerik.Web.UI.RadToolBarButton = <Telerik.Web.UI.RadToolBarButton>args.get_item();
        switch (toolbarButton.get_commandName()) {
            case "add": {
                if (this._rcbWorkflowRepository.get_selectedItem().get_value() === "") {
                    alert("Selezionare flusso di lavoro");
                    return;
                }
                this._ajaxLoadingPanel.show(this._pnlWorkflowDetails.get_element().id);
                let processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel = <ProcessFascicleWorkflowRepositoryModel>{};
                processFascicleWorkflowRepository.Process = <ProcessModel>{};
                processFascicleWorkflowRepository.Process.UniqueId = uscProcessDetails.selectedProcessId;
                processFascicleWorkflowRepository.DossierFolder = <DossierFolderModel>{};
                processFascicleWorkflowRepository.DossierFolder.UniqueId = uscProcessDetails.selectedDossierFolderId;
                processFascicleWorkflowRepository.WorkflowRepository = <WorkflowRepositoryModel>{};
                processFascicleWorkflowRepository.WorkflowRepository.UniqueId = this._rcbWorkflowRepository.get_selectedItem().get_value();
                let selectedWorkflowRepository: WorkflowRepositoryModel = <WorkflowRepositoryModel>{};
                for (let workflowRepository of this.workflowRepositories) {
                    if (workflowRepository.UniqueId === this._rcbWorkflowRepository.get_selectedItem().get_value()) {
                        selectedWorkflowRepository = workflowRepository;
                        break;
                    }
                }
                let exist: boolean = false;

                for (let i = 0; i < this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().get_count(); i++) {
                    if (this._rcbWorkflowRepository.get_selectedItem().get_text() === this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().getItem(i).get_text())
                        exist = true;
                }

                if (!exist) {
                    this._processFascicleWorkflowRepositoryService.insert(processFascicleWorkflowRepository, (data) => {
                        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(selectedWorkflowRepository.Name);
                        node.set_value(data.UniqueId);
                        this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().add(node);
                        this._rtvWorkflowRepository.get_nodes().getNode(0).expand();
                        this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                    }, (error) => {
                        this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                        this.showNotificationException(error);
                    });
                }
                else {
                    this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                    alert("Un flusso del lavoro con il nome scelto è già esistente.");
                }
                break;
            }
            case "delete": {
                if (this._rtvWorkflowRepository.get_selectedNode() === null || this._rtvWorkflowRepository.get_selectedNode().get_level() === 0) {
                    alert("Selezionare flusso di lavoro");
                    return;
                }
                this._ajaxLoadingPanel.show(this._pnlWorkflowDetails.get_element().id);
                let processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel = <ProcessFascicleWorkflowRepositoryModel>{};
                processFascicleWorkflowRepository.UniqueId = this._rtvWorkflowRepository.get_selectedNode().get_value();
                this._processFascicleWorkflowRepositoryService.delete(processFascicleWorkflowRepository, (data) => {
                    this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().remove(this._rtvWorkflowRepository.get_selectedNode());
                    this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                }, (error) => {
                    this._ajaxLoadingPanel.hide(this._pnlWorkflowDetails.get_element().id);
                    this.showNotificationException(error);
                });
                break;
            }
        }
    }

    rbAddFascicle_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let processFascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
        processFascicleTemplate.DossierFolder = <DossierFolderModel>{};
        processFascicleTemplate.Process = <ProcessModel>{};
        processFascicleTemplate.UniqueId = uscProcessDetails.selectedProcessFascicleTemplateId;
        processFascicleTemplate.Process.UniqueId = uscProcessDetails.selectedProcessId;
        this.populateFascicleTemplateInfo().then((jsonModel) => {
            processFascicleTemplate.JsonModel = jsonModel;
            processFascicleTemplate.Name = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_PROCESS_FASCICLE_TEMPLATE_NAME);
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_PROCESS_FASCICLE_TEMPLATE_NAME, JSON.stringify(processFascicleTemplate));
            this._processFascicleTemplateService.update(processFascicleTemplate, (data) => {
                alert("Modificato con successo");
                this._ajaxLoadingPanel.hide(this._pnlFascicleDetails.get_element().id);
            }, (error) => {
                this._ajaxLoadingPanel.hide(this._pnlFascicleDetails.get_element().id);
                this.showNotificationException(error);
            });
        });
    }

    public populateFascicleTemplateInfo(): JQueryPromise<string> {
        let promise: JQueryDeferred<string> = $.Deferred<string>();
        let fascicleModel: FascicleModel = new FascicleModel();
        fascicleModel.FascicleObject = this._rtbFascicleSubject.get_value();
        if (this._rcbFascicleType.get_selectedItem()) {
            fascicleModel.FascicleType = FascicleType[this._rcbFascicleType.get_selectedItem().get_value()];
        }

        let rbFascicleVisibilityType_isVisible: any = this._rbFascicleVisibilityType.get_visible();
        fascicleModel.VisibilityType = (rbFascicleVisibilityType_isVisible && this._rbFascicleVisibilityType.get_checked())
            ? VisibilityType.Accessible
            : VisibilityType.Confidential;
        uscProcessDetails.raciRoles = this._uscAuthorizedRoles.getRaciRoles();
        fascicleModel.FascicleRoles = [];
        if (uscProcessDetails.responsibleRole) {
            let fascicleRole: FascicleRoleModel = new FascicleRoleModel();
            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
            fascicleRole.IsMaster = true;
            fascicleRole.Role = uscProcessDetails.responsibleRole;
            fascicleModel.FascicleRoles.push(fascicleRole);
        }
        for (let authorizedRole of uscProcessDetails.authorizedRoles) {
            let fascicleRole: FascicleRoleModel = new FascicleRoleModel();
            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
            fascicleRole.IsMaster = false;
            fascicleRole.Role = authorizedRole;
            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
            if (uscProcessDetails.raciRoles && uscProcessDetails.raciRoles.some(x => x.IdRole === authorizedRole.IdRole)) {
                fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
            }
            fascicleModel.FascicleRoles.push(fascicleRole);
        }
        this.fascicleFolders = [];
        this.getFascicleFolderListFromTree(this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0));
        fascicleModel.FascicleFolders = this.fascicleFolders;
        fascicleModel.Contacts = uscProcessDetails.contacts;
        fascicleModel.MetadataRepository = new MetadataRepositoryModel();
        if (this._rcbMetadataRepository.get_selectedItem() && this._rcbMetadataRepository.get_selectedItem().get_value()) {
            fascicleModel.MetadataRepository.Name = this._rcbMetadataRepository.get_selectedItem().get_text();
            fascicleModel.MetadataRepository.UniqueId = this._rcbMetadataRepository.get_selectedItem().get_value();
        }
        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                let customActions: FascicleCustomActionModel = instance.getCustomActions<FascicleCustomActionModel>();
                fascicleModel.CustomActions = JSON.stringify(customActions);
                promise.resolve(JSON.stringify(fascicleModel));
            });
        return promise.promise();
    }

    private populateDetails(data: any) {
        let processFascicleTemplate: ProcessFascicleTemplateModel = data;
        if (processFascicleTemplate.JsonModel === "") {
            return;
        }
        let fascicle: FascicleModel = JSON.parse(processFascicleTemplate.JsonModel);
        this._rtbFascicleSubject.set_value(fascicle.FascicleObject);
        if (fascicle.FascicleType) {
            let fascicleTypeItem: Telerik.Web.UI.RadComboBoxItem = this._rcbFascicleType.findItemByValue(FascicleType[fascicle.FascicleType]);
            fascicleTypeItem.select();
        }
        this._rbFascicleVisibilityType.set_checked(fascicle.VisibilityType === VisibilityType.Accessible);
        if (fascicle.FascicleRoles.filter(x => x.IsMaster === true).map(x => x.Role).length > 0) {
            uscProcessDetails.responsibleRole = fascicle.FascicleRoles.filter(x => x.IsMaster === true).map(x => x.Role)[0];
            this._uscResponsibleRoles.renderRolesTree([uscProcessDetails.responsibleRole]);
        }
        uscProcessDetails.authorizedRoles = fascicle.FascicleRoles.filter(x => x.IsMaster === false).map(x => x.Role);
        uscProcessDetails.raciRoles = fascicle.FascicleRoles
            .filter(x => x.IsMaster === false && x.AuthorizationRoleType === AuthorizationRoleType.Responsible).map(x => x.Role);

        //set popup roles source
        this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Process.toString(), uscProcessDetails.selectedProcessId];
        $(`#${this.uscResponsibleRolesId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);
        if (uscProcessDetails.raciRoles) {
            this._uscAuthorizedRoles.setRaciRoles(uscProcessDetails.raciRoles);
        }
        this._uscAuthorizedRoles.renderRolesTree(uscProcessDetails.authorizedRoles);
        this.populateFascicleFoldersTree(fascicle.FascicleFolders);
        uscProcessDetails.contacts = fascicle.Contacts;
        this._uscContactRest.renderContactsTree(uscProcessDetails.contacts);
        if (fascicle.MetadataRepository) {
            let rcbItem: Telerik.Web.UI.RadComboBoxItem = this._rcbMetadataRepository.findItemByValue(fascicle.MetadataRepository.UniqueId);
            if (rcbItem) {
                rcbItem.select();
            }
        }
        if (fascicle.CustomActions) {
            this.loadCustomActions(<FascicleCustomActionModel>JSON.parse(fascicle.CustomActions));
        }
    }

    getFascicleFolderListFromTree(fascicleFoldersNode: Telerik.Web.UI.RadTreeNode): void {
        for (let index = 0; index < fascicleFoldersNode.get_nodes().get_count(); index++) {
            let child: Telerik.Web.UI.RadTreeNode = fascicleFoldersNode.get_nodes().getNode(index);
            let fascicleFolder: FascicleFolderModel = <FascicleFolderModel>{};
            fascicleFolder.Name = child.get_text();
            fascicleFolder.UniqueId = child.get_value();
            fascicleFolder.Typology = child.get_attributes().getAttribute("Typology");
            fascicleFolder.Status = FascicleFolderStatus.Active;
            fascicleFolder.ParentInsertId = fascicleFoldersNode.get_level() === 1
                ? fascicleFoldersNode.get_parent().get_value()
                : fascicleFoldersNode.get_value();
            this.fascicleFolders.push(fascicleFolder);
            this.getFascicleFolderListFromTree(child);
        }
    }

    private alreadySavedInTree(nodeValue: string, radTreeView: Telerik.Web.UI.RadTreeView): boolean {
        let alreadySavedInTree: boolean = false;
        if (radTreeView.get_nodes().get_count() !== 0) {
            var allNodes = radTreeView.get_nodes().getNode(0).get_allNodes();
            for (var i = 0; i < allNodes.length; i++) {
                var node = allNodes[i];
                if (node.get_value() === nodeValue) {
                    alreadySavedInTree = true;
                    break;
                }
            }
        }
        return alreadySavedInTree;
    }

    protected addNodesToRadTreeView(nodeValue: string, nodeText: string, text: string, nodeImageUrl: string, radTreeView: Telerik.Web.UI.RadTreeView): void {
        let rtvNode: Telerik.Web.UI.RadTreeNode;

        if (radTreeView.get_nodes().get_count() === 0) {
            rtvNode = new Telerik.Web.UI.RadTreeNode();
            rtvNode.set_text(text);
            radTreeView.get_nodes().add(rtvNode);
        }
        rtvNode = new Telerik.Web.UI.RadTreeNode();
        rtvNode.set_text(nodeText);
        rtvNode.set_value(nodeValue);
        rtvNode.set_imageUrl(nodeImageUrl);
        radTreeView.get_nodes().getNode(0).get_nodes().add(rtvNode);
        radTreeView.get_nodes().getNode(0).expand();
    }

    populateFascicleFoldersTree(fascicleFolders: FascicleFolderModel[]): void {
        this._uscFascicleFolders.setRootNode("");
        this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().clear();
        let fascicleFolder = <FascicleFolderModel>{};
        fascicleFolder.Status = FascicleFolderStatus.Active;
        fascicleFolder.Name = "Fascicolo";
        let model = <AjaxModel>{};
        model.ActionName = "ManageParent";
        model.Value = [];
        fascicleFolder.UniqueId = Guid.newGuid();
        fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
        model.Value.push(JSON.stringify(fascicleFolder));
        this._uscFascicleFolders.addNewFolder(model);
        for (let index: number = 1; index < fascicleFolders.length; index++) {
            let fascicleFolder = fascicleFolders[index];
            let model = <AjaxModel>{};
            model.ActionName = "ManageParent";
            model.Value = [];
            model.Value.push(JSON.stringify(fascicleFolder));
            let parentNode: Telerik.Web.UI.RadTreeNode = this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().getNode(0);
            parentNode.expand();
            this.addFascicleFolderNodeRecursive(fascicleFolder, parentNode);
        }
    }

    addFascicleFolderNodeRecursive(fasciclefolder: FascicleFolderModel, node: Telerik.Web.UI.RadTreeNode): void {
        if (fasciclefolder.ParentInsertId === "" || node.get_value() === fasciclefolder.ParentInsertId) {
            let child: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            child.set_text(fasciclefolder.Name);
            child.set_value(fasciclefolder.UniqueId);
            child.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
            child.get_attributes().setAttribute(this.TYPOLOGY_ATTRIBUTE, fasciclefolder.Typology);
            child.expand();
            node.get_nodes().add(child);
            return;
        }
        for (let index: number = 0; index < node.get_nodes().get_count(); index++) {
            let parent: Telerik.Web.UI.RadTreeNode = node.get_nodes().getNode(index);
            this.addFascicleFolderNodeRecursive(fasciclefolder, parent);
        }
    }

    clearFascicleInputs(): void {
        this._rtbFascicleSubject.clear();
        this._rcbFascicleType.clearSelection();
        this._rbFascicleVisibilityType.set_checked(false);
        this._uscResponsibleRoles.renderRolesTree([]);
        this._uscAuthorizedRoles.renderRolesTree([]);
        this.initializeUserControls();
        this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().getNode(0).get_nodes().clear();
        this._uscContactRest.renderContactsTree([]);
        this._rcbMetadataRepository.set_selectedItem(this._rcbMetadataRepository.get_items().getItem(0));
    }

    private loadFascicleTypes(): void {
        let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        emptyItem.set_text("");
        this._rcbFascicleType.get_items().add(emptyItem);
        this.setFascicleTypeItem(this._rcbFascicleType, [FascicleType.Procedure, FascicleType.Activity]);
    }

    setFascicleTypeItem(comboBox: Telerik.Web.UI.RadComboBox, fascicleTypes: FascicleType[]): void {
        for (let itemType of fascicleTypes) {
            let item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this._enumHelper.getFascicleTypeDescription(itemType));
            item.set_value(FascicleType[itemType]);
            comboBox.get_items().add(item);
        }
    }

    setPanelVisibility(panelName: string, isVisible: boolean): void {
        switch (panelName) {
            case uscProcessDetails.InformationDetails_PanelName: {
                this._pnlInformations.set_visible(isVisible);
                break;
            }
            case uscProcessDetails.CategoryInformationDetails_PanelName: {
                this._pnlCategoryInformations.set_visible(isVisible);
                break;
            }
            case uscProcessDetails.RoleDetails_PanelName: {
                this._pnlRoleDetails.set_visible(isVisible);
                break;
            }
            case uscProcessDetails.WorkflowDetails_PanelName: {
                this._pnlWorkflowDetails.set_visible(isVisible);
                break;
            }
            case uscProcessDetails.FascicleDetails_PanelName: {
                this._pnlFascicleDetails.set_visible(isVisible);
                break;
            }
        }
    }

    setPanelLoading(panelName: string, isVisible: boolean): void {
        switch (panelName) {
            case uscProcessDetails.InformationDetails_PanelName: {
                this.setLoading(this._pnlInformations.get_element().id, isVisible);
                break;
            }
            case uscProcessDetails.CategoryInformationDetails_PanelName: {
                this.setLoading(this._pnlCategoryInformations.get_element().id, isVisible);
                break;
            }
            case uscProcessDetails.RoleDetails_PanelName: {
                this.setLoading(this._pnlRoleDetails.get_element().id, isVisible);
                break;
            }
            case uscProcessDetails.WorkflowDetails_PanelName: {
                this.setLoading(this._pnlWorkflowDetails.get_element().id, isVisible);
                break;
            }
            case uscProcessDetails.FascicleDetails_PanelName: {
                this.setLoading(this._pnlFascicleDetails.get_element().id, isVisible);
                break;
            }
        }
    }

    private setLoading(elementId: string, isVisible: boolean): void {
        if (isVisible) {
            this._ajaxLoadingPanel.show(elementId);
        }
        else {
            this._ajaxLoadingPanel.hide(elementId);
        }
    }

    rcbFascicleType_selectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let fascicleIsProcedureOrDefault: boolean = ["", FascicleType[FascicleType.Procedure]].indexOf(args.get_item().get_value()) > -1;
        $("#uscContactRestFieldset").toggle(fascicleIsProcedureOrDefault);
        $("#responsibleRoleFieldset").toggle(fascicleIsProcedureOrDefault);
        this._rbFascicleVisibilityType.set_visible(fascicleIsProcedureOrDefault);
    }

    private showNotificationException(exception: ExceptionDTO, customMessage?: string): void {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(customMessage)
        }
    }

    private showNotificationMessage(customMessage: string): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    private loadCustomActions(customActions: FascicleCustomActionModel) {
        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                instance.loadItems(customActions);
            });
    }
}

export = uscProcessDetails;