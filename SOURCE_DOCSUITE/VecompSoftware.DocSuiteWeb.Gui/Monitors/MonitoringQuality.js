var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Monitors/MonitoringQualityBase", "App/DTOs/MonitoringQualitySearchFilterDTO", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, MonitoringQualityBase, MonitoringQualitySearchFilterDTO, ServiceConfigurationHelper) {
    var MonitoringQuality = /** @class */ (function (_super) {
        __extends(MonitoringQuality, _super);
        function MonitoringQuality(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MonitoringQualityBase.MonitoringQuality_TYPE_NAME)) || this;
            _this.btnSearch_onClick = function (sender, args) {
                _this.loadResults(0);
            };
            _this.btnClean_onClick = function (sender, args) {
                _this.cleanSearchFilters();
            };
            _this.cleanSearchFilters = function () {
                _this._dpStartDateFrom.clear();
                _this._dpEndDateFrom.clear();
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        MonitoringQuality.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            //region [ Grid Configuration Initialization ]
            this._monitoringQualityGrid = $find(this.monitoringQualityGridId);
            this._masterTableView = this._monitoringQualityGrid.get_masterTableView();
            this._monitoringQualityGrid.get_detailTables();
            this._masterTableView.set_currentPageIndex(0);
            this.bindLoaded();
            //endregion
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.monitoringQualityGridId)).bind(MonitoringQuality.LOADED_EVENT, function () {
                _this.loadMonitoringQualityGrid();
            });
            $("#".concat(this.monitoringQualityGridId)).bind(MonitoringQuality.PAGE_CHANGED_EVENT, function (args) {
                if (!jQuery.isEmptyObject(_this._monitoringQualityGrid)) {
                    _this.pageChange();
                }
            });
            this._dpStartDateFrom = $find(this.dpStartDateFromId);
            this._dpEndDateFrom = $find(this.dpEndDateFromId);
            this._btnSearch = $find(this.btnSearchId);
            //this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            //this._btnClean.add_clicking(this.btnClean_onClick);
        };
        //region [ Grid Configuration Methods ]
        MonitoringQuality.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            $("#".concat(this.pageId)).triggerHandler(MonitoringQuality.PAGE_CHANGED_EVENT);
        };
        MonitoringQuality.prototype.onGridDataBound = function () {
            var row = this._masterTableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
        };
        MonitoringQuality.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(MonitoringQuality.LOADED_EVENT);
        };
        MonitoringQuality.prototype.setDataSource = function (results) {
            this._masterTableView.set_dataSource(results);
            this._masterTableView.dataBind();
        };
        MonitoringQuality.prototype.setItemCount = function (count) {
            this._masterTableView.set_virtualItemCount(count);
            this._masterTableView.dataBind();
        };
        //endregion
        MonitoringQuality.prototype.loadMonitoringQualityGrid = function () {
            if (!jQuery.isEmptyObject(this._monitoringQualityGrid)) {
                this.loadResults(0);
            }
        };
        MonitoringQuality.prototype.pageChange = function () {
            this._loadingPanel.show(this.monitoringQualityGridId);
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_currentPageIndex();
            this.loadResults(skip);
        };
        MonitoringQuality.prototype.loadResults = function (skip) {
            var _this = this;
            this._loadingPanel.show(this.monitoringQualityGridId);
            var startDateFromFilter = "";
            if (this._dpStartDateFrom.get_selectedDate()) {
                startDateFromFilter = this._dpStartDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
            }
            var endDateFromFilter = "";
            if (this._dpEndDateFrom.get_selectedDate()) {
                endDateFromFilter = this._dpEndDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
            }
            var searchDTO = new MonitoringQualitySearchFilterDTO();
            searchDTO.dateFrom = startDateFromFilter;
            searchDTO.dateTo = endDateFromFilter;
            var top = skip + this._masterTableView.get_pageSize();
            if (searchDTO.dateFrom !== "" || searchDTO.dateTo !== "")
                this.service.getMonitoringQualitySummary(searchDTO, function (data) {
                    if (!data)
                        return;
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
                                WithoutDocument: 0
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
                    _this._detailTables = _this._monitoringQualityGrid.get_detailTables();
                    _this._masterTableView.set_dataSource(resultGrouped);
                    _this._masterTableView.dataBind();
                    _this._loadingPanel.hide(_this.monitoringQualityGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.monitoringQualityGridId);
                    $("#".concat(_this.monitoringQualityGridId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
        };
        MonitoringQuality.LOADED_EVENT = "onLoaded";
        MonitoringQuality.PAGE_CHANGED_EVENT = "onPageChanged";
        return MonitoringQuality;
    }(MonitoringQualityBase));
    return MonitoringQuality;
});
//# sourceMappingURL=MonitoringQuality.js.map