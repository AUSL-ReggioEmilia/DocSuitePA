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
define(["require", "exports", "Monitors/MonitoringSeriesSectionBase", "App/DTOs/MonitoringSeriesSectionSearchFilterDTO", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, MonitoringSeriesSectionBase, MonitoringSeriesSectionSearchFilterDTO, ServiceConfigurationHelper) {
    var MonitoringSeriesSection = /** @class */ (function (_super) {
        __extends(MonitoringSeriesSection, _super);
        function MonitoringSeriesSection(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, MonitoringSeriesSectionBase.MonitoringSeriesSection_TYPE_NAME)) || this;
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
        MonitoringSeriesSection.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            //region [ Grid Configuration Initialization ]
            this._monitoringSeriesSectionGrid = $find(this.monitoringSeriesSectionGridId);
            this._masterTableView = this._monitoringSeriesSectionGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this.bindLoaded();
            //endregion
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.monitoringSeriesSectionGridId)).bind(MonitoringSeriesSection.LOADED_EVENT, function () {
                _this.loadMonitoringSeriesSectionGrid();
            });
            $("#".concat(this.monitoringSeriesSectionGridId)).bind(MonitoringSeriesSection.PAGE_CHANGED_EVENT, function (args) {
                if (!jQuery.isEmptyObject(_this._monitoringSeriesSectionGrid)) {
                    _this.pageChange();
                }
            });
            this._dpStartDateFrom = $find(this.dpStartDateFromId);
            this._dpEndDateFrom = $find(this.dpEndDateFromId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
        };
        //region [ Grid Configuration Methods ]
        MonitoringSeriesSection.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            $("#".concat(this.pageId)).triggerHandler(MonitoringSeriesSection.PAGE_CHANGED_EVENT);
        };
        MonitoringSeriesSection.prototype.onGridDataBound = function () {
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
        MonitoringSeriesSection.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(MonitoringSeriesSection.LOADED_EVENT);
        };
        MonitoringSeriesSection.prototype.setDataSource = function (results) {
            this._masterTableView.set_dataSource(results);
            this._masterTableView.dataBind();
        };
        MonitoringSeriesSection.prototype.setItemCount = function (count) {
            this._masterTableView.set_virtualItemCount(count);
            this._masterTableView.dataBind();
        };
        //endregion
        MonitoringSeriesSection.prototype.loadMonitoringSeriesSectionGrid = function () {
            if (!jQuery.isEmptyObject(this._monitoringSeriesSectionGrid)) {
                this.loadResults(0);
            }
        };
        MonitoringSeriesSection.prototype.pageChange = function () {
            this._loadingPanel.show(this.monitoringSeriesSectionGridId);
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_currentPageIndex();
            this.loadResults(skip);
        };
        MonitoringSeriesSection.prototype.loadResults = function (skip) {
            var _this = this;
            this._loadingPanel.show(this.monitoringSeriesSectionGridId);
            var startDateFromFilter = "";
            if (this._dpStartDateFrom.get_selectedDate()) {
                startDateFromFilter = this._dpStartDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
            }
            var endDateFromFilter = "";
            if (this._dpEndDateFrom.get_selectedDate()) {
                endDateFromFilter = this._dpEndDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
            }
            var searchDTO = new MonitoringSeriesSectionSearchFilterDTO();
            searchDTO.dateFrom = startDateFromFilter;
            searchDTO.dateTo = endDateFromFilter;
            var top = skip + this._masterTableView.get_pageSize();
            if (searchDTO.dateFrom !== "" || searchDTO.dateTo !== "")
                this.service.getMonitoringSeriesSection(searchDTO, function (data) {
                    if (!data)
                        return;
                    for (var i = 0; i < data.length; i++)
                        data[i].LastUpdated = data[i].LastUpdated === "Invalid date" ? "" : data[i].LastUpdated;
                    _this._masterTableView.set_dataSource(data);
                    _this._masterTableView.dataBind();
                    _this._loadingPanel.hide(_this.monitoringSeriesSectionGridId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.monitoringSeriesSectionGridId);
                    $("#".concat(_this.monitoringSeriesSectionGridId)).hide();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
        };
        MonitoringSeriesSection.LOADED_EVENT = "onLoaded";
        MonitoringSeriesSection.PAGE_CHANGED_EVENT = "onPageChanged";
        return MonitoringSeriesSection;
    }(MonitoringSeriesSectionBase));
    return MonitoringSeriesSection;
});
//# sourceMappingURL=MonitoringSeriesSection.js.map