/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');


declare var Page_IsValid: any;
class commonUploadZIPMiscellanea {

    txtPrefixId: string;
    uscNotificationId: string;
    insertsPageContentId: string;
    btnSaveId: string;
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    actionType: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _btnSave: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

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
  *------------------------- Events -----------------------------
  */

    /**
   * Evento scatenato al click del pulsante ConfermaInserimento
   * @method
   * @param sender
   * @param eventArgs
   * @returns
   */
    btnSave_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        if (Page_IsValid) {
            this.showLoadingPanel(this.insertsPageContentId);
            this._btnSave.set_enabled(false);
        }
    }

    /**
* Callback per l'inserimento/aggiornamento di un MiscellaneaDocumentModel
* @param entity
*/
    confirmCallback(documentChainId: string) {
        try {

            let model = <AjaxModel>{};
            model.ActionName = this.actionType;
            model.Value = [documentChainId];
            this.closeWindow(model);
            this.hideLoadingPanel(this.insertsPageContentId)
        }
        catch (error) {
            this.hideLoadingPanel(this.insertsPageContentId);
            this.showNotificationMessage(this.uscNotificationId, "Errore in esecuzione dell'attività di salvataggio.");
            console.log(JSON.stringify(error));
        }
    }

    /**
 *------------------------- Methods -----------------------------
 */

    /**
    * Initialize
    */
    initialize() {

        this._btnSave = <Telerik.Web.UI.RadButton>$find(this.btnSaveId);
        this._btnSave.add_clicked(this.btnSave_Clicked);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
    }

    /**
 * Visualizza un nuovo loading panel nella pagina
 */
    private showLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.show(updatedElementId);
    }


    /**
     * Nasconde il loading panel nella pagina
     */
    private hideLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.hide(updatedElementId);
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


    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

}
export = commonUploadZIPMiscellanea;
