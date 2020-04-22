import FascBase = require("./FascBase");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("../App/Helpers/ServiceConfigurationHelper");
import DossierFolderService = require("../App/Services/Dossiers/DossierFolderService");
import ExceptionDTO = require("../App/DTOs/ExceptionDTO");
import ProcessService = require("../App/Services/Processes/ProcessService");
import ProcessModel = require("../App/Models/Processes/ProcessModel");
import DossierFolderModel = require("../App/Models/Dossiers/DossierFolderModel");
import ProcessFascicleTemplateService = require("../App/Services/Processes/ProcessFascicleTemplateService");
import ProcessFascicleTemplateModel = require("../App/Models/Processes/ProcessFascicleTemplateModel");
import FascicleModel = require("../App/Models/Fascicles/FascicleModel");
import UscCategoryRest = require("../UserControl/uscCategoryRest");
import CategoryModel = require("../App/Models/Commons/CategoryModel");
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import UscContattiSelRest = require("../UserControl/uscContattiSelRest");
import ContactModel = require("../App/Models/Commons/ContactModel");
import UscRoleRest = require("../UserControl/uscRoleRest");
import RoleModel = require("../App/Models/Commons/RoleModel");
import FascicleRoleModel = require("../App/Models/Fascicles/FascicleRoleModel");
import FascicleService = require("../App/Services/Fascicles/FascicleService");
import FascicleType = require("../App/Models/Fascicles/FascicleType");
import UscDynamicMetadata = require("../UserControl/uscDynamicMetadata");
import AjaxModel = require("../App/Models/AjaxModel");
import MetadataRepositoryModel = require("../App/Models/Commons/MetadataRepositoryModel");

declare var Page_IsValid: any;

class FascProcessInserimento extends FascBase {

    private static DOSSIER_FOLDER_TYPE_NAME = "DossierFolder";
    private static PROCESS_TYPE_NAME = "Process";
    private static PROCESS_FASCICLE_TEMPLATE_TYPE_NAME = "ProcessFascicleTemplate";

    private _serviceConfigurations: ServiceConfiguration[];
    private _dossierFolderService: DossierFolderService;
    private _processService: ProcessService;
    private _processFascicleTemplateService: ProcessFascicleTemplateService;
    private _fascicleService: FascicleService;

    private _ddlProcess: Telerik.Web.UI.RadComboBox;
    private _rtvProcessFolders: Telerik.Web.UI.RadTreeView;
    private _ddlTemplate: Telerik.Web.UI.RadComboBox;
    private _rowTemplate: any;
    private _rowDossierFolders: any;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _processFascicleTemplates: ProcessFascicleTemplateModel[];
    private _processes: ProcessModel[];
    private _currentProcess: ProcessModel;
    private _currentFascicleTemplate: ProcessFascicleTemplateModel;
    private _currentDossierFolder: DossierFolderModel; 
    private _dossierFolders: DossierFolderModel[];
    private _fascicleModel: FascicleModel;
    private _uscCategoryRest: UscCategoryRest;
    private _uscMetadataRepository: UscMetadataRepositorySel;
    private _uscDynamicMetadata: UscDynamicMetadata;
    private _uscContact: UscContattiSelRest;
    private _uscRoleMaster: UscRoleRest;
    private _uscRole: UscRoleRest;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _txtConservation: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _radStartDate: Telerik.Web.UI.RadDatePicker;
    private _btnInsert: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

    ddlProcessId: string;
    rtvProcessFoldersId: string;
    rtvFascicleFoldersId: string;
    ddlTemplateId: string;
    rowTemplateId: string;
    rowDossierFoldersId: string;
    ajaxLoadingPanelId: string;
    pnlFascProcessInsertId: string;
    ajaxManagerId: string;
    uscCategoryId: string;
    txtObjectId: string;
    uscMetadataRepositorySelId: string;
    uscDynamicMetadataId: string;
    uscContactId: string;
    uscRoleMasterId: string;
    uscRoleId: string;
    txtConservationId: string;
    txtNoteId: string;
    radStartDateId: string;
    btnInsertId: string;
    uscNotificationId: string;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize(): void {
        super.initialize();

        let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascProcessInserimento.DOSSIER_FOLDER_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let processConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascProcessInserimento.PROCESS_TYPE_NAME);
        this._processService = new ProcessService(processConfiguration);

        let processFascicleTemplateConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascProcessInserimento.PROCESS_FASCICLE_TEMPLATE_TYPE_NAME);
        this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);

        let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_TYPE_NAME);
        this._fascicleService = new FascicleService(fascicleConfiguration);

        this._ddlProcess = <Telerik.Web.UI.RadComboBox>$find(this.ddlProcessId);
        this._ddlProcess.add_selectedIndexChanged(this._ddlProcess_OnClientSelectedIndexChanged);
        this._rtvProcessFolders = <Telerik.Web.UI.RadTreeView>$find(this.rtvProcessFoldersId);
        this._rtvProcessFolders.add_nodeClicked(this._rtvProcessFolders_OnNodeClicked);
        this._ddlTemplate = <Telerik.Web.UI.RadComboBox>$find(this.ddlTemplateId);
        this._ddlTemplate.add_selectedIndexChanged(this._ddlTemplate_OnClientSelectedIndexChanged);
        this._rowTemplate = $('#'.concat(this.rowTemplateId));
        this._rowDossierFolders = $('#'.concat(this.rowDossierFoldersId));
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._uscCategoryRest = <UscCategoryRest>$("#".concat(this.uscCategoryId)).data();
        this._uscCategoryRest.setFascicleTypeParam(FascicleType.Procedure);
        this._uscMetadataRepository = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataRepositorySelId)).data();
        this._uscDynamicMetadata = <UscDynamicMetadata>$("#".concat(this.uscDynamicMetadataId)).data();
        this._uscContact = <UscContattiSelRest>$("#".concat(this.uscContactId)).data();
        this._uscRoleMaster = <UscRoleRest>$("#".concat(this.uscRoleMasterId)).data();
        this._uscRole = <UscRoleRest>$("#".concat(this.uscRoleId)).data();        
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._txtConservation = <Telerik.Web.UI.RadTextBox>$find(this.txtConservationId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._radStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.radStartDateId);
        this._btnInsert = <Telerik.Web.UI.RadButton>$find(this.btnInsertId);
        this._btnInsert.add_clicking(this._btnInsert_OnClick);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._fascicleModel = new FascicleModel();
        this._fascicleModel.FascicleRoles = [];
        this._fascicleModel.Contacts = [];

        this._rowDossierFolders.hide();
        this._rowTemplate.hide();

        this._uscRoleMaster.renderRolesTree([]);
        this._uscRole.renderRolesTree([]);
        this._uscContact.renderContactsTree([]);

        this.loadProcesses();
        this.registerUscRoleRestEventHandlers();
        this.registerUscContactRestEventHandlers();
        this.setMetadataRepositorySelectedIndexEvent();
    }

    _ddlProcess_OnClientSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this._loadingPanel.show(this.pnlFascProcessInsertId);
        this._rtvProcessFolders.get_nodes().clear();
        this._currentProcess = this._processes.filter(x => x.UniqueId === args.get_item().get_value())[0];
        this._dossierFolderService.getProcessFolders(null, this._ddlProcess.get_value(), false, false,
            (dossierFolders: DossierFolderModel[]) => {  
                this._dossierFolders = dossierFolders;
                let node: Telerik.Web.UI.RadTreeNode = null;
                if (dossierFolders.length < 2) {
                    this._rowDossierFolders.hide();
                }
                else {
                    this._rowDossierFolders.show();
                }
                this._rowDossierFolders.show();
                for (let dossierFolder of dossierFolders) {
                    this._rtvProcessFolders.trackChanges();

                    node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(dossierFolder.Name);
                    node.set_value(dossierFolder.UniqueId);
                    node.set_imageUrl('../App_Themes/DocSuite2008/imgset16/fascicle_open.png');
                    this._rtvProcessFolders.get_nodes().add(node);
                    this.fillDossiersRecursive(dossierFolder.DossierFolders, node);
                }

                let firstNode = this._rtvProcessFolders.get_nodes().getNode(0);
                firstNode.set_selected(true);
                this.loadDossier(firstNode.get_value());
                this.loadProcessTemplates(firstNode.get_value());
                this.loadCategory(this._currentProcess.Category);
                this._rtvProcessFolders.commitChanges();
                this.clearFascicleFields();
                this._loadingPanel.hide(this.pnlFascProcessInsertId);
            },
            (exception: ExceptionDTO) => {

            }
        );
    }

    _rtvProcessFolders_OnNodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        if (!args.get_node()) {
            return;
        }

        this.loadDossier(args.get_node().get_value());
        this.loadProcessTemplates(args.get_node().get_value());
    }

    _ddlTemplate_OnClientSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        if (!args.get_item()) {
            return;
        }
        this._currentFascicleTemplate  = this._processFascicleTemplates.filter(x => x.UniqueId === args.get_item().get_value())[0];

        this.loadFascicleFields(this._currentFascicleTemplate.JsonModel);        
    }

    _btnInsert_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        if (!Page_IsValid) {
            args.set_cancel(true);
            return;
        }

        this._btnInsert.set_enabled(false);

        this._fascicleModel.Category = this.getCategory();
        this._fascicleModel.FascicleObject = this._txtObject.get_textBoxValue();
        this._fascicleModel.Note = this._txtNote.get_textBoxValue();
        this._fascicleModel.StartDate = this._radStartDate.get_selectedDate();
        this._fascicleModel.Conservation = parseInt(this._txtConservation.get_textBoxValue());
        this._fascicleModel.FascicleType = this._currentProcess.FascicleType;
        let dossierFolders: DossierFolderModel[] = [];
        dossierFolders.push(this._currentDossierFolder);
        this._fascicleModel.DossierFolders = dossierFolders;
        this._fascicleModel.FascicleTemplate = this._currentFascicleTemplate;
        this._loadingPanel.show(this.pnlFascProcessInsertId);
                
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "Insert";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        args.set_cancel(true);
    }

    insertCallback(metadataModel: string): void {
        this._fascicleModel.MetadataValues = metadataModel;
        if (sessionStorage.getItem("MetadataRepository")) {
            let metadataRepository: MetadataRepositoryModel = new MetadataRepositoryModel();
            metadataRepository.UniqueId = sessionStorage.getItem("MetadataRepository");
            this._fascicleModel.MetadataRepository = metadataRepository;
        }  

        this._fascicleService.insertFascicle(this._fascicleModel,
            (data: FascicleModel) => {
                window.location.href = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${data.UniqueId}`;
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlFascProcessInsertId);
                this._btnInsert.set_enabled(true);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private loadDossier(uniqueId: string) {
        this._currentDossierFolder = this._dossierFolders.filter(x => x.UniqueId == uniqueId)[0];

        this.clearFascicleFields();
        this._ddlTemplate.set_enabled(true);
        this._ddlTemplate.get_items().clear();
        this._ddlTemplate.clearSelection();
        this._ddlTemplate.enable();
    }

    private loadProcesses() {
        this._loadingPanel.show(this.pnlFascProcessInsertId);
        this._processService.getAvailableProcesses(null, false, null, null,
            (processes: ProcessModel[]) => {
                this._processes = processes;
                let today = new Date();
                let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
                processes = processes.filter(x => new Date(x.StartDate) < today && new Date(x.EndDate) > today);
                for (let process of processes) {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(process.Name);
                    cmbItem.set_value(process.UniqueId);
                    this._ddlProcess.get_items().add(cmbItem);
                    if (processes.length == 1) {
                        let selectedItem: Telerik.Web.UI.RadComboBoxItem = this._ddlProcess.findItemByValue(process.UniqueId);
                        selectedItem.select();
                        this._ddlProcess.disable();
                    };
                }
                this._loadingPanel.hide(this.pnlFascProcessInsertId);
            },
            (exception: ExceptionDTO) => {

            }
        );
    }

    private fillDossiersRecursive(dossierFolders: DossierFolderModel[], currentNode: Telerik.Web.UI.RadTreeNode) {
        let dossierChildNode = null;
        for (let dossierFolder of dossierFolders) {

            dossierChildNode = new Telerik.Web.UI.RadTreeNode();
            dossierChildNode.set_text(dossierFolder.Name);
            dossierChildNode.set_value(dossierFolder.UniqueId);
            dossierChildNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/fascicle_open.png');
            currentNode.get_nodes().add(dossierChildNode);

            if (dossierFolder.DossierFolders.length >= 1) {
                this.fillDossiersRecursive(dossierFolder.DossierFolders, dossierChildNode);
            }            
        }
    } 

    private loadProcessTemplates(dossierFolderId: string) {
        this._processFascicleTemplateService.getFascicleTemplateByDossierFolderId(dossierFolderId,
            (processFascicleTemplates: ProcessFascicleTemplateModel[]) => {
                this._processFascicleTemplates = processFascicleTemplates;
                let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
                if (processFascicleTemplates.length === 0) {
                    this._rowTemplate.hide();
                }
                else {
                    this._rowTemplate.show();
                }
                for (let template of processFascicleTemplates) {
                    cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                    cmbItem.set_text(template.Name);
                    cmbItem.set_value(template.UniqueId);
                    this._ddlTemplate.get_items().add(cmbItem);
                    if (processFascicleTemplates.length == 1) {
                        let selectedItem: Telerik.Web.UI.RadComboBoxItem = this._ddlTemplate.findItemByValue(template.UniqueId);
                        selectedItem.select();
                        this._ddlTemplate.disable();
                    };
                }
            },
            (exception: ExceptionDTO) => {

            });
    }

    private loadFascicleFields(jsonModel: string) {
        let fascicleModel: FascicleModel = new FascicleModel();

        try {
            fascicleModel = JSON.parse(jsonModel);

            this._txtObject.set_value(fascicleModel.FascicleObject);
            this.loadContacts(fascicleModel.Contacts);

            if (fascicleModel.MetadataRepository) {
                this.loadMetadataRepository(fascicleModel.MetadataRepository.UniqueId);
            }
            else {
                this._uscMetadataRepository.clearComboboxText();
            }

            if(fascicleModel.FascicleRoles.filter(x => x.IsMaster)[0]) {
                this._uscRoleMaster.setToolbarVisibility(false);
            }

            if (fascicleModel.FascicleRoles.filter(x => !x.IsMaster)[0]) {
                this._uscRole.setToolbarVisibility(false);
            }

            if (fascicleModel.Contacts.length > 0) {
                this._uscContact.setToolbarVisibility(false);
            }

            this.loadRoles(fascicleModel.FascicleRoles);

            this._fascicleModel = fascicleModel;
            
        }
        catch (e) {
            this.clearFascicleFields();
            return;
        }
    }

    private loadMetadataRepository(id: string) {
        this._uscMetadataRepository.setComboboxText(id);
    }

    private loadContacts(contactsModel: ContactModel[]) {
        this._uscContact.renderContactsTree(contactsModel);
    }

    private loadCategory(categoryModel: CategoryModel) {
        if (!categoryModel) {
            return;
        }
        this._uscCategoryRest.addDefaultNode(categoryModel);
        this._uscCategoryRest.disableButtons();
    }

    private loadRoles(fascicleRoles: FascicleRoleModel[]) {
        let rolesModel: RoleModel[] = [];
        let masterRolesModel: RoleModel[] = [];

        for (let fascicleRole of fascicleRoles) {
            if (fascicleRole.IsMaster) {
                masterRolesModel.push(fascicleRole.Role);
            }
            else {
                rolesModel.push(fascicleRole.Role);
            }
        }

        this._uscRoleMaster.renderRolesTree(masterRolesModel);
        this._uscRole.renderRolesTree(rolesModel);
    }

    private clearFascicleFields() {
        this._txtObject.clear();
        let fascicleRoles: FascicleRoleModel[] = [];
        let contacts: ContactModel[] = [];
        this.loadRoles(fascicleRoles);//pass empty array so that usc sets no roles
        this._uscRole.setToolbarVisibility(true);
        this._uscRoleMaster.setToolbarVisibility(true);
        this.loadContacts(contacts);//pass empty array so that usc sets no contacts
        this._uscContact.setToolbarVisibility(true);
        this._uscMetadataRepository.clearComboboxText();
        this._uscDynamicMetadata.loadDynamicMetadata("");
    }

    private registerUscRoleRestEventHandlers(): void {
        let uscRoleRestEvents = this._uscRole.uscRoleRestEvents;

        this._uscRole.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteRoleFromModelPromise);
        this._uscRole.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.addRoleToModelPromise);

        this._uscRoleMaster.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteRoleFromModelPromise);
        this._uscRoleMaster.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.addRoleMasterToModelPromise);
    } 

    private registerUscContactRestEventHandlers(): void {
        let uscContactRestEvents = this._uscContact.uscContattiSelRestEvents;

        this._uscContact.registerEventHandler(uscContactRestEvents.ContactDeleted, this.deleteContactFromModelPromise);
        this._uscContact.registerEventHandler(uscContactRestEvents.NewContactsAdded, this.addContactToModelPromise);
    }

    private deleteContactFromModelPromise = (contactIdToDelete: number): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (!contactIdToDelete)
            return promise.promise();

        let contactParentId = this._fascicleModel.Contacts.filter(x => x.EntityId === contactIdToDelete)[0].IncrementalFather;
        this._fascicleModel.Contacts = this._fascicleModel.Contacts.filter(x => x.EntityId !== contactIdToDelete);

        return promise.resolve(contactParentId);
    }

    private addContactToModelPromise = (newAddedContact: ContactModel): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (!newAddedContact)
            return promise.promise();

        this._fascicleModel.Contacts.push(newAddedContact);

        return promise.resolve(newAddedContact);
    }

    private deleteRoleFromModelPromise = (roleIdToDelete: number): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (!roleIdToDelete)
            return promise.promise();
        this._fascicleModel.FascicleRoles = this._fascicleModel.FascicleRoles.filter(x => x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);

        return promise.resolve();
    }

    private addRoleToModelPromise = (newAddedRoles: RoleModel[]): JQueryPromise<any> => {               
        return this.addRole(newAddedRoles, false);
    }

    private addRoleMasterToModelPromise = (newAddedRoles: RoleModel[]): JQueryPromise<any> => {
        return this.addRole(newAddedRoles, true);
    }

    private addRole(newAddedRoles: RoleModel[], isMaster: boolean) {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (!newAddedRoles.length)
            return promise.promise();

        for (let newAddedRole of newAddedRoles) {
            let fascicleRole = new FascicleRoleModel();
            fascicleRole.IsMaster = isMaster;
            fascicleRole.Role = newAddedRole;
            this._fascicleModel.FascicleRoles.push(fascicleRole);
        }

        return promise.resolve();
    }

    private getCategory(): CategoryModel {
        let category: CategoryModel = new CategoryModel;

        category.Code = this._currentProcess.Category.Code;
        category.Name = this._currentProcess.Category.Name;
        category.EntityShortId = this._currentProcess.Category.IdCategory;

        return category;
    }

    private setMetadataRepositorySelectedIndexEvent() {
        $("#".concat(this.uscMetadataRepositorySelId)).off(UscMetadataRepositorySel.SELECTED_INDEX_EVENT);
        $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, (args, data) => {
            if (!jQuery.isEmptyObject(this._uscDynamicMetadata)) {
                setTimeout(() => {
                    this._uscDynamicMetadata.loadDynamicMetadata(data);
                }, 500);
            }
        });
    }

}

export = FascProcessInserimento;