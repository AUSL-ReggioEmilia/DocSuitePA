/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/ServiceBus/ServiceBusTopicService", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceBusTopicService, ExceptionDTO) {
    var EventPECSummaryErrorBase = /** @class */ (function () {
        function EventPECSummaryErrorBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        EventPECSummaryErrorBase.prototype.initialize = function () {
            this.service = new ServiceBusTopicService(this._serviceConfiguration);
        };
        EventPECSummaryErrorBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        EventPECSummaryErrorBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        EventPECSummaryErrorBase.SERVICEBUSTOPIC_TYPE_NAME = "ServiceBusTopicMessage";
        return EventPECSummaryErrorBase;
    }());
    return EventPECSummaryErrorBase;
});
//# sourceMappingURL=EventPECSummaryErrorBase.js.map