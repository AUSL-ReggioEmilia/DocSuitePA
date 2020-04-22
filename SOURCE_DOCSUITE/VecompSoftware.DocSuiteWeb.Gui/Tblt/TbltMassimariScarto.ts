/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import CategoryService = require('App/Services/Commons/CategoryService');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import MassimarioScartoModel = require('App/Models/MassimariScarto/MassimarioScartoModel');
import MassimarioScartoStatusType = require('App/Models/MassimariScarto/MassimarioScartoStatusType');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import uscMassimarioScarto = require('UserControl/uscMassimarioScarto');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class TbltMassimariScarto {
    btnSaveId: string;
    categoryId: number;
    uscMassimarioScartoId: string;
    ajaxManagerId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;

    private _categoryService: CategoryService;
    private _btnSave: Telerik.Web.UI.RadButton;
    private _uscMassimarioScarto: uscMassimarioScarto;
        /**
     * Costruttore
     * @param serviceConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId,"Errore in inizializzazione. Nessun servizio configurato per il Classificatore");
            return;
        }

        this._categoryService = new CategoryService(serviceConfiguration);
        $(document).ready(function () {
        });
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante di Conferma
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    btnSave_OnClicking = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        eventArgs.set_cancel(true);
        this._btnSave.set_enabled(false);
        this.showLoadingPanel();
        this._uscMassimarioScarto = <uscMassimarioScarto>$("#".concat(this.uscMassimarioScartoId)).data();
        let selectedModel: MassimarioScartoModel = this._uscMassimarioScarto.getSelectedMassimario();
        if ((selectedModel.MassimarioScartoLevel == undefined || selectedModel.MassimarioScartoLevel < 2)
                || (selectedModel.Status == undefined || selectedModel.Status == MassimarioScartoStatusType.LogicalDelete)) {
            this.showWarningMessage(this.uscNotificationId,'Selezionare un massimario di scarto valido.');
            this.closeLoadingPanel();
            this._btnSave.set_enabled(true);
            return false;
        }
        var params: string = "AddMassimario|".concat(selectedModel.UniqueId);
        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(params);
    }

    /**
     *------------------------- Methods -----------------------------
     */

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }


    /**
     * Metodo di inizializzazione
     */
    initialize() {
        this._btnSave = <Telerik.Web.UI.RadButton>$find(this.btnSaveId);
        this._btnSave.add_clicking(this.btnSave_OnClicking);
    }

    /**
     * Recupera una RadWindow dalla pagina
     */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    /**
     * Chiude la RadWindow
     */
    closeWindow(): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(true);
    }

    /**
      * Visualizza un nuovo loading panel nella pagina
      */
    private showLoadingPanel(): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.show(this.uscMassimarioScartoId);
    }

    /**
     * Nasconde il loading panel nella pagina
     */
    private closeLoadingPanel(): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.hide(this.uscMassimarioScartoId);
    }
}

export = TbltMassimariScarto;