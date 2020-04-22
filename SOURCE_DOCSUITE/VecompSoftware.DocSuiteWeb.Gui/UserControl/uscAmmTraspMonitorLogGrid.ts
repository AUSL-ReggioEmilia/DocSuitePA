import TransparentAdministrationMonitorLogGridViewModel = require('App/ViewModels/Monitors/TransparentAdministrationMonitorLogGridViewModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TransparentAdministrationMonitorLogBase = require('Monitors/TransparentAdministrationMonitorLogBase');
import TransparentAdministrationMonitorLogSearchFilterDTO = require('App/DTOs/TransparentAdministrationMonitorLogSearchFilterDTO');

class uscAmmTraspMonitorLogGrid extends TransparentAdministrationMonitorLogBase {
    uscAmmTraspMonitorLogId: string;
    authorizedTransparentAdministrationMonitorLogs: TransparentAdministrationMonitorLogGridViewModel[];
    pageId: string;
    documentSeriesItemId: number;
    
    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";

    private _uscAmmTraspMonitorLogGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME));
        $(document).ready(() => {
        });
    }

    initialize() {
        super.initialize();
        this._uscAmmTraspMonitorLogGrid = <Telerik.Web.UI.RadGrid>$find(this.uscAmmTraspMonitorLogId);
        this._masterTableView = this._uscAmmTraspMonitorLogGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this.bindLoaded();
    }

    onPageChanged() {
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        $("#".concat(this.pageId)).triggerHandler(uscAmmTraspMonitorLogGrid.PAGE_CHANGED_EVENT);
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

    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscAmmTraspMonitorLogGrid.LOADED_EVENT);
    }

    setDataSource(results: TransparentAdministrationMonitorLogGridViewModel[]) {
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
}

export = uscAmmTraspMonitorLogGrid;