import ErrorHelper = require('App/Helpers/ErrorHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');

class uscErrorNotification {

    radNotificationId: string;
    radNotificationWarningId: string;
    radNotificationValidationId: string;
    radRotatorId: string;
    messageRotatorId: string;
    exceptionMessageId: string;
    pageId: string;

    public static LOADED_EVENT: string = "onLoaded";

    private _notification: Telerik.Web.UI.RadNotification;
    private _notificationWarning: Telerik.Web.UI.RadNotification;
    private _notificationValidation: Telerik.Web.UI.RadNotification;
    private _errorHelper: ErrorHelper;
    private _radRotator: Telerik.Web.UI.RadRotator;
    private _exceptionMessage: JQuery;  

    initialize() {
        this._errorHelper = new ErrorHelper();
        this._notification = <Telerik.Web.UI.RadNotification>$find(this.radNotificationId);
        this._notificationWarning = <Telerik.Web.UI.RadNotification>$find(this.radNotificationWarningId);
        this._notificationValidation = <Telerik.Web.UI.RadNotification>$find(this.radNotificationValidationId);
        this._radRotator = <Telerik.Web.UI.RadRotator>$find(this.radRotatorId);
        this._exceptionMessage = <JQuery>$("#".concat(this.exceptionMessageId));
        this.bindLoaded();
    }

    showNotification(exception: ExceptionDTO) {
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
    }

    showNotificationMessage(message: string) {
        let exception: ExceptionDTO = new ExceptionDTO();
        exception.statusText = message;
        this.showNotification(exception);
    }

    showWarningMessage(message: string) {
        let exception: ExceptionDTO = new ExceptionDTO();
        exception.statusText = message;
        this._notificationWarning.set_text(exception.statusText);
        this._notificationValidation.hide();
        this._notification.hide();
        this._notificationWarning.show();
    }

    /**
    * Scatena l'evento di "load completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscErrorNotification.LOADED_EVENT);
    }


}

export = uscErrorNotification