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
define(["require", "exports", "Dossiers/DossierBase", "App/Services/Dossiers/DossierLogService", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, DossierBase, DossierLogService, ServiceConfigurationHelper) {
    var DossierLog = /** @class */ (function (_super) {
        __extends(DossierLog, _super);
        /**
        * Costruttore
        */
        function DossierLog(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        DossierLog.prototype.onPageChanged = function () {
            this._loadingPanel.show(this.dossierLogGridId);
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            this.getLogs(skip);
        };
        DossierLog.prototype.onGridDataBound = function () {
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
        DossierLog.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            var dossierLogConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERLOG_TYPE_NAME);
            this._dossierLogService = new DossierLogService(dossierLogConfiguration);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.dossierLogGridId);
            this._dossierLogGrid = $find(this.dossierLogGridId);
            this._masterTableView = this._dossierLogGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this._masterTableView.set_virtualItemCount(0);
            this._titleContainer = $("#".concat(this.titleContainerId));
            if (this._titleContainer) {
                var titleLabel = this._titleContainer.children("span");
                titleLabel.html("Dossier - Log ".concat(this.dossierTitle));
            }
            this.getLogs(0);
        };
        DossierLog.prototype.getLogs = function (skip) {
            var _this = this;
            var top = this._masterTableView.get_pageSize();
            this._dossierLogService.getDossierLogs(this.dossierId, skip, top, function (response) {
                if (!response)
                    return;
                _this._masterTableView.set_dataSource(response.value);
                _this._masterTableView.set_virtualItemCount(response.count);
                _this._masterTableView.dataBind();
                _this._loadingPanel.hide(_this.dossierLogGridId);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._loadingPanel.hide(_this.dossierLogGridId);
            });
        };
        return DossierLog;
    }(DossierBase));
    return DossierLog;
});
//# sourceMappingURL=DossierLog.js.map