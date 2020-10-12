define(["require", "exports", "App/DTOs/ExceptionDTO", "App/DTOs/ExceptionStatusCode"], function (require, exports, ExceptionDTO, ExceptionStatusCode) {
    var CommonSetiContactSel = /** @class */ (function () {
        function CommonSetiContactSel(serviceConfigurations) {
            var _this = this;
            this.onShowing = function (sender, args) {
                _this._txtSearch.set_textBoxValue("");
                _this._rtvSetiContact.get_nodes().getNode(0).get_nodes().clear();
            };
            this.btnSearch_onClick = function (sender, args) {
                if (!_this._txtSearch.get_textBoxValue() || _this._txtSearch.get_textBoxValue().length < 2) {
                    alert("Inserisci almeno due caratteri");
                    return;
                }
                _this._ajaxLoadingPanel.show(_this.rtvSetiContactId);
                var ajaxModel = {};
                ajaxModel.ActionName = CommonSetiContactSel.SEARCH_SETI_CONTACT;
                ajaxModel.Value = [_this._txtSearch.get_textBoxValue()];
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            };
            this.contactsErrorCallback = function (errorMessage) {
                var exception = new ExceptionDTO();
                exception.statusCode = ExceptionStatusCode.InternalServerError;
                exception.statusText = "Anomalia critica nell'esecuzione della richiesta. Contattare l'assistenza.";
                _this._uscNotification.showNotification(exception);
                console.error(errorMessage);
                _this._ajaxLoadingPanel.hide(_this.rtvSetiContactId);
                _this._btnConfirm.set_enabled(false);
            };
            this.btnConfirm_onClick = function (sender, args) {
                if (_this._rtvSetiContact.get_selectedNode().get_level() > 0) {
                    _this.closeWindow(_this._rtvSetiContact.get_selectedNode().get_attributes().getAttribute(CommonSetiContactSel.SETI_CONTACT_MODEL));
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        ;
        CommonSetiContactSel.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._uscNotification = $("#" + this.uscNotificationId).data();
            this._txtSearch = $find(this.txtSearchId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_onClick);
            this._rtvSetiContact = $find(this.rtvSetiContactId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_onClick);
            this._wndSetiContacts = this.getRadWindow();
            this._wndSetiContacts.add_show(this.onShowing);
            this._btnConfirm.set_enabled(false);
        };
        CommonSetiContactSel.prototype.contactsCallback = function (jsonResonse) {
            var contacts = JSON.parse(jsonResonse);
            this._rtvSetiContact.get_nodes().getNode(0).get_nodes().clear();
            for (var _i = 0, contacts_1 = contacts; _i < contacts_1.length; _i++) {
                var contact = contacts_1[_i];
                var node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(contact.Nome + " " + contact.Cognome + " - (" + contact.TesseraSanitaria + " - " + contact.CodiceFiscale + ")");
                node.set_value(contact.AnasId);
                node.get_attributes().setAttribute(CommonSetiContactSel.SETI_CONTACT_MODEL, contact);
                this._rtvSetiContact.get_nodes().getNode(0).get_nodes().add(node);
            }
            this._rtvSetiContact.get_nodes().getNode(0).expand();
            this._ajaxLoadingPanel.hide(this.rtvSetiContactId);
            this._btnConfirm.set_enabled(true);
        };
        CommonSetiContactSel.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        CommonSetiContactSel.prototype.closeWindow = function (dataToReturn) {
            var wnd = this.getRadWindow();
            wnd.close(dataToReturn);
        };
        CommonSetiContactSel.SETI_CONTACT_MODEL = "contactModel";
        CommonSetiContactSel.SEARCH_SETI_CONTACT = "SearchByText";
        return CommonSetiContactSel;
    }());
    return CommonSetiContactSel;
});
//# sourceMappingURL=CommonSetiContactSel.js.map