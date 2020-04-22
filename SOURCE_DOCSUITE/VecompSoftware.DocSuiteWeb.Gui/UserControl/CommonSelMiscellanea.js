/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports"], function (require, exports) {
    var commonSelMiscellanea = /** @class */ (function () {
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function commonSelMiscellanea(serviceConfigurations) {
            var _this = this;
            /**
          *------------------------- Events -----------------------------
          */
            /**
           * Evento scatenato al click del pulsante ConfermaInserimento
           * @method
           * @param sender
           * @param eventArgs
           * @returns
           */
            this.btnSave_Clicked = function (sender, eventArgs) {
                if (Page_IsValid) {
                    _this.showLoadingPanel(_this.insertsPageContentId);
                    _this._btnSave.set_enabled(false);
                }
            };
            /**
        * Recupera una RadWindow dalla pagina
        */
            this.getRadWindow = function () {
                var wnd = null;
                if (window.radWindow)
                    wnd = window.radWindow;
                else if (window.frameElement.radWindow)
                    wnd = window.frameElement.radWindow;
                return wnd;
            };
            /**
        * Chiude la RadWindow
        */
            this.closeWindow = function (message) {
                var wnd = _this.getRadWindow();
                wnd.close(message);
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
    * Callback per l'inserimento/aggiornamento di un MiscellaneaDocumentModel
    * @param entity
    */
        commonSelMiscellanea.prototype.confirmCallback = function (documentChainId) {
            try {
                var model = {};
                model.ActionName = this.actionType;
                model.Value = [documentChainId];
                this.closeWindow(model);
                this.hideLoadingPanel(this.insertsPageContentId);
            }
            catch (error) {
                this.hideLoadingPanel(this.insertsPageContentId);
                this.showNotificationMessage(this.uscNotificationId, "Errore in esecuzione dell'attività di salvataggio.");
                console.log(JSON.stringify(error));
            }
        };
        /**
     *------------------------- Methods -----------------------------
     */
        /**
        * Initialize
        */
        commonSelMiscellanea.prototype.initialize = function () {
            this._btnSave = $find(this.btnSaveId);
            this._btnSave.add_clicked(this.btnSave_Clicked);
            this._ajaxManager = $find(this.ajaxManagerId);
        };
        /**
     * Visualizza un nuovo loading panel nella pagina
     */
        commonSelMiscellanea.prototype.showLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(updatedElementId);
        };
        /**
         * Nasconde il loading panel nella pagina
         */
        commonSelMiscellanea.prototype.hideLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(updatedElementId);
        };
        commonSelMiscellanea.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        commonSelMiscellanea.ADD_ACTION = 'Add';
        commonSelMiscellanea.EDIT_ACTION = 'Edit';
        return commonSelMiscellanea;
    }());
    return commonSelMiscellanea;
});
//# sourceMappingURL=CommonSelMiscellanea.js.map