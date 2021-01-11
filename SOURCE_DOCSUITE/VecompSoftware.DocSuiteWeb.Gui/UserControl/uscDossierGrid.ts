import DossierGridViewModel = require('App/ViewModels/Dossiers/DossierGridViewModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierBase = require('Dossiers/DossierBase');
import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import AjaxModel = require('App/Models/AjaxModel');

class uscDossierGrid extends DossierBase {
    dossierGridId: string;
    authorizedDossiers: DossierGridViewModel[];
    pageId: string;
    isWindowPopupEnable: boolean;

    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";

    private _dossierGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
 
      /**
    * Costruttore
    * @param webApiConfiguration
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        $(document).ready(() => {
        });
    }

    /**
    * Inizializzazione
    */
    initialize() {
        super.initialize();
        this._dossierGrid = <Telerik.Web.UI.RadGrid>$find(this.dossierGridId);
        this._masterTableView = this._dossierGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this.bindLoaded();
    }


    /**
    *------------------------- Events -----------------------------
    */

    /**
     * Evento scatenato al click del pulsante di inserimento
     * @param sender
     * @param args
     */
    onPageChanged() {
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        $("#".concat(this.pageId)).triggerHandler(uscDossierGrid.PAGE_CHANGED_EVENT);
    }

    onGridDataBound() {
        let row = this._masterTableView.get_dataItems();
        for (let i = 0; i < row.length; i++) {
            if (i % 2) {
                row[i].addCssClass("Chiaro");
            }
            else {
                row[i].addCssClass("Scuro");
            }
        }
    }


     /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Inizializza lo user control del sommario di fascicolo
     */
    
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscDossierGrid.LOADED_EVENT);
    }

    setDataSource(results: DossierGridViewModel[]) {
        this._masterTableView.set_dataSource(results);
        this._masterTableView.dataBind();
    }

    setItemCount(count: number) {
        this._masterTableView.set_virtualItemCount(count);
        this._masterTableView.dataBind(); 
    }

    getGridPageSize(): number {
        return this._masterTableView.get_pageSize();
    }

    getGridCurrentPageIndex(): number {
        return this._masterTableView.get_currentPageIndex();
    }

    closeResultWindow(dossierId: string) {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(`${dossierId}`);
    }
}

export = uscDossierGrid;