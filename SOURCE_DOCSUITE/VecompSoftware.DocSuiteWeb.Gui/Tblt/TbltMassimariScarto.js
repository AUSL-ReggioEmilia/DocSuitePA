/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/CategoryService", "App/Models/MassimariScarto/MassimarioScartoStatusType"], function (require, exports, ServiceConfigurationHelper, CategoryService, MassimarioScartoStatusType) {
    var TbltMassimariScarto = /** @class */ (function () {
        /**
     * Costruttore
     * @param serviceConfiguration
     */
        function TbltMassimariScarto(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante di Conferma
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.btnSave_OnClicking = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                _this._btnSave.set_enabled(false);
                _this.showLoadingPanel();
                _this._uscMassimarioScarto = $("#".concat(_this.uscMassimarioScartoId)).data();
                var selectedModel = _this._uscMassimarioScarto.getSelectedMassimario();
                if ((selectedModel.MassimarioScartoLevel == undefined || selectedModel.MassimarioScartoLevel < 2)
                    || (selectedModel.Status == undefined || selectedModel.Status == MassimarioScartoStatusType.LogicalDelete)) {
                    _this.showWarningMessage(_this.uscNotificationId, 'Selezionare un massimario di scarto valido.');
                    _this.closeLoadingPanel();
                    _this._btnSave.set_enabled(true);
                    return false;
                }
                var params = "AddMassimario|".concat(selectedModel.UniqueId);
                $find(_this.ajaxManagerId).ajaxRequest(params);
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
            if (!serviceConfiguration) {
                this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Classificatore");
                return;
            }
            this._categoryService = new CategoryService(serviceConfiguration);
            $(document).ready(function () {
            });
        }
        /**
         *------------------------- Methods -----------------------------
         */
        TbltMassimariScarto.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltMassimariScarto.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        /**
         * Metodo di inizializzazione
         */
        TbltMassimariScarto.prototype.initialize = function () {
            this._btnSave = $find(this.btnSaveId);
            this._btnSave.add_clicking(this.btnSave_OnClicking);
        };
        /**
         * Recupera una RadWindow dalla pagina
         */
        TbltMassimariScarto.prototype.getRadWindow = function () {
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
        TbltMassimariScarto.prototype.closeWindow = function () {
            var wnd = this.getRadWindow();
            wnd.close(true);
        };
        /**
          * Visualizza un nuovo loading panel nella pagina
          */
        TbltMassimariScarto.prototype.showLoadingPanel = function () {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(this.uscMassimarioScartoId);
        };
        /**
         * Nasconde il loading panel nella pagina
         */
        TbltMassimariScarto.prototype.closeLoadingPanel = function () {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(this.uscMassimarioScartoId);
        };
        return TbltMassimariScarto;
    }());
    return TbltMassimariScarto;
});
//# sourceMappingURL=TbltMassimariScarto.js.map