define(["require", "exports", "App/Services/UDS/UDSLogService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, UDSLogService, ServiceConfigurationHelper, ExceptionDTO) {
    var UDSLog = /** @class */ (function () {
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function UDSLog(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        UDSLog.prototype.onPageChanged = function () {
            this._loadingPanel.show(this.UDSLogGridId);
            var skip = this._masterTableView.get_currentPageIndex() * this._masterTableView.get_pageSize();
            if (!this.HasAdminRight && this.UDSLogShowOnlyCurrentIfNotAdmin) {
                this.getOnlyMyLogs(0);
            }
            else {
                this.getLogs(skip);
            }
        };
        UDSLog.prototype.onGridDataBound = function () {
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
        UDSLog.prototype.initialize = function () {
            var UDSLogConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSLog");
            this._UDSLogService = new UDSLogService(UDSLogConfiguration);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.UDSLogGridId);
            this._udsLogGrid = $find(this.UDSLogGridId);
            this._masterTableView = this._udsLogGrid.get_masterTableView();
            this._masterTableView.set_currentPageIndex(0);
            this._masterTableView.set_virtualItemCount(0);
            this._titleContainer = $("#".concat(this.titleContainerId));
            if (this._titleContainer) {
                var titleLabel = this._titleContainer.children("span");
                titleLabel.html("Archivi - Log ");
            }
            if (!this.HasAdminRight && this.UDSLogShowOnlyCurrentIfNotAdmin) {
                this.getOnlyMyLogs(0);
            }
            else {
                this.getLogs(0);
            }
        };
        UDSLog.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        UDSLog.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        UDSLog.prototype.getLogs = function (skip) {
            var _this = this;
            var top = this._masterTableView.get_pageSize();
            this._UDSLogService.getUDSLogs(this.UDSId, skip, top, function (response) {
                if (!response)
                    return;
                _this._masterTableView.set_dataSource(response.value);
                _this._masterTableView.set_virtualItemCount(response.count);
                _this._masterTableView.dataBind();
                _this._loadingPanel.hide(_this.UDSLogGridId);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._loadingPanel.hide(_this.UDSLogGridId);
            });
        };
        UDSLog.prototype.getOnlyMyLogs = function (skip) {
            var _this = this;
            var top = this._masterTableView.get_pageSize();
            this._UDSLogService.getMyUDSLog(this.UDSId, skip, top, function (response) {
                if (!response)
                    return;
                _this._masterTableView.set_dataSource(response.value);
                _this._masterTableView.set_virtualItemCount(response.count);
                _this._masterTableView.dataBind();
                _this._loadingPanel.hide(_this.UDSLogGridId);
            }, function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this._loadingPanel.hide(_this.UDSLogGridId);
            });
        };
        return UDSLog;
    }());
    return UDSLog;
});
//# sourceMappingURL=UDSLog.js.map