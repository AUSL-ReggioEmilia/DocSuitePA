define(["require", "exports"], function (require, exports) {
    var CommonSelContactOmniBus = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function CommonSelContactOmniBus() {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante di ricerca contatti
             * @param sender
             * @param args
             */
            this.btnFind_onClick = function (sender, args) {
                _this._ajaxManager.ajaxRequestWithTarget(_this.btnFindUniqueId, '');
            };
            /**
             * Evento scatenato al click del pulsante di conferma
             * @param sender
             * @param args
             */
            this.btnConfirm_onClick = function (sender, args) {
                _this._rtvResults = $find(_this.rtvResultsId);
                var selectedContact = _this._rtvResults.get_selectedNode();
                if (selectedContact == undefined || selectedContact.get_value() == "root") {
                    alert("Nessun contatto selezionato");
                    return;
                }
                var serializedContact = selectedContact.get_attributes().getAttribute("serializedModel");
                _this.returnValuesJson(serializedContact);
            };
            /**
             * Evento scatenato al click del pulsante di conferma e nuovo
             * @param sender
             * @param args
             */
            this.btnConfirmAndNew_onClick = function (sender, args) {
                _this._rtvResults = $find(_this.rtvResultsId);
                var selectedContact = _this._rtvResults.get_selectedNode();
                if (selectedContact == undefined || selectedContact.get_value() == "root") {
                    alert("Nessun contatto selezionato");
                    return;
                }
                var serializedContact = selectedContact.get_attributes().getAttribute("serializedModel");
                _this.returnValuesJsonAndNew(serializedContact);
            };
        }
        /**
         * Inizializzazione della classe
         */
        CommonSelContactOmniBus.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._txtNome = $find(this.txtNomeId);
            this._txtCognome = $find(this.txtCognomeId);
            this._rcbDistretto = $find(this.rcbDistrettoId);
            this._txtCdc = $find(this.txtCdcId);
            this._btnFind = $find(this.btnFindId);
            this._btnFind.add_clicked(this.btnFind_onClick);
            this._rtvResults = $find(this.rtvResultsId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_onClick);
            this._btnConfirmAndNew = $find(this.btnConfirmAndNewId);
            this._btnConfirmAndNew.add_clicked(this.btnConfirmAndNew_onClick);
            this._txtNome.focus();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        CommonSelContactOmniBus.prototype.returnValuesJson = function (serializedContact) {
            this.closeWindow(serializedContact);
            return;
        };
        CommonSelContactOmniBus.prototype.returnValuesJsonAndNew = function (serializedContact) {
            window.GetRadWindow().BrowserWindow[this.callerId.concat("_AddUsersToControl")](serializedContact);
        };
        CommonSelContactOmniBus.prototype.closeWindow = function (contact) {
            window.GetRadWindow().close(contact);
        };
        return CommonSelContactOmniBus;
    }());
    return CommonSelContactOmniBus;
});
//# sourceMappingURL=CommonSelContactOmniBus.js.map