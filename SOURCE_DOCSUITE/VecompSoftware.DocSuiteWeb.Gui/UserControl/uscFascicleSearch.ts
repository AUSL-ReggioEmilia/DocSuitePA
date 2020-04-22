/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import uscFascSummary = require("./uscFascSummary");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import uscErrorNotification = require("./uscErrorNotification");

class uscFascicleSearch {
    btnSearchId: string;
    uscFascicleSummaryId: string;
    managerWindowsId: string;
    searchWindowId: string;
    ajaxFlatLoadingPanelId: string;
    finderContentId: string;
    uscNotificationId: string;
    summaryContentId: string;
    pageId: string;

    private readonly _serviceConfigurations: ServiceConfiguration[];
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _uscFascicleSummary: uscFascSummary;
    private _fascicleService: FascicleService;
    private _flatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _sessionStorageFascicleKey: string;

    private get summaryContentPanel(): JQuery {
        return $(`#${this.summaryContentId}`);
    }

    static LOADED_EVENT: string = "onLoaded";
    static FASCICLE_SELECTED_EVENT: string = "onFascicleSelected";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    btnSearch_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let url: string = '../Fasc/FascRicerca.aspx?Type=Fasc&Action=SearchFascicles';
        this.openWindow(url, "searchFascicles", 750, 600, this.closeSearchFasciclesWindow);
    }

    closeSearchFasciclesWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            try {
                let fascicleId: string = args.get_argument();
                sessionStorage.removeItem(this._sessionStorageFascicleKey);
                this._flatLoadingPanel.show(this.btnSearchId);
                this.loadFascicle(fascicleId)
                    .fail((exception: any) => {
                        this.showNotificationError(exception);
                    })
                    .always(() => this._flatLoadingPanel.hide(this.btnSearchId));
            }
            catch (error) {
                console.error(JSON.stringify(error));
                this.showNotificationError("Errore nella richiesta. Nessun fascicolo selezionato.")
            }
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize(): void {
        this._flatLoadingPanel = $find(this.ajaxFlatLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._btnSearch = $find(this.btnSearchId) as Telerik.Web.UI.RadButton;
        this._btnSearch.add_clicked(this.btnSearch_OnClick);

        this._sessionStorageFascicleKey = `${this.pageId}_selectedFascicle`;
        sessionStorage.removeItem(this._sessionStorageFascicleKey);

        let fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleServiceConfiguration);

        this.bindLoaded();
    }

    private bindLoaded(): void {
        $(`#${this.pageId}`).data(this);
        $(`#${this.pageId}`).triggerHandler(uscFascicleSearch.LOADED_EVENT);
    }

    clearSelections(): void {
        sessionStorage.removeItem(this._sessionStorageFascicleKey);
        this.summaryContentPanel.hide();
    }

    loadFascicle(fascicleId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!fascicleId) {
            return promise.reject("Nessun id fascicolo definito per la ricerca");
        }

        this._fascicleService.getFascicle(fascicleId,
            (data: any) => {
                if (!data) {
                    return promise.reject(`Nessun fascicolo trovato con id ${fascicleId}`);
                }

                this._uscFascicleSummary = $(`#${this.uscFascicleSummaryId}`).data() as uscFascSummary;
                if (jQuery.isEmptyObject(this._uscFascicleSummary)) {
                    return promise.reject(`E' avvenuto un errore durante il carimento delle informazioni del fascicolo selezionato. Si prega di riprovare.`);
                }

                sessionStorage.setItem(this._sessionStorageFascicleKey, JSON.stringify(data));
                $(`#${this.pageId}`).triggerHandler(uscFascicleSearch.FASCICLE_SELECTED_EVENT, data);
                this.summaryContentPanel.show();
                this._uscFascicleSummary.loadData(data as FascicleModel)
                    .done(() => promise.resolve())
                    .fail((exception) => promise.reject(exception));
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        )
        return promise.promise();
    }

    getSelectedFascicle(): FascicleModel {
        if (sessionStorage[this._sessionStorageFascicleKey]) {
            return JSON.parse(sessionStorage[this._sessionStorageFascicleKey]) as FascicleModel;
        }
        return undefined;
    }

    setButtonSearchEnabled(value: boolean): void {
        this._btnSearch.set_enabled(value);
    }

    private openWindow(url, name, width, height, closeHandler: (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => void): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = $find(this.managerWindowsId) as Telerik.Web.UI.RadWindowManager;
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.add_close(closeHandler);
        wnd.center();
        return false;
    }

    private showNotificationError(exception: string)
    private showNotificationError(exception: ExceptionDTO)
    private showNotificationError(exception: any) {
        let uscNotification: uscErrorNotification = $(`#${this.uscNotificationId}`).data() as uscErrorNotification;
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(exception);
            }
        }
    }
}

export = uscFascicleSearch;