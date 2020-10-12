/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />

import PageClassHelper = require("App/Helpers/PageClassHelper");
import FascicleType = require("App/Models/Fascicles/FascicleType");
import uscCategoryRest = require("UserControl/uscCategoryRest");
import uscContattiSelREST = require("UserControl/uscContattiSelREST");
import uscRoleRest = require("./uscRoleRest");
import UscRoleRestEventType = require("App/Models/Commons/UscRoleRestEventType");
import uscMetadataRepositorySel = require("./uscMetadataRepositorySel");
import uscDynamicMetadataRest = require("./uscDynamicMetadataRest");
import FascicleRoleModel = require("App/Models/Fascicles/FascicleRoleModel");
import RoleModel = require("App/Models/Commons/RoleModel");
import ContactModel = require("App/Models/Commons/ContactModel");
import EnumHelper = require("App/Helpers/EnumHelper");
import ProcessFascicleTemplateModel = require("App/Models/Processes/ProcessFascicleTemplateModel");
import ProcessModel = require("App/Models/Processes/ProcessModel");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import ProcessNodeType = require("App/Models/Processes/ProcessNodeType");
import ExternalSourceActionEnum = require("App/Helpers/ExternalSourceActionEnum");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierFolderService = require("App/Services/Dossiers/DossierFolderService");
import ProcessService = require("App/Services/Processes/ProcessService");
import ProcessFascicleTemplateService = require("App/Services/Processes/ProcessFascicleTemplateService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import uscErrorNotification = require("./uscErrorNotification");
import VisibilityType = require("App/Models/Fascicles/VisibilityType");
import CategoryModel = require("App/Models/Commons/CategoryModel");
import SetiContactModel = require("App/Models/Commons/SetiContactModel");
import MetadataDesignerViewModel = require("App/ViewModels/Metadata/MetadataDesignerViewModel");
import uscCustomActionsRest = require('UserControl/uscCustomActionsRest');
import FascicleCustomActionModel = require('App/Models/Commons/FascicleCustomActionModel');
import CategoryFascicleViewModel = require("App/ViewModels/Commons/CategoryFascicleViewModel");
import AuthorizationRoleType = require("App/Models/Commons/AuthorizationRoleType");

class uscFascicleProcessInsert {
    clientId: string;
    pageId: string;
    ajaxManagerId: string;
    uscCategoryId: string;
    uscMetadataRepositorySelId: string;
    uscDynamicMetadataRestId: string;
    uscContactId: string;
    uscRoleMasterId: string;
    uscRoleId: string;
    uscNotificationId: string;
    txtObjectId: string;
    txtConservationId: string;
    txtNoteId: string;
    rcbFascicleTypeId: string;
    pnlContactId: string;
    pnlRoleMasterId: string;
    pnlConservationId: string;
    pnlFascProcessInsertId: string;
    activityFascicleEnabled: boolean;
    uscCustomActionsRestId: string;
    customActionFromSession: boolean = false;

    private static DOSSIER_FOLDER_TYPE_NAME: string = "DossierFolder";
    private static PROCESS_TYPE_NAME: string = "Process";
    private static PROCESS_FASCICLE_TEMPLATE_TYPE_NAME: string = "ProcessFascicleTemplate";
    public static LOADED_EVENT: string = "onLoaded";

    private needRolesFromExternalSource_eventArgs: string[];

    private readonly _dossierFolderService: DossierFolderService;
    private readonly _processService: ProcessService;
    private readonly _processFascicleTemplateService: ProcessFascicleTemplateService;
    private readonly _enumHelper: EnumHelper = new EnumHelper();

    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _txtConservation: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _rcbFascicleType: Telerik.Web.UI.RadComboBox;

    private _pnlContact(): JQuery {
        return $(`#${this.pnlContactId}`);
    }

    private _pnlRoleMaster(): JQuery {
        return $(`#${this.pnlRoleMasterId}`);
    }

    private _pnlConservation(): JQuery {
        return $(`#${this.pnlConservationId}`);
    }

    private selectedResponsibleRole: RoleModel;

    /**
    * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscFascicleProcessInsert.DOSSIER_FOLDER_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let processConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscFascicleProcessInsert.PROCESS_TYPE_NAME);
        this._processService = new ProcessService(processConfiguration);

        let processFascicleTemplateConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, uscFascicleProcessInsert.PROCESS_FASCICLE_TEMPLATE_TYPE_NAME);
        this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */

    rcbFascicleType_selectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let elements: JQuery[] = [this._pnlContact(), this._pnlRoleMaster(), this._pnlConservation()];
        let procedureSelected: boolean = args.get_item().get_value() === FascicleType[FascicleType.Procedure];
        this.setVisibilityForManyElements(elements, procedureSelected);
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelREST>(this.uscContactId)
            .done((instance) => {
                instance.forceBehaviourValidationState(procedureSelected);
                instance.enableValidators(procedureSelected);
            });

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId)
            .done((instance) => {
                instance.forceBehaviourValidationState(procedureSelected);
                instance.enableValidators(procedureSelected);
            });

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                instance.forceBehaviourValidationState(!procedureSelected);
                instance.enableValidators(!procedureSelected);
                instance.disableRaciRoleButton();
            });
    }

    uscCategory_CategoryAdded = (eventObject: JQueryEventObject): void => {
        PageClassHelper.callUserControlFunctionSafe<uscCategoryRest>(this.uscCategoryId)
            .done((instance) => {
                switch (instance.getSelectedNode().get_attributes().getAttribute("NodeType")) {
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        this.clearFascicleFields();
                        this._processFascicleTemplateService.getById(instance.getSelectedNode().get_value(), (data: ProcessFascicleTemplateModel) => {
                            this.setFascicleTemplateToSession(data);
                            this.loadFascicleFields(data);
                        });
                        this._processService.getById(instance.getProcessId(), (data: ProcessModel) => {
                            this.setProcessToSession(data);
                        });
                        this._dossierFolderService.getDossierFolderById(instance.getProcessFascicleTemplateFolderId(), (data: DossierFolderModel[]) => {
                            this.setDossierFolderToSession(data[0]);
                        });
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        this._processService.getById(instance.getProcessId(), (data: ProcessModel) => {
                            this.setProcessToSession(data);
                        });
                        this._dossierFolderService.getDossierFolderById(instance.getProcessFascicleTemplateFolderId(), (data: DossierFolderModel[]) => {
                            this.setDossierFolderToSession(data[0]);

                            //set popup roles source
                            this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.DossierFolder.toString(), instance.getProcessFascicleTemplateFolderId()];
                            $(`#${this.uscRoleMasterId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);

                            this._rcbFascicleType.findItemByValue(FascicleType[FascicleType.Procedure]).select();
                            this._rcbFascicleType.enable();
                        });
                        break;
                    }
                    case ProcessNodeType.Category: {
                        PageClassHelper.callUserControlFunctionSafe<uscCategoryRest>(this.uscCategoryId)
                            .done((instance) => {
                                let selectedCategoryId: number = instance.getSelectedNode().get_value();
                                let selectedCategoryText: string = instance.getSelectedNode().get_text();
                                //set popup roles source
                                this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Category.toString(), selectedCategoryId.toString()];
                                $(`#${this.uscRoleMasterId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);

                                let category: CategoryModel = new CategoryModel();
                                category.EntityShortId = selectedCategoryId;
                                category.Code = +(selectedCategoryText.split(".")[0]);
                                category.Name = selectedCategoryText.split(".")[1];
                                this.setCategoryToSession(category);
                                if (this.customActionFromSession) {
                                    instance.getCategoryFascicles(selectedCategoryId).then((data: CategoryFascicleViewModel[]) => {
                                        let customActionsJson: string = data[0].CustomActions;
                                        if (customActionsJson) {
                                            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                                                .done((instance) => {
                                                    instance.loadItems(<FascicleCustomActionModel>JSON.parse(customActionsJson));
                                                });
                                        }
                                    });
                                }
                            });
                    }
                }
            });
    }

    /**
    *------------------------- Methods -----------------------------
    */

    initialize() {
        PageClassHelper.callUserControlFunctionSafe<uscCategoryRest>(this.uscCategoryId)
            .done((instance) => instance.setFascicleTypeParam(FascicleType.Procedure));

        this._txtObject = $find(this.txtObjectId) as Telerik.Web.UI.RadTextBox;
        this._txtConservation = $find(this.txtConservationId) as Telerik.Web.UI.RadTextBox;
        this._txtNote = $find(this.txtNoteId) as Telerik.Web.UI.RadTextBox;
        this._ajaxManager = $find(this.ajaxManagerId) as Telerik.Web.UI.RadAjaxManager;
        this._rcbFascicleType = $find(this.rcbFascicleTypeId) as Telerik.Web.UI.RadComboBox;
        this._rcbFascicleType.add_selectedIndexChanged(this.rcbFascicleType_selectedIndexChanged);

        this.clearFascicleFields();

        this.registerUscRoleRestEventHandlers();
        this.registerUscContactRestEventHandlers();
        this.setMetadataRepositorySelectedIndexEvent();
        this.loadFascicleTypes();

        $(`#${this.uscCategoryId}`).bind(uscCategoryRest.ADDED_EVENT, this.uscCategory_CategoryAdded);

        /*event for filing out the fields with the chosen Seti contact*/
        $("#".concat(this.uscMetadataRepositorySelId)).on(uscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, (sender, args: SetiContactModel) => {
            PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
                .done((instance) => instance.populateMetadataRepository(args));
        });

        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                instance.loadItems(<FascicleCustomActionModel>{
                    AutoClose: false,
                    AutoCloseAndClone: false
                });
            });

        this.bindLoaded();
    }

    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscFascicleProcessInsert.LOADED_EVENT);
    }

    private setVisibilityForManyElements(elements: JQuery[], isVisible: boolean): void {
        for (let element of elements) {
            isVisible ? element.show() : element.hide();
        }
    }

    private registerUscRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.addRoleToModel(this.uscRoleMasterId, newAddedRoles, (role) => {
                        existedRole = role;
                    });
                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
            });

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.addRoleToModel(this.uscRoleId, newAddedRoles, (role) => {
                        existedRole = role;
                    });
                    if (!existedRole) {
                        this.selectedResponsibleRole = newAddedRoles[0];
                    }
                    return $.Deferred<RoleModel>().resolve(existedRole, true);
                });
            });
    }

    private registerUscContactRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelREST>(this.uscContactId)
            .done((instance) => {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, (contactIdToDelete: number) => {
                    this.deleteContactFromModel(contactIdToDelete);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, (newAddedContact: ContactModel) => {
                    this.addContactToModel(newAddedContact);
                    return $.Deferred<void>().resolve();
                });
            });
    }

    private setMetadataRepositorySelectedIndexEvent() {
        $(`#${this.uscMetadataRepositorySelId}`).off(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT);
        $(`#${this.uscMetadataRepositorySelId}`).on(uscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, (args, data) => {
            PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
                .done((instance) => {
                    if (data) {
                        instance.loadMetadataRepository(data);
                    } else {
                        // when no selection (or selected index is an invalid value like the first empty string in a drop down)
                        instance.clearPage();
                    }
                });
            
        });
    }

    private deleteRoleFromModel(roleIdToDelete: number): void {
        if (!roleIdToDelete)
            return;

        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        fascicleRoles = fascicleRoles.filter(x => x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
        this.setFascicleRolesToSession(fascicleRoles);
    }

    private setFascicleRolesToSession(fascicleRoles: FascicleRoleModel[]): void {
        if (!fascicleRoles) {
            sessionStorage.removeItem(`${this.clientId}_FascicleRolesToAdd`);
        }

        sessionStorage[`${this.clientId}_FascicleRolesToAdd`] = JSON.stringify(fascicleRoles);
    }

    private setFascicleContactsToSession(fascicleContacts: ContactModel[]): void {
        if (!fascicleContacts) {
            sessionStorage.removeItem(`${this.clientId}_FascicleContactsToAdd`);
        }

        sessionStorage[`${this.clientId}_FascicleContactsToAdd`] = JSON.stringify(fascicleContacts);
    }

    private setFascicleTemplateToSession(fascicleTemplate: ProcessFascicleTemplateModel): void {
        if (!fascicleTemplate) {
            sessionStorage.removeItem(`${this.clientId}_CurrentFascicleTemplate`);
        }

        sessionStorage[`${this.clientId}_CurrentFascicleTemplate`] = JSON.stringify(fascicleTemplate);
    }

    private setProcessToSession(process: ProcessModel): void {
        if (!process) {
            sessionStorage.removeItem(`${this.clientId}_CurrentProcess`);
        }

        sessionStorage[`${this.clientId}_CurrentProcess`] = JSON.stringify(process);
    }

    private setDossierFolderToSession(dossierFolder: DossierFolderModel): void {
        if (!dossierFolder) {
            sessionStorage.removeItem(`${this.clientId}_CurrentDossierFolder`);
        }

        sessionStorage[`${this.clientId}_CurrentDossierFolder`] = JSON.stringify(dossierFolder);
    }

    private setCategoryToSession(category: CategoryModel): void {
        if (!category) {
            sessionStorage.removeItem(`${this.clientId}_CurrentCategory`);
        }

        sessionStorage[`${this.clientId}_CurrentCategory`] = JSON.stringify(category);
    }

    private addRoleToModel(toCheckControlId: string, newAddedRoles: RoleModel[], existedRoleCallback?: (RoleModel) => void): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(toCheckControlId)
            .done((instance) => {
                let existedRole: RoleModel = instance.existsRole(newAddedRoles);
                if (existedRole) {
                    alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore ${toCheckControlId == this.uscRoleMasterId ? "responsabile" : "autorizzato"} del fascicolo`);
                    existedRoleCallback(existedRole);
                    newAddedRoles = newAddedRoles.filter(x => x.IdRole !== existedRole.IdRole);
                }
                if (toCheckControlId === this.uscRoleMasterId) {
                    return this.addRole(newAddedRoles, false);
                }
            });
    }

    private addRole(newAddedRoles: RoleModel[], isMaster: boolean): void {
        if (!newAddedRoles.length)
            return;

        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        for (let newAddedRole of newAddedRoles) {
            let fascicleRole = new FascicleRoleModel();
            fascicleRole.IsMaster = isMaster;
            fascicleRole.AuthorizationRoleType = isMaster
                ? AuthorizationRoleType.Responsible
                : AuthorizationRoleType.Accounted;
            fascicleRole.Role = newAddedRole;
            fascicleRoles.push(fascicleRole);
        }
        this.setFascicleRolesToSession(fascicleRoles);
    }

    private deleteContactFromModel(contactIdToDelete: number): void {
        if (!contactIdToDelete)
            return;

        let fascicleContacts: ContactModel[] = [];
        if (this.getFascicleContactsToAdd()) {
            fascicleContacts = this.getFascicleContactsToAdd();
        }

        fascicleContacts = fascicleContacts.filter(x => x.EntityId !== contactIdToDelete);
        this.setFascicleContactsToSession(fascicleContacts);
    }

    private addContactToModel(newAddedContact: ContactModel): void {
        if (!newAddedContact)
            return;

        let fascicleContacts: ContactModel[] = [];
        if (this.getFascicleContactsToAdd()) {
            fascicleContacts = this.getFascicleContactsToAdd();
        }

        fascicleContacts.push(newAddedContact);
        this.setFascicleContactsToSession(fascicleContacts);
    }

    private loadFascicleTypes(): void {
        let fascicleTypes: FascicleType[] = [FascicleType.Procedure];
        if (this.activityFascicleEnabled) {
            fascicleTypes.push(FascicleType.Activity);
        }
        this.setFascicleTypeItem(this._rcbFascicleType, fascicleTypes);
        let item: Telerik.Web.UI.RadComboBoxItem = this._rcbFascicleType.findItemByValue(FascicleType[FascicleType.Procedure]);
        item.select();

        if (this._rcbFascicleType.get_items().get_count() == 1) {
            this._rcbFascicleType.disable();
        }
    }

    private setFascicleTypeItem(comboBox: Telerik.Web.UI.RadComboBox, fascicleTypes: FascicleType[]): void {
        for (let itemType of fascicleTypes) {
            let item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(this._enumHelper.getFascicleTypeDescription(itemType));
            item.set_value(FascicleType[itemType]);
            comboBox.get_items().add(item);
        }
    }

    clearFascicleFields() {
        this._txtObject.clear();
        this.setFascicleRolesToSession([]);
        this.setFascicleContactsToSession([]);
        this.setFascicleTemplateToSession(null);
        this.setProcessToSession(null);
        this.setDossierFolderToSession(null);
        this.setCategoryToSession(null);
        this.loadRoles([]);
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => instance.setToolbarVisibility(true));
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => instance.setToolbarVisibility(true));
        this.loadContacts([]);
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelREST>(this.uscContactId).done((instance) => instance.setToolbarVisibility(true));
        PageClassHelper.callUserControlFunctionSafe<uscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.clearComboboxText());
        PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId).done((instance) => instance.clearPage());
    }

    private loadRoles(items: FascicleRoleModel[]) {
        let rolesModel: RoleModel[] = [];
        let masterRolesModel: RoleModel[] = [];
        let raciRoles: RoleModel[] = [];

        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        for (let fascicleRole of items) {
            if (fascicleRole.IsMaster) {
                masterRolesModel.push(fascicleRole.Role);
            }
            else {
                if (fascicleRole.AuthorizationRoleType === AuthorizationRoleType.Responsible) {
                    raciRoles.push(fascicleRole.Role);
                }
                rolesModel.push(fascicleRole.Role);
            }
            fascicleRoles.push(fascicleRole);
        }

        this.setFascicleRolesToSession(fascicleRoles);
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => {
            if (raciRoles) {
                instance.setRaciRoles(raciRoles);
            }
            instance.renderRolesTree(rolesModel);
        });
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => instance.renderRolesTree(masterRolesModel));
    }

    private loadContacts(contacts: ContactModel[]) {
        let fascicleContacts: ContactModel[] = [];
        if (this.getFascicleContactsToAdd()) {
            fascicleContacts = this.getFascicleContactsToAdd();
        }

        for (let contact of contacts) {
            fascicleContacts.push(contact);
        }
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelREST>(this.uscContactId).done((instance) => instance.renderContactsTree(contacts));
    }

    private loadFascicleFields(fascicleTemplate: ProcessFascicleTemplateModel) {
        if (fascicleTemplate.JsonModel) {
            try {
                let fascicleTemplateModel: FascicleModel = JSON.parse(this.getCurrentFascicleTemplate().JsonModel);
                if (fascicleTemplateModel.FascicleType) {
                    this._rcbFascicleType.findItemByValue(FascicleType[fascicleTemplateModel.FascicleType]).select();
                    this._rcbFascicleType.disable();
                }
                if (fascicleTemplateModel.FascicleType === FascicleType.Activity) {
                    this.loadActivityFascicleFields(fascicleTemplateModel);
                }
                else {
                    this.loadProcedureFascicleFields(fascicleTemplateModel);
                }
                $(`#${this.pnlFascProcessInsertId}`).show();
            }
            catch (e) {
                this.clearFascicleFields();
                return;
            }
        }
        else {
            this.loadProcedureFascicleFields(new FascicleModel());
            $(`#${this.pnlFascProcessInsertId}`).show();
        }
    }

    loadActivityFascicleFields(fascicleTemplateModel: FascicleModel, generateMetadataInputs: boolean = true): void {
        let elements: JQuery[] = [this._pnlContact(), this._pnlRoleMaster(), this._pnlConservation()];
        this.setVisibilityForManyElements(elements, false);

        this._txtObject.set_value(fascicleTemplateModel.FascicleObject);

        if (fascicleTemplateModel.MetadataRepository && fascicleTemplateModel.MetadataRepository.UniqueId) {
            this.loadMetadataRepository(fascicleTemplateModel.MetadataRepository.UniqueId, generateMetadataInputs);
        }

        else {
            PageClassHelper.callUserControlFunctionSafe<uscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.clearComboboxText());
        }

        this.loadRoles(fascicleTemplateModel.FascicleRoles);

        if (fascicleTemplateModel.CustomActions) {
            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                .done((instance) => {
                    instance.loadItems(<FascicleCustomActionModel>JSON.parse(fascicleTemplateModel.CustomActions));
                });
        }
    }

    loadProcedureFascicleFields(fascicleTemplateModel: FascicleModel, generateMetadataInputs: boolean = true): void {
        let elements: JQuery[] = [this._pnlContact(), this._pnlRoleMaster(), this._pnlConservation()];
        this.setVisibilityForManyElements(elements, true);

        this._txtObject.set_value(fascicleTemplateModel.FascicleObject);
        this.loadContacts(fascicleTemplateModel.Contacts);

        if (fascicleTemplateModel.MetadataRepository && fascicleTemplateModel.MetadataRepository.UniqueId) {
            this.loadMetadataRepository(fascicleTemplateModel.MetadataRepository.UniqueId, generateMetadataInputs);
        }
        else {
            PageClassHelper.callUserControlFunctionSafe<uscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.clearComboboxText());
        }

        if (fascicleTemplateModel.FascicleRoles.filter(x => x.IsMaster)[0]) {
            PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleMasterId).done((instance) => instance.setToolbarVisibility(false));
        }
        else {
            PageClassHelper.callUserControlFunctionSafe<uscCategoryRest>(this.uscCategoryId).done((instance) => {
                //set popup roles source
                this.needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.DossierFolder.toString(), instance.getProcessFascicleTemplateFolderId()];
                $(`#${this.uscRoleMasterId}`).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, this.needRolesFromExternalSource_eventArgs);
            });
        }

        if (fascicleTemplateModel.Contacts.length > 0) {
            this.setFascicleContactsToSession(fascicleTemplateModel.Contacts);
            PageClassHelper.callUserControlFunctionSafe<uscContattiSelREST>(this.uscContactId).done((instance) => instance.setToolbarVisibility(false));
        }

        this.loadRoles(fascicleTemplateModel.FascicleRoles);

        if (fascicleTemplateModel.CustomActions) {
            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                .done((instance) => {
                    instance.loadItems(<FascicleCustomActionModel>JSON.parse(fascicleTemplateModel.CustomActions));
                });
        }
    }

    private loadMetadataRepository(id: string, generateMetadataInputs: boolean) {
        PageClassHelper.callUserControlFunctionSafe<uscMetadataRepositorySel>(this.uscMetadataRepositorySelId).done((instance) => instance.setComboboxText(id, generateMetadataInputs));
    }

    loadDefaultCategory(idCategory: number) {
        PageClassHelper.callUserControlFunctionSafe<uscCategoryRest>(this.uscCategoryId).done((instance) => {
            instance.addDefaultCategory(idCategory, true)
                .done(() => {
                    if (!instance.getSelectedCategory()) {
                        this.printCategoryNotFascicolable();
                    }
                })
                .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
        });
    }

    private printCategoryNotFascicolable(): void {
        this.showWarningMessage("Attenzione! Il classificatore selezionato non ha nessuna tipologia di fascicolazione associata");
    }

    protected showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            PageClassHelper.callUserControlFunctionSafe<uscErrorNotification>(this.uscNotificationId).done((instance) => instance.showNotification(exception));
        } else {
            this.showNotificationMessage(customMessage)
        }
    }

    private showNotificationMessage(customMessage: string) {
        PageClassHelper.callUserControlFunctionSafe<uscErrorNotification>(this.uscNotificationId).done((instance) => instance.showNotificationMessage(customMessage));
    }

    private showWarningMessage(customMessage: string) {
        PageClassHelper.callUserControlFunctionSafe<uscErrorNotification>(this.uscNotificationId).done((instance) => instance.showWarningMessage(customMessage));
    }

    getFascicle = (): JQueryPromise<FascicleModel> => {
        let promise: JQueryDeferred<FascicleModel> = $.Deferred<FascicleModel>();
        let fascicle: FascicleModel = new FascicleModel();
        fascicle.FascicleType = this.getSelectedFascicleType();
        fascicle.Category = this.getCategory();
        fascicle.FascicleObject = this._txtObject.get_textBoxValue();
        fascicle.Note = this._txtNote.get_textBoxValue();
        fascicle.Conservation = this._txtConservation.get_textBoxValue() ? parseInt(this._txtConservation.get_textBoxValue()) : null;
        fascicle.Contacts = this.getFascicleContactsToAdd();
        this.getFascicleRoles().then((fascicleRoles: FascicleRoleModel[]) => {
            fascicle.FascicleRoles = fascicleRoles;
            let dossierFolders: DossierFolderModel[] = [];
            let dossier: DossierFolderModel = {} as DossierFolderModel;
            if (this.getCurrentDossierFolder()) {
                let fascicleDossierFolder: DossierFolderModel = this.getCurrentDossierFolder();
                dossier.UniqueId = fascicleDossierFolder.UniqueId;
                dossier.Status = fascicleDossierFolder.Status;
                dossierFolders.push(dossier);
            }
            fascicle.DossierFolders = dossierFolders;
            fascicle.FascicleTemplate = this.getCurrentFascicleTemplate();
            if (!fascicle.VisibilityType) {
                fascicle.VisibilityType = VisibilityType.Confidential;
            }

            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
                .done((instance) => {
                    let customActions: FascicleCustomActionModel = instance.getCustomActions<FascicleCustomActionModel>();
                    fascicle.CustomActions = JSON.stringify(customActions);
                    promise.resolve(fascicle);
                });
        });
        return promise.promise();
    }

    private getCategory(): CategoryModel {
        let category: CategoryModel = new CategoryModel();
        let currentProcess: ProcessModel = this.getCurrentProcess();
        category.Code = currentProcess ? currentProcess.Category.Code : this.getCurrentCategory().Code;
        category.Name = currentProcess ? currentProcess.Category.Name : this.getCurrentCategory().Name;
        category.EntityShortId = currentProcess ? currentProcess.Category.EntityShortId : this.getCurrentCategory().EntityShortId;
        category.IdCategory = category.EntityShortId;
        return category;
    }

    disableFascicleTypeSelection(): void {
        this._rcbFascicleType.disable();
    }

    private getFascicleRolesToAdd(): FascicleRoleModel[] {
        let itemsFromSession: string = sessionStorage.getItem(`${this.clientId}_FascicleRolesToAdd`);
        if (itemsFromSession) {
            return JSON.parse(itemsFromSession) as FascicleRoleModel[];
        }
        return null;
    }

    private getFascicleContactsToAdd(): ContactModel[] {
        let itemsFromSession: string = sessionStorage.getItem(`${this.clientId}_FascicleContactsToAdd`);
        if (itemsFromSession) {
            return JSON.parse(itemsFromSession) as ContactModel[];
        }
        return null;
    }

    private getCurrentFascicleTemplate(): ProcessFascicleTemplateModel {
        let itemFromSession: string = sessionStorage.getItem(`${this.clientId}_CurrentFascicleTemplate`);
        if (itemFromSession) {
            return JSON.parse(itemFromSession) as ProcessFascicleTemplateModel;
        }
        return null;
    }

    private getCurrentProcess(): ProcessModel {
        let itemFromSession: string = sessionStorage.getItem(`${this.clientId}_CurrentProcess`);
        if (itemFromSession) {
            return JSON.parse(itemFromSession) as ProcessModel;
        }
        return null;
    }

    private getCurrentDossierFolder(): DossierFolderModel {
        let itemFromSession: string = sessionStorage.getItem(`${this.clientId}_CurrentDossierFolder`);
        if (itemFromSession) {
            return JSON.parse(itemFromSession) as DossierFolderModel;
        }
        return null;
    }

    private getCurrentCategory(): CategoryModel {
        let itemFromSession: string = sessionStorage.getItem(`${this.clientId}_CurrentCategory`);
        if (itemFromSession) {
            return JSON.parse(itemFromSession) as CategoryModel;
        }
        return null;
    }

    getSelectedFascicleType(): FascicleType {
        return FascicleType[this._rcbFascicleType.get_selectedItem().get_value()];
    }

    fillMetadataModel(): JQueryPromise<[string, string]> {
        let promise: JQueryDeferred<[string, string]> = $.Deferred<[string, string]>();
        PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
            .done((instance) => {
                let metadataRepository = instance.getMetadataRepository();
                let setiIntegrationEnabledField: boolean = false;
                if (metadataRepository && metadataRepository.JsonMetadata) {
                    let metadataJson: MetadataDesignerViewModel = JSON.parse(metadataRepository.JsonMetadata);
                    setiIntegrationEnabledField = metadataJson.SETIFieldEnabled;
                }

                promise.resolve(instance.bindModelFormPage(setiIntegrationEnabledField));
            });
        return promise.promise();
    }

    getCustomActions(): JQueryPromise<FascicleCustomActionModel> {
        let promise: JQueryDeferred<FascicleCustomActionModel> = $.Deferred<FascicleCustomActionModel>();
        PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId)
            .done((instance) => {
                promise.resolve(instance.getCustomActions<FascicleCustomActionModel>());
            });
        return promise.promise();
    }

    getFascicleRaciRoles(): JQueryPromise<RoleModel[]> {
        let promise: JQueryDeferred<RoleModel[]> = $.Deferred<RoleModel[]>();
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                return promise.resolve(instance.getRaciRoles());
            });
        return promise.promise();
    }

    private getFascicleRoles(): JQueryPromise<FascicleRoleModel[]> {
        let promise: JQueryDeferred<FascicleRoleModel[]> = $.Deferred<FascicleRoleModel[]>();
        let fascicleRoles: FascicleRoleModel[] = this.getFascicleRolesToAdd();
        this.getFascicleRaciRoles().then((raciRoles: RoleModel[]) => {
            for (let fascicleRole of fascicleRoles) {
                if (!fascicleRole.IsMaster && fascicleRole.AuthorizationRoleType === AuthorizationRoleType.Responsible) {
                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
                }
                if (raciRoles.some(x => x.EntityShortId === fascicleRole.Role.EntityShortId)) {
                    fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Responsible;
                }
            }
            if (this.selectedResponsibleRole) {
                fascicleRoles.push(<FascicleRoleModel>{
                    Role: this.selectedResponsibleRole,
                    IsMaster: true
                });
            }
            return promise.resolve(fascicleRoles);
        });
        return promise.promise();
    }

    populateInputs(fascicleTemplateModel: FascicleModel) {
        this.customActionFromSession = true;
        PageClassHelper.callUserControlFunctionSafe<uscCategoryRest>(this.uscCategoryId).done((instance) => {
            instance.addDefaultCategory(fascicleTemplateModel.Category.EntityShortId, false)
                .done(() => {
                    if (fascicleTemplateModel.FascicleType === FascicleType.Activity) {
                        this.loadActivityFascicleFields(fascicleTemplateModel, false);
                    }
                    else {
                        this.loadProcedureFascicleFields(fascicleTemplateModel, false);
                    }
                })
                .fail((exception: ExceptionDTO) => this.showNotificationException(exception))
        });
        if (fascicleTemplateModel.FascicleType) {
            this._rcbFascicleType.findItemByValue(FascicleType[fascicleTemplateModel.FascicleType]).select();
        }

        this._txtNote.set_value(fascicleTemplateModel.Note);
        if (fascicleTemplateModel.Conservation) {
            this._txtConservation.set_value(fascicleTemplateModel.Conservation.toString());
        }

        if (fascicleTemplateModel.MetadataDesigner && fascicleTemplateModel.MetadataValues) {
            PageClassHelper.callUserControlFunctionSafe<uscDynamicMetadataRest>(this.uscDynamicMetadataRestId)
                .done((instance) => {
                    instance.loadPageItems(fascicleTemplateModel.MetadataDesigner, fascicleTemplateModel.MetadataValues);
                });
        }
    }
}

export = uscFascicleProcessInsert;