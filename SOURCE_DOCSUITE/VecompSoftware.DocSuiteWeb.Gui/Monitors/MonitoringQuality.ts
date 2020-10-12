import MonitoringQualityBase = require('Monitors/MonitoringQualityBase');
import MonitoringQualitySearchFilterDTO = require('App/DTOs/MonitoringQualitySearchFilterDTO');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MonitoringQualityGridViewModel = require('App/ViewModels/Monitors/MonitoringQualityGridViewModel');

class MonitoringQuality extends MonitoringQualityBase {

    //region [ Grid Configuration Properties ]
    pageId: string;
    public static LOADED_EVENT: string = "onLoaded";
    public static PAGE_CHANGED_EVENT: string = "onPageChanged";
    private _monitoringQualityGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;
    private _detailTables: Telerik.Web.UI.GridTableView[];
    //endregion

    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    dpStartDateFromId: string;
    dpEndDateFromId: string;
    btnSearchId: string;
    btnCleanId: string;
    monitoringQualityGridId: string;
    gridResult: MonitoringQualityGridViewModel[];

    private _dpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MonitoringQualityBase.MonitoringQuality_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();

        //region [ Grid Configuration Initialization ]
        this._monitoringQualityGrid = <Telerik.Web.UI.RadGrid>$find(this.monitoringQualityGridId);
        this._masterTableView = this._monitoringQualityGrid.get_masterTableView();
        this._monitoringQualityGrid.get_detailTables();
        this._masterTableView.set_currentPageIndex(0);
        this.bindLoaded();
        //endregion

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        $("#".concat(this.monitoringQualityGridId)).bind(MonitoringQuality.LOADED_EVENT, () => {
            this.loadMonitoringQualityGrid();
        });

        $("#".concat(this.monitoringQualityGridId)).bind(MonitoringQuality.PAGE_CHANGED_EVENT, (args) => {
            if (!jQuery.isEmptyObject(this._monitoringQualityGrid)) {
                this.pageChange();
            }
        });

        this._dpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpStartDateFromId);
        this._dpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpEndDateFromId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        //this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        //this._btnClean.add_clicking(this.btnClean_onClick);
    }

    //region [ Grid Configuration Methods ]
    onPageChanged() {
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
        $("#".concat(this.pageId)).triggerHandler(MonitoringQuality.PAGE_CHANGED_EVENT);
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
        $("#".concat(this.pageId)).triggerHandler(MonitoringQuality.LOADED_EVENT);
    }

    setDataSource(results: MonitoringQualityGridViewModel[]) {
        this._masterTableView.set_dataSource(results);
        this._masterTableView.dataBind();
    }

    setItemCount(count: number) {
        this._masterTableView.set_virtualItemCount(count);
        this._masterTableView.dataBind();
    }
    //endregion

    private loadMonitoringQualityGrid() {
        if (!jQuery.isEmptyObject(this._monitoringQualityGrid)) {
            this.loadResults(0);
        }
    }

    private pageChange() {
        this._loadingPanel.show(this.monitoringQualityGridId);
        let skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_currentPageIndex();
        this.loadResults(skip);
    }

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.loadResults(0);
    }

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.cleanSearchFilters();
    }

    cleanSearchFilters = () => {
        this._dpStartDateFrom.clear();
        this._dpEndDateFrom.clear();
    }

    loadResults(skip: number) {
        this._loadingPanel.show(this.monitoringQualityGridId);
        let startDateFromFilter: string = "";
        if (this._dpStartDateFrom.get_selectedDate()) {
            startDateFromFilter = this._dpStartDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
        }
        let endDateFromFilter: string = "";
        if (this._dpEndDateFrom.get_selectedDate()) {
            endDateFromFilter = this._dpEndDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
        }
        let searchDTO: MonitoringQualitySearchFilterDTO = new MonitoringQualitySearchFilterDTO();
        searchDTO.dateFrom = startDateFromFilter;
        searchDTO.dateTo = endDateFromFilter;
        let top: number = skip + this._masterTableView.get_pageSize();
        if (searchDTO.dateFrom !== "" || searchDTO.dateTo !== "")
            this.service.getMonitoringQualitySummary(searchDTO,
                (data: MonitoringQualityGridViewModel[]) => {
                    if (!data) return;

                    //region [ Build grid data ]
                    var resultGrouped = [];
                    data.reduce(function (res, value) {
                        if (!res[value.IdDocumentSeries]) {
                            res[value.IdDocumentSeries] = {
                                IdDocumentSeries: value.IdDocumentSeries,
                                DocumentSeries: value.DocumentSeries,
                                Published: 0,
                                FromResolution: 0,
                                FromProtocol: 0,
                                WithoutLink: 0,
                                WithoutDocument:0
                            };
                            resultGrouped.push(res[value.IdDocumentSeries]);
                        }
                        res[value.IdDocumentSeries].Published += value.Published;
                        res[value.IdDocumentSeries].FromResolution += value.FromResolution;
                        res[value.IdDocumentSeries].FromProtocol += value.FromProtocol;
                        res[value.IdDocumentSeries].WithoutLink += value.WithoutLink;
                        res[value.IdDocumentSeries].WithoutDocument += value.WithoutDocument;
                        return res;
                    }, {});
                    //endregion [ Build grid data ]

                    this._detailTables = this._monitoringQualityGrid.get_detailTables();
                    this._masterTableView.set_dataSource(resultGrouped);
                    this._masterTableView.dataBind();
                    this._loadingPanel.hide(this.monitoringQualityGridId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.monitoringQualityGridId);
                    $("#".concat(this.monitoringQualityGridId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                });
    }
}

export = MonitoringQuality;