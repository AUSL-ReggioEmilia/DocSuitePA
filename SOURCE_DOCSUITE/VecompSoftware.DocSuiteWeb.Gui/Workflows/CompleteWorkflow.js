/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
define(["require", "exports", "App/Helpers/ErrorHelper"], function (require, exports, ErrorHelper) {
    var CompleteWorkflow = /** @class */ (function () {
        /**
        * Costruttore
             * @param serviceConfiguration
        */
        function CompleteWorkflow(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
        * Initialize
        */
        CompleteWorkflow.prototype.initialize = function () {
            this._errorHelper = new ErrorHelper();
            this._ajaxManager = $find(this.ajaxManagerId);
            this._notification = $find(this.radNotificationId);
            this._notificationSuccess = $find(this.radNotificationSuccessId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
        };
        return CompleteWorkflow;
    }());
    return CompleteWorkflow;
});
//# sourceMappingURL=CompleteWorkflow.js.map