/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Monitors/MonitoringSeriesSectionService", "App/DTOs/ExceptionDTO"], function (require, exports, MonitoringSeriesSectionService, ExceptionDTO) {
    var MonitoringSeriesSectionBase = /** @class */ (function () {
        function MonitoringSeriesSectionBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        MonitoringSeriesSectionBase.prototype.initialize = function () {
            this.service = new MonitoringSeriesSectionService(this._serviceConfiguration);
        };
        MonitoringSeriesSectionBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        MonitoringSeriesSectionBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        MonitoringSeriesSectionBase.MonitoringSeriesSection_TYPE_NAME = "DocumentSeries";
        return MonitoringSeriesSectionBase;
    }());
    return MonitoringSeriesSectionBase;
});
//# sourceMappingURL=MonitoringSeriesSectionBase.js.map