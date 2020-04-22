/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Monitors/MonitoringQualityService", "App/DTOs/ExceptionDTO"], function (require, exports, MonitoringQualityService, ExceptionDTO) {
    var MonitoringQualityBase = /** @class */ (function () {
        function MonitoringQualityBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        MonitoringQualityBase.prototype.initialize = function () {
            this.service = new MonitoringQualityService(this._serviceConfiguration);
        };
        MonitoringQualityBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        MonitoringQualityBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        MonitoringQualityBase.MonitoringQuality_TYPE_NAME = "DocumentSeries";
        return MonitoringQualityBase;
    }());
    return MonitoringQualityBase;
});
//# sourceMappingURL=MonitoringQualityBase.js.map