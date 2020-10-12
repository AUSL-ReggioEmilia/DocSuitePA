/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleBase = require('Fasc/FascBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscFascicleLink = require('UserControl/uscFascicleLink')
import FascicleModel = require('App/Models/Fascicles/FascicleModel');

class FascAddUDLink extends FascicleBase {
    currentPageId: string;
    btnConfermaId: string;
    ajaxManagerId: string;
    managerId: string;
    uscFascLinkId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;

    private _manager: Telerik.Web.UI.RadWindowManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _rgvLinkedFascicles: Telerik.Web.UI.RadGrid;
    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rcbOtherFascicles: Telerik.Web.UI.RadComboBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _lblViewFascicle: JQuery;
    private _fascicleSummary: JQuery;
    private _currentFascicleId: string;

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;

    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
    * Inizializzazione
    */
    initialize() {
        super.initialize();
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btnConferma_ButtonClicked); 
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        $("#".concat(this.uscFascLinkId)).bind(UscFascicleLink.LOADED_EVENT, (args) => {
            
        });

    }


    /**
    * Evento scatenato al click del pulsante ConfermaInserimento
    * @method
    * @param sender
    * @param eventArgs
    * @returns
    */
    btnConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);
       
        let uscFascLink: UscFascicleLink = <UscFascicleLink>$("#".concat(this.uscFascLinkId)).data();       
        if (!jQuery.isEmptyObject(uscFascLink)) {
            if (!uscFascLink.currentFascicleId) {
                this.showNotificationMessage(this.uscNotificationId, "Nessun Fascicolo selezionato");
                this._loadingPanel.hide(this.currentPageId);
                this._btnConferma.set_enabled(true);
                return;
            }   
            
            this.closeWindow(uscFascLink.currentFascicleId);
        }
        
    }


    /**
    *------------------------- Methods -----------------------------
    */

    /**
* Chiude la RadWindow
*/
    /**
* Chiude la RadWindow
*/
    protected closeWindow(message?: any): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }


    /**
    * Recupera una RadWindow dalla pagina
    */
    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }
}
export = FascAddUDLink;