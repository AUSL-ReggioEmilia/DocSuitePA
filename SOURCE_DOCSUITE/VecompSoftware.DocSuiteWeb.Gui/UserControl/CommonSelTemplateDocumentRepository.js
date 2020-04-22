/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "UserControl/uscTemplateDocumentRepository", "App/DTOs/ExceptionDTO"], function (require, exports, uscTemplateDocumentRepository, ExceptionDTO) {
    var CommonSelTemplateDocumentRepository = /** @class */ (function () {
        /**
         * Costruttore
         */
        function CommonSelTemplateDocumentRepository() {
            var _this = this;
            /**
             * Evento scatenato al click del pulsante conferma
             * @param sender
             * @param eventArgs
             */
            this.btnConfirm_OnClicked = function (sender, arg) {
                _this.returnValuesJson();
            };
        }
        /**
         * Gestisce il callback degli errori
         */
        CommonSelTemplateDocumentRepository.prototype.errorEventCallback = function (exception) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception && exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    console.log(JSON.stringify(exception));
                    uscNotification.showNotificationMessage("Errore nel caricamento.");
                }
            }
        };
        /**
         * Inizializzazione
         */
        CommonSelTemplateDocumentRepository.prototype.initialize = function () {
            var _this = this;
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);
            this.showLoadingPanel(this.pnlPageId);
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_START_LOAD_EVENT, function (args) {
                _this.showLoadingPanel(_this.pnlPageId);
            });
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_END_LOAD_EVENT, function (args) {
                _this.hideLoadingPanel(_this.pnlPageId);
            });
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_ERROR_EVENT, function (args, data) {
                _this.errorEventCallback(data);
                _this.hideLoadingPanel(_this.pnlPageId);
            });
        };
        /**
         * Visualizza un nuovo loading panel nella pagina
         */
        CommonSelTemplateDocumentRepository.prototype.showLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(updatedElementId);
        };
        /**
         * Nasconde il loading panel nella pagina
         */
        CommonSelTemplateDocumentRepository.prototype.hideLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(updatedElementId);
        };
        /**
         * Metodo che gestire il ritorno dei valori alla pagina chiamante
         * @param close
         */
        CommonSelTemplateDocumentRepository.prototype.returnValuesJson = function () {
            var userControl = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
            var selectedTemplate = userControl.getSelectedTemplateDocument();
            if (!selectedTemplate) {
                var uscNotification = $("#".concat(this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showWarningMessage('Nessun template selezionato');
                }
                return;
            }
            var returnArgs = selectedTemplate.IdArchiveChain.concat('|', selectedTemplate.Name);
            this.closeWindow(returnArgs);
        };
        /**
         * Metodo per il recupero di una specifica radwindow
         */
        CommonSelTemplateDocumentRepository.prototype.getRadWindow = function () {
            var radWindow;
            if (window.radWindow) {
                radWindow = window.radWindow;
            }
            else if (window.frameElement.radWindow) {
                radWindow = window.frameElement.radWindow;
            }
            return radWindow;
        };
        /**
         * Metodo per chiudere una radwindow con passaggio di argomenti
         * @param args
         */
        CommonSelTemplateDocumentRepository.prototype.closeWindow = function (args) {
            this.getRadWindow().close(args);
        };
        CommonSelTemplateDocumentRepository.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        return CommonSelTemplateDocumentRepository;
    }());
    return CommonSelTemplateDocumentRepository;
});
//# sourceMappingURL=CommonSelTemplateDocumentRepository.js.map