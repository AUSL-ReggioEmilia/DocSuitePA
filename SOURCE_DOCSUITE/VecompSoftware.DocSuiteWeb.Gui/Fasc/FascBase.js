/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Fascicles/FascicleService", "App/DTOs/ExceptionDTO"], function (require, exports, FascicleService, ExceptionDTO) {
    var FascBase = /** @class */ (function () {
        function FascBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        FascBase.prototype.initialize = function () {
            this.service = new FascicleService(this._serviceConfiguration);
        };
        FascBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        FascBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        FascBase.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        FascBase.FASCICLE_TYPE_NAME = "Fascicle";
        FascBase.FASCICLEROLE_TYPE_NAME = 'FascicleRole';
        FascBase.DOCUMENT_UNIT_TYPE_NAME = "DocumentUnit";
        FascBase.PROTOCOL_TYPE_NAME = "Protocol";
        FascBase.RESOLUTION_TYPE_NAME = "Resolution";
        FascBase.UDSREPOSITORY_TYPE_NAME = "UDSRepository";
        FascBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME = "FascicleDocumentUnit";
        FascBase.FASCICLE_LINK_TYPE_NAME = "FascicleLink";
        FascBase.FASCICLE_LOG_TYPE_NAME = "FascicleLog";
        FascBase.FASCICLE_DOCUMENT_TYPE_NAME = "FascicleDocument";
        FascBase.DOMAIN_TYPE_NAME = "DomainUserModel";
        FascBase.FASCICLE_CATEGORY_FASCICLE = "CategoryFascicle";
        FascBase.FASCICLEFOLDER_TYPE_NAME = "FascicleFolder";
        return FascBase;
    }());
    return FascBase;
});
//# sourceMappingURL=FascBase.js.map