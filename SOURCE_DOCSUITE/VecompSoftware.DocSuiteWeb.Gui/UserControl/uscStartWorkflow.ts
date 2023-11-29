/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalR.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import WorkflowStartService = require('App/Services/Workflows/WorkflowStartService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import DSWEnvironment = require('App/Models/Environment');
import AjaxModel = require('App/Models/AjaxModel');
import WorkflowStartModel = require('App/Models/Workflows/WorkflowStartModel');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import RoleModel = require('App/Models/Commons/RoleModel');
import WorkflowRoleModel = require('App/Models/Workflows/WorkflowRoleModel');
import WorkflowReferenceModel = require('App/Models/Workflows/WorkflowReferenceModel');
import WorkflowReferenceType = require('App/Models/Workflows/WorkflowReferenceType');
import WorkflowArgumentModel = require('App/Models/Workflows/WorkflowArgumentModel');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel')
import WorkflowAccountModel = require('App/Models/Workflows/WorkflowAccountModel');
import UscUploadDocumentRest = require('UserControl/uscUploadDocumentRest');
import DocumentModel = require('App/Models/Commons/DocumentModel');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import ContactModel = require('App/Models/Commons/ContactModel');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import WorkflowReferenceBiblosModel = require('App/Models/Workflows/WorkflowReferenceBiblosModel');
import ChainType = require("App/Models/DocumentUnits/ChainType");
import EnumHelper = require('App/Helpers/EnumHelper');
import TenantModelSelection = require('App/Models/Tenants/TenantModelSelection');
import uscTenantsSelRest = require('UserControl/uscTenantsSelRest');
import uscRoleRest = require('UserControl/uscRoleRest');
import uscRoleUserSelRest = require('UserControl/uscRoleUserSelRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import UscRoleRestEventType = require('App/Models/Commons/UscRoleRestEventType');
import uscDomainUserSelRest = require('UserControl/uscDomainUserSelRest');
import uscWorkflowFolderSelRest = require('UserControl/uscWorkflowFolderSelRest');
import DSWEnvironmentType = require('App/Models/Workflows/WorkflowDSWEnvironmentType');
import WorfklowFolderPropertiesModel = require('App/Models/Workflows/WorfklowFolderPropertiesModel');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
import FascicleBuildModel = require("App/Models/Fascicles/FascicleBuildModel");
import Guid = require("App/Helpers/GuidHelper");
import WorkflowReferenceDocumentUnitModel = require('App/Models/Workflows/WorkflowReferenceDocumentUnitModel');
import UscTemplateCollaborationSelRest = require('UserControl/uscTemplateCollaborationSelRest');
import uscErrorNotification = require('UserControl/uscErrorNotification');
import UDSConstants = require('App/Core/UDS/UDSConstants');
import WorkflowStorage = require('App/Core/WorkflowStorage/WorkflowStorage');
import MessageWorkflowResumeStatus = require('App/Core/WorkflowStorage/MessageWorkflowResumeStatus');
import GuidHelper = require('App/Helpers/GuidHelper');
import RoleUserModel = require('App/Models/RoleUsers/RoleUserModel');
import DocumentUnitRoleModel = require('App/Models/DocumentUnits/DocumentUnitRoleModel');

declare var Page_ClientValidate: any;
class uscStartWorkflow {

    contentId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    rdlWorkflowRepositoryId: string;
    dswEnvironment: string;
    btnConfirmId: string;
    txtObjectId: string;
    tenantName: string;
    tenantId: string;
    ctrlTxtWfId: string;

    proposerRoleRowId: string;
    proposerContactRowId: string;
    proposerContactRowLabelId: string;
    tenantRowId: string;

    uscRecipientContactRestId: string;
    uscProposerContactRestId: string;
    recipientRoleRowId: string;
    recipientContactRowId: string;
    recipientContactRowLabelId: string;

    uploadDocumentId: string;
    lblUploadDocumentId: string;
    uscDocumentId: string;
    rblPriorityId: string;
    dueDateId: string;
    lblTemplateCollaborationRowId: string;
    ddlTemplateCollaborationRowId: string;
    templateCollaborationRequired: boolean;

    lblChainTypeRowId: string;
    rgvDocumentListsId: string;
    chainTypeRowId: string;
    chainTypeRequired: boolean;

    redirectToProtocol: boolean;
    redirectToCollaboration: boolean;
    redirectToFascicleSingDocument: boolean;
    rdlCCDocumentId: string;
    copiaConformeRowId: string;
    documentOriginalTypeRequired: boolean;
    workflowStartTenantRequired: boolean;
    roleUserRequired: boolean;
    roleUserSelectionEnabled: boolean;

    showOnlyNoInstanceWorkflows: boolean;
    docSuiteVersion: string;
    tenantAOOId: string;
    uscTenantsSelRestId: string;
    showOnlyHasIsFascicleClosedRequired: boolean;

    uscRoleProposerRestId: string;
    uscRoleRecipientRestId: string;

    uscWorkflowFolderSelRestId: string;
    lrUscWorkflowFolderSelRestId: string;
    lblDossierTitleId: string;
    uscTemplateCollaborationSelRestId: string;
    signalRServerAddress: string;
    radListMessagesId: string;
    pnlWorkflowId: string;
    pnlNotificationMessagesId: string;

    roleUserSelRowId: string;
    uscRoleUserSelRestId: string;

    roleInsertId: number[];
    sourceProposerRoles: RoleModel[];
    sourceRecipientRoles: RoleModel[];

    private wstorage: WorkflowStorage;
    private wStorageEnabled: boolean = false;

    public static LOADED_EVENT: string = "onLoaded";
    public static DATA_LOADED_EVENT: string = "onDataLoaded";
    public static TENANT_CHANGE_EVENT: string = "OnTenantChange";

    private static WORKFLOWSTART_TYPE_NAME: string = "WorkflowStart";
    private static LOAD_EXTERNAL_DATA: string = "LoadExternalData";
    private static UPDATE_CALLBACK: string = "uscStartWorkflow.updateCallback()";
    private static SET_WORKFLOW_RECIPIENT: string = "uscStartWorkflow.setWorkflowRecipient()";
    private static SET_WORKFLOW_PROPOSER: string = "uscStartWorkflow.setWorkflowProposer()";
    private static SET_PAGE_VISIBILITIES: string = "uscStartWorkflow.setPageVisibilities()";
    private static USC_PROPOSER_ACCOUNT: string = "usc_proposer_account";
    private static USC_PROPOSER_ROLE: string = "usc_proposer_role";
    private static USC_RECIPIENT_ROLE: string = "usc_recipient_role";
    private static USC_RECIPIENT_ACCOUNT: string = "usc_recipient_account";
    private static ENVIRONMENT: string = "Environment";
    private static GET_RECIPIENT_CONTACT: string = "Get_Recipient_Contact";
    private static SET_RECIPIENT_PROPERTIES: string = "SetRecipientProperties";
    private static CONFIRM_BUTTON_CONFIRM_TEXT: string = "Conferma";
    private static CONFIRM_BUTTON_CLOSE_TEXT: string = "Chiudi";

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _workflowRepositoryService: WorkflowRepositoryService;
    private _workflowStartService: WorkflowStartService;
    private _rdlWorkflowRepository: Telerik.Web.UI.RadComboBox;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _dueDate: any;
    private _workflowStartModel: WorkflowStartModel;
    private _serviceConfiguration: ServiceConfiguration;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _repository: WorkflowRepositoryModel;
    private _workflowEvaluationProperties: WorkflowEvaluationProperty[];
    private _masterRoles: RoleModel[];
    private _uscUploadDocumentRest: Telerik.Web.UI.RadAsyncUpload;
    private _templateCollaborationService: TemplateCollaborationService;
    private _rgvDocumentLists: Telerik.Web.UI.RadGrid;
    private _rgvDocumentMasterTableView: Telerik.Web.UI.GridTableView;
    private _radListMessages: Telerik.Web.UI.RadListBox;

    private _uscRoleProposerRest: uscRoleRest;
    private _uscRoleRecipientRest: uscRoleRest;
    private _uscRecipientContactRest: uscDomainUserSelRest;
    private _uscProposerContactRest: uscDomainUserSelRest;
    private _uscWorkflowFolderSelRest: uscWorkflowFolderSelRest;
    private _currentSelectedTemplateCollaborationId: string;
    private _uscRoleUserSelRest: uscRoleUserSelRest;

    private dswSignalR: DSWSignalR;
    private correlationId: string = null;

    /**
    * Costruttore
         * @param serviceConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;;
        $(document).ready(() => {
        });
    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
    * Evento scatenato al click del pulsante ConfermaModifica
        * @method
        * @param sender
        * @param eventArgs
        * @returns
    */
    btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        if (this._btnConfirm.get_text() === uscStartWorkflow.CONFIRM_BUTTON_CLOSE_TEXT) {
            this.closeWindow();
            return;
        }

        this._loadingPanel.show(this.contentId);
        this._btnConfirm.set_enabled(false);
        let selectedWorkflowRepository: Telerik.Web.UI.RadComboBoxItem = this._rdlWorkflowRepository.get_selectedItem();

        this.setRecipientValidation();
        this.setProposerValidation();
        this.setUscWorkflowFolderValidation();

        let isValid = Page_ClientValidate('');
        let documentTypeRequired: boolean = this.documentHasSelectedType();

        let uscTenantSelRest: uscTenantsSelRest = <uscTenantsSelRest>$(`#${this.uscTenantsSelRestId}`).data();
        if (!isValid) {
            args.set_cancel(true);
            this.hideLoadingAndEnableConfirmBtn();
            return;
        }
        if (selectedWorkflowRepository == null && !String.isNullOrEmpty(this._rdlWorkflowRepository.get_text())) {
            this.onError("Selezionare una attività valida");
            args.set_cancel(true);
            this.hideLoadingAndEnableConfirmBtn();
            return;
        }
        if (documentTypeRequired === false) {
            this.onError("E' necessario specificare il tipo di documento per tutti i documenti");
            args.set_cancel(true);
            this.hideLoadingAndEnableConfirmBtn();
            return;
        }
        if (this.workflowStartTenantRequired && !uscTenantSelRest.hasValue()) {
            this.onError("E' necessario selezionare una UO");
            args.set_cancel(true);
            this.hideLoadingAndEnableConfirmBtn();
            return;
        }

        if (this.roleUserRequired && !this._uscRoleUserSelRest.roleUserSelected()) {
            this.onError("E' necessario selezionare un utente dal disegno di funzione");
            args.set_cancel(true);
            this.hideLoadingAndEnableConfirmBtn();
            return;
        }

        this.startWorkflow();
    }

    private hideLoadingAndEnableConfirmBtn(): void {
        this._loadingPanel.hide(this.contentId);
        this._btnConfirm.set_enabled(true);
    }

    private documentHasSelectedType(): boolean {
        if (this.chainTypeRequired) {
            let workflowReferenceModel: WorkflowReferenceModel = <WorkflowReferenceModel>{};
            workflowReferenceModel.Documents = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL));
            let documentTypeSelected: boolean;
            workflowReferenceModel.Documents.forEach(function (item: WorkflowReferenceBiblosModel) {
                let val: string = $(`input:radio[name='${item.ArchiveDocumentId}_chainTypes']:checked`).val();
                if (val === undefined) {
                    documentTypeSelected = false;
                }
            });
            return documentTypeSelected;
        }
        return true;
    }

    /**
    * ------------------------- Methods -----------------------------
    */

    /**
    * Initialize
    */
    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rdlWorkflowRepository = <Telerik.Web.UI.RadComboBox>$find(this.rdlWorkflowRepositoryId);
        this._rdlWorkflowRepository.add_itemsRequesting(this.loadWorkflowRepository);
        this._rdlWorkflowRepository.add_selectedIndexChanged(this.onRdlWorkflowRepository_SelectedIndexChanged);
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._dueDate = $(`#${this.dueDateId}`);
        let workflowRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
        this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
        let workflowStartConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, uscStartWorkflow.WORKFLOWSTART_TYPE_NAME)
        this._workflowStartService = new WorkflowStartService(workflowStartConfiguration);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
        this._radListMessages = <Telerik.Web.UI.RadListBox>$find(this.radListMessagesId);

        this.clearSessionStorage();

        this._rgvDocumentLists = <Telerik.Web.UI.RadGrid>$find(this.rgvDocumentListsId);
        this._rgvDocumentMasterTableView = this._rgvDocumentLists.get_masterTableView();
        this._rgvDocumentMasterTableView.set_currentPageIndex(0);
        this._rgvDocumentMasterTableView.set_virtualItemCount(0);


        this._uscRoleProposerRest = <uscRoleRest>$(`#${this.uscRoleProposerRestId}`).data();
        this._uscRoleRecipientRest = <uscRoleRest>$(`#${this.uscRoleRecipientRestId}`).data();
        this._uscRecipientContactRest = <uscDomainUserSelRest>$(`#${this.uscRecipientContactRestId}`).data();
        this._uscProposerContactRest = <uscDomainUserSelRest>$(`#${this.uscProposerContactRestId}`).data();
        this._uscRoleUserSelRest = <uscRoleUserSelRest>$(`#${this.uscRoleUserSelRestId}`).data();

        let templateCollaborationConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "TemplateCollaboration");
        this._templateCollaborationService = new TemplateCollaborationService(templateCollaborationConfiguration);

        this._loadingPanel.show(this.contentId);
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();

        let env: number = parseInt(this.dswEnvironment);
        if (isNaN(env)) {
            env = DSWEnvironment[this.dswEnvironment];
        }

        let defaultReadOnlyProposer: boolean = false;
        switch (env) {
            case DSWEnvironment.Fascicle:
            case DSWEnvironment.Dossier:
                defaultReadOnlyProposer = true;
                break;
        }
        ajaxModel.Value.push(JSON.stringify(defaultReadOnlyProposer));
        ajaxModel.Value.push(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE);
        this._masterRoles = new Array<RoleModel>();
        switch (env) {
            case DSWEnvironment.Fascicle: {
                let fascicle: FascicleModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL))
                this._txtObject.set_value(fascicle.FascicleObject);
                let fascicleMasterRoles: FascicleRoleModel[] = $.grep(fascicle.FascicleRoles, (r) => r.IsMaster);
                for (let role of fascicleMasterRoles) {
                    this._masterRoles.push(role.Role);
                }
                break;
            }
            case DSWEnvironment.Dossier: {
                let dossier: DossierSummaryViewModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL))
                //per ora so che le api ritornano solo quello attivo (che è uno solo)
                let dossierRoles: BaseEntityViewModel[] = dossier.Roles;
                for (let role of dossierRoles) {
                    let roleModel: RoleModel = <RoleModel>{};
                    roleModel.EntityShortId = role.EntityShortId;
                    roleModel.IdRole = role.EntityShortId;
                    roleModel.Name = role.Name;
                    this._masterRoles.push(roleModel);
                }
                break;
            }
        }

        if (env >= 100 || env == 1) {
            let documentUnit: DocumentUnitModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL));
            this._txtObject.set_value(documentUnit.Subject);
            //let documentUnitRoles: DocumentUnitRoleModel[] = documentUnit.DocumentUnitRoles;
            //for (let role of documentUnitRoles) {
            //    let roleModel: RoleModel = <RoleModel>{};
            //    roleModel.EntityShortId = role.EntityShortId;
            //    roleModel.IdRole = role.EntityShortId;
            //    roleModel.Name = role.RoleLabel;
            //    this._masterRoles.push(roleModel);
            //}
        }

        ajaxModel.ActionName = uscStartWorkflow.LOAD_EXTERNAL_DATA;
        ajaxModel.Value.push(JSON.stringify(this._masterRoles));
        ajaxModel.Value.push(uscStartWorkflow.UPDATE_CALLBACK);
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));

        this.setRecipientValidation();
        this.setProposerValidation();
        this.bindLoaded();
        $(`#${this.uscTenantsSelRestId}`).on(uscStartWorkflow.TENANT_CHANGE_EVENT, this.uscTenantSel_selectedTenantAOO);
        $(`#${this.pnlNotificationMessagesId}`).hide();
    }

    /**
    * Caricamento dei dati dello user control
    */
    loadData(): void {
        this.loadWorkflowRepository(this._rdlWorkflowRepository, new Telerik.Web.UI.RadComboBoxRequestCancelEventArgs());
    }

    /**
    * Caricamento dei workflow repository disponibili
    */
    loadWorkflowRepository = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestCancelEventArgs) => {
        this._loadingPanel.show(this.contentId);
        let env: number = parseInt(this.dswEnvironment);
        if (isNaN(env)) {
            env = DSWEnvironment[this.dswEnvironment];
        }
        let onlyDocumentWorkflows: boolean = false;
        let sessionStorageValue: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
        if (sessionStorageValue && sessionStorageValue !== "[]") {
            onlyDocumentWorkflows = true;
        }

        let onlyDocumentUnitWorkflows: boolean = false;
        let sessionStorageDocumentUnitValue: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_UNITS_REFERENCE_MODEL);
        if (sessionStorageDocumentUnitValue && sessionStorageDocumentUnitValue !== "[]") {
            const documentUnits: WorkflowReferenceDocumentUnitModel[] = JSON.parse(sessionStorageDocumentUnitValue) as WorkflowReferenceDocumentUnitModel[];
            const filteredResults = documentUnits.filter((value, index, source) => {
                return value.Environment != DSWEnvironment.Document;
            });
            if (filteredResults.length > 0) {
                onlyDocumentUnitWorkflows = true;
            }
        }
        //il false dovrà essere gestito da un checkbox
        this._workflowRepositoryService.getByEnvironment(env, args.get_text(), false, onlyDocumentWorkflows, onlyDocumentUnitWorkflows, this.showOnlyNoInstanceWorkflows, this.showOnlyHasIsFascicleClosedRequired,
            (data: any) => {
                this._rdlWorkflowRepository.clearItems();
                let repositories: WorkflowRepositoryModel[] = <WorkflowRepositoryModel[]>data;
                this.addWorkflowRepositories(repositories, this._rdlWorkflowRepository);
                this._loadingPanel.hide(this.contentId);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception, "Anomalia nel recupero dei WorkflowRepositories autorizzati all'utente.");
            });
        this._loadingPanel.hide(this.contentId);
    }

    setToolbarRoleVisibility(isReadOnly: boolean, uscID: string) {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(uscID)
            .done((instance) => {
                instance.setToolbarRoleVisibility(isReadOnly);
            });
    }

    /**
    * Metodo che riempie la RadComboBox dei workflow repository
    */
    protected addWorkflowRepositories(repositories: WorkflowRepositoryModel[], rdlWorkflowRepository: Telerik.Web.UI.RadComboBox) {
        let item: Telerik.Web.UI.RadComboBoxItem;
        for (let repository of repositories) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(repository.Name);
            item.get_attributes().setAttribute(uscStartWorkflow.ENVIRONMENT, repository.DSWEnvironment.toString());
            item.set_value(repository.UniqueId.toString());
            rdlWorkflowRepository.get_items().add(item);
        }
        //set default if list contains only one repository
        if (repositories.length === 1) {
            rdlWorkflowRepository.get_items().getItem(0).select();
        }

        rdlWorkflowRepository.trackChanges();
    }

    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $(`#${this.contentId}`).data(this);
        $(`#${this.contentId}`).triggerHandler(uscStartWorkflow.LOADED_EVENT);
        $(`#${this.contentId}`).triggerHandler(uscStartWorkflow.DATA_LOADED_EVENT);
    }

    public InitializeCollaborationTreeview(): void {
        PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationSelRest>(this.uscTemplateCollaborationSelRestId)
            .done((instance) => {
                instance.clearCurrentSelection();

                let defaultTemplate: WorkflowEvaluationProperty = this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT;
                })[0];

                this._currentSelectedTemplateCollaborationId = defaultTemplate ? defaultTemplate.ValueGuid : null;
                if (defaultTemplate && defaultTemplate.ValueGuid) {
                    instance.SelectAndForceLoadNode(defaultTemplate.ValueGuid);
                } else {
                    instance.ReloadRoot();
                }

                let readonlyTemplate: WorkflowEvaluationProperty = this._workflowEvaluationProperties.filter(function (item) {
                    return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_READONLY;
                })[0];

                if (readonlyTemplate && readonlyTemplate.ValueBoolean && this._currentSelectedTemplateCollaborationId) {
                    instance.disableTreeview();
                } else {
                    instance.OnFixedTemplateClick(this.uscTemplateCollaborationSelRestId, (fixedTemplate) => {
                        this._currentSelectedTemplateCollaborationId = fixedTemplate.UniqueId;
                    });
                    instance.OnTemplateClick(this.uscTemplateCollaborationSelRestId, (fixedTemplate, template) => {
                        this._currentSelectedTemplateCollaborationId = template.UniqueId;
                    });
                    instance.OnFolderClick_DisableConfirmaButton(this.uscTemplateCollaborationSelRestId, (disableButton) => {
                        this._btnConfirm.set_enabled(!disableButton);
                    });
                }
            });
    }

    onRdlWorkflowRepository_SelectedIndexChanged = () => {
        if (this._rdlWorkflowRepository.get_selectedItem() == null) {
            return;
        }

        this._loadingPanel.show(this.contentId);
        this.setPageVisibilities();
        this.clearSessionContacts();
    }

    clearSessionContacts() {
        let uscRecipientDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscRecipientContactRestId}`).data();
        uscRecipientDomainUserContacts.clearDomainUsersContactsTree();
        let uscProposerDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscRecipientContactRestId}`).data();
        uscProposerDomainUserContacts.clearDomainUsersContactsTree();
    }

    setPageVisibilities = () => {
        this._loadingPanel.show(this.contentId);
        this._repository = null;
        this._workflowRepositoryService.getById(this._rdlWorkflowRepository.get_selectedItem().get_value(),
            (data: any) => {
                this._repository = <WorkflowRepositoryModel>data;
                this.dswEnvironment = DSWEnvironment[this._repository.DSWEnvironment.toString()] || this._repository.DSWEnvironment.toString();
                this._workflowEvaluationProperties = this._repository.WorkflowEvaluationProperties;
                if (this._workflowEvaluationProperties == null) {
                    this._workflowEvaluationProperties = [];
                }

                let varStr: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
                let fascicle: FascicleModel = new FascicleModel();
                if (varStr) {
                    fascicle = JSON.parse(varStr);
                }

                this.checkWorkflowEvaluationPropertyValues();
                if (this.redirectToFascicleSingDocument) {
                    if (fascicle) {
                        varStr = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
                        if (varStr) {
                            let documents: WorkflowReferenceBiblosModel[] = JSON.parse(varStr);
                            window.location.href = `../Fasc/FascDocumentsInsert.aspx?Type=Fasc&IdFascicle=${fascicle.UniqueId}&OnlySignEnabled=true&FilterByArchiveDocumentId=${documents[0].ArchiveDocumentId}`
                        }
                    }
                }

                this.setRecipientProperties();
                this.setProposerProperties();

                if (this.templateCollaborationRequired) {
                    this.InitializeCollaborationTreeview();
                }

                this.setUscWorkflowFolderProperties(fascicle.UniqueId);

                this._loadingPanel.hide(this.contentId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.contentId);
                this.showNotificationException(this.uscNotificationId, exception, "Anomalia nel recupero della definizione dell'attività.");
            });
    }

    private setUscWorkflowFolderProperties(fascicleId: string) {
        this._uscWorkflowFolderSelRest = <uscWorkflowFolderSelRest>$(`#${this.uscWorkflowFolderSelRestId}`).data();
        let worfklowFolderProperties: WorfklowFolderPropertiesModel = <WorfklowFolderPropertiesModel>{};

        let dossierFolderEnable = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOSSIER_FOLDER;
        });

        let dossierType = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_ACTION_DOSSIER_FILTER_TYPE;
        });

        let onlyFolderHasTemplate = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_ACTION_DOSSIERFOLDER_FILTER_HASTEMPLATE;
        });

        let setResponsibleRole = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_ACTION_FASCICLE_SET_RESPONSIBLE_ROLE;
        });

        if (this.dswEnvironment == DSWEnvironmentType.Fascicle.toString()) {
            worfklowFolderProperties.IdFascicle = fascicleId;
        }

        if (dossierFolderEnable.length > 0 && <boolean>dossierFolderEnable[0].ValueBoolean) {
            worfklowFolderProperties.DossierEnable = true;
        }

        if (dossierType.length > 0 && dossierType[0].ValueInt) {
            worfklowFolderProperties.DossierType = Number(dossierType[0].ValueInt);
        }

        if (setResponsibleRole.length > 0 && <boolean>setResponsibleRole[0].ValueBoolean) {
            worfklowFolderProperties.SetRecipientRole = true;
        }

        if (onlyFolderHasTemplate.length > 0 && <boolean>onlyFolderHasTemplate[0].ValueBoolean) {
            worfklowFolderProperties.OnlyFolderHasTemplate = true;
        }
        if (worfklowFolderProperties.DossierEnable && worfklowFolderProperties.IdFascicle) {
            PageClassHelper.callUserControlFunctionSafe<uscWorkflowFolderSelRest>(this.uscWorkflowFolderSelRestId)
                .done((instance) => {
                    instance.populateTreeByProperties(worfklowFolderProperties);
                });
            $(`#${this.lrUscWorkflowFolderSelRestId}`).show();
            $(`#${this.lblDossierTitleId}`).show();
        }
    }

    checkWorkflowEvaluationPropertyValues = () => {
        let results: Array<WorkflowEvaluationProperty>;
        let proposerRoleRow: any = $(`#${this.proposerRoleRowId}`);
        let proposerContactRow: any = $(`#${this.proposerContactRowId}`);
        let lblProposerContact: any = $(`#${this.proposerContactRowLabelId}`);
        let recipientRoleRow: any = $(`#${this.recipientRoleRowId}`);
        let recipientContactRow: any = $(`#${this.recipientContactRowId}`);
        let lblRecipientContact: any = $(`#${this.recipientContactRowLabelId}`);
        let lrUscWorkflowFolderSelRest: any = $(`#${this.lrUscWorkflowFolderSelRestId}`);
        let lblDossierTitle: any = $(`#${this.lblDossierTitleId}`);
        let tenantRow: any = $(`#${this.tenantRowId}`);
        let roleUserSelRow: any = $(`#${this.roleUserSelRowId}`);

        //motivazione avvia workflow obbligatorietà
        let startMotivationRequired: boolean = false;
        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_MOTIVATION_REQUIRED;
        });
        if (results && results.length > 0) {
            startMotivationRequired = <boolean>results[0].ValueBoolean;
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_TEMPLATECOLLABORATION_REQUIRED;
        });
        this.templateCollaborationRequired = false;
        if (results && results.length > 0) {
            this.templateCollaborationRequired = <boolean>results[0].ValueBoolean;
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION;
        });
        this.documentOriginalTypeRequired = false;
        if (results && results.length > 0) {
            this.documentOriginalTypeRequired = <boolean>results[0].ValueBoolean;
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_ACTION_REDIRECT_TO_COLLABORATION;
        });
        this.redirectToCollaboration = false;
        if (results && results.length > 0) {
            this.redirectToCollaboration = <boolean>results[0].ValueBoolean;
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_ACTION_REDIRECT_TO_PROTOCOL;
        });
        this.redirectToProtocol = false;
        if (results && results.length > 0) {
            this.redirectToProtocol = <boolean>results[0].ValueBoolean;
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_ACTION_REDIRECT_TO_FASICLE_SIGN_DOCUMENT;
        });
        this.redirectToFascicleSingDocument = false;
        if (results && results.length > 0) {
            this.redirectToFascicleSingDocument = <boolean>results[0].ValueBoolean;
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_DOCUMENT;
        });
        if (results && results.length > 0) {
            if (results[0].ValueBoolean == true) {
                $(`#${this.uploadDocumentId}`).show();
                $(`#${this.lblUploadDocumentId}`).show();
            }
        }

        let isRecipientContactType: boolean = this.hasCurrentWorkflowRecipientContact();
        let isRecipientRoleType: boolean = this.hasCurrentWorkflowRecipientRole();
        let isProposerContactType: boolean = this.hasCurrentWorkflowProposerContact();
        let isProposerRoleType: boolean = this.hasCurrentWorkflowProposerRole();

        if (proposerRoleRow) {
            proposerRoleRow.hide();
        }
        if (proposerContactRow) {
            proposerContactRow.hide();
        }
        if (lblProposerContact) {
            lblProposerContact.hide();
        }
        if (recipientRoleRow) {
            recipientRoleRow.hide();
        }
        if (recipientContactRow) {
            recipientContactRow.hide();
        }
        if (lblRecipientContact) {
            lblRecipientContact.hide();
        }

        if (lrUscWorkflowFolderSelRest) {
            lrUscWorkflowFolderSelRest.hide();
            lblDossierTitle.hide();
        }

        if (tenantRow) {
            tenantRow.hide();
        }

        if (roleUserSelRow) {
            roleUserSelRow.hide();
        }

        if (isProposerRoleType && proposerRoleRow) {
            proposerRoleRow.show();
        }
        if (isProposerContactType && proposerContactRow && lblProposerContact) {
            proposerContactRow.show();
            lblProposerContact.show();
        }
        if (isRecipientRoleType && recipientRoleRow) {
            recipientRoleRow.show();
            this.registerUscRecipientRoleRestEventHandlers();

            results = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLE_RECIPIENT_VALIDATION_TOOLTIP;
            });

            if (results && results.length > 0) {
                PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
                    .done((instance) => {
                        instance.setAddActionTooltip(results[0].ValueString);
                    });
            }
        }
        if (isRecipientContactType && recipientContactRow && lblRecipientContact) {
            recipientContactRow.show();
            lblRecipientContact.show();
        }

        $(`#${this.uploadDocumentId}`).hide();
        $(`#${this.lblUploadDocumentId}`).hide();
        $(`#${this.chainTypeRowId}`).hide();
        $(`#${this.lblChainTypeRowId}`).hide();

        $(`#${this.lblTemplateCollaborationRowId}`).hide();
        $(`#${this.ddlTemplateCollaborationRowId}`).hide();

        if (this.templateCollaborationRequired) {
            $(`#${this.lblTemplateCollaborationRowId}`).show();
            $(`#${this.ddlTemplateCollaborationRowId}`).show();
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_CHAIN_TYPE_SELECTION;
        });

        this.chainTypeRequired = false;
        let documents: WorkflowReferenceBiblosModel[] = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL));
        if (results && results.length > 0 && documents) {
            this._rgvDocumentMasterTableView.set_dataSource(documents);
            this._rgvDocumentMasterTableView.set_virtualItemCount(documents.length);
            this._rgvDocumentMasterTableView.dataBind();

            for (let i = 0; i < documents.length; i++) {
                this.createChangeEvent(`${documents[i].ArchiveDocumentId}_chainTypes`, documents);
            }
            $(`#${this.lblChainTypeRowId}`).show();
            $(`#${this.chainTypeRowId}`).show();
            this.chainTypeRequired = true;

        }

        if (this.documentOriginalTypeRequired) {
            $(`#${this.copiaConformeRowId}`).show();
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_TENANT_REQUIRED;
        });
        this.workflowStartTenantRequired = false;
        if (results && results.length > 0) {
            this.workflowStartTenantRequired = <boolean>results[0].ValueBoolean;
        }

        if (this.workflowStartTenantRequired) {
            tenantRow.show();
        }

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SET_RECIPIENT_FROM_ROLE_USER;
        });
        this.roleUserRequired = false;
        this.roleUserSelectionEnabled = false;
        if (results && results.length > 0) {
            this.roleUserSelectionEnabled = true;
            this.roleUserRequired = <boolean>results[0].ValueBoolean;
        }

        if (this.roleUserSelectionEnabled) {
            results = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLE_USER_TYPE;
            });
            if (results && results.length > 0) {
                this._uscRoleUserSelRest.setRoleUserType(<string>results[0].ValueString);
            }
            results = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_MULTIPLE_RECIPIENT_FROM_ROLE_USER;
            });
            if (results && results.length > 0) {
                this._uscRoleUserSelRest.setMultipleSelectionEnabled(<boolean>results[0].ValueBoolean);
            }
            results = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLE_USER_RECIPIENT_SELECTION_AUTO_EXPAND;
            });
            if (results && results.length > 0) {
                this._uscRoleUserSelRest.setAutoExpandTreeEnabled(<boolean>results[0].ValueBoolean);
            }
        }

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
            .done((instance) => {
                instance.setOnlyMyRole(this.roleUserSelectionEnabled);
            });

        results = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_ROLE_RECIPIENT_LABEL;
        });
        if (results && results.length > 0) {
            PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
                .done((instance) => {
                    instance.setCaption(results[0].ValueString);
                });
        }

        if (this.workflowStartTenantRequired && recipientRoleRow.is(":visible")) {
            PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
                .done((instance) => {
                    instance.disableButtons();
                });
        }
    }

    private createChangeEvent(chainTypeGroupName, documents): void {
        $(`input[type=radio][name=${chainTypeGroupName}]`).click((ev) => {
            return this.allowOnlyOneMainDocumentToBeSelected(ev, documents);
        });
    }

    private allowOnlyOneMainDocumentToBeSelected(ev: JQueryEventObject, documents: WorkflowReferenceBiblosModel[]) {
        let selectedId = ev.target.id;
        let count: number = 0;
        for (let i = 0; i < documents.length; i++) {
            let mainDocument: JQuery = $(`#${documents[i].ArchiveDocumentId}_1`);
            if (mainDocument.is(':checked') && selectedId.substring(selectedId.length - 2) === "_1") {
                count++;
            }
            if (count === 2) {
                alert(`E' possibile selezionare un solo documento principale all volta. Cambiare prima la tipologia del documento principale e poi rifare l'operazione.`);
                count = 0;
                return ev.preventDefault();
            }
        }

    }

    setProposerProperties = () => {
        let isProposerContact: boolean = this.hasCurrentWorkflowProposerContact();
        let isProposerRole: boolean = this.hasCurrentWorkflowProposerRole();

        let proposerDisabled: boolean = false;
        let startRecipient: WorkflowEvaluationProperty[] = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT;
        });

        let proposerReadonly: Array<WorkflowEvaluationProperty> = this._workflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_PROPOSER_AUTHORIZATION_READONLY;
        });

        if (proposerReadonly && proposerReadonly.length > 0) {
            proposerDisabled = <boolean>proposerReadonly[0].ValueBoolean;
        }

        //richiedenti workflow
        let proposerDefault: Array<WorkflowEvaluationProperty> = this._workflowEvaluationProperties.filter(function (item) {
            return (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_DEFAULT);
        });

        let proposerType: string = uscStartWorkflow.USC_PROPOSER_ROLE;
        let proposerModel: any = [];

        if (isProposerContact) {
            proposerType = uscStartWorkflow.USC_PROPOSER_ACCOUNT
            let uscProposerDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscProposerContactRestId}`).data();
            uscProposerDomainUserContacts.setImageButtonsVisibility(!proposerDisabled);

            proposerModel = new Array<WorkflowAccountModel>();

            uscProposerDomainUserContacts.setCurrentUser(!proposerDefault.length);

            if (proposerDefault.length) {
                let account: WorkflowAccountModel = this.buildContactDefault(proposerDefault[0].ValueString);
                let contacts: ContactModel[] = [];
                let contactModel: ContactModel = <ContactModel>{
                    Description: account.DisplayName,
                    Code: account.AccountName,
                    EmailAddress: account.EmailAddress
                };
                contacts.push(contactModel);
                uscProposerDomainUserContacts.createDomainUsersContactsTree(contacts);
            }

            let contactsModel: ContactModel[] = uscProposerDomainUserContacts.getContacts();

            for (let contactModel of contactsModel) {
                let workflowAccount: WorkflowAccountModel = {
                    AccountName: contactModel.Code,
                    DisplayName: contactModel.Description,
                    EmailAddress: contactModel.EmailAddress,
                    Required: false
                };
                proposerModel.push(workflowAccount);
            }
        }
        if (isProposerRole) {
            proposerModel = this._masterRoles;
            if (proposerDefault != null && proposerDefault.length > 0) {
                proposerModel = new Array<RoleModel>();
                let role = this.buildRoleDefault(proposerDefault[0].ValueString);
                proposerModel.push(role);
            }
        }

        let roles: RoleModel[] = proposerModel;
        this.sourceProposerRoles = roles;
        this._uscRoleProposerRest.renderRolesTree(roles);
        this.registerUscProposerRoleRestEventHandlers();

        if (isProposerContact || isProposerRole) {
            this.setToolbarRoleVisibility(!proposerDisabled, this.uscRoleProposerRestId);
            this.setWorkflowRecipient();
        }
        this._loadingPanel.hide(this.contentId);
    }

    private buildContactDefault = (sourceContact: any) => {
        let account: WorkflowAccountModel = <WorkflowAccountModel>{};
        let workflowAccount: any = JSON.parse(sourceContact);
        account.AccountName = workflowAccount.Account.AccountName;
        account.DisplayName = workflowAccount.Account.DisplayName;
        account.EmailAddress = workflowAccount.Account.EmailAddress;
        account.Required = workflowAccount.Account.Required;

        return account;
    }

    private buildRoleDefault = (sourceRole: any) => {
        let role: RoleModel = <RoleModel>{};
        let workflowRole: any = JSON.parse(sourceRole);
        role.EntityShortId = workflowRole.Role.IdRole;
        role.IdRole = workflowRole.Role.IdRole;
        role.Name = workflowRole.Role.Name;
        role.IdTenantAOO = workflowRole.Role.IdTenantAOO;
        role.UniqueId = workflowRole.Role.UniqueId;
        return role;
    }

    setWorkflowRecipient = () => {

        this._loadingPanel.show(this.contentId);

        //destinatari workflow
        let recipientDisabled: boolean = false;
        let recipientReadonly: Array<WorkflowEvaluationProperty> = this._workflowEvaluationProperties.filter(function (item) {
            return (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_RECIPIENT_AUTHORIZATION_READONLY);
        });
        if (recipientReadonly && recipientReadonly.length > 0) {
            recipientDisabled = <boolean>recipientReadonly[0].ValueBoolean;
        }
        let workflowRecipients: any = [];

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(JSON.stringify(recipientDisabled));
        ajaxModel.ActionName = uscStartWorkflow.LOAD_EXTERNAL_DATA;
        let isRecipientContact: boolean = this.hasCurrentWorkflowRecipientContact();
        let isRecipientRole: boolean = this.hasCurrentWorkflowRecipientRole();

        if (isRecipientContact) {
            let uscDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscRecipientContactRestId}`).data();
            uscDomainUserContacts.setImageButtonsVisibility(!recipientDisabled);

            let accountRecipient: Array<WorkflowEvaluationProperty> = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
            });

            //me ne aspetto uno solo
            workflowRecipients = new Array<WorkflowAccountModel>();
            if (accountRecipient != null && accountRecipient[0] != null) {
                let account: WorkflowAccountModel = this.buildContactDefault(accountRecipient[0].ValueString);
                let contacts: ContactModel[] = [];
                let contactModel: ContactModel = <ContactModel>{
                    Description: account.DisplayName,
                    Code: account.AccountName,
                    EmailAddress: account.EmailAddress,

                };
                contacts.push(contactModel);

                let uscDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscRecipientContactRestId}`).data();
                uscDomainUserContacts.createDomainUsersContactsTree(contacts);
                workflowRecipients.push(account);
            }
        }
        if (isRecipientRole) {
            this.setToolbarRoleVisibility(!recipientDisabled, this.uscRoleRecipientRestId);
            let rolesProp: Array<WorkflowEvaluationProperty> = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
            });

            //me ne aspetto uno solo
            workflowRecipients = new Array<RoleModel>();
            if (rolesProp != null && rolesProp[0] != null) {
                let role: RoleModel = this.buildRoleDefault(rolesProp[0].ValueString);
                workflowRecipients.push(role);
            }
        }

        let roles: RoleModel[] = workflowRecipients;

        this.sourceRecipientRoles = roles;
        this._uscRoleRecipientRest.renderRolesTree(roles);
        this.registerUscRecipientRoleRestEventHandlers();

        ajaxModel.Value.push(JSON.stringify(workflowRecipients));

        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
    }

    setRecipientProperties() {
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = uscStartWorkflow.SET_RECIPIENT_PROPERTIES;

        let multiProp = this._repository.WorkflowEvaluationProperties.filter(function (item) {
            return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_RECIPIENT_MULTIPLE;
        });
        if (multiProp && multiProp.length > 0) {
            ajaxModel.Value.push(JSON.stringify(multiProp[0].ValueBoolean));
        }

        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
    }

    /**
    * Callback
    */
    updateCallback = () => {
        this._loadingPanel.hide(this.contentId);
    }

    setRecipientValidation = () => {
        let isRecipientContactType: boolean = this.hasCurrentWorkflowRecipientContact();
        let isRecipientRoleType: boolean = this.hasCurrentWorkflowRecipientRole();

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
            .done((instance) => {
                instance.forceBehaviourValidationState(isRecipientRoleType);
                instance.enableValidators(isRecipientRoleType);
            });
    }

    setProposerValidation = () => {
        let isProposerContactType: boolean = this.hasCurrentWorkflowProposerContact();
        let isProposerRoleType: boolean = this.hasCurrentWorkflowProposerRole();

        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleProposerRestId)
            .done((instance) => {
                instance.forceBehaviourValidationState(isProposerRoleType);
                instance.enableValidators(isProposerRoleType);
            });
    }

    setUscWorkflowFolderValidation = () => {
        PageClassHelper.callUserControlFunctionSafe<uscWorkflowFolderSelRest>(this.uscWorkflowFolderSelRestId)
            .done((instance) => {
                instance.enableValidator($(`#${this.uscWorkflowFolderSelRestId}`).is(":visible"));
                return;
            });

        let nodeSelectedFromUscWorkflowFolder: boolean;
        PageClassHelper.callUserControlFunctionSafe<uscWorkflowFolderSelRest>(this.uscWorkflowFolderSelRestId)
            .done((instance) => {
                nodeSelectedFromUscWorkflowFolder = instance.getSelectedNode();
                instance.enableTemplateValidator(!nodeSelectedFromUscWorkflowFolder);
            });

        if ($(`#${this.uscWorkflowFolderSelRestId}`).is(":visible") && !nodeSelectedFromUscWorkflowFolder) {
            this.hideLoadingAndEnableConfirmBtn();
            return;
        }
    }

    hasCurrentWorkflowPropValueInt = (propName: string, intValue: number): boolean => {
        let isProperty = false;
        if (this._workflowEvaluationProperties) {
            const property: WorkflowEvaluationProperty[] = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name === propName;
            });
            if (property && property.length > 0 && property[0].ValueInt === intValue) {
                isProperty = true;
            }
        }
        return isProperty;
    }

    hasCurrentWorkflowPropValueBool = (propName: string): boolean => {
        let isProperty = false;
        if (this._workflowEvaluationProperties) {
            const property: WorkflowEvaluationProperty[] = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name === propName;
            });
            if (property && property.length > 0 && property[0].ValueBoolean === true) {
                isProperty = true;
            }
        }
        return isProperty;
    }
    /**
    * Metodo che determina se il workflow ha il destinatario di tipo "contatto"
    */
    hasCurrentWorkflowRecipientContact = (): boolean => {
        return this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT, 1);
    }

    /**
   * Metodo che determina se il workflow ha il proponente di tipo "contatto"
   */
    hasCurrentWorkflowProposerContact = (): boolean => {
        return this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_PROPOSER, 1);
    }

    /**
    * Metodo che determina se il workflow ha il destinatario di tipo "contatto"
    */
    hasCurrentWorkflowRecipientRole = (): boolean => {
        return this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT, 0);
    }

    /**
    * Metodo che determina se il workflow ha il destinatario di tipo utente da disegno di funzione
    */
    hasCurrentWorkflowRecipientRoleUser = (): boolean => {
        return this.hasCurrentWorkflowPropValueBool(WorkflowPropertyHelper.DSW_PROPERTY_SET_RECIPIENT_FROM_ROLE_USER);
    }

    /**
    * Metodo che determina se il workflow ha il destinatario di tipo "contatto"
    */
    hasCurrentWorkflowProposerRole = (): boolean => {
        return this.hasCurrentWorkflowPropValueInt(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_PROPOSER, 0);
    }

    /**
    * Metodo che determina se il workflow richiede la costruzione del FascicleBuildModel
    */
    hasCurrentWorkflowFascicleBuildModel = (): boolean => {
        return this.hasCurrentWorkflowPropValueBool(WorkflowPropertyHelper.DSW_PROPERTY_BUILD_MODEL_CREATE);
    }

    hasWorkflowWaitCompletionEnabled = (): boolean => {
        return this.hasCurrentWorkflowPropValueBool(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_WAITCOMPLETION_ENABLED);
    }

    hasWorkflowStartTenantEnabled = (): boolean => {
        return this.hasCurrentWorkflowPropValueBool(WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_TENANT_REQUIRED);
    }

    /**
    * Metodo che completa il modello per avviare un workflow e spedisce il comando di avvio
    */
    startWorkflow = () => {
        let isRecipientContact: boolean = this.hasCurrentWorkflowRecipientContact();
        let isRecipientRoleUser: boolean = this.hasCurrentWorkflowRecipientRoleUser();
        let isRecipientRole: boolean = this.hasCurrentWorkflowRecipientRole() && !isRecipientRoleUser;
        let isProposerContact: boolean = this.hasCurrentWorkflowProposerContact();
        let isProposerRole: boolean = this.hasCurrentWorkflowProposerRole();
        let isWorkflowWaitCompletionEnabled: boolean = this.hasWorkflowWaitCompletionEnabled();
        let isWorkflowStartTenantEnabled: boolean = this.hasWorkflowStartTenantEnabled();
        let tenants: string = "";
        let roles: string = "";

        this._workflowStartModel = <WorkflowStartModel>{};
        this._workflowStartModel.Arguments = {};
        this._workflowStartModel.StartParameters = {};

        const currentTenantModelSelection: TenantModelSelection[] = JSON.parse(sessionStorage.getItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL));

        const selectedWorkflowRepository: Telerik.Web.UI.RadComboBoxItem = this._rdlWorkflowRepository.get_selectedItem();
        if (!currentTenantModelSelection || currentTenantModelSelection.length === 0) {
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, this.tenantName);
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, this.tenantId);
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ArgumentType.PropertyGuid, this.tenantAOOId);
        } else {
            for (let i = 0; i < currentTenantModelSelection.length; i++) {
                tenants = `${tenants}${currentTenantModelSelection[i].TenantName} `;
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ArgumentType.PropertyString, currentTenantModelSelection[i].TenantName);
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ArgumentType.PropertyGuid, currentTenantModelSelection[i].IdTenant);
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ArgumentType.PropertyGuid, currentTenantModelSelection[i].IdTenantAOO);
            }
        }
        this._workflowStartModel.WorkflowName = selectedWorkflowRepository.get_text();
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_NAME, ArgumentType.PropertyString, "DocSuite");
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_PRODUCT_VERSION, ArgumentType.PropertyString, this.docSuiteVersion);
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_REFERENCE_UNIQUEID] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_REFERENCE_UNIQUEID, ArgumentType.PropertyGuid, sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_UNIQUEID));
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_REFERENCE_ENVIRONMENT] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_REFERENCE_ENVIRONMENT, ArgumentType.PropertyInt, this.dswEnvironment);
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ROLES] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ROLES, ArgumentType.Json, sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_RECIPIENT_ROLES));

        //Priority
        const rblPriorityVal: string = $(`input:radio[name='${this.rblPriorityId}']:checked`).val();
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PRIORITY, ArgumentType.PropertyInt, rblPriorityVal);

        //Date
        const dueDateVal: string = this._dueDate.val();
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_DUEDATE, ArgumentType.PropertyDate, dueDateVal);

        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_PROPOSER_ROLES)) {
            const workflowProposerRoleModel: WorkflowRoleModel = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_PROPOSER_ROLES));
            if (workflowProposerRoleModel) {
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE, ArgumentType.Json, JSON.stringify(workflowProposerRoleModel));
            }
        }

        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_FOLDER_SELECTED] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_FOLDER_SELECTED, ArgumentType.Json, sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_FOLDER_ID));

        if (isProposerRole) {
            let workflowProposerRole: WorkflowRoleModel = <WorkflowRoleModel>{};
            let proposerRoleFromUscRole: Array<WorkflowRoleModel> = new Array<WorkflowRoleModel>();
            //settore proponente
            proposerRoleFromUscRole = this.getUscRoles(this.sourceProposerRoles);
            if (proposerRoleFromUscRole.length > 0) {
                //ce ne sarà solo uno
                proposerRoleFromUscRole.forEach(function (item) {
                    workflowProposerRole.IdTenantAOO = item.IdTenantAOO;
                    workflowProposerRole.IdRole = item.IdRole;
                    workflowProposerRole.UniqueId = item.UniqueId;
                });

                let argumentProposer: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE, ArgumentType.Json, JSON.stringify(workflowProposerRole));
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE] = argumentProposer;
            }
        }
        if (isProposerContact) {
            let uscProposerDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscProposerContactRestId}`).data();
            let contactsModel: ContactModel[] = uscProposerDomainUserContacts.getContacts();
            let accountName: string = "";
            let displayName: string = "";
            let emailAddress: string = "";
            if (contactsModel && contactsModel.length > 0) {
                let proposer: ContactModel = contactsModel[0];
                accountName = proposer.Code;
                displayName = proposer.Description;
                emailAddress = proposer.EmailAddress;
            }
            let workflowAccountModel: WorkflowAccountModel = {
                AccountName: accountName,
                DisplayName: displayName,
                EmailAddress: emailAddress,
                Required: false
            };
            const argumentProposer: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER, ArgumentType.Json, JSON.stringify(workflowAccountModel));
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_USER] = argumentProposer;
        }
        if (isRecipientRole) {
            this.setStartWorkflowRecipientRoles()
        }
        if (isRecipientContact) {
            this.setStartWorkflowRecipientContacts();
        }
        if (isRecipientRoleUser) {
            this.setStartWorkflowRecipientRoleUser();
        }
        if (this.workflowStartTenantRequired) {
            this.setStartWorkflowRecipientRoles();
        }
        if (this.hasCurrentWorkflowFascicleBuildModel()) {
            const fascicleBuildModel: FascicleBuildModel = {
                WorkflowAutoComplete: true,
                Fascicle: JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL)),
                WorkflowName: selectedWorkflowRepository.get_text()
            };
            fascicleBuildModel.Fascicle.UniqueId = Guid.newGuid();
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, fascicleBuildModel.Fascicle.UniqueId);
            if (fascicleBuildModel.Fascicle.Category) {
                fascicleBuildModel.Fascicle.Category.IdCategory = fascicleBuildModel.Fascicle.Category.EntityShortId;
            }
            if (fascicleBuildModel.Fascicle.MetadataRepository) {
                fascicleBuildModel.Fascicle.MetadataRepository.Id = fascicleBuildModel.Fascicle.MetadataRepository.UniqueId;
            }

            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(fascicleBuildModel));
        }

        const workflowReferenceModel: WorkflowReferenceModel = {} as WorkflowReferenceModel;
        const env: number = parseInt(this.dswEnvironment);
        this.correlationId = GuidHelper.newGuid();
        workflowReferenceModel.ReferenceId = isWorkflowWaitCompletionEnabled
            ? this.correlationId
            : sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID);
        workflowReferenceModel.ReferenceType = DSWEnvironment[this.dswEnvironment];
        if (env >= 100) {
            workflowReferenceModel.ReferenceType = DSWEnvironment.UDS;
        }
        workflowReferenceModel.ReferenceModel = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
        workflowReferenceModel.Documents = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL));
        workflowReferenceModel.DocumentUnits = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_UNITS_REFERENCE_MODEL));
        workflowReferenceModel.Title = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE);
        workflowReferenceModel.WorkflowReferenceType = WorkflowReferenceType.Json;
        const argumentReferenceModel: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, ArgumentType.Json, JSON.stringify(workflowReferenceModel));
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = argumentReferenceModel;

        //oggetto
        const argumentSubject: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT, ArgumentType.PropertyString, this._txtObject.get_value());
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT] = argumentSubject;
        const startMotivation: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION, ArgumentType.PropertyString, this._txtObject.get_value());
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_ACTIVITY_START_MOTIVATION] = startMotivation;

        //evaluationProperties
        this._workflowEvaluationProperties.forEach(function (item) {
            this.Arguments[item.Name] = item;
        }, this._workflowStartModel);

        if (env >= 100) {
            const documentUnit: DocumentUnitModel = JSON.parse(workflowReferenceModel.ReferenceModel);
            const idUDS: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_UDS_ID, ArgumentType.PropertyGuid, documentUnit.UniqueId);
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_UDS_ID] = idUDS;
            const idUDSRepository: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID, ArgumentType.PropertyGuid, documentUnit.UDSRepository.UniqueId);
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_UDS_REPOSITORY_ID] = idUDSRepository;
            const udsModel: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_UDS_MODEL);
            if (udsModel) {
                const dsw_p_Model: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_MODEL, ArgumentType.Json, udsModel);
                this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_MODEL] = dsw_p_Model;
            }
        }

        const documentMetadataValues: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_METADATAS);
        if (documentMetadataValues) {
            let documentMetadatas: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_FIELD_GENERATE_DOCUMENT_METADATAS, ArgumentType.Json, documentMetadataValues);
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_FIELD_GENERATE_DOCUMENT_METADATAS] = documentMetadatas;
        }

        if (this.templateCollaborationRequired) {
            const templateCollaboration: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION, ArgumentType.PropertyGuid, this._currentSelectedTemplateCollaborationId);
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION] = templateCollaboration;
        }

        if (this.documentOriginalTypeRequired) {
            const documentOriginal: string = $(`input:radio[name='${this.rdlCCDocumentId}']:checked`).val();
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_DOCUMENT_ORIGINAL_TYPE_SELECTION, ArgumentType.PropertyBoolean, (documentOriginal === "1"));
        }

        if (this.chainTypeRequired) {
            const enumHelper: EnumHelper = new EnumHelper();
            workflowReferenceModel.Documents.forEach(function (item: WorkflowReferenceBiblosModel) {
                let val: string = $(`input:radio[name='${item.ArchiveDocumentId}_chainTypes']:checked`).val();
                item.ChainType = enumHelper.getChainType(val);
            });
        }

        if (isWorkflowStartTenantEnabled) {
            this.sourceRecipientRoles.forEach(function (item: RoleModel) {
                roles = `${roles}${item.Name} `
            });
            let workflowActivityName = `${this._workflowStartModel.WorkflowName} - AOO: ${tenants}- Settori: ${roles}`;
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, ArgumentType.PropertyString, workflowActivityName);
        }

        if (this.redirectToCollaboration) {
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_REFERENCE_MODEL, JSON.stringify(workflowReferenceModel));
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_START_MODEL, JSON.stringify(this._workflowStartModel));
            let defaultTemplateId: string
            let defaultTemplate: Array<WorkflowEvaluationProperty> = this._workflowEvaluationProperties.filter(function (item) {
                return item.Name == WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT;
            });
            if (defaultTemplate && defaultTemplate.length > 0) {
                defaultTemplateId = defaultTemplate[0].ValueGuid;
            }
            this._loadingPanel.hide(this.contentId);
            let result: AjaxModel = <AjaxModel>{};
            result.ActionName = "redirect";
            result.Value = new Array<string>();
            result.Value.push(`../User/UserCollGestione.aspx?Titolo=Inserimento&Action=Add&Title2=Ins.%20alla%20visione/firma&DefaultTemplateId=${defaultTemplateId}&Type=Prot&Action2=CI&FromWorkflowUI=True`);
            this.closeWindow(result);
            return;
        }
        if (this.redirectToProtocol) {
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_REFERENCE_MODEL, JSON.stringify(workflowReferenceModel));
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_START_MODEL, JSON.stringify(this._workflowStartModel));
            this._loadingPanel.hide(this.contentId);
            let result: AjaxModel = <AjaxModel>{};
            result.ActionName = "redirect";
            result.Value = new Array<string>();
            result.Value.push(`../Prot/ProtInserimento.aspx?Type=Prot&Action=FromWorkflowUI`);
            this.closeWindow(result);
            return;
        }

        if (isWorkflowWaitCompletionEnabled) {
            this.initializeWorkflowStorage();
            this.dswSignalR = new DSWSignalR(this.signalRServerAddress);
            this.dswSignalR.setup("WorkflowHub", {
                'correlationId': this.correlationId
            });

            this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusDone, this.actionHubWorkflowStatusDone);
            this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusError, this.actionHubWorkflowStatusError);
            //this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationInfo, this.actionHubWorkflowNotificationInfo);
            //this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationWarning, this.actionHubWorkflowNotificationWarning);
            //this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationError, this.actionHubWorkflowNotificationError);
            ////connect to resume channel and wait for response
            //this.dswSignalR.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowResumeStatus, this.actionHubWorkflowResumeStatus);

            this.dswSignalR.startConnection(this.onDoneSignalRConnection, this.onErrorSignalRCallback);
        }
        else {
            (<WorkflowStartService>this._workflowStartService).startWorkflow(this._workflowStartModel,
                (data: any) => {
                    this._loadingPanel.hide(this.contentId);
                    let result: AjaxModel = <AjaxModel>{};
                    result.ActionName = "Attività avviata correttamente";
                    result.Value = new Array<string>();
                    this.closeWindow(result);
                },
                (exception: ExceptionDTO) => {
                    this.hideLoadingAndEnableConfirmBtn();
                    this.showNotificationException(this.uscNotificationId, exception, "Anomalia nel avvio dell'attività.");
                }
            );
        }
    }

    //lamba function to solve scoping of a callback 
    public onDoneSignalRConnection = () => {
        $(`#${this.pnlWorkflowId}`).hide();
        $(`#${this.pnlNotificationMessagesId}`).show();

        let serverFunction: string = UDSConstants.HubMethods.SubscribeStartWorkflow;
        this.dswSignalR.sendServerMessages(serverFunction, this.correlationId, JSON.stringify(this._workflowStartModel),
            this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
    }

    private onErrorSignalRCallback = (error) => {
        this.dswSignalR.stopClient();
        this.hideLoadingAndEnableConfirmBtn();
        this.showNotificationException(this.uscNotificationId, error, "Anomalia nel avvio dell'attività.");
    }

    private onDoneSignalRSubscriptionCallback = (error) => {
    }

    onError = (message) => {
        alert(message);
    }

    /**
    * Recupera una RadWindow dalla pagina
    */
    getRadWindow = () => {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    /**
     * Chiude la RadWindow
    */
    closeWindow = (message?: AjaxModel) => {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    getUscRoles = (roles: RoleModel[]) => {
        let workflowRoles: Array<WorkflowRoleModel> = new Array<WorkflowRoleModel>();

        let source: any = roles;
        if (source != null) {
            for (let s of source) {
                let role: WorkflowRoleModel = <WorkflowRoleModel>{};
                role.IdRole = s.EntityShortId;
                role.IdTenantAOO = s.IdTenantAOO;
                role.UniqueId = s.UniqueId;
                workflowRoles.push(role);
            }
        }
        return workflowRoles;
    }

    setStartWorkflowRecipientRoles(): void {
        let workflowAuthorizedRoles: Array<WorkflowRoleModel> = new Array<WorkflowRoleModel>();
        workflowAuthorizedRoles = this.getUscRoles(this.sourceRecipientRoles);
        let argumentRoles: WorkflowArgumentModel = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ROLES, ArgumentType.Json, JSON.stringify(workflowAuthorizedRoles));
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ROLES] = argumentRoles;
    }

    setStartWorkflowRecipientContacts(): void {
        let uscDomainUserContacts: uscDomainUserSelRest = <uscDomainUserSelRest>$(`#${this.uscRecipientContactRestId}`).data();
        let contactsModel: ContactModel[] = uscDomainUserContacts.getContacts();
        let workflowAccounts: WorkflowAccountModel[] = [];

        for (let contactModel of contactsModel) {
            let workflowAccount: WorkflowAccountModel = {
                AccountName: contactModel.Code,
                DisplayName: contactModel.Description,
                EmailAddress: contactModel.EmailAddress,
                Required: false
            };

            workflowAccounts.push(workflowAccount);
        }

        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, ArgumentType.Json, JSON.stringify(workflowAccounts));
    }

    setStartWorkflowRecipientRoleUser(): void {
        let workflowAccounts: WorkflowAccountModel[] = [];

        this._uscRoleUserSelRest = <uscRoleUserSelRest>$(`#${this.uscRoleUserSelRestId}`).data();
        let roleUsers: RoleUserModel[] = this._uscRoleUserSelRest.getSelectedRoleUsers();

        for (let roleUser of roleUsers) {
            let workflowAccount: WorkflowAccountModel = {
                AccountName: roleUser.Account,
                DisplayName: roleUser.Description,
                EmailAddress: roleUser.Email,
                Required: false
            };
            workflowAccounts.push(workflowAccount);
        }

        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS] = this.buildWorkflowArgument(WorkflowPropertyHelper.DSW_PROPERTY_ACCOUNTS, ArgumentType.Json, JSON.stringify(workflowAccounts));
    }

    getUscDocument = (uscId: string) => {
        let workflowDocuments: Array<DocumentModel> = new Array<DocumentModel>();
        let uscDocuments: UscUploadDocumentRest = <UscUploadDocumentRest>$(`#${uscId}`).data();
        if (!jQuery.isEmptyObject(uscDocuments)) {
            let source: any = JSON.parse(uscDocuments.getDocument());
            if (source != null) {
                for (let s of source) {
                    let document: DocumentModel = <DocumentModel>{};
                    document.FileName = s.FileName;
                    document.ContentStream = s.ContentStream;
                    workflowDocuments.push(document);
                }
            }
        }
        return workflowDocuments;
    }

    buildWorkflowArgument = (propertyName: string, propertyType: ArgumentType, propertyValue: any) => {
        let property: WorkflowArgumentModel = <WorkflowArgumentModel>{};
        property.Name = propertyName;
        property.PropertyType = propertyType;
        property.ValueInt = null;
        property.ValueDate = null;
        property.ValueDouble = null;
        property.ValueBoolean = null;
        property.ValueGuid = null;
        property.ValueString = null;

        switch (propertyType) {
            case ArgumentType.PropertyInt:
                {
                    property.ValueInt = propertyValue;
                    break;
                }
            case ArgumentType.PropertyDate:
                {
                    property.ValueDate = propertyValue;
                    break;
                }
            case ArgumentType.PropertyDouble:
                {
                    property.ValueDouble = propertyValue;
                    break;
                }
            case ArgumentType.PropertyBoolean:
                {
                    property.ValueBoolean = propertyValue;
                    break;
                }
            case ArgumentType.PropertyGuid:
                {
                    property.ValueGuid = propertyValue;
                    break;
                }
            case ArgumentType.PropertyString:
            case ArgumentType.Json:
                {
                    property.ValueString = propertyValue;
                    break;
                }
        }
        return property;

    }

    private clearSessionStorage(): void {
        sessionStorage.removeItem(uscTenantsSelRest.SESSION_TENANT_SELECTION_MODEL);
    }

    private registerUscProposerRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleProposerRestId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.roleInsertId = [newAddedRoles[0].IdRole];
                    this.sourceProposerRoles = newAddedRoles;
                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
            });
    }

    private registerUscRecipientRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    if (this.roleUserSelectionEnabled) {
                        PageClassHelper.callUserControlFunctionSafe<uscRoleUserSelRest>(this.uscRoleUserSelRestId)
                            .done((instance) => {
                                instance.clearTree();
                            });
                        let roleUserSelRow: any = $(`#${this.roleUserSelRowId}`);
                        roleUserSelRow.hide();
                    }
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    let existedRole: RoleModel;
                    this.roleInsertId = [newAddedRoles[0].IdRole];
                    this.sourceRecipientRoles = newAddedRoles;
                    if (this.roleUserSelectionEnabled) {
                        PageClassHelper.callUserControlFunctionSafe<uscRoleUserSelRest>(this.uscRoleUserSelRestId)
                            .done((instance) => {
                                instance.populateDropdownTree(newAddedRoles[0].UniqueId);
                            });
                        let roleUserSelRow: any = $(`#${this.roleUserSelRowId}`);
                        roleUserSelRow.show();
                    }
                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
            });
    }

    uscTenantSel_selectedTenantAOO = (eventObject: JQueryEventObject, idTenantAOO: string) => {
        if ($(`#${this.uscRoleRecipientRestId}`).is(":visible")) {
            PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRecipientRestId)
                .done((instance) => {
                    instance.countRoles(idTenantAOO).then(countRoles => {
                        if (countRoles == 0) {
                            instance.disableButtons();
                            return;
                        }
                        instance.setAddActionTooltip("Selezionare settori");
                        $(`#${this.uscRoleRecipientRestId}`).triggerHandler(uscRoleRest.TENANT_CHANGE_EVENT, idTenantAOO);
                    }).fail(exception => { this.showNotificationException(this.uscNotificationId, exception); });
                });
        }
    }

    initializeWorkflowStorage(): void {
        try {
            //initializing a store to keep track of started activities
            this.wstorage = new WorkflowStorage();
            if (this.wstorage.IsValid) {
                this.wStorageEnabled = true;

                //now that we have the store, let's check if there is pending item in storage
            }
        }
        catch (err) {
            this.wStorageEnabled = false;
            //disabling the confirm button if the client does not support local or session storage
            this._btnConfirm.set_enabled(false);
            window.alert("Questa funzionalità non è supportata con l'attuale browser. E' necessario utilizzare un browser moderno come IE10+, Edge o Chrome");
        }
    }

    public actionHubWorkflowStatusDone = (model) => {
        if (this.wStorageEnabled) {
            this.wstorage.Unset();
        }

        this.addItemDone(model);
        this.dswSignalR.stopClient();
        this._loadingPanel.hide(this.contentId);
        this._btnConfirm.set_text(uscStartWorkflow.CONFIRM_BUTTON_CLOSE_TEXT);
        this._btnConfirm.set_enabled(true);
    }

    public actionHubWorkflowStatusError = (model) => {
        if (this.wStorageEnabled) {
            this.wstorage.Unset();
        }

        this.addItemError(model);
        this.dswSignalR.stopClient();
        this._loadingPanel.hide(this.contentId);
        this._btnConfirm.set_text(uscStartWorkflow.CONFIRM_BUTTON_CLOSE_TEXT);
        this._btnConfirm.set_enabled(true);
    }

    public addItemDone = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/star.png");
    }

    public addItemError = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png");
    }

    public addItem = (text: string, imageUrl: string) => {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(text);
        item.set_imageUrl(imageUrl)
        this._radListMessages.get_items().add(item);
        this._radListMessages.commitChanges();
    }
}

export = uscStartWorkflow;