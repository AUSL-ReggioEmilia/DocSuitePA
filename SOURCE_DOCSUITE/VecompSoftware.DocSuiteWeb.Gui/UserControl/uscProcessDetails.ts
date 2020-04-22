import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import EnumHelper = require("App/Helpers/EnumHelper");
import ProcessService = require('App/Services/Processes/ProcessService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ProcessModel = require('App/Models/Processes/ProcessModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
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
import UscErrorNotification = require('UserControl/uscErrorNotification');
import uscRoleRest = require('UserControl/uscRoleRest');
import RoleModel = require('App/Models/Commons/RoleModel');
import ProcessNodeType = require('App/Models/Processes/ProcessNodeType');
import DossierFolderRoleModel = require('App/Models/Dossiers/DossierFolderRoleModel');
import AuthorizationRoleType = require('App/Models/Fascicles/AuthorizationRoleType');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import uscContattiSelRest = require('UserControl/uscContattiSelRest');
import ProcessFascicleTemplateService = require('App/Services/Processes/ProcessFascicleTemplateService');

class uscProcessDetails {

    processesModel: ProcessModel[];
    static processFascicleWorkflowRepositories: ProcessFascicleWorkflowRepositoryModel[];
    workflowRepositories: WorkflowRepositoryModel[];
    metadataRepositories: MetadataRepositoryViewModel[];
    fascicleFolders: FascicleFolderModel[];
    static responsibleRoles: RoleModel[];
    static authorizedRoles: RoleModel[];
    static contacts: ContactModel[];

    static selectedProcessId: string;
    static selectedDossierFolderId: string;
    static selectedProcessFascicleTemplateId: string;
    static selectedEntityType: ProcessNodeType;

    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    pnlDetailsId: string;
    lblNameId: string;
    lblClasificationNameId: string;
    lblFascicleTypeId: string;
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

    private _lblProcessName: HTMLLabelElement;
    private _lblClasificationName: HTMLLabelElement;
    private _lblFascicleType: HTMLLabelElement;
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

    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;

    private _processService: ProcessService;
    private _dossierFolderService: DossierFolderService;
    private _processFascicleWorkflowRepositoryService: ProcessFascicleWorkflowRepositoryService;
    private _workflowRepositoryService: WorkflowRepositoryService;
    private _metadataRepositoryService: MetadataRepositoryService;
    private _processFascicleTemplateService: ProcessFascicleTemplateService;

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
        this.bindLoaded();
    }

    initializeServices(): void {
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
        this._lblClasificationName = <HTMLLabelElement>document.getElementById(this.lblClasificationNameId);
        this._lblFascicleType = <HTMLLabelElement>document.getElementById(this.lblFascicleTypeId);
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
            case ProcessNodeType.Process: {
                this._processService.getById(uscProcessDetails.selectedProcessId, (data) => {
                    let process: ProcessModel = data;
                    process.Roles = process.Roles
                        .filter(role => role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
                    this._processService.update(process, (data) => {
                        promise.resolve(data);
                    },
                        (error: ExceptionDTO) => {
                            this._ajaxLoadingPanel.hide("ItemDetailTable");
                            this.showNotificationException(error);
                        });
                }, (error) => {
                    this._ajaxLoadingPanel.hide("ItemDetailTable");
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
                        this._ajaxLoadingPanel.hide("ItemDetailTable");
                        this.showNotificationException(error);
                    });
                }, (error) => {
                    this._ajaxLoadingPanel.hide("workflowDetails");
                    this.showNotificationException(error);
                });
                break;
            }
            case ProcessNodeType.ProcessFascicleTemplate: {
                switch (senderId) {
                    case this.uscResponsibleRolesId: {
                        uscProcessDetails.responsibleRoles = uscProcessDetails.responsibleRoles
                            .filter(role => role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
                        promise.resolve(uscProcessDetails.responsibleRoles);
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
            case ProcessNodeType.Process: {
                this._processService.getById(uscProcessDetails.selectedProcessId, (data) => {
                    let process: ProcessModel = data;
                    process.Roles = [...process.Roles, ...newAddedRoles];
                    this._processService.update(process, (data) => {
                        promise.resolve(data);
                    },
                        (error: ExceptionDTO) => {
                            this._ajaxLoadingPanel.hide("ItemDetailTable");
                            this.showNotificationException(error);
                        });
                }, (error) => {
                    this._ajaxLoadingPanel.hide("ItemDetailTable");
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
                        this._ajaxLoadingPanel.hide("ItemDetailTable");
                        this.showNotificationException(error);
                    });
                }, (error) => {
                    this._ajaxLoadingPanel.hide("workflowDetails");
                    this.showNotificationException(error);
                });
                break;
            }
            case ProcessNodeType.ProcessFascicleTemplate: {
                switch (senderId) {
                    case this.uscResponsibleRolesId: {
                        uscProcessDetails.responsibleRoles = [...uscProcessDetails.responsibleRoles, ...newAddedRoles];
                        promise.resolve(uscProcessDetails.responsibleRoles);
                        break;
                    }
                    case this.uscAuthorizedRolesId: {
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
            item.set_value("");
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

    setProcessDetails(): void {
        this._rcbWorkflowRepository.get_items().getItem(0).select();
        this._rcbMetadataRepository.get_items().getItem(0).select();
        this._uscResponsibleRoles.renderRolesTree([]);
        this._uscAuthorizedRoles.renderRolesTree([]);
        this._processService.getById(uscProcessDetails.selectedProcessId, (data) => {
            let process: ProcessModel = data;
            this._lblProcessName.innerText = process.Name;
            this._lblClasificationName.innerText = process.Category.Name;
            this._lblFascicleType.innerText = FascicleType[FascicleType[process.FascicleType]];
            if (uscProcessDetails.selectedEntityType === ProcessNodeType.Process) {
                this._uscRoleRest.renderRolesTree(process.Roles);
            }
            else if (uscProcessDetails.selectedEntityType === ProcessNodeType.ProcessFascicleTemplate) {
                uscProcessDetails.responsibleRoles = [];
                uscProcessDetails.authorizedRoles = [];
            }
            this._ajaxLoadingPanel.hide("ItemDetailTable");
        }, (error) => {
            this._ajaxLoadingPanel.hide("ItemDetailTable");
            this.showNotificationException(error);
        });
    }

    clearProcessDetails(): void {
        this._lblProcessName.innerText = "";
        this._lblClasificationName.innerText = "";
        this._lblFascicleType.innerText = "";
    }

    setDossierFolderWorkflowRepositories(): void {
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
            this._ajaxLoadingPanel.hide("workflowDetails");
        }, (error) => {
            this._ajaxLoadingPanel.hide("workflowDetails");
            this.showNotificationException(error);
        });
    }

    setDossierFolderRoles(): void {
        this._dossierFolderService.getDossierFolderById(uscProcessDetails.selectedDossierFolderId, (data) => {
            let dossierFolder: DossierFolderModel = data[0];
            if (dossierFolder.DossierFolderRoles && dossierFolder.DossierFolderRoles.length > 0) {
                this._uscRoleRest.renderRolesTree(dossierFolder.DossierFolderRoles.map(x => x.Role));
            }
        }, (error) => {
            this._ajaxLoadingPanel.hide("workflowDetails");
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
                this._ajaxLoadingPanel.show("workflowDetails");
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
                this._processFascicleWorkflowRepositoryService.insert(processFascicleWorkflowRepository, (data) => {
                    let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(selectedWorkflowRepository.Name);
                    node.set_value(data.UniqueId);
                    this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().add(node);
                    this._rtvWorkflowRepository.get_nodes().getNode(0).expand();
                    this._ajaxLoadingPanel.hide("workflowDetails");
                }, (error) => {
                    this._ajaxLoadingPanel.hide("workflowDetails");
                    this.showNotificationException(error);
                });
                break;
            }
            case "delete": {
                if (this._rtvWorkflowRepository.get_selectedNode() === null || this._rtvWorkflowRepository.get_selectedNode().get_level() === 0) {
                    alert("Selezionare flusso di lavoro");
                    return;
                }
                this._ajaxLoadingPanel.show("workflowDetails");
                let processFascicleWorkflowRepository: ProcessFascicleWorkflowRepositoryModel = <ProcessFascicleWorkflowRepositoryModel>{};
                processFascicleWorkflowRepository.UniqueId = this._rtvWorkflowRepository.get_selectedNode().get_value();
                this._processFascicleWorkflowRepositoryService.delete(processFascicleWorkflowRepository, (data) => {
                    this._rtvWorkflowRepository.get_nodes().getNode(0).get_nodes().remove(this._rtvWorkflowRepository.get_selectedNode());
                    this._ajaxLoadingPanel.hide("workflowDetails");
                }, (error) => {
                    this._ajaxLoadingPanel.hide("workflowDetails");
                    this.showNotificationException(error);
                });
                break;
            }
        }
    }

    rbAddFascicle_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._ajaxLoadingPanel.show("fascicleDetails");
        let fascicleModel: FascicleModel = new FascicleModel();
        fascicleModel.FascicleObject = this._rtbFascicleSubject.get_textBoxValue();
        fascicleModel.VisibilityType = this._rbFascicleVisibilityType.get_checked() ? VisibilityType.Accessible : VisibilityType.Confidential;
        fascicleModel.FascicleRoles = [];
        for (let responsibleRole of uscProcessDetails.responsibleRoles) {
            let fascicleRole: FascicleRoleModel = new FascicleRoleModel();
            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
            fascicleRole.IsMaster = true;
            fascicleRole.Role = responsibleRole;
            fascicleModel.FascicleRoles.push(fascicleRole);
        }
        for (let authorizedRole of uscProcessDetails.authorizedRoles) {
            let fascicleRole: FascicleRoleModel = new FascicleRoleModel();
            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
            fascicleRole.IsMaster = false;
            fascicleRole.Role = authorizedRole;
            fascicleModel.FascicleRoles.push(fascicleRole);
        }
        this.fascicleFolders = [];
        this.getFascicleFolderListFromTree(this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0));
        fascicleModel.FascicleFolders = this.fascicleFolders;
        fascicleModel.Contacts = uscProcessDetails.contacts;
        fascicleModel.MetadataRepository = new MetadataRepositoryModel();
        if (this._rcbMetadataRepository.get_selectedItem()) {
            fascicleModel.MetadataRepository.Name = this._rcbMetadataRepository.get_selectedItem().get_text();
            fascicleModel.MetadataRepository.UniqueId = this._rcbMetadataRepository.get_selectedItem().get_value();
        }
        let processFascicleTemplate: ProcessFascicleTemplateModel = <ProcessFascicleTemplateModel>{};
        processFascicleTemplate.DossierFolder = <DossierFolderModel>{};
        processFascicleTemplate.Process = <ProcessModel>{};
        processFascicleTemplate.UniqueId = uscProcessDetails.selectedProcessFascicleTemplateId;
        processFascicleTemplate.Process.UniqueId = uscProcessDetails.selectedProcessId;
        processFascicleTemplate.JsonModel = JSON.stringify(fascicleModel);
        sessionStorage.setItem("ProcessFascicleTemplate", JSON.stringify(processFascicleTemplate));
        this._processFascicleTemplateService.update(processFascicleTemplate, (data) => {
            alert("Modificato con successo");
            this._ajaxLoadingPanel.hide("fascicleDetails");
        }, (error) => {
            this._ajaxLoadingPanel.hide("fascicleDetails");
            this.showNotificationException(error);
        });
    }

    private populateDetails(data: any) {
        let processFascicleTemplate: ProcessFascicleTemplateModel = data;
        if (processFascicleTemplate.JsonModel === "") {
            return;
        }
        let fascicle: FascicleModel = JSON.parse(processFascicleTemplate.JsonModel);
        this._rtbFascicleSubject.set_value(fascicle.FascicleObject);
        this._rbFascicleVisibilityType.set_checked(fascicle.VisibilityType === VisibilityType.Accessible);
        uscProcessDetails.responsibleRoles = fascicle.FascicleRoles.filter(x => x.IsMaster === true).map(x => x.Role);
        this._uscResponsibleRoles.renderRolesTree(uscProcessDetails.responsibleRoles);
        uscProcessDetails.authorizedRoles = fascicle.FascicleRoles.filter(x => x.IsMaster === false).map(x => x.Role);
        this._uscAuthorizedRoles.renderRolesTree(uscProcessDetails.authorizedRoles);
        this.populateFascicleFoldersTree(fascicle.FascicleFolders);
        uscProcessDetails.contacts = fascicle.Contacts;
        this._uscContactRest.renderContactsTree(uscProcessDetails.contacts);
        let rcbItem: Telerik.Web.UI.RadComboBoxItem = this._rcbMetadataRepository.findItemByValue(fascicle.MetadataRepository.UniqueId);
        if (rcbItem) {
            rcbItem.select();
        }
    }

    getFascicleFolderListFromTree(fascicleFoldersNode: Telerik.Web.UI.RadTreeNode): void {
        if (fascicleFoldersNode.get_level() === 0) {
            let fascicleFolder: FascicleFolderModel = <FascicleFolderModel>{};
            fascicleFolder.Name = fascicleFoldersNode.get_text();
            fascicleFolder.UniqueId = fascicleFoldersNode.get_value();
            fascicleFolder.Typology = FascicleFolderTypology.Fascicle;
            fascicleFolder.Status = FascicleFolderStatus.Active;
            this.fascicleFolders.push(fascicleFolder);
            fascicleFoldersNode = fascicleFoldersNode.get_nodes().getNode(0);
        }
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
            if (child.get_attributes().getAttribute("hasChildren")) {
                this.getFascicleFolderListFromTree(child);
            }
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
        this._rbFascicleVisibilityType.set_checked(false);
        this._uscResponsibleRoles.renderRolesTree([]);
        this._uscAuthorizedRoles.renderRolesTree([]);
        this.initializeUserControls();
        this._uscFascicleFolders.getFascicleFolderTree().get_nodes().getNode(0).get_nodes().getNode(0).get_nodes().clear();
        this._uscContactRest.renderContactsTree([]);
        this._rcbMetadataRepository.set_selectedItem(this._rcbMetadataRepository.get_items().getItem(0));
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
}

export = uscProcessDetails;