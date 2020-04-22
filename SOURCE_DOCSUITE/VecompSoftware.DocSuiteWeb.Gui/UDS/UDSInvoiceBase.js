/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/UDS/UDSRepositoryService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, UDSRepositoryService, ServiceConfigurationHelper, ExceptionDTO) {
    var UDSInvoiceBase = /** @class */ (function () {
        function UDSInvoiceBase(serviceConfiguration) {
            this._serviceConfigurations = serviceConfiguration;
        }
        UDSInvoiceBase.prototype.initialize = function () {
            var UDSRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, UDSInvoiceBase.UDSRepositoryInvoice_TYPE_NAME);
            this.udsRepositoryService = new UDSRepositoryService(UDSRepositoryConfiguration);
        };
        UDSInvoiceBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        UDSInvoiceBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        UDSInvoiceBase.UDSRepositoryInvoice_TYPE_NAME = "UDSRepository";
        return UDSInvoiceBase;
    }());
    return UDSInvoiceBase;
});
//# sourceMappingURL=UDSInvoiceBase.js.map