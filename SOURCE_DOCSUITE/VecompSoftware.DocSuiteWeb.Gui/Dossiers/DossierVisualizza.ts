/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierBase = require('Dossiers/DossierBase');
import DossierService = require('App/Services/Dossiers/DossierService');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import UscDossier = require('UserControl/uscDossier');
import UscDossierFolders = require('UserControl/uscDossierFolders');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderStatus = require('App/Models/Dossiers/DossierFolderStatus');
import DossierDocumentService = require('App/Services/Dossiers/DossierDocumentService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowAuthorizationService = require('App/Services/Workflows/WorkflowAuthorizationService');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel')
import WorkflowAuthorizationModel = require('App/Models/Workflows/WorkflowAuthorizationModel');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import Environment = require('App/Models/Environment');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import AjaxModel = require('App/Models/AjaxModel');
import HandlerWorkflowManager = require('App/Managers/HandlerWorkflowManager');
import UscStartWorkflow = require('UserControl/uscStartWorkflow');
import BaseEntityRoleViewModel = require('App/ViewModels/BaseEntityRoleViewModel');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class DossierVisualizza extends DossierBase {

    currentDossierId: string;
    splContentId: string;
    uscDossierId: string;
    uscDossierFoldersId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    btnDocumentiId: string;
    btnModificaId: string;
    btnSendToSecretariesId: string;
    btnSendToRolesId: string;
    btnInsertiId: string;
    btnCloseId: string;
    btnLogId: string;
    miscellaneaLocationEnabled: boolean;
    btnAutorizzaId: string;
    btnAvviaWorkflowId: string;
    radWindowManagerCollegamentiId: string;
    uscNotificationId: string;
    fascPaneId: string;
    dossierPageId: string;
    selectedFascicleId: string;
    currentDossierTitle: string;
    btnCompleteWorkflowId: string;
    workflowActivityId: string;
    currentUser: string;
    radNotificationId: string;
    windowStartWorkflowId: string;
    windowCompleteWorkflowId: string;
    workflowEnabled: string;
    sendToSecretariesEnabled: string;
    currentFascicleId: string;
    isWindowPopupEnable: boolean;

    isViewable: boolean;
    isManageable: boolean;
    isEditable: boolean;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _notificationInfo: Telerik.Web.UI.RadNotification;
    private _windowStartWorkflow: Telerik.Web.UI.RadWindow;
    private _windowCompleteWorkflow: Telerik.Web.UI.RadWindow;
    private _serviceConfigurations: ServiceConfiguration[];
    private _DossierModel: DossierSummaryViewModel;
    private _DossierContacts: Array<BaseEntityViewModel>;
    private _DossierRoles: Array<BaseEntityRoleViewModel>;
    private _DossierFolders: Array<DossierSummaryFolderViewModel>;
    private _DossierDocuments: Array<BaseEntityViewModel>;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _service: DossierService;
    private _dossierFolderService: DossierFolderService;
    private _dossierDocumentService: DossierDocumentService;
    private _hasViewableDossier: boolean;
    private _btnDocumenti: Telerik.Web.UI.RadButton;
    private _btnModifica: Telerik.Web.UI.RadButton;
    private _btnSendToSecretaries: Telerik.Web.UI.RadButton;
    private _btnSendToRoles: Telerik.Web.UI.RadButton;
    private _btnClose: Telerik.Web.UI.RadButton;
    private _btnLog: Telerik.Web.UI.RadButton;
    private _btnAutorizza: Telerik.Web.UI.RadButton;
    private _btnCompleteWorkflow: Telerik.Web.UI.RadButton;
    private _btnWorkflow: Telerik.Web.UI.RadButton;
    private _btnInserti: Telerik.Web.UI.RadButton;
    private _fascPage: Telerik.Web.UI.RadPane;
    private _dossierPage: Telerik.Web.UI.RadPane;
    private _fascicleService: FascicleService;
    private _isManager: boolean = false;
    private _isFromWorkflow: boolean;
    private _workflowActivityService: WorkflowActivityService;
    private _workflowAuthorizationService: WorkflowAuthorizationService;
    private _workflowActivity: WorkflowActivityModel;
    private _isWorkflowEnabled: boolean;
    private _isSendToSecretariesEnabled: boolean;
    private _handlerManager: HandlerWorkflowManager;

    /**
    * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        this._handlerManager = new HandlerWorkflowManager(serviceConfigurations);
        $(document).ready(() => {
        });
    }


    /**
    *------------------------- Events -----------------------------
    */

    /**
   * Evento scatenato al click del pulsante Modifica
   * @method
   * @param sender
   * @param eventArgs
   * @returns
   */
    btnModifica_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.splContentId);
        this.setButtonEnable(false);
        window.location.href = "DossierModifica.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId);
    }

    btnAutorizza_OnCLick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.splContentId);
        this.setButtonEnable(false);
        window.location.href = "DossierAutorizza.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId);
    }

    /**
    * Evento al click del pulsante di Avvio Workflow
    * @param sender
    * @param args
    */
    btnWorkflow_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(this._DossierModel));
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, this.currentDossierId);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, this._DossierModel.Year.toString().concat("/", this._DossierModel.Number.toString()));

        var url = "../Workflows/StartWorkflow.aspx?Type=Dossier".concat("&ManagerID=", this.radWindowManagerCollegamentiId, "&DSWEnvironment=Dossier&Callback=", window.location.href);
        return this.openWindow(url, "windowStartWorkflow", 730, 550);
    }
    /**
    * Evento scatenato al click del pulsante Documenti
    * @method
    * @param sender
    * @param eventArgs
    * @returns
    */
    btnDocumenti_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.splContentId);
        this.setButtonEnable(false);
        window.location.href = "../Viewers/DossierViewer.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId);
    }

    /**
    * Evento al click del pulsante di Log
    * @param sender
    * @param args
    */
    btnLog_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._loadingPanel.show(this.splContentId);
        this.setButtonEnable(false);
        window.location.href = "DossierLog.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId, "&DossierTitle=", this._DossierModel.Year.toString(), "/", this._DossierModel.Number.toString());
    }

    /**
    * Evento al click del pulsante CompletaWorkflow
    * @param sender
    * @param args
    */

    btnCompleteWorkflow_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        var url = "../Workflows/CompleteWorkflow.aspx?=Dossier&IdDossier=".concat(this.currentDossierId, "&IdWorkflowActivity=", this.workflowActivityId);
        return this.openWindow(url, "windowCompleteWorkflow", 700, 500);
    }


    /**
     * Evento di chiusura della finestra di Workflow
     */
    onWorkflowCloseWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let result: AjaxModel = <AjaxModel>{};
            result = <AjaxModel>args.get_argument();
            this._notificationInfo.show();
            this._notificationInfo.set_text(result.ActionName);
            this.workflowActivityId = result.Value ? result.Value[0] : null;
            this._isFromWorkflow = !String.isNullOrEmpty(this.workflowActivityId);
            this._workflowActivity = null;
            this.workflowCallback();
        }
    }

    /**
* Evento scatenato al click del pulsante Inserti
* @method
* @param sender
* @param eventArgs
* @returns
*/
    btnInserti_OnClick = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.splContentId);
        this.setButtonEnable(false);
        window.location.href = "DossierMiscellanea.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId);
    }

    /**
    * ------------------------- Methods -----------------------------
    */

    /**
    * Initialize
    * La visibilità dei pulsanti di WF va riattivata quando si attiverà il modulo dei Wf su Dossier presso i clienti
    */
    initialize() {
        super.initialize();
        this._isWorkflowEnabled = this.workflowEnabled && this.workflowEnabled.toLowerCase() === 'true';
        this._isSendToSecretariesEnabled = this.sendToSecretariesEnabled && this.sendToSecretariesEnabled.toLowerCase() === 'true';
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationId);
        this._btnDocumenti = <Telerik.Web.UI.RadButton>$find(this.btnDocumentiId);
        this._windowStartWorkflow = <Telerik.Web.UI.RadWindow>$find(this.windowStartWorkflowId);
        this._btnInserti = <Telerik.Web.UI.RadButton>$find(this.btnInsertiId);
        this._windowStartWorkflow.add_close((this.onWorkflowCloseWindow));
        this._windowCompleteWorkflow = <Telerik.Web.UI.RadWindow>$find(this.windowCompleteWorkflowId);
        this._windowCompleteWorkflow.add_close((this.onWorkflowCloseWindow));
        this._btnDocumenti.add_clicked(this.btnDocumenti_OnClicked);
        this._btnClose = <Telerik.Web.UI.RadButton>$find(this.btnCloseId);
        this._btnModifica = <Telerik.Web.UI.RadButton>$find(this.btnModificaId);
        this._btnSendToSecretaries = <Telerik.Web.UI.RadButton>$find(this.btnSendToSecretariesId);
        this._btnSendToRoles = <Telerik.Web.UI.RadButton>$find(this.btnSendToRolesId);
        this._btnWorkflow = <Telerik.Web.UI.RadButton>$find(this.btnAvviaWorkflowId);
        this._btnCompleteWorkflow = <Telerik.Web.UI.RadButton>$find(this.btnCompleteWorkflowId);
        this._btnAutorizza = <Telerik.Web.UI.RadButton>$find(this.btnAutorizzaId);
        this._btnLog = <Telerik.Web.UI.RadButton>$find(this.btnLogId);
        this._DossierModel = <DossierSummaryViewModel>{};
        this._DossierRoles = new Array<BaseEntityRoleViewModel>();
        this._DossierContacts = new Array<BaseEntityViewModel>();
        this._DossierFolders = new Array<DossierSummaryFolderViewModel>();
        this.setButtonEnable(false);
        this._btnModifica.add_clicking(this.btnModifica_OnClick);
        this._btnInserti.add_clicking(this.btnInserti_OnClick);
        this._btnAutorizza.add_clicking(this.btnAutorizza_OnCLick);
        this._btnLog.add_clicking(this.btnLog_OnClick);
        this._btnSendToSecretaries.set_visible(false);
        this._btnSendToRoles.set_visible(false);
        this._btnInserti.set_visible(false);

        this._fascPage = <Telerik.Web.UI.RadPane>$find(this.fascPaneId);
        this._dossierPage = <Telerik.Web.UI.RadPane>$find(this.dossierPageId);
        this._fascPage.collapse(Telerik.Web.UI.SplitterDirection.Backward);

        let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERFOLDER_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let dossierDocumentConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERDOCUMENT_TYPE_NAME);
        this._dossierDocumentService = new DossierDocumentService(dossierDocumentConfiguration);

        let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleConfiguration);

        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        let WorkflowAuthorizationConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowAuthorization");
        this._workflowAuthorizationService = new WorkflowAuthorizationService(WorkflowAuthorizationConfiguration);

        $("#".concat(this.uscDossierFoldersId)).on(UscDossierFolders.FASCICLE_TREE_NODE_CLICK, (args, data) => {
            let url = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(data);
            this._fascPage.set_contentUrl(url);

            this._dossierPage.collapse(Telerik.Web.UI.SplitterDirection.Forward);
            this._fascPage.expand(Telerik.Web.UI.SplitterDirection.Backward);
            this._dossierPage.collapse(Telerik.Web.UI.SplitterDirection.Forward);
            this._fascPage.expand(Telerik.Web.UI.SplitterDirection.Backward);
        });

        this.resizeDetails();

        $("#".concat(this.uscDossierFoldersId)).on(UscDossierFolders.ROOT_NODE_CLICK, (args) => {
            this.resizeDetails();
        });

        if (!this.isWindowPopupEnable) {
            this._dossierPage.expand(Telerik.Web.UI.SplitterDirection.Forward);
        }

        this.service.hasRootNode(this.currentDossierId,
            (data: any) => {
                if (data) {
                    this.checkManageable();
                }
                else {
                    this.setButtonEnable(false);
                    this.hideUscLoadingPanels();
                    $("#".concat(this.splContentId)).hide();
                    this._loadingPanel.hide(this.splContentId);
                    this.showNotificationMessage(this.uscNotificationId, "Impossibile operare sul Dossier selezionato.<br/> Contattare l'assistenza.");
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splContentId);
                this.hideUscLoadingPanels();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private checkViewable(): void {
        (<DossierService>this.service).isViewableDossier(this.currentDossierId,
            (data: any) => {
                this.isViewable = data;

                if (!this.isViewable) {
                    $("#".concat(this.splContentId)).hide();
                    this._loadingPanel.hide(this.splContentId);
                    this.hideUscLoadingPanels();
                    this.showNotificationMessage(this.uscNotificationId, "Impossibile visualizzare il Dossier.<br/> Non si dispone dei diritti necessari.");
                    return;
                }

                this.loadData();
                this.setButtonVisibility(false);
                this._btnDocumenti.set_visible(true);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splContentId);
                this.hideUscLoadingPanels();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private checkManageable(): void {
        this.service.isManageableDossier(this.currentDossierId,
            (data: any) => {
                this.isManageable = data;

                if (this.isManageable) {
                    this._isManager = true;
                    if (this._isWorkflowEnabled) {
                        this.setWorkflowConfiguration();
                        return;
                    } else {
                        this.loadData();
                        return;
                    }
                } else {
                    this.checkViewable();
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private checkEditable(): void {
        (<DossierService>this.service).hasModifyRight(this.currentDossierId,
            (data: any) => {
                this.isEditable = data;

                this._btnModifica.set_visible(this.isEditable);
                this._btnAutorizza.set_visible(this.isEditable);
            });
    }

    private resizeDetails(): void {
        this._fascPage.collapse(Telerik.Web.UI.SplitterDirection.Backward);
        this._dossierPage.expand(Telerik.Web.UI.SplitterDirection.Forward);
        this._fascPage.collapse(Telerik.Web.UI.SplitterDirection.Backward);
        this._dossierPage.expand(Telerik.Web.UI.SplitterDirection.Forward);
    }

    /**
   * funzione per nascondere il loading panel del pannello dell'alberatura delle cartelle'
   */
    private hideUscLoadingPanels() {
        let uscDossierFolders: UscDossierFolders = <UscDossierFolders>$("#".concat(this.uscDossierFoldersId)).data();
        if (!jQuery.isEmptyObject(uscDossierFolders)) {
            uscDossierFolders.hideLoadingPanel();
        }
        let uscDossier: UscDossier = <UscDossier>$("#".concat(this.uscDossierId)).data();
        if (!jQuery.isEmptyObject(uscDossier)) {
            uscDossier.hideLoadingPanel();
        }
    }

    /**
    * funzione per discriminare l'autorizzazione al workflow
    */
    filterWorkflowAuthorizationsByAccount = (arr: WorkflowAuthorizationModel[], criteria: string) => {
        return arr.filter(function (item) {
            if (item.Account.toLowerCase() == criteria.toLowerCase()) {
                return item;
            }
        });
    }

    /*
    * Carico i dati nello USC dei dossier
    */

    loadUscDossier(): JQueryPromise<void> {
        this._loadingPanel.show(this.splContentId);
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        $.when(this.loadDossier(), this.loadRoles(), this.loadContacts(), this.loadMiscellaneous()).done(() => {
            this._DossierModel.Roles = this._DossierRoles;
            this._DossierModel.Contacts = this._DossierContacts;
            this._DossierModel.Documents = this._DossierDocuments;
            this.loadDossierSummary();
            promise.resolve();
        }).fail((exception) => {
            promise.reject(exception);
        }).always(() => {
            this._loadingPanel.hide(this.splContentId);
        });
        return promise.promise();
    }

    /*
    * funzione di caricamento dei dati
    */
    loadData(): void {
        $.when(this.loadUscDossier(), this.loadFolders()).done(() => {
            this.setButtonEnable(true);
            this.checkEditable();
            this.updateSentEmailVisibility();
        }).fail((exception) => {
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento del Dossier.");
        });
    }

    /*
    * Carico il dossier corrente senza navigation properties
    */
    loadDossier(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.service.getDossier(this.currentDossierId,
                (data: any) => {
                    try {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        this._DossierModel = data;
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /*
    * Carico i settori del Dossier
    */

    loadRoles(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.service.getDossierRoles(this.currentDossierId,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        //ritorna solo quello attivo
                        this._DossierRoles = data;
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }
    /**
    * carico i contatti del Dossier
    */

    loadContacts(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.service.getDossierContacts(this.currentDossierId,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        this._DossierContacts = data;
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /*
    * Imposto la visibilità dei bottoni per l'utilizzo del workflow
    */
    setWorkflowConfiguration() {
        let wfCheckActivityAction: () => JQueryPromise<string>;
        if (this.workflowActivityId) {
            wfCheckActivityAction = () => this._handlerManager.manageHandlingWorkflow(this.workflowActivityId);
        } else {
            wfCheckActivityAction = () => this._handlerManager.manageHandlingWorkflow(this.currentDossierId, Environment.Dossier);
        }

        wfCheckActivityAction()
            .done((idActivity: any) => {
                this.workflowActivityId = idActivity;
                this.setWorkflowButtons();
            })
            .fail((exception: ExceptionDTO) => this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento delle attività del fusso di lavoro associate al fascicolo."));
    }

    /**
     * Imposto i bottoni del workflow a seconda delle mie autorizzazioni
     */
    setWorkflowButtons() {
        let isHandlingDocumentEnabled = false;
        let completeWorkflowUserEnabled: boolean = false;
        this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
            (workflowActivity: any) => {
                if (workflowActivity) {
                    this._workflowActivity = workflowActivity;
                    let userAuthorization: WorkflowAuthorizationModel[] = this.filterWorkflowAuthorizationsByAccount(this._workflowActivity.WorkflowAuthorizations, this.currentUser);
                    let status: number = parseInt(WorkflowStatus[this._workflowActivity.Status]);
                    if (isNaN(status)) {
                        status = WorkflowStatus[this._workflowActivity.Status.toString()];
                    }

                    completeWorkflowUserEnabled = ((userAuthorization.length > 0) && status != WorkflowStatus.Done);
                    //this._btnCompleteWorkflow.set_visible(completeWorkflowUserEnabled);
                    //this._btnCompleteWorkflow.set_enabled(completeWorkflowUserEnabled);
                    //this._btnWorkflow.set_visible(!completeWorkflowUserEnabled)
                }
                this.loadData();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
    * carico le cartelle del Dossier
   */
    loadFolders(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._dossierFolderService.getChildren(this.currentDossierId, 0,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        this._DossierFolders = data;
                        this.loadDossierFoldersPanel();
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /**
   * carico gli inserti del Dossier
   */
    loadMiscellaneous(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            (<DossierDocumentService>this._dossierDocumentService).getDossierDocuments(this.currentDossierId,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        this._DossierDocuments = data;
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /**
    * Inizializzo lo user control del sommario di Dossier
    */
    private loadDossierSummary(): void {
        let uscDossier: UscDossier = <UscDossier>$("#".concat(this.uscDossierId)).data();
        if (!jQuery.isEmptyObject(uscDossier)) {
            uscDossier.workflowActivityId = this.workflowActivityId;
            uscDossier.loadData(this._DossierModel);
        }
    }

    /**
    * Inizializzo lo user control delle cartelline
    */
    private loadDossierFoldersPanel(): void {
        let uscDossierFolders: UscDossierFolders = <UscDossierFolders>$("#".concat(this.uscDossierFoldersId)).data();
        if (!jQuery.isEmptyObject(uscDossierFolders)) {
            if (this.currentFascicleId) {
                uscDossierFolders.loadFascicleDossierFolders(this.currentDossierId, this.currentFascicleId, this._DossierFolders);
            }
            else {
                uscDossierFolders.setRootNode(this.currentDossierTitle, this.currentDossierId);
                uscDossierFolders.loadNodes(this._DossierFolders);
            }
            uscDossierFolders.setToolbarButtonsVisibility(this._isManager);
        }
    }

    private setButtonEnable(value: boolean): void {
        //this._btnWorkflow.set_enabled(value);
        this._btnDocumenti.set_enabled(value);
        this._btnModifica.set_enabled(value);
        this._btnSendToSecretaries.set_enabled(value && this._isSendToSecretariesEnabled);
        this._btnSendToRoles.set_enabled(value);
        //this._btnClose.set_enabled(value);

        this._btnAutorizza.set_enabled(value);
        this._btnLog.set_enabled(value);
        //this._btnCompleteWorkflow.set_enabled(value);
        //this._btnInserti.set_enabled(value);
        //if (!this.miscellaneaLocationEnabled) {
            //this._btnInserti.set_enabled(false);
            //this._btnInserti.set_toolTip("Nessuna configurazione definita per l'inserimento degli inserti.Contattare Assistenza.");
        //}
    }

    private updateSentEmailVisibility() {
        this._dossierFolderService.hasAssociatedFascicles(this.currentDossierId, (exists: boolean) => {
            this._btnSendToSecretaries.set_visible(this._isSendToSecretariesEnabled && this._isManager && exists);
            this._btnSendToRoles.set_visible(this._isManager && exists);
        }, (exception: ExceptionDTO) => {
            console.log(exception);
        });

    }

    private setButtonVisibility(value: boolean): void {
        this._btnDocumenti.set_visible(value);
        this._btnModifica.set_visible(value);
        this._btnAutorizza.set_visible(value);
        this._btnLog.set_visible(value);
        this._btnInserti.set_visible(value);
        if (!this.miscellaneaLocationEnabled) {
            this._btnInserti.set_visible(false);
            this._btnInserti.set_toolTip("Nessuna configurazione definita per l'inserimento degli inserti.Contattare Assistenza.");
        }

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

    /*
    * Funzione chiamata al callback di chiusura della finestra di gestione del workflow
    */
    workflowCallback = () => {

        this.setButtonEnable(false);

        this.loadData();
        this.setWorkflowConfiguration();
        this._loadingPanel.hide(this.splContentId);
        if (this.workflowActivityId) {
            this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
                (dataWorkflow: any) => {
                    if (dataWorkflow) {
                        this._workflowActivity = dataWorkflow;
                    }
                }
            );
        }
    }
}

export = DossierVisualizza;