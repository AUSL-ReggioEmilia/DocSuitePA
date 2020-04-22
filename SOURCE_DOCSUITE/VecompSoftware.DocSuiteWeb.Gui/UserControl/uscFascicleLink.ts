/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import AjaxModel = require('App/Models/AjaxModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import UscFascSummary = require('UserControl/uscFascSummary');

class uscFascicleLink {

    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    pageId: string;
    rcbOtherFascicles: string;
    managerId: string;
    lblCategoryMessageId: string;
    fascicleSummaryId: string;
    currentFascicleId: string;
    selectedCategoryId: number;
    uscCategoryId: string;
    uscNotificationId: string;
    uscFascSummaryId: string;

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _notification: Telerik.Web.UI.RadNotification;
    private _rcbOtherFascicles: Telerik.Web.UI.RadComboBox;
    private _serviceConfigurations: ServiceConfiguration[];
    private _fascicleService: FascicleService;
    private _lblCategoryMessage: JQuery;
    private _fascicleSummary: JQuery;
   
    private _domainUserService: DomainUserService;

    public static LOADED_EVENT: string = "onLoaded";

    /**
    * Costruttore
    * @param serviceConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle");
        this._serviceConfigurations = serviceConfigurations;
        if (!serviceConfiguration) {
            return;
        }
        this._fascicleService = new FascicleService(serviceConfiguration);
    }


    /**
    * Inizializzazione
    */
    initialize(): void {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._lblCategoryMessage = $("#".concat(this.lblCategoryMessageId));
        this._rcbOtherFascicles = <Telerik.Web.UI.RadComboBox>$find(this.rcbOtherFascicles);
        this._rcbOtherFascicles.add_selectedIndexChanged(this.rcbOtherFascicles_OnSelectedIndexChange);
        this._rcbOtherFascicles.add_itemsRequested(this.rcbOtherFascicles_OnClientItemsRequested);
        this._fascicleSummary = $("#".concat(this.fascicleSummaryId));
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._fascicleSummary.hide();

        let scrollContainer: JQuery = $(this._rcbOtherFascicles.get_dropDownElement()).find('div.rcbScroll');
        $(scrollContainer).scroll(this.rcbOtherFascicles_onScroll);

        let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        let fascicleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleConfiguration);

   
        this.bindLoaded();
    }


    /**
    * -------------------------- Events ---------------------------
    */

    rcbOtherFascicles_OnSelectedIndexChange = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = sender.get_selectedItem();
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        if (domEvent.type == 'mousedown') {
            return;
        }

        if (selectedItem == null || selectedItem.get_value() == "") {
            this._fascicleSummary.hide();
            return;
        }

        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            let emptyItem: Telerik.Web.UI.RadComboBoxItem = sender.findItemByText('');
            sender.clearItems();
            sender.get_items().add(emptyItem);
            sender.get_items().add(selectedItem);
            sender.get_attributes().setAttribute('currentFilter', selectedItem.get_text());
            sender.get_attributes().setAttribute('otherFascicleCount', '1');
        }

        this._loadingPanel.show(this.pageId);
        this._fascicleService.getFascicle(selectedItem.get_value(),
            (data: any) => {
                if (data == null) {
                    this._fascicleSummary.hide();
                    return;
                }

                let fascicle: FascicleModel = data;
                this.currentFascicleId = fascicle.UniqueId;
                this._domainUserService.getUser(fascicle.RegistrationUser,
                    (user: DomainUserModel) => {
                        this.setFascicleSummaryData(fascicle);                       
                        $("#".concat(this.pageId)).data(this);
                        this.onExternalCategoryChange(fascicle.Category.EntityShortId);
                        this._loadingPanel.hide(this.pageId);
                    },
                    (exception: ExceptionDTO) => {
                        //Carico ugualmente il sommario del fascicolo
                        this.setFascicleSummaryData(fascicle);
                        this._loadingPanel.hide(this.pageId);
                    }
                );
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Evento scatenato allo scrolling della RadComboBox di selezione fascicoli
     * @param args
     */
    rcbOtherFascicles_onScroll = (args: JQueryEventObject) => {
        var element = args.target;
        if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
            let filter: string = this._rcbOtherFascicles.get_text();
            this.rcbOtherFascicles_OnClientItemsRequested(this._rcbOtherFascicles, new (<any>Telerik.Web.UI.RadComboBoxRequestEventArgs)(filter, args));
        }
    }


    /**
     * Evento scatenato dalla RadComboBox per inizializzare i dati da visualizzare
     * @param sender
     * @param args
     */
    rcbOtherFascicles_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {

        if (!this.selectedCategoryId) {
            this.selectedCategoryId = 0;
        }

        let numberOfItems: number = sender.get_items().get_count();
        if (numberOfItems > 0) {
            //Decremento di 1 perchè la combo visualizza anche un item vuoto
            numberOfItems -= 1;
        }

        let currentOtherFascicleItems: number = numberOfItems;
        let currentComboFilter: string = sender.get_attributes().getAttribute('currentFilter');
        let otherFascicleCount: number = Number(sender.get_attributes().getAttribute('otherFascicleCount'));
        let updating: boolean = sender.get_attributes().getAttribute('updating') == 'true';
        if (isNaN(otherFascicleCount) || currentComboFilter != args.get_text()) {
            //Se il valore del filtro è variato re-inizializzo la radcombobox per chiamare le WebAPI
            otherFascicleCount = undefined;
        }
        sender.get_attributes().setAttribute('currentFilter', args.get_text());

        if ((otherFascicleCount == undefined || currentOtherFascicleItems < otherFascicleCount) && !updating) {
            sender.get_attributes().setAttribute('updating', 'true');
            this._fascicleService.getFascicleByCategory(this.selectedCategoryId, args.get_text(),
                (data: ODATAResponseModel<FascicleModel>) => {
                    try {
                        this.refreshFascicles(data.value);
                        let scrollToPosition: boolean = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollContainer.scrollTop($(sender.get_items().getItem(currentOtherFascicleItems + 1).get_element()).position().top);
                            }
                        }
                        sender.get_attributes().setAttribute('updating', 'false');
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                    }
                }
            );
        }
    }

    /**
* ------------------------------ Methods ----------------------------
*/


    /**
     * Metodo per popolare la RadComboBox di selezione fascicoli
     * @param data
     */
    refreshFascicles = (data: FascicleModel[]) => {
        if (data.length > 0) {
            this._rcbOtherFascicles.clearItems();
            this._rcbOtherFascicles.beginUpdate();

            if (this._rcbOtherFascicles.get_items().get_count() == 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._rcbOtherFascicles.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, fascicle) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(fascicle.Title.concat(" ", fascicle.FascicleObject));
                item.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
                item.set_value(fascicle.UniqueId);
                this._rcbOtherFascicles.get_items().add(item);
            });
            this._rcbOtherFascicles.showDropDown();
            this._rcbOtherFascicles.endUpdate();

        }
    }

    /*
    * Cambio esternamente il classificatore
    */
    onExternalCategoryChange(idCategory: number) {
        this._ajaxManager.ajaxRequest(this.uscCategoryId.concat("|").concat(idCategory.toString()));
    }


    /**
    * Evento al cambio di classificatore
    */
    onCategoryChanged = (idCategory: number) => {
        this._rcbOtherFascicles.clearItems();
        this.selectedCategoryId = idCategory;
        this._lblCategoryMessage.html("");

        $("#".concat(this.pageId)).data(this);
    }

    /**
 * Popolo i dati del fieldset di sommario di fascicolo selezionato
 * @param fascicle
 */
    private setFascicleSummaryData(fascicle: FascicleModel): void {
        this.selectedCategoryId = fascicle.Category.EntityShortId;
        let uscFascSummary: UscFascSummary = <UscFascSummary>$("#".concat(this.uscFascSummaryId)).data();
        if (!jQuery.isEmptyObject(uscFascSummary)) {
            uscFascSummary.loadData(fascicle);
        }

        this._fascicleSummary.show();
    }

    /**
    * Chiude la RadWindow
    */
    closeWindow(args?: any): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(args);
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
* Scateno l'evento di "Load Completed" del controllo
*/
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscFascicleLink.LOADED_EVENT);

    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

}
export = uscFascicleLink;