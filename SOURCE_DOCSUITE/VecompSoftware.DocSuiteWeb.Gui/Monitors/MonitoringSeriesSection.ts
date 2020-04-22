import MonitoringSeriesSectionBase = require('Monitors/MonitoringSeriesSectionBase');
import MonitoringSeriesSectionSearchFilterDTO = require('App/DTOs/MonitoringSeriesSectionSearchFilterDTO');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MonitoringSeriesSectionGridViewModel = require('App/ViewModels/Monitors/MonitoringSeriesSectionGridViewModel');

class MonitoringSeriesSection extends MonitoringSeriesSectionBase {

    //region [ Grid Configuration Properties ]
    pageId: string;
    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";
    private _monitoringSeriesSectionGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    //endregion

    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    dpStartDateFromId: string;
    dpEndDateFromId: string;
    btnSearchId: string;
    btnCleanId: string;
    monitoringSeriesSectionGridId: string;
    gridResult: MonitoringSeriesSectionGridViewModel[];

    private _dpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MonitoringSeriesSectionBase.MonitoringSeriesSection_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();

        //region [ Grid Configuration Initialization ]
        this._monitoringSeriesSectionGrid = <Telerik.Web.UI.RadGrid>$find(this.monitoringSeriesSectionGridId);
        this._masterTableView = this._monitoringSeriesSectionGrid.get_masterTableView();
        this._masterTableView.set_currentPageIndex(0);
        this.bindLoaded();
        //endregion

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        $("#".concat(this.monitoringSeriesSectionGridId)).bind(MonitoringSeriesSection.LOADED_EVENT, () => {
            this.loadMonitoringSeriesSectionGrid();
        });

        $("#".concat(this.monitoringSeriesSectionGridId)).bind(MonitoringSeriesSection.PAGE_CHANGED_EVENT, (args) => {
            if (!jQuery.isEmptyObject(this._monitoringSeriesSectionGrid)) {
                this.pageChange();
            }
        });

        this._dpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpStartDateFromId);
        this._dpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpEndDateFromId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
    }

    //region [ Grid Configuration Methods ]
    onPageChanged() {
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        $("#".concat(this.pageId)).triggerHandler(MonitoringSeriesSection.PAGE_CHANGED_EVENT);
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
        $("#".concat(this.pageId)).triggerHandler(MonitoringSeriesSection.LOADED_EVENT);
    }

    setDataSource(results: MonitoringSeriesSectionGridViewModel[]) {
        this._masterTableView.set_dataSource(results);
        this._masterTableView.dataBind();
    }

    setItemCount(count: number) {
        this._masterTableView.set_virtualItemCount(count);
        this._masterTableView.dataBind();
    }
    //endregion

    private loadMonitoringSeriesSectionGrid() {
        if (!jQuery.isEmptyObject(this._monitoringSeriesSectionGrid)) {
            this.loadResults(0);
        }
    }

    private pageChange() {
        this._loadingPanel.show(this.monitoringSeriesSectionGridId);
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_currentPageIndex();
        this.loadResults(skip);
    }

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.loadResults(0);
    }

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.cleanSearchFilters();
    }

    cleanSearchFilters = () => {
        this._dpStartDateFrom.clear();
        this._dpEndDateFrom.clear();
    }

    loadResults(skip: number) {
        this._loadingPanel.show(this.monitoringSeriesSectionGridId);
        let startDateFromFilter: string = "";
        if (this._dpStartDateFrom.get_selectedDate()) {
            startDateFromFilter = this._dpStartDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
        }
        let endDateFromFilter: string = "";
        if (this._dpEndDateFrom.get_selectedDate()) {
            endDateFromFilter = this._dpEndDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
        }
        let searchDTO: MonitoringSeriesSectionSearchFilterDTO = new MonitoringSeriesSectionSearchFilterDTO();
        searchDTO.dateFrom = startDateFromFilter;
        searchDTO.dateTo = endDateFromFilter;
        let top: number = skip + this._masterTableView.get_pageSize();
        if (searchDTO.dateFrom !== "" || searchDTO.dateTo !== "")
            this.service.getMonitoringSeriesSection(searchDTO,
                (data) => {
                    if (!data) return;
                    for (let i = 0; i < data.length; i++)
                        data[i].LastUpdated = data[i].LastUpdated === "Invalid date" ? "" : data[i].LastUpdated;
                    this._masterTableView.set_dataSource(data);
                    this._masterTableView.dataBind();
                    this._loadingPanel.hide(this.monitoringSeriesSectionGridId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.monitoringSeriesSectionGridId);
                    $("#".concat(this.monitoringSeriesSectionGridId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                });
    }
}

export = MonitoringSeriesSection;