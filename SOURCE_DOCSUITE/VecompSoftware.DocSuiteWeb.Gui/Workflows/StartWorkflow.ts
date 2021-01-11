/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import ErrorHelper = require('App/Helpers/ErrorHelper');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscStartWorkflow = require('UserControl/uscStartWorkflow');
import AjaxModel = require('App/Models/AjaxModel');

declare var Page_ClientValidate: any;
class StartWorkflow {

    uscContentId: string;
    radNotificationId: string;
    radNotificationSuccessId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;   
    uscStartWorkflowId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notification: Telerik.Web.UI.RadNotification;
    private _notificationSuccess: Telerik.Web.UI.RadNotification;
    private _serviceConfigurations: ServiceConfiguration[];
    private _errorHelper: ErrorHelper;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _workflowRepositoryService: WorkflowRepositoryService;
    /**
    * Costruttore
         * @param serviceConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    /**
    * Initialize
    */
    initialize() {
        this._errorHelper = new ErrorHelper();
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._notification = <Telerik.Web.UI.RadNotification>$find(this.radNotificationId);
        this._notificationSuccess = <Telerik.Web.UI.RadNotification>$find(this.radNotificationSuccessId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        $("#".concat(this.uscContentId)).on(UscStartWorkflow.LOADED_EVENT, (args) => {
            this.loadUscStartWorkflow();
        });
        this.loadUscStartWorkflow();
    }

    /**
    * ------------------------- Methods -----------------------------
    */
    /*
    * funzione di caricamento dei dati
    */
    loadData(): void {
        $.when(this.loadUscStartWorkflow()).done(() => {
        }).fail(() => {
            this._notification.show();
            this._notification.set_text("Errore nel caricamento della pagina.");
        });
    }


    loadUscStartWorkflow(): void {
        let uscStartWorkflow: UscStartWorkflow = <UscStartWorkflow>$("#".concat(this.uscContentId)).data();
        if (!jQuery.isEmptyObject(uscStartWorkflow)) {
            uscStartWorkflow.loadData();
        }
    }
}

export = StartWorkflow;