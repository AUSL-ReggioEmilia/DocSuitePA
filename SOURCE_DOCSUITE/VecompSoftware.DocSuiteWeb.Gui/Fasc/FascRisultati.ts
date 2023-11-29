
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import uscFascicleSearch = require('UserControl/uscFascicleSearch');


class FascRisultati extends FascicleBase {
    btnDocumentsId: string;
    btnSelectAllId: string;
    btnDeselectAllId: string;
    btnExportAllId: string;
    gridId: string;
    ajaxLoadingPanelId: string;
    selectableFasciclesThreshold: number;
    uscNotificationId: string;
    backBtnId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnDocuments: Telerik.Web.UI.RadButton;
    private _btnSelectAll: Telerik.Web.UI.RadButton;
    private _btnDeselectAll: Telerik.Web.UI.RadButton;
    private _backBtn: Telerik.Web.UI.RadButton;
    private _btnExportAll: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _grid: Telerik.Web.UI.RadGrid;

    /**
    * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
    }

    /**
     * Initialize
     */
    initialize(): void {
        super.initialize();
        this._backBtn = <Telerik.Web.UI.RadButton>$find(this.backBtnId);
        if (this._backBtn) {
            this._backBtn.add_clicked(this.navigateBack);
        }
        this._btnDocuments = <Telerik.Web.UI.RadButton>$find(this.btnDocumentsId);
        this._btnSelectAll = <Telerik.Web.UI.RadButton>$find(this.btnSelectAllId);
        this._btnDeselectAll = <Telerik.Web.UI.RadButton>$find(this.btnDeselectAllId);
        this._btnExportAll = <Telerik.Web.UI.RadButton>$find(this.btnExportAllId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._grid = <Telerik.Web.UI.RadGrid>$find(this.gridId);

        if (this._btnDocuments) {
            this._btnDocuments.add_clicking(this.btnDocuments_OnClick);
        }
        if (this._btnSelectAll) {
            this._btnSelectAll.add_clicking(this.btnSelectAll_OnClick);
        }
        if (this._btnDeselectAll) {
            this._btnDeselectAll.add_clicking(this.btnDeselectAll_OnClick);
        }
    }

    /**
    *------------------------- Events -----------------------------
    */

    private navigateBack(): void {
        window.history.back();
    }

    /**
     * Evento scatenato al click del pulsante di Visualizza documenti
     * @param sender
     * @param args
     */
    btnDocuments_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let selection: string[] = new Array();
        $.each(this.getSelectedItemIDs(), (index, id) => {
            selection.push(id);
        });

        if (selection.length > 0) {
            if (selection.length > this.selectableFasciclesThreshold) {
                this.showNotificationMessage(this.uscNotificationId,"Non si possono selezionare più di ".concat(this.selectableFasciclesThreshold.toString(), " fascicoli"));
                return false;
            }
            this._loadingPanel.show(this.gridId);
            let fascicleIds: string = encodeURIComponent(JSON.stringify(selection));
            let editUrl: string = "../Viewers/FascicleViewer.aspx?Type=Fasc&FascicleIds=".concat(fascicleIds);
            window.location.href = editUrl;
        }
        else {
            this.showWarningMessage(this.uscNotificationId,"Nessun fascicolo selezionato");
        }
    }

    /**
    * Evento scatenato al click del pulsante di Seleziona tutti
    * @param sender
    * @param args
    */
    btnSelectAll_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        let count: number = 0;
        $.each(this._grid.get_masterTableView().get_dataItems(), (index, item) => {
            if (count >= this.selectableFasciclesThreshold) {
                this.showWarningMessage(this.uscNotificationId,"Non si possono selezionare più di ".concat(this.selectableFasciclesThreshold.toString(), " fascicoli"));
                return false;
            }
            let element = <HTMLInputElement>item.findElement("cbSelect");
            if (!element.disabled) {                               
                element.checked = true;
                count++;
            }
        });
    }

    /**
    * Evento scatenato al click del pulsante di Seleziona tutti
    * @param sender
    * @param args
    */
    btnDeselectAll_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        $.each(this.getSelectedItems(), (index, item) => {
            let element = <HTMLInputElement>item.findElement("cbSelect");
            element.checked = false;
        });
    }



    /**
     *------------------------- Methods -----------------------------
     */

    getSelectedItems(): Telerik.Web.UI.GridDataItem[] {
        let selectedItems : Telerik.Web.UI.GridDataItem[] = new Array();
        let gridItems = this._grid.get_masterTableView().get_dataItems();
        $.each(gridItems, (index, item) => {
            let element = <HTMLInputElement>item.findElement("cbSelect");
            if (element.checked) {
                selectedItems.push(item);
            }
        });
        return selectedItems;
    }

    getSelectedItemIDs(): string[] {
        let ids: string[] = new Array();
        let selectedItems: Telerik.Web.UI.GridDataItem[] = this.getSelectedItems();
        $.each(selectedItems, (index, item) => {            
                ids.push(item.getDataKeyValue("IdFascicle"));            
        });
        return ids;
    }

    /**
 * Metodo per il recupero di una specifica radwindow
 */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let radWindow: Telerik.Web.UI.RadWindow;
        if ((<any>window).radWindow) radWindow = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) radWindow = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return radWindow;
    }

    /**
     * Metodo di chiusura di una radwindow
     * @param callback
     */
    closeWindow(callback: any): void {
        let radWindow: Telerik.Web.UI.RadWindow = this.getRadWindow();
        if (radWindow != null) radWindow.close(callback);
    }

    loadFolders(selectedFascicleFolderId: string, currentFascicleId: string, destinationFascicleId: any): void {
        let url: string = `../Fasc/FascMoveItems.aspx?Type=Fasc&idFascicle=${currentFascicleId}&ItemsType=DocumentType&IdFascicleFolder=${selectedFascicleFolderId}&DestinationFascicleId=${destinationFascicleId}&MoveToFascicle=${true}`;
        location.href = url;
    }
}
export = FascRisultati;