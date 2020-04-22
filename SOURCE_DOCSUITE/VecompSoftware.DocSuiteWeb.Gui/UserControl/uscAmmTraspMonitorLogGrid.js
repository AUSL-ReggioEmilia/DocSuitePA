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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "Monitors/TransparentAdministrationMonitorLogBase"], function (require, exports, ServiceConfigurationHelper, TransparentAdministrationMonitorLogBase) {
    var uscAmmTraspMonitorLogGrid = /** @class */ (function (_super) {
        __extends(uscAmmTraspMonitorLogGrid, _super);
        function uscAmmTraspMonitorLogGrid(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME)) || this;
            $(document).ready(function () {
            });
            return _this;
        }
        uscAmmTraspMonitorLogGrid.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._uscAmmTraspMonitorLogGrid = $find(this.uscAmmTraspMonitorLogId);
            this._masterTableView = this._uscAmmTraspMonitorLogGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this.bindLoaded();
        };
        uscAmmTraspMonitorLogGrid.prototype.onPageChanged = function () {
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            $("#".concat(this.pageId)).triggerHandler(uscAmmTraspMonitorLogGrid.PAGE_CHANGED_EVENT);
        };
        uscAmmTraspMonitorLogGrid.prototype.onGridDataBound = function () {
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
        uscAmmTraspMonitorLogGrid.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscAmmTraspMonitorLogGrid.LOADED_EVENT);
        };
        uscAmmTraspMonitorLogGrid.prototype.setDataSource = function (results) {
            this._masterTableView.set_dataSource(results);
            this._masterTableView.dataBind();
        };
        uscAmmTraspMonitorLogGrid.prototype.setItemCount = function (count) {
            this._masterTableView.set_virtualItemCount(count);
            this._masterTableView.dataBind();
        };
        uscAmmTraspMonitorLogGrid.prototype.getGridPageSize = function () {
            return this._masterTableView.get_pageSize();
        };
        uscAmmTraspMonitorLogGrid.prototype.getGridCurrentPageIndex = function () {
            return this._masterTableView.get_currentPageIndex();
        };
        uscAmmTraspMonitorLogGrid.LOADED_EVENT = "onLoaded";
        uscAmmTraspMonitorLogGrid.PAGE_CHANGED_EVENT = "onPageChanged";
        return uscAmmTraspMonitorLogGrid;
    }(TransparentAdministrationMonitorLogBase));
    return uscAmmTraspMonitorLogGrid;
});
//# sourceMappingURL=uscAmmTraspMonitorLogGrid.js.map