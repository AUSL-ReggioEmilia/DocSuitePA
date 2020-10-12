/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
define(["require", "exports", "App/Helpers/ErrorHelper", "UserControl/uscStartWorkflow"], function (require, exports, ErrorHelper, UscStartWorkflow) {
    var StartWorkflow = /** @class */ (function () {
        /**
        * Costruttore
             * @param serviceConfiguration
        */
        function StartWorkflow(serviceConfigurations) {
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
        * Initialize
        */
        StartWorkflow.prototype.initialize = function () {
            var _this = this;
            this._errorHelper = new ErrorHelper();
            this._ajaxManager = $find(this.ajaxManagerId);
            this._notification = $find(this.radNotificationId);
            this._notificationSuccess = $find(this.radNotificationSuccessId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#".concat(this.uscContentId)).on(UscStartWorkflow.LOADED_EVENT, function (args) {
                _this.loadUscStartWorkflow();
            });
            this.loadUscStartWorkflow();
        };
        /**
        * ------------------------- Methods -----------------------------
        */
        /*
        * funzione di caricamento dei dati
        */
        StartWorkflow.prototype.loadData = function () {
            var _this = this;
            $.when(this.loadUscStartWorkflow()).done(function () {
            }).fail(function () {
                _this._notification.show();
                _this._notification.set_text("Errore nel caricamento della pagina.");
            });
        };
        StartWorkflow.prototype.loadUscStartWorkflow = function () {
            var uscStartWorkflow = $("#".concat(this.uscContentId)).data();
            if (!jQuery.isEmptyObject(uscStartWorkflow)) {
                uscStartWorkflow.loadData();
            }
        };
        return StartWorkflow;
    }());
    return StartWorkflow;
});
//# sourceMappingURL=StartWorkflow.js.map