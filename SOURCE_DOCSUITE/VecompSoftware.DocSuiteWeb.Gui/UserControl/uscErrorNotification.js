define(["require", "exports", "App/Helpers/ErrorHelper", "App/DTOs/ExceptionDTO", "App/DTOs/ValidationExceptionDTO"], function (require, exports, ErrorHelper, ExceptionDTO, ValidationExceptionDTO) {
    var uscErrorNotification = /** @class */ (function () {
        function uscErrorNotification() {
        }
        uscErrorNotification.prototype.initialize = function () {
            this._errorHelper = new ErrorHelper();
            this._notification = $find(this.radNotificationId);
            this._notificationWarning = $find(this.radNotificationWarningId);
            this._notificationValidation = $find(this.radNotificationValidationId);
            this._radRotator = $find(this.radRotatorId);
            this._exceptionMessage = $("#".concat(this.exceptionMessageId));
            this.bindLoaded();
        };
        uscErrorNotification.prototype.showNotification = function (exception) {
            if (exception instanceof ValidationExceptionDTO) {
                this._notification.hide();
                this._exceptionMessage.text(exception.statusText);
                this._radRotator.set_dataSource(exception.validationMessages);
                this._radRotator.dataBind();
                this._notificationWarning.hide();
                this._notificationValidation.show();
            }
            else {
                this._notification.set_text(exception.statusText);
                this._notificationWarning.hide();
                this._notificationValidation.hide();
                this._notification.show();
            }
        };
        uscErrorNotification.prototype.showNotificationMessage = function (message) {
            var exception = new ExceptionDTO();
            exception.statusText = message;
            this.showNotification(exception);
        };
        uscErrorNotification.prototype.showWarningMessage = function (message) {
            var exception = new ExceptionDTO();
            exception.statusText = message;
            this._notificationWarning.set_text(exception.statusText);
            this._notificationValidation.hide();
            this._notification.hide();
            this._notificationWarning.show();
        };
        /**
        * Scatena l'evento di "load completed" del controllo
        */
        uscErrorNotification.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscErrorNotification.LOADED_EVENT);
        };
        uscErrorNotification.LOADED_EVENT = "onLoaded";
        return uscErrorNotification;
    }());
    return uscErrorNotification;
});
//# sourceMappingURL=uscErrorNotification.js.map