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
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleLogService", "Fasc/FascBase"], function (require, exports, ServiceConfigurationHelper, FascicleLogService, FascBase) {
    var FascicleLog = /** @class */ (function (_super) {
        __extends(FascicleLog, _super);
        /**
       * Costruttore
       */
        function FascicleLog(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        FascicleLog.prototype.onPageChanged = function () {
            this._loadingPanel.show(this.fascicleLogGridId);
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            this.getLogs(skip);
        };
        FascicleLog.prototype.onGridDataBound = function () {
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
        FascicleLog.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            var fascicleLogConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_LOG_TYPE_NAME);
            this._fascicleLogService = new FascicleLogService(fascicleLogConfiguration);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.fascicleLogGridId);
            this._fascicleLogGrid = $find(this.fascicleLogGridId);
            this._masterTableView = this._fascicleLogGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this._masterTableView.set_virtualItemCount(0);
            this._titleContainer = $("#".concat(this.titleContainerId));
            if (this._titleContainer) {
                var titleLabel = this._titleContainer.children("span");
                titleLabel.html("Fascicle - Log ".concat(this.fascicleTitle));
            }
            this.getLogs(0);
        };
        FascicleLog.prototype.getLogs = function (skip) {
            var _this = this;
            var top = this._masterTableView.get_pageSize();
            this._fascicleLogService.getFascicleLogs(this.fascicleId, skip, top, function (response) {
                if (!response)
                    return;
                _this._masterTableView.set_dataSource(response.value);
                _this._masterTableView.set_virtualItemCount(response.count);
                _this._masterTableView.dataBind();
                _this._loadingPanel.hide(_this.fascicleLogGridId);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._loadingPanel.hide(_this.fascicleLogGridId);
            });
        };
        return FascicleLog;
    }(FascBase));
    return FascicleLog;
});
//# sourceMappingURL=FascicleLog.js.map