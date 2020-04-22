/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/PECMails/PECMailBoxService", "App/Services/PECMails/PECMailBoxConfigurationService", "App/Services/Commons/LocationService", "App/Services/JeepServiceHosts/JeepServiceHostService", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, PECMailBoxService, PECMailBoxConfigurationService, LocationService, JeepServiceHostService, ExceptionDTO) {
    var TbltPECMailBoxBase = /** @class */ (function () {
        function TbltPECMailBoxBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        TbltPECMailBoxBase.prototype.initialize = function () {
            this.pecMailBoxService = new PECMailBoxService(this._serviceConfiguration);
        };
        TbltPECMailBoxBase.prototype.initializeServices = function (serviceConfigurations) {
            this.pecMailBoxConfigurationService =
                new PECMailBoxConfigurationService(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBoxConfiguraton_TYPE_NAME));
            this.locationService =
                new LocationService(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.Location_TYPE_NAME));
            this.jeepServiceHostService =
                new JeepServiceHostService(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.JeepServiceHost_TYPE_NAME));
        };
        TbltPECMailBoxBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltPECMailBoxBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltPECMailBoxBase.PECMailBox_TYPE_NAME = "PECMailBox";
        TbltPECMailBoxBase.PECMailBoxConfiguraton_TYPE_NAME = "PECMailBoxConfiguration";
        TbltPECMailBoxBase.Location_TYPE_NAME = "Location";
        TbltPECMailBoxBase.JeepServiceHost_TYPE_NAME = "JeepServiceHost";
        return TbltPECMailBoxBase;
    }());
    return TbltPECMailBoxBase;
});
//# sourceMappingURL=TbltPECMailBoxBase.js.map