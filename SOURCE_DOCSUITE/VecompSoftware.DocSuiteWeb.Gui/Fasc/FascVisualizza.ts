/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import ProtocolModel = require('App/Models/Protocols/ProtocolModel');
import ResolutionModel = require('App/Models/Resolutions/ResolutionModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import UscFascicolo = require('UserControl/uscFascicolo');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import DocumentUnitFilterModel = require('App/Models/DocumentUnits/DocumentUnitFilterModel');
import FascicleRightsViewModel = require('App/ViewModels/Fascicles/FascicleRightsViewModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DocumentUnitService = require('App/Services/DocumentUnits/DocumentUnitService');
import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import IFascicolableBaseService = require('App/Services/Fascicles/IFascicolableBaseService');
import Environment = require('App/Models/Environment');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel')
import ConservationModel = require('App/Models/Commons/ConservationModel')
import ConservationService = require('App/Services/Commons/ConservationService');
import UpdateActionType = require("App/Models/UpdateActionType");
import DeleteActionType = require("App/Models/DeleteActionType");
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import WorkflowAuthorizationModel = require('App/Models/Workflows/WorkflowAuthorizationModel');
import AjaxModel = require('App/Models/AjaxModel');
import HandlerWorkflowManager = require('App/Managers/HandlerWorkflowManager');
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import FascicleFolderTypology = require('App/Models/Fascicles/FascicleFolderTypology');
import UscFascicleFolders = require('UserControl/uscFascicleFolders');
import FascicleDocumentService = require('App/Services/Fascicles/FascicleDocumentService');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import FascicleMoveItemViewModel = require('App/ViewModels/Fascicles/FascicleMoveItemViewModel');
import FascMoveItems = require('Fasc/FascMoveItems');
import CategoryFascicleService = require('App/Services/Commons/CategoryFascicleService');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import FascicleRights = require('App/Rules/Rights/Entities/Fascicles/FascicleRights');
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import FascicleDocumentUnitService = require('App/Services/Fascicles/FascicleDocumentUnitService');
import WorkflowReferenceBiblosModel = require('App/Models/Workflows/WorkflowReferenceBiblosModel');
import uscFascicleSearch = require('UserControl/uscFascicleSearch');
import WorkflowReferenceDocumentUnitModel = require('App/Models/Workflows/WorkflowReferenceDocumentUnitModel');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import uscStartWorkflow = require('UserControl/uscStartWorkflow');
import WorkflowRoleModel = require('App/Models/Workflows/WorkflowRoleModel');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
import DocumentRootFolderViewModel = require("App/ViewModels/SignDocuments/DocumentRootFolderViewModel");
import DocumentFolderViewModel = require("App/ViewModels/SignDocuments/DocumentFolderViewModel");
import DocumentViewModel = require("App/ViewModels/SignDocuments/DocumentViewModel");
import DocumentSignBehaviour = require("App/ViewModels/SignDocuments/DocumentSignBehaviour");
import FileHelper = require("App/Helpers/FileHelper");

class FascVisualizza extends FascicleBase {
    currentFascicleId: string;
    ajaxManagerId: string;
    radWindowManagerCollegamentiId: string;
    currentPageId: string;
    windowInsertProtocolloId: string;
    windowStartWorkflowId: string;
    windowCompleteWorkflowId: string;
    windowWorkflowInstanceLogId: string;
    btnSendToRolesId: string;
    radGridUDId: string;
    panelUDId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    btnCloseId: string;
    uscNotificationId: string;
    btnInserisciId: string;
    btnModificaId: string;
    btnDocumentiId: string;
    uscFascicoloId: string;
    actionPage: string;
    signalRServerAddress: string;
    radNotificationInfoId: string;
    btnLinkId: string;
    btnRemoveId: string;
    btnAutorizzaId: string;
    btnSignId: string;
    radWindowManagerId: string;
    btnWorkflowId: string;
    btnOpenId: string;
    btnCompleteWorkflowId: string;
    workflowActivityId: string;
    isWorkflowActivityClosed: boolean;
    currentUser: string;
    pnlButtonsId: string;
    workflowEnabled: boolean;
    btnWorkflowLogsId: string;
    btnFascicleLogId: string;
    miscellaneaLocationEnabled: boolean;
    buttonLogVisible: boolean;
    uscFascFoldersId: string;
    btnUndoId: string;
    btnMoveId: string;
    windowMoveItemsId: string;
    btnCopyToFascicleId: string;
    idDocumentUnit: string;
    selectedFascicle: FascicleModel;
    windowFascicleSearchId: string;
    currentTenantAOOId: string;
    isClosed: boolean
    hasDgrooveSigner: string;

    private static UniqueId_ATTRIBUTE_NAME = "UniqueId";
    private static DocumentUnitName_ATTRIBUTE_NAME = "DocumentUnitName";
    private static Environment_ATTRIBUTE_NAME = "Environment";
    private static BiblosChainId_ATTRIBUTE_NAME = "BiblosChainId";
    private static BiblosDocumentId_ATTRIBUTE_NAME = "BiblosDocumentId";
    private static BiblosDocumentName_ATTRIBUTE_NAME = "BiblosDocumentName";
    private static btnUDLink_CONTROL_NAME = "btnUDLink";

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _windowInsertProtocol: Telerik.Web.UI.RadWindow;
    private _windowStartWorkflow: Telerik.Web.UI.RadWindow;
    private _windowCompleteWorkflow: Telerik.Web.UI.RadWindow;
    private _windowWorkflowInstanceLogs: Telerik.Web.UI.RadWindow;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notificationInfo: Telerik.Web.UI.RadNotification;
    private _btnInsert: Telerik.Web.UI.RadButton;
    private _btnEdit: Telerik.Web.UI.RadButton;
    private _btnDocuments: Telerik.Web.UI.RadButton;
    private _btnClose: Telerik.Web.UI.RadButton;
    private _btnLink: Telerik.Web.UI.RadButton;
    private _btnRemove: Telerik.Web.UI.RadButton;
    private _btnAutorizza: Telerik.Web.UI.RadButton;
    private _btnSign: Telerik.Web.UI.RadButton;
    private _btnWorkflow: Telerik.Web.UI.RadButton;
    private _btnCompleteWorkflow: Telerik.Web.UI.RadButton;
    private _btnWorkflowLogs: Telerik.Web.UI.RadButton;
    private _btnFascicleLog: Telerik.Web.UI.RadButton;
    private _btnSendToRoles: Telerik.Web.UI.RadButton;
    private _btnOpen: Telerik.Web.UI.RadButton;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _fascicleModel: FascicleModel;
    private _signalR: DSWSignalR;
    private _documentUnitService: DocumentUnitService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _udsRepositoryService: UDSRepositoryService;
    private _workflowActivityService: WorkflowActivityService;
    private _pnlButtons: JQuery;
    private _workflowActivity: WorkflowActivityModel;
    private _conservation: ConservationModel;
    private _conservationService: ConservationService;
    private _handlerManager: HandlerWorkflowManager;
    private _fascicleRights: FascicleRightsViewModel;
    private _fascicleDocumentService: FascicleDocumentService;
    private _fascicleDocumentUnitService: FascicleDocumentUnitService;
    private _btnUndo: Telerik.Web.UI.RadButton;
    private _btnMove: Telerik.Web.UI.RadButton;
    private _windowMoveItems: Telerik.Web.UI.RadWindow;
    private _categoryFascicleService: CategoryFascicleService;
    private _workflowRepositoriyService: WorkflowRepositoryService;
    private _currentDocumentToSign: string;
    private _btnCopyToFascicle: Telerik.Web.UI.RadButton;
    private _windowFascicleSearch: Telerik.Web.UI.RadWindow;

    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        this._handlerManager = new HandlerWorkflowManager(serviceConfigurations);
        $(document).ready(() => {

        });
    }

    /**
     * Initialize
     */


    initialize() {
        super.initialize();
        this.cleanWorkflowSessionStorage();
        this._currentDocumentToSign = undefined;
        this._windowInsertProtocol = <Telerik.Web.UI.RadWindow>$find(this.windowInsertProtocolloId);
        this._windowInsertProtocol.add_close(this.sendUDUpdate);
        this._windowStartWorkflow = <Telerik.Web.UI.RadWindow>$find(this.windowStartWorkflowId);
        this._windowStartWorkflow.add_close((this.onWorkflowCloseWindow));
        this._windowCompleteWorkflow = <Telerik.Web.UI.RadWindow>$find(this.windowCompleteWorkflowId);
        this._windowCompleteWorkflow.add_close((this.onWorkflowCloseWindow));
        this._windowWorkflowInstanceLogs = <Telerik.Web.UI.RadWindow>$find(this.windowWorkflowInstanceLogId);
        this._windowMoveItems = <Telerik.Web.UI.RadWindow>$find(this.windowMoveItemsId);
        this._windowMoveItems.add_close(this.onMoveCloseWindow);
        this._windowFascicleSearch = <Telerik.Web.UI.RadWindow>$find(this.windowFascicleSearchId);
        this._windowFascicleSearch.add_close(this.onFascicleSearchCloseWindow);


        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationInfoId);
        this._btnInsert = <Telerik.Web.UI.RadButton>$find(this.btnInserisciId);
        this._btnEdit = <Telerik.Web.UI.RadButton>$find(this.btnModificaId);
        this._btnDocuments = <Telerik.Web.UI.RadButton>$find(this.btnDocumentiId);
        this._btnClose = <Telerik.Web.UI.RadButton>$find(this.btnCloseId);
        this._btnLink = <Telerik.Web.UI.RadButton>$find(this.btnLinkId);
        this._btnRemove = <Telerik.Web.UI.RadButton>$find(this.btnRemoveId);
        this._btnAutorizza = <Telerik.Web.UI.RadButton>$find(this.btnAutorizzaId);
        this._btnSign = <Telerik.Web.UI.RadButton>$find(this.btnSignId);
        this._btnWorkflow = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowId);
        this._btnOpen = <Telerik.Web.UI.RadButton>$find(this.btnOpenId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._btnCompleteWorkflow = <Telerik.Web.UI.RadButton>$find(this.btnCompleteWorkflowId);
        this._btnWorkflowLogs = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowLogsId);
        this._btnFascicleLog = <Telerik.Web.UI.RadButton>$find(this.btnFascicleLogId);
        this._btnSendToRoles = <Telerik.Web.UI.RadButton>$find(this.btnSendToRolesId);
        this._btnUndo = <Telerik.Web.UI.RadButton>$find(this.btnUndoId);
        this._btnMove = <Telerik.Web.UI.RadButton>$find(this.btnMoveId);
        this._btnCopyToFascicle = <Telerik.Web.UI.RadButton>$find(this.btnCopyToFascicleId);

        this._btnEdit.add_clicking(this.btnEdit_OnClick);
        this._btnDocuments.add_clicking(this.btnDocuments_OnClick);
        this._btnClose.add_clicking(this.btnClose_OnClick);
        this._btnInsert.add_clicking(this.btnInsert_OnClick);
        this._btnLink.add_clicking(this.btnLink_OnClick);
        this._btnRemove.add_clicking(this.btnRemove_OnClick);
        this._btnCompleteWorkflow.add_clicking(this.btnCompleteWorkflow_OnClick);
        this._btnAutorizza.add_clicking(this.btnAutorizza_OnClick);

        let isIE = /(MSIE|Trident\/|Edge\/)/i.test(navigator.userAgent);
        if (this.hasDgrooveSigner === "True" && isIE === false) {
            this._btnSign.add_clicking(this.btnSign_DgrooveSigner_OnClick);
        } else {
            this._btnSign.add_clicking(this.btnSign_OnClick);
        }

        this._btnWorkflow.add_clicking(this.btnWorkflow_OnClick);
        this._btnWorkflowLogs.add_clicking(this.btnWorkflowLogs_OnClick);
        this._btnFascicleLog.add_clicking(this.btnFascicleLog_OnClick);
        this._btnOpen.add_clicking(this.btnOpen_OnClick);
        this._btnUndo.add_clicking(this.btnUndo_OnClick);
        this._btnMove.add_clicking(this.btnMove_OnClick);
        this._btnCopyToFascicle.add_clicking(this.btnCopyToFascicle_OnClick);

        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOCUMENT_UNIT_TYPE_NAME);
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        let udsRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.UDSREPOSITORY_TYPE_NAME);
        this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);

        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        let conservationConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Conservation");
        this._conservationService = new ConservationService(conservationConfiguration);

        let fascicleDocumentConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENT_TYPE_NAME);
        this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);

        let fascicleDocumentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
        this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);

        let workflowRepositoriyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowRepository");
        this._workflowRepositoriyService = new WorkflowRepositoryService(workflowRepositoriyConfiguration);

        this._pnlButtons = $("#".concat(this.pnlButtonsId));
        this._pnlButtons.hide();

        //Bind evento onRebind dello user control uscFascicolo
        $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.REBIND_EVENT, (args) => {
            this.sendRefreshUDRequest();
        });


        $("#".concat(this.uscFascFoldersId)).bind(UscFascicleFolders.ROOT_NODE_CLICK, (args) => this.folderNodeClickCallback(false))
            .bind(UscFascicleFolders.FASCICLE_TREE_NODE_CLICK, (args) => this.folderNodeClickCallback(true))
            .bind(UscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK, (args) => this.folderNodeClickCallback(true));

        this.setButtonEnable(false);

        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) return;
                this._fascicleModel = data;

                let fascicleRoleModel: FascicleRoleModel = this._fascicleModel.FascicleRoles.filter(x => x.IsMaster == true)[0];
                if (fascicleRoleModel != undefined) {
                    let workflowRoleModel: WorkflowRoleModel = <WorkflowRoleModel>{ IdRole: fascicleRoleModel.Role.IdRole, IdTenantAOO: fascicleRoleModel.Role.IdTenantAOO, UniqueId: fascicleRoleModel.Role.UniqueId };
                    sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_PROPOSER_ROLES, JSON.stringify(workflowRoleModel));
                }
                let wfCheckActivityAction: () => JQueryPromise<string>;
                if (this.workflowEnabled && this.workflowActivityId) {
                    wfCheckActivityAction = () => this._handlerManager.manageHandlingWorkflow(this.workflowActivityId);
                } else {
                    wfCheckActivityAction = () => this._handlerManager.manageHandlingWorkflow(this.currentFascicleId, Environment.Fascicle);
                }

                wfCheckActivityAction()
                    .done((idActivity) => {
                        this.workflowActivityId = idActivity;
                        this.initializeRights(this._fascicleModel)
                            .done((fascicleRights) => {
                                this._fascicleRights = fascicleRights;
                                this.initializePageByRigths(fascicleRights);
                            })
                            .fail((exception: ExceptionDTO) => {
                                this._loadingPanel.hide(this.pageContentId);
                                $("#".concat(this.pageContentId)).hide();
                                this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.");
                            });
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        $("#".concat(this.pageContentId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo.");
                    });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                $("#".concat(this.pageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     *------------------------- Events -----------------------------
     */

    private onFascicleSearchCloseWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs): void => {
        if (args.get_argument() === true) {
            this._notificationInfo.set_text("Copiato con successo");
            this._notificationInfo.show();
        }
    }

    /**
     * Evento al click del pulsante "Modifica"
     * @param sender
     * @param args
     */
    btnEdit_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let editUrl: string = "../Fasc/FascModifica.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId);
        if (this.actionPage != "") {
            editUrl = editUrl.concat("&Action=", this.actionPage);
        }
        window.location.href = editUrl;
    }

    /**
     * Evento al click del pulsante "Documenti"
     * @param sender
     * @param args
     */
    btnDocuments_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this.setWorkflowSessionStorage();
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
        let documentUrl: string = "../Viewers/FascicleViewer.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId);
        if (this.actionPage != "") {
            documentUrl = documentUrl.concat("&Action=", this.actionPage);
        }
        this._loadingPanel.show(this.pageContentId);
        window.location.href = documentUrl;
    }

    /**
     * Evento al click del pulsante "Inserisci"
     * @param sender
     * @param args
     */
    btnInsert_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let selectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder();
        if (!selectedFolder) {
            this.showWarningMessage(this.uscNotificationId, "E' necessario selezionare una cartella del fascicolo");
            return;
        }

        let typology: FascicleFolderTypology = FascicleFolderTypology[selectedFolder.Typology];
        if (isNaN(typology)) {
            typology = FascicleFolderTypology[typology.toString()];
        }

        switch (typology) {
            case FascicleFolderTypology.Fascicle:
            case FascicleFolderTypology.SubFascicle:
                {
                    var url = "../Fasc/FascDocumentsInsert.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId, "&IdFascicleFolder=", selectedFolder.UniqueId);
                    if (selectedFolder.idCategory) {
                        url = url.concat("&IdCategory=", selectedFolder.idCategory.toString());
                    }
                    if (this.radWindowManagerCollegamentiId) {
                        url = url.concat("&ManagerID=", this.radWindowManagerCollegamentiId)
                    }
                    this.openWindow(url, "windowInsertProtocollo", 850, 600);
                }
                break;
        }
    }

    /**
     * Evento al click del pulsante di chiusura Fascicolo
     * @param sender
     * @param args
     */
    btnClose_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);

        this._manager.radconfirm("Sei sicuro di voler chiudere il fascicolo?", (arg) => {
            if (arg) {
                this._loadingPanel.show(this.pageContentId);
                this.setButtonEnable(false);
                this._fascicleModel.EndDate = moment().toDate();
                this.service.closeFascicle(this._fascicleModel,
                    (data: any) => {
                        this._btnClose.set_visible(false);
                        this._btnEdit.set_visible(false);
                        this._btnInsert.set_visible(false);
                        this._btnMove.set_visible(false);
                        this._btnRemove.set_visible(false);
                        this._btnLink.set_visible(false);
                        this._btnAutorizza.set_visible(false);
                        this._btnSign.set_visible(false);
                        this._btnSendToRoles.set_visible(false);
                        this._btnWorkflow.set_visible(false);
                        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ISBUTTON_WORKFLOW_VISIBLE, String(false));
                        this._btnCompleteWorkflow.set_visible(false);
                        this.setBtnOpenVisibility();
                        this._btnUndo.set_visible(false);
                        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
                        if (!jQuery.isEmptyObject(uscFascicolo)) {
                            uscFascicolo.loadData(this._fascicleModel);
                        }
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        this.setButtonEnable(true);
                        this.showNotificationException(this.uscNotificationId, exception);
                    }
                );
            }
        }, 300, 160);
    }


    /**
    * Evento al click del pulsante "Collegamenti"
    * @param sender
    * @param args
    */
    btnLink_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var linkUrl = "../Fasc/FascicleLink.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId);
        window.location.href = linkUrl;
    }

    /**
    * Evento al click del pulsante "Sign"
    * @param sender
    * @param args
    */
    btnSign_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this.setWorkflowSessionStorage();
        let varStr: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
        if (!varStr) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return false;
        }
        let fascicle: FascicleModel = JSON.parse(varStr);
        if (!fascicle) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return false;
        }
        varStr = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
        if (!varStr) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return false;
        }
        this._loadingPanel.show(this.pageContentId);
        let documents: WorkflowReferenceBiblosModel[] = JSON.parse(varStr);
        if (!documents || documents.length === 0) {
            this._loadingPanel.hide(this.pageContentId);
            this._notificationInfo.set_text("Nessun documento selezionato");
            this._notificationInfo.show();
            return;
        }
        let ajaxRequest: AjaxModel = <AjaxModel>{};
        ajaxRequest.ActionName = "InitializeSignDocument";
        ajaxRequest.Value = new Array<string>();
        ajaxRequest.Value.push(documents[0].ArchiveDocumentId);
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
        return false;
    }

    /**
    * Evento al click del pulsante "Sign" (DgrooveSign)
    * @param sender
    * @param args
    */
    btnSign_DgrooveSigner_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this.setWorkflowSessionStorage();
        let varStr: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
        if (!varStr) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return false;
        }
        let fascicle: FascicleModel = JSON.parse(varStr);
        if (!fascicle) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return false;
        }
        varStr = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
        if (!varStr) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return false;
        }
        this._loadingPanel.show(this.pageContentId);
        let documents: WorkflowReferenceBiblosModel[] = JSON.parse(varStr);
        if (!documents || documents.length === 0) {
            this._loadingPanel.hide(this.pageContentId);
            this._notificationInfo.set_text("Nessun documento selezionato");
            this._notificationInfo.show();
            return;
        }

        let radGridUD: Telerik.Web.UI.RadGrid = <Telerik.Web.UI.RadGrid>$find(this.radGridUDId);
        let dataItems: Telerik.Web.UI.GridDataItem[] = radGridUD.get_selectedItems();

        let documentRoot: DocumentRootFolderViewModel = {} as DocumentRootFolderViewModel;
        documentRoot.Id = this._fascicleModel.Number;
        documentRoot.UniqueId = this._fascicleModel.UniqueId;
        documentRoot.Name = this._fascicleModel.Title;
        documentRoot.SignBehaviour = DocumentSignBehaviour.Fascicle;
        documentRoot.DocumentFolders = [];

        let documentFolder: DocumentFolderViewModel = {} as DocumentFolderViewModel;
        documentFolder.Name = "Documenti";
        documentFolder.ChainType = ChainType.Miscellanea;

        let documentsToSign: DocumentViewModel[] = [];
        let element: Telerik.Web.UI.RadButton;
        let archiveChainId: string;
        let archiveDocumentId: string;
        let documentName: string;

        let document: DocumentViewModel;
        for (let item of dataItems) {
            element = <Telerik.Web.UI.RadButton>(item.findControl(FascVisualizza.btnUDLink_CONTROL_NAME));
            archiveChainId = element.get_element().getAttribute(FascVisualizza.BiblosChainId_ATTRIBUTE_NAME);
            archiveDocumentId = element.get_element().getAttribute(FascVisualizza.BiblosDocumentId_ATTRIBUTE_NAME);
            documentName = element.get_element().getAttribute(FascVisualizza.BiblosDocumentName_ATTRIBUTE_NAME);
            document = {} as DocumentViewModel;
            document.Mandatory = false;
            document.MandatorySelectable = false;
            document.DocumentId = archiveDocumentId;
            document.Name = documentName;
            document.ChainId = archiveChainId;
            document.FascicleId = this._fascicleModel.UniqueId;

            let extensions: string[] = documentName.split(".");
            if (extensions != null && extensions.length > 0) {
                let extension = extensions.pop().toLowerCase();
                document.PadesCompliant = extension === FileHelper.PDF ? true : false;
            }

            documentsToSign.push(document);
        }

        documentFolder.Documents = documentsToSign;
        documentRoot.DocumentFolders.push(documentFolder);

        let docsToSigns: DocumentRootFolderViewModel[] = [documentRoot];

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_DOCS_TO_SIGN, JSON.stringify(docsToSigns));

        var linkUrl = "../Comm/DgrooveSigns.aspx";
        window.location.href = linkUrl;

        return false;
    }

    /**
* Evento al click del pulsante "Autorizza"
* @param sender
* @param args
*/
    btnAutorizza_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._loadingPanel.show(this.pageContentId);
        var linkUrl = "../Fasc/FascAutorizza.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId);
        window.location.href = linkUrl;
    }

    /**
    * Evento al click del pulsante "Rimuovi"
    * @param sender
    * @param args
    */
    btnRemove_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let radGridUD: Telerik.Web.UI.RadGrid = <Telerik.Web.UI.RadGrid>$find(this.radGridUDId);
        let dataItems: Telerik.Web.UI.GridDataItem[] = radGridUD.get_selectedItems();
        if (dataItems.length == 0) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return;
        }
        this._manager.radconfirm("Sei sicuro di voler eliminare il documento selezionato dal fascicolo corrente?", (arg) => {
            if (arg) {
                this._loadingPanel.show(this.pageContentId);
                let documentsToDelete: any = {};
                let udToDeletePromises: JQueryPromise<void>[] = [$.Deferred<void>().resolve().promise()];
                for (let item of dataItems) {
                    let element: Telerik.Web.UI.RadButton = <Telerik.Web.UI.RadButton>(item.findControl(FascVisualizza.btnUDLink_CONTROL_NAME));
                    let uniqueId: string = element.get_element().getAttribute(FascVisualizza.UniqueId_ATTRIBUTE_NAME);
                    let documentName: string = element.get_element().getAttribute(FascVisualizza.BiblosDocumentName_ATTRIBUTE_NAME);
                    let environment: string = element.get_element().getAttribute(FascVisualizza.Environment_ATTRIBUTE_NAME);
                    switch (<Environment>Number(environment)) {
                        case Environment.Document:
                            documentsToDelete[uniqueId] = documentName;
                            break;
                        default:
                            udToDeletePromises.push(this.removeDocumentUnitFromFascicle(uniqueId));
                            break;
                    }
                }

                $.when.apply(null, udToDeletePromises)
                    .done(() => {
                        if (documentsToDelete && !$.isEmptyObject(documentsToDelete)) {
                            let ajaxRequest: AjaxModel = {} as AjaxModel;
                            ajaxRequest.ActionName = "Delete_Miscellanea_Document";
                            ajaxRequest.Value = new Array<string>();
                            ajaxRequest.Value.push(JSON.stringify(documentsToDelete));
                            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
                        } else {
                            this.sendRefreshUDRequest();
                        }
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        $("#".concat(this.pageContentId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            }
        }, 300, 160);
    }

    private getDocumentUnitAndFascicleAsync(uniqueId, currentFascicleId): JQueryPromise<any> {
        let promise: JQueryDeferred<FascicleRightsViewModel> = $.Deferred<FascicleRightsViewModel>();

        this._fascicleDocumentUnitService.getByDocumentUnitAndFascicle(uniqueId, currentFascicleId, (data: any) => {
            promise.resolve(data);
        });
        return promise.promise();
    }

    /**
    * Evento al click del pulsante "Completa"
    * @param sender
    * @param args
    */
    btnCompleteWorkflow_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var url = `../Workflows/CompleteWorkflow.aspx?Type=Fasc&IdFascicle=${this.currentFascicleId}&IdWorkflowActivity=${this.workflowActivityId}`;
        return this.openWindow(url, "windowCompleteWorkflow", 700, 500);
    }

    /**
    * Evento al click del pulsante "Inserti"
    * @param sender
    * @param args
    */
    btnInserts_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var url = "../Fasc/FascMiscellanea.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId);
        window.location.href = url;
    }

    /**
     * evento al click del pulsante Log flusso di lavoro
     * @param sender
     * @param args
     */
    btnWorkflowLogs_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var url = "../Fasc/FascInstanceLog.aspx?Type=Fasc&IdFascicle=".concat(this.currentFascicleId, "&ManagerID=", this.radWindowManagerCollegamentiId);
        return this.openWindow(url, "windowWorkflowInstanceLog", 1000, 650);
    }

    /**
     * evento al click del pulsante Log
     * @param sender
     * @param args
     */
    btnFascicleLog_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var url = "../Fasc/FascicleLog.aspx?IdFascicle=".concat(this.currentFascicleId);
        window.location.href = url;
    }

    /**
    * evento al click del pulsante Apri Fascicolo
    * @param sender
    * @param args
    */
    btnOpen_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        this._loadingPanel.show(this.pageContentId);
        args.set_cancel(true);
        this.openFascicleClosed();
    }

    /**
    * evento al click del pulsante Annulla Fascicolo
    * @param sender
    * @param args
    */
    btnUndo_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._manager.radconfirm("Sei sicuro di voler cancellare il fascicolo?", (arg) => {
            if (arg) {
                this._loadingPanel.show(this.pageContentId);
                args.set_cancel(true);
                this.undoFascicle();
            }
        });
    }


    /**
     * Evento alla chiusura della finestra di inserimento UD nel fascicolo che effettua una push a signalr per aggiornare i riferimenti per gli altri client collegati
     * @param sender
     * @param args
     */
    sendUDUpdate = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        this._loadingPanel.hide(this.radGridUDId);
        this._loadingPanel.show(this.radGridUDId);
        this.sendRefreshUDRequest();
        this.sendRefreshLinkRequest();
    }

    /**
     * Evento da SignalR all'aggiunta di una UD al fascicolo da parte di un client terzo
     */
    onUpdatedUDRequest = () => {
        this.sendRefreshUDRequest(this, () => {
            this._notificationInfo.show();
            this._notificationInfo.set_text("Aggiunto un nuovo documento al Fascicolo");
            this.sendRefreshLinkRequest();
        });
    }

    /**
     * Evento per il caricamento della griglia delle UD associate tramite SignalR
     * @param sender
     * @param onDoneCallback
     */
    sendRefreshUDRequest = (sender?: any, onDoneCallback?: any, afterRemove: boolean = false) => {
        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
        let qs: string = uscFascicolo.getFilterModel();
        let currentIdFascicleFolder: string;
        let selectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder();
        if (selectedFolder) {
            currentIdFascicleFolder = selectedFolder.UniqueId;
        }
        this.getFascicleDocumentUnits(qs, currentIdFascicleFolder)
            .done((data) => {
                this.refreshUD(data, afterRemove);
            }).fail((exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                $("#".concat(this.pageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private getFascicleDocumentUnits(qs: string, currentIdFascicleFolder: string): JQueryPromise<FascicleDocumentUnitModel[]> {
        let promise: JQueryDeferred<FascicleDocumentUnitModel[]> = $.Deferred<FascicleDocumentUnitModel[]>();

        this._fascicleDocumentUnitService.getFascicleDocumentUnits(this._fascicleModel, qs, this.currentTenantAOOId, currentIdFascicleFolder, (data: FascicleDocumentUnitModel[]) => {
            promise.resolve(data);
        }, (exception: ExceptionDTO) => {
            promise.reject(exception);
        });
        return promise.promise();
    }

    /**
     * Aggiorna la griglia delle UD
     * @param sender
     * @param args
     */
    refreshUD = (models: FascicleDocumentUnitModel[], updateFascicleDocument: boolean = false) => {
        PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
            .done((instance) => {
                let selectedFolderId: string = "";
                let selectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder();
                if (selectedFolder) {
                    selectedFolderId = selectedFolder.UniqueId;
                }

                this.getByFolder(selectedFolderId)
                    .done((data) => {
                        let insertsArchiveChains: string[] = data.filter((x) => x.ChainType.toString() == ChainType[ChainType.Miscellanea]).map((m) => m.IdArchiveChain);
                        if (selectedFolderId != "" && updateFascicleDocument) {
                            this._fascicleDocumentService.updateFascicleDocument(data[0]);
                        }

                        $("#".concat(this.uscFascicoloId)).unbind(UscFascicolo.GRID_REFRESH_EVENT);
                        $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.GRID_REFRESH_EVENT, (arg) => {
                            this._fascicleRights.HasFascicolatedUD = (models.filter(function (e) { return e.ReferenceType.toString() == FascicleReferenceType[FascicleReferenceType.Fascicle] }).length > 0);
                            this.setBtnCloseVisibility();
                            this.setBtnOpenVisibility();
                            this.setButtonEnable(true);
                            this._loadingPanel.hide(this.pageContentId);
                        });
                        instance.refreshGridUD(models, insertsArchiveChains);
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        $("#".concat(this.pageContentId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            });
    }

    private getByFolder(selectedFolderId: string): JQueryPromise<any> {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        this._fascicleDocumentService.getByFolder(this._fascicleModel.UniqueId, selectedFolderId, (data: any) => {
            promise.resolve(data);
        }, (exception: ExceptionDTO) => {
            promise.reject(exception);
        });
        return promise.promise();
    }

    sendRefreshLinkRequest = (sender?: any, onDoneCallback?: any) => {
        this.service.getLinkedFascicles(this._fascicleModel, null,
            (data: FascicleModel) => {
                this.refreshLink(data);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    refreshLink = (model: FascicleModel) => {
        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
        if (!jQuery.isEmptyObject(uscFascicolo)) {
            uscFascicolo.refreshLinkedFascicles(model);
        }
    }

    btnWorkflow_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this.setWorkflowSessionStorage();

        let url: string = `../Workflows/StartWorkflow.aspx?Type=Fasc&ManagerID=${this.radWindowManagerCollegamentiId}&DSWEnvironment=Fascicle&Callback=${window.location.href}`;
        if (this.workflowActivityId && !this.isWorkflowActivityClosed) {
            url = `${url}&ShowOnlyNoInstanceWorkflows=true`
        }
        if (this.isClosed) {
            url = `${url}&ShowOnlyHasIsFascicleClosedRequired=true`
        }
        return this.openWindow(url, "windowStartWorkflow", 730, 550);
    }

    btnMove_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let radGridUD: Telerik.Web.UI.RadGrid = <Telerik.Web.UI.RadGrid>$find(this.radGridUDId);
        let dataItems: Telerik.Web.UI.GridDataItem[] = radGridUD.get_selectedItems();
        if (dataItems.length == 0) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return;
        }

        let selectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder();
        let dtos: FascicleMoveItemViewModel[] = [];
        let element: Telerik.Web.UI.RadButton;
        let uniqueId: string;
        let environment: string;
        let UDName: string;
        let dto: FascicleMoveItemViewModel;
        for (let item of dataItems) {
            element = <Telerik.Web.UI.RadButton>(item.findControl(FascVisualizza.btnUDLink_CONTROL_NAME));
            uniqueId = element.get_element().getAttribute(FascVisualizza.UniqueId_ATTRIBUTE_NAME);
            UDName = element.get_text();
            environment = element.get_element().getAttribute(FascVisualizza.Environment_ATTRIBUTE_NAME);
            dto = {} as FascicleMoveItemViewModel;
            dto.uniqueId = uniqueId;
            dto.name = UDName;
            dto.environment = Number(environment) as Environment;
            dtos.push(dto);
        }

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASC_MOVE_ITEMS, JSON.stringify(dtos));

        var url = `FascMoveItems.aspx?Type=Fasc&idFascicle=${this.currentFascicleId}&ItemsType=DocumentType&IdFascicleFolder=${selectedFolder.UniqueId}`;
        return this.openWindow(url, "windowMoveItems", 750, 550);
    }

    /**
     * Evento di chiusura della finestra di Workflow
     */
    onWorkflowCloseWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let result: AjaxModel = <AjaxModel>{};
            result = <AjaxModel>args.get_argument();
            if (result && result.ActionName === "redirect" && result.Value && result.Value.length > 0) {
                this._loadingPanel.show(this.pageContentId);
                window.location.href = result.Value[0];
                return;
            }
            this._notificationInfo.show();
            this._notificationInfo.set_text(result.ActionName);
            this.workflowActivityId = result.Value ? result.Value[0] : null;
            this._workflowActivity = null;
            this.workflowCallback();
        }
    }

    folderNodeClickCallback = (arg: boolean) => {
        this.sendUDUpdate(this, undefined);
        this._btnInsert.set_enabled(arg);
        this._btnMove.set_enabled(arg);
    }

    onMoveCloseWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            this.sendUDUpdate(sender, args);
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    private initializeRights(fascicle: FascicleModel): JQueryPromise<FascicleRightsViewModel> {
        let promise: JQueryDeferred<FascicleRightsViewModel> = $.Deferred<FascicleRightsViewModel>();
        let fascicleRights: FascicleRightsViewModel = {};
        let fascicleRule: FascicleRights = new FascicleRights(fascicle, this._serviceConfigurations);
        $.when(fascicleRule.hasViewableRight(), fascicleRule.hasManageableRight(), fascicleRule.isManager(), fascicleRule.isProcedureSecretary(),
            this.hasAuthorizedWorkflows(), this.hasFascicolatedUD(fascicle.UniqueId))
            .done((view, edit, manager, secretary, wf, ud) => {
                fascicleRights.IsViewable = view;
                fascicleRights.IsEditable = edit;
                fascicleRights.IsManageable = edit;
                fascicleRights.IsManager = manager;
                fascicleRights.IsSecretary = secretary;
                fascicleRights.HasAuthorizedWorkflows = wf;
                fascicleRights.HasFascicolatedUD = ud;
                promise.resolve(fascicleRights);
            })
            .fail((exception: ExceptionDTO) => promise.reject(exception));

        return promise.promise();
    }

    private cleanWorkflowSessionStorage() {
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL);
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID);
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE);
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL);
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_METADATAS);
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_UDS_MODEL);
    }

    private setWorkflowSessionStorage() {
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(this._fascicleModel));
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, this.currentFascicleId);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_UNIQUEID, this.currentFascicleId);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, this._fascicleModel.Title);

        let radGridUD: Telerik.Web.UI.RadGrid = <Telerik.Web.UI.RadGrid>$find(this.radGridUDId);
        let dataItems: Telerik.Web.UI.GridDataItem[] = radGridUD.get_selectedItems();

        let element: Telerik.Web.UI.RadButton;
        let archiveChainId: string;
        let archiveDocumentId: string;
        let documentName: string;
        let environment: number;
        let uniqueId: string;

        let dtos: WorkflowReferenceBiblosModel[] = [];
        let referenceDocumentUnits: WorkflowReferenceDocumentUnitModel[] = [];
        for (let item of dataItems) {
            element = <Telerik.Web.UI.RadButton>(item.findControl(FascVisualizza.btnUDLink_CONTROL_NAME));
            environment = +element.get_element().getAttribute(FascVisualizza.Environment_ATTRIBUTE_NAME);

            uniqueId = element.get_element().getAttribute(FascVisualizza.UniqueId_ATTRIBUTE_NAME);
            referenceDocumentUnits.push(<WorkflowReferenceDocumentUnitModel>{
                Environment: environment,
                UniqueId: uniqueId
            });

            if (environment != Environment.Document) {
                continue;
            }

            archiveChainId = element.get_element().getAttribute(FascVisualizza.BiblosChainId_ATTRIBUTE_NAME);
            archiveDocumentId = element.get_element().getAttribute(FascVisualizza.BiblosDocumentId_ATTRIBUTE_NAME);
            documentName = element.get_element().getAttribute(FascVisualizza.BiblosDocumentName_ATTRIBUTE_NAME);
            let dto: WorkflowReferenceBiblosModel = {
                ArchiveChainId: archiveChainId,
                ChainType: ChainType.Miscellanea,
                ArchiveDocumentId: archiveDocumentId,
                ArchiveName: "",
                DocumentName: documentName,
                ReferenceDocument: null
            };

            dtos.push(dto);
        }

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENTS_REFERENCE_MODEL, JSON.stringify(dtos));
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_DOCUMENT_UNITS_REFERENCE_MODEL, JSON.stringify(referenceDocumentUnits));
    }


    public openSignWindow(serializedDoc: string) {
        this._loadingPanel.hide(this.pageContentId);
        this._currentDocumentToSign = serializedDoc;
        let url: string = `../Comm/SingleSign.aspx?${serializedDoc}`;
        this.openWindow(url, 'signWindow', 750, 500);
    }

    private hasAuthorizedWorkflows(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._workflowRepositoriyService.hasAuthorizedWorkflowRepositories(Environment.Fascicle, false,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    private hasFascicolatedUD(idFascicle): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this.service.hasFascicolatedDocumentUnits(idFascicle,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    /**
     * Inizializza lo user control del sommario di fascicolo
     */
    private loadFascicoloSummary(): void {
        PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
            .done((instance) => {
                instance.workflowActivityId = this.workflowActivityId
                $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, (args) => {
                    this.sendRefreshUDRequest();
                });
                instance.loadData(this._fascicleModel);
            });
    }

    /**
     * Metodo di callback inizializzazione
     * @param viewRights
     */
    initializePageByRigths(viewRights: FascicleRightsViewModel): void {

        this._fascicleRights = viewRights;

        if (!viewRights.IsViewable) {
            this.setButtonEnable(false);
            $("#".concat(this.pageContentId)).hide();
            this._loadingPanel.hide(this.pageContentId);
            this.showNotificationMessage(this.uscNotificationId, "Fascicolo n. ".concat(this._fascicleModel.Title, "<br />Impossibile visualizzare il fascicolo. Non si dispone dei diritti necessari."));
            return;
        }

        PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
            .done((instance) => {
                this.loadFascicoloSummary();
            });

        let procedureFascicleType: string = FascicleType[FascicleType.Procedure];
        let isProcedureFascicle: boolean = this._fascicleModel.FascicleType.toString() == procedureFascicleType;
        let isPeriodicFascicle: boolean = this._fascicleModel.FascicleType.toString() == FascicleType[FascicleType.Period];

        if (!($.type(this._fascicleModel.FascicleType) === "string")) {
            this._fascicleModel.FascicleType = FascicleType[this._fascicleModel.FascicleType.toString()];
            isProcedureFascicle = this._fascicleModel.FascicleType.toString() == procedureFascicleType;
            isPeriodicFascicle = this._fascicleModel.FascicleType.toString() == FascicleType[FascicleType.Period];
        }

        this.isClosed = this._fascicleModel.EndDate != null;
        this.setBtnCloseVisibility();
        this.setBtnOpenVisibility();
        this._btnInsert.set_visible(!this.isClosed);
        this._btnMove.set_visible(!this.isClosed);
        this._btnRemove.set_visible(!this.isClosed);
        this._btnSendToRoles.set_visible(!this.isClosed);
        this._btnEdit.set_visible(!this.isClosed || isPeriodicFascicle);
        this._btnLink.set_visible(!this.isClosed);
        this._btnAutorizza.set_visible(!this.isClosed);
        this._btnSign.set_visible(!this.isClosed);
        this._btnDocuments.set_visible(viewRights.IsViewable || isPeriodicFascicle);
        let isWorkflowEnabled: boolean = this.workflowEnabled && isProcedureFascicle;
        let workflowButtonVisibility = isWorkflowEnabled && viewRights.HasAuthorizedWorkflows && viewRights.IsManageable;
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_ISBUTTON_WORKFLOW_VISIBLE, String(workflowButtonVisibility));
        this._btnWorkflow.set_visible(workflowButtonVisibility);
        this._btnCompleteWorkflow.set_visible(!this.isClosed && this.hasActiveWorkflowActivityWorkflow());
        this._btnWorkflowLogs.set_visible(viewRights.IsManager || viewRights.IsSecretary);
        this._btnFascicleLog.set_visible(viewRights.IsManager || viewRights.IsSecretary);
        this._btnUndo.set_visible(!this.isClosed && (viewRights.IsManager || viewRights.IsSecretary))


        if (!this.isClosed) {
            this._btnEdit.set_visible((viewRights.IsEditable || viewRights.IsManager) && !isPeriodicFascicle);
            this._btnLink.set_visible(viewRights.IsManageable);
            this._btnAutorizza.set_visible((viewRights.IsEditable || viewRights.IsManager) && (isProcedureFascicle || isPeriodicFascicle));
            this._btnSign.set_visible((viewRights.IsEditable || viewRights.IsManager) && (isProcedureFascicle || isPeriodicFascicle));
            this._btnSendToRoles.set_visible(viewRights.IsViewable && this._fascicleModel.FascicleRoles.length != 0);
            this._btnInsert.set_visible(viewRights.IsManageable || viewRights.IsManager);
            this._btnMove.set_visible(viewRights.IsManageable || viewRights.IsManager);
            this._btnRemove.set_visible(viewRights.IsManageable || viewRights.IsManager);
            this._btnCopyToFascicle.set_visible(viewRights.IsManageable || viewRights.IsManager);
            let uscFascFolder: UscFascicleFolders = <UscFascicleFolders>$("#".concat(this.uscFascFoldersId)).data();
            if (!jQuery.isEmptyObject(uscFascFolder)) {
                uscFascFolder.setManageFascicleFolderVisibility(viewRights.IsManageable || viewRights.IsManager);
            }

            if (this.hasActiveWorkflowActivityWorkflow()) {
                this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
                    (dataWorkflow: any) => {
                        if (dataWorkflow) {
                            this._workflowActivity = dataWorkflow;
                            let completeWorkflowUserEnabled: boolean = false;
                            let isHandler: boolean = false;
                            let isHandlingDocumentEnabled = false;
                            this.isWorkflowActivityClosed = true;
                            if (this._workflowActivity != null && this._workflowActivity.WorkflowAuthorizations != null) {
                                let userAuthorization: WorkflowAuthorizationModel[] = this.filterWorkflowAuthorizationsByAccount(this._workflowActivity.WorkflowAuthorizations, this.currentUser);
                                let status: number = parseInt(WorkflowStatus[this._workflowActivity.Status]);
                                if (isNaN(status)) {
                                    status = WorkflowStatus[this._workflowActivity.Status.toString()];
                                }
                                this.isWorkflowActivityClosed = (status == WorkflowStatus.Done);
                                completeWorkflowUserEnabled = ((userAuthorization.length > 0) && !this.isWorkflowActivityClosed);
                                isHandler = userAuthorization.filter(function (item) {
                                    if (item.IsHandler == true) {
                                        return item;
                                    }
                                }).length > 0;
                            }
                            isHandlingDocumentEnabled = (viewRights.IsManageable || (isHandler && completeWorkflowUserEnabled && this.hasActiveWorkflowActivityWorkflow()));
                            this._btnCompleteWorkflow.set_visible(this.hasActiveWorkflowActivityWorkflow() && completeWorkflowUserEnabled && isWorkflowEnabled);
                            this._btnInsert.set_visible(isHandlingDocumentEnabled);
                            this._btnMove.set_visible(isHandlingDocumentEnabled);
                            this._btnRemove.set_visible(isHandlingDocumentEnabled);
                            this._btnCopyToFascicle.set_visible(isHandlingDocumentEnabled);

                            let uscFascFolder: UscFascicleFolders = <UscFascicleFolders>$("#".concat(this.uscFascFoldersId)).data();
                            if (!jQuery.isEmptyObject(uscFascFolder)) {
                                uscFascFolder.setManageFascicleFolderVisibility(isHandlingDocumentEnabled);
                            }
                        }
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        $("#".concat(this.pageContentId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    }
                );
            }
        }
        else {
            let uscFascFolder: UscFascicleFolders = <UscFascicleFolders>$("#".concat(this.uscFascFoldersId)).data();
            if (!jQuery.isEmptyObject(uscFascFolder)) {
                uscFascFolder.setCloseAttributeFascicleFolder();
            }
        }

        let uscFascFolder: UscFascicleFolders = <UscFascicleFolders>$("#".concat(this.uscFascFoldersId)).data();
        if (!jQuery.isEmptyObject(uscFascFolder)) {
            uscFascFolder.fileManagementButtonsVisibility(viewRights.IsManager || viewRights.IsSecretary);
        }

        this._pnlButtons.show();
    }

    filterWorkflowAuthorizationsByAccount = (arr: WorkflowAuthorizationModel[], criteria: string) => {
        return arr.filter(function (item) {
            if (item.Account.toLowerCase() == criteria.toLowerCase()) {
                return item;
            }
        });
    }

    /**
     * Apre una nuova nuova RadWindow
     * @param url
     * @param name
     * @param width
     * @param height
     */
    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerCollegamentiId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    private removeDocumentUnitFromFascicle(documentUnitId: string): JQueryPromise<void> {
        const promise: JQueryDeferred<void> = $.Deferred<void>();
        this.getDocumentUnitAndFascicleAsync(documentUnitId, this.currentFascicleId)
            .done((data) => {
                let fascicleDocumentUnitModel: FascicleDocumentUnitModel = new FascicleDocumentUnitModel(this.currentFascicleId);
                fascicleDocumentUnitModel.DocumentUnit = data.DocumentUnit;
                fascicleDocumentUnitModel.UniqueId = data.UniqueId;
                this.removeFascicleUD(fascicleDocumentUnitModel, this._fascicleDocumentUnitService).done(() => {
                    promise.resolve();
                }).fail((exception: ExceptionDTO) => promise.reject(exception));
            })
            .fail((exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    removeFascicleUD(model: IFascicolableBaseModel, service: IFascicolableBaseService<IFascicolableBaseModel>): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        service.deleteFascicleUD(model,
            (data: any) => {
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
                promise.reject();
            }
        );
        return promise.promise();
    }


    /**
     * Imposta l'attributo enable dei pulsanti
     * @param value
     */
    private setButtonEnable(value: boolean): void {
        this._btnEdit.set_enabled(value);
        this._btnInsert.set_enabled(value && this.getSelectedFascicleFolder() != undefined);
        this._btnMove.set_enabled(value && this._btnInsert.get_enabled());
        this._btnRemove.set_enabled(value);
        this._btnDocuments.set_enabled(value);
        this._btnClose.set_enabled(value);
        this._btnLink.set_enabled(value);
        this._btnAutorizza.set_enabled(value);
        this._btnSign.set_enabled(value);
        this._btnSendToRoles.set_enabled(value);
        this._btnWorkflow.set_enabled(value);
        this._btnCompleteWorkflow.set_enabled(value);
        this._btnWorkflowLogs.set_enabled(value);
        this._btnUndo.set_enabled(value);
        this._btnCopyToFascicle.set_enabled(value);
    }

    workflowCallback = () => {
        this._pnlButtons = $("#".concat(this.pnlButtonsId));
        this._pnlButtons.hide();

        this.setButtonEnable(false);

        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) return;
                this._fascicleModel = data;
                //se non ho più l'attività di workflow attiva, allora cerco se ne ho aperta un'altra
                let wfAction = () => $.Deferred<void>().resolve().promise();
                if (!this.workflowActivityId) {
                    wfAction = () => this.loadActiveWorkflowActivities();
                }

                wfAction()
                    .done(() => {
                        this.initializeRights(this._fascicleModel)
                            .done((fascicleRights) => {
                                this._fascicleRights = fascicleRights;
                                this.initializePageByRigths(fascicleRights);
                            })
                            .fail((exception: ExceptionDTO) => {
                                this._loadingPanel.hide(this.pageContentId);
                                $("#".concat(this.pageContentId)).hide();
                                this.showNotificationException(this.uscNotificationId, exception);
                            });
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        $("#".concat(this.pageContentId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    })
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                $("#".concat(this.pageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private loadActiveWorkflowActivities(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        //se non ho in query string l'id dell'attività, cerco se il fascicolo ha attività da completare        
        if (!this.hasActiveWorkflowActivityWorkflow()) {
            try {
                (this._workflowActivityService).getActiveActivitiesByReferenceIdAndEnvironment(this.currentFascicleId, Environment.Fascicle,
                    (data: any) => {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        //TODO: per ora prendo la prima, ma dopo dovremo fare una gestione per selezionare quale attività
                        this._workflowActivity = <WorkflowActivityModel>data;
                        this.workflowActivityId = data.UniqueId
                        promise.resolve();
                    },
                    (exception: ExceptionDTO): void => {
                        promise.reject(exception);
                    });
            } catch (error) {
                console.log((<Error>error).stack);
                promise.reject(error);
            }
        }
        else {
            promise.resolve();
        }

        return promise.promise();
    }

    private hasActiveWorkflowActivityWorkflow = () => {
        return !String.isNullOrEmpty(this.workflowActivityId);
    }

    private setBtnCloseVisibility() {
        let isProcedureFascicle: boolean = this._fascicleModel.FascicleType == FascicleType.Procedure;
        let isPeriodicFascicle: boolean = this._fascicleModel.FascicleType == FascicleType.Period;
        let isClosed: boolean = this._fascicleModel.EndDate != null;
        let isClosable: boolean = ((this._fascicleRights.IsManager || this._fascicleRights.IsSecretary) && !isClosed && !isPeriodicFascicle);
        if (isProcedureFascicle) {
            isClosable = isClosable && this._fascicleRights.HasFascicolatedUD;
        }
        this._btnClose.set_visible(isClosable);
    }

    private setBtnOpenVisibility() {
        let isClosed: boolean = this._fascicleModel.EndDate != null;
        let isVisible: boolean = ((this._fascicleRights.IsManager || this._fascicleRights.IsSecretary) && isClosed);
        this._btnOpen.set_visible(isVisible);
        this._btnOpen.set_enabled(false);

        if (isVisible) {
            this.getIdConservation();
        }
    }

    private getSelectedFascicleFolder(): FascicleSummaryFolderViewModel {
        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
        if (!jQuery.isEmptyObject(uscFascicolo)) {
            let selectedFolder: FascicleSummaryFolderViewModel = uscFascicolo.getSelectedFascicleFolder();
            if (selectedFolder && selectedFolder.UniqueId == this._fascicleModel.UniqueId) {
                return undefined;
            }
            return selectedFolder;
        }
        return undefined;
    }

    getIdConservation() {

        this._conservationService.getById(this.currentFascicleId,
            (data) => {
                if (data == null) {
                    this._btnOpen.set_enabled(true);
                    return;
                }
                this._btnOpen.set_enabled(false);
                this._btnOpen.set_toolTip("Il fascicolo è stato conservato. Funzionalità disabilitata");

            },
            (exception: ExceptionDTO) => {
                this.setButtonEnable(false)
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );

    }

    openFascicleClosed() {
        let fascicle: FascicleModel = <FascicleModel>{};
        fascicle.UniqueId = this.currentFascicleId;
        fascicle.Title = this._fascicleModel.Title;
        this.service.updateFascicle(fascicle, UpdateActionType.OpenFascicleClosed,
            (data: any) => {
                if (data == null) return;
                this._btnOpen.set_enabled(false);
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(fascicle.UniqueId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
                this._btnOpen.set_enabled(false);
                this.setButtonEnable(false);
            }
        );

    }

    undoFascicle() {
        let fascicle: FascicleModel = <FascicleModel>{};
        fascicle.UniqueId = this.currentFascicleId;
        this.service.deleteFascicle(fascicle,
            (data: any) => {
                if (data == null) return;
                this._btnUndo.set_enabled(false);
                window.location.href = "../Fasc/FascRicerca.aspx?Type=Fasc";
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
                this.setButtonEnable(false);
            }
        );

    }

    closeSignWindow(sender: any, args: any) {
        if (args.get_argument() && this._currentDocumentToSign) {
            this._loadingPanel.show(this.currentPageId);
            let ajaxRequest: AjaxModel = <AjaxModel>{};
            ajaxRequest.ActionName = "Signed";
            ajaxRequest.Value = new Array<string>();
            ajaxRequest.Value.push(args.get_argument());
            ajaxRequest.Value.push(this._currentDocumentToSign);
            this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
        }
        this._currentDocumentToSign = undefined;
    }
    btnCopyToFascicle_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let radGridUD: Telerik.Web.UI.RadGrid = <Telerik.Web.UI.RadGrid>$find(this.radGridUDId);
        let dataItems: Telerik.Web.UI.GridDataItem[] = radGridUD.get_selectedItems();
        if (dataItems.length == 0) {
            this.showWarningMessage(this.uscNotificationId, "Nessun documento selezionato");
            return;
        }
        let selectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder();
        let dtos: FascicleMoveItemViewModel[] = [];
        let element: Telerik.Web.UI.RadButton;
        let uniqueId: string;
        let environment: string;
        let UDName: string;
        let dto: FascicleMoveItemViewModel;
        for (let item of dataItems) {
            element = <Telerik.Web.UI.RadButton>(item.findControl(FascVisualizza.btnUDLink_CONTROL_NAME));
            uniqueId = element.get_element().getAttribute(FascVisualizza.UniqueId_ATTRIBUTE_NAME);
            UDName = element.get_text();
            environment = element.get_element().getAttribute(FascVisualizza.Environment_ATTRIBUTE_NAME);
            dto = {} as FascicleMoveItemViewModel;
            dto.uniqueId = uniqueId;
            dto.name = UDName;
            dto.environment = Number(environment) as Environment;
            dtos.push(dto);
        }

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASC_MOVE_ITEMS, JSON.stringify(dtos));
        let url: string = `../Fasc/FascRicerca.aspx?Type=Fasc&Action=SearchFascicles&ChoiseFolderEnabled=true&SelectedFascicleFolderId=${selectedFolder.UniqueId}&CurrentFascicleId=${this.currentFascicleId}`;
        this.openWindow(url, "windowFascicleSearch", 750, 600);
    }

}
export = FascVisualizza;