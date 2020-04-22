/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Fascicles/FascicleService", "App/DTOs/ExceptionDTO"], function (require, exports, FascicleService, ExceptionDTO) {
    var PECBase = /** @class */ (function () {
        function PECBase(serviceConfiguration) {
            this._serviceConfiguration = serviceConfiguration;
        }
        PECBase.prototype.initialize = function () {
            this.service = new FascicleService(this._serviceConfiguration);
        };
        PECBase.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        PECBase.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        PECBase.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        PECBase.FASCICLE_TYPE_NAME = "Fascicle";
        PECBase.FASCICLEROLE_TYPE_NAME = 'FascicleRole';
        PECBase.DOCUMENT_UNIT_TYPE_NAME = "DocumentUnit";
        PECBase.PROTOCOL_TYPE_NAME = "Protocol";
        PECBase.RESOLUTION_TYPE_NAME = "Resolution";
        PECBase.UDSREPOSITORY_TYPE_NAME = "UDSRepository";
        PECBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME = "FascicleDocumentUnit";
        PECBase.FASCICLE_LINK_TYPE_NAME = "FascicleLink";
        PECBase.FASCICLE_LOG_TYPE_NAME = "FascicleLog";
        PECBase.FASCICLE_DOCUMENT_TYPE_NAME = "FascicleDocument";
        PECBase.DOMAIN_TYPE_NAME = "DomainUserModel";
        PECBase.FASCICLE_CATEGORY_FASCICLE = "CategoryFascicle";
        PECBase.FASCICLEFOLDER_TYPE_NAME = "FascicleFolder";
        return PECBase;
    }());
    return PECBase;
});
//# sourceMappingURL=PECBase.js.map