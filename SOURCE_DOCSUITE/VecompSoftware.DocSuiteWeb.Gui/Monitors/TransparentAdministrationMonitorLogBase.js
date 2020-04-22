/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Monitors/TransparentAdministrationMonitorLogService", "App/DTOs/ExceptionDTO"], function (require, exports, TransparentAdministrationMonitorLogService, ExceptionDTO) {
    var TransparentAdministrationMonitorLogBase = /** @class */ (function () {
        function TransparentAdministrationMonitorLogBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        TransparentAdministrationMonitorLogBase.prototype.initialize = function () {
            this.service = new TransparentAdministrationMonitorLogService(this._serviceConfiguration);
        };
        TransparentAdministrationMonitorLogBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TransparentAdministrationMonitorLogBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME = "TransparentAdministrationMonitorLog";
        return TransparentAdministrationMonitorLogBase;
    }());
    return TransparentAdministrationMonitorLogBase;
});
//# sourceMappingURL=TransparentAdministrationMonitorLogBase.js.map