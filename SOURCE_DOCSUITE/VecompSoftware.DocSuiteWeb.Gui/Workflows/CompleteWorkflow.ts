/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />


import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ErrorHelper = require('App/Helpers/ErrorHelper');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscCompleteWorkflow = require('UserControl/uscCompleteWorkflow');
import AjaxModel = require('App/Models/AjaxModel');

declare var Page_ClientValidate: any;
class CompleteWorkflow {

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
    }

    /**
    * ------------------------- Methods -----------------------------
    */

}

export = CompleteWorkflow;