define(["require", "exports", "App/Helpers/EnumHelper", "App/Models/Commons/AUSSubjectType"], function (require, exports, EnumHelper, AUSSubjectType) {
    var CommonSelAUSContact = /** @class */ (function () {
        function CommonSelAUSContact(serviceConfigurations) {
            var _this = this;
            this.btnSearch_onClick = function (sender, args) {
                var searchText = _this._txtSearch.get_textBoxValue();
                var selectedSubjectType = $("#".concat(_this.chkSubjectTypeId).concat(" input:checked")).val();
                var subjectType = AUSSubjectType[selectedSubjectType];
                var ajaxModel = {};
                ajaxModel.ActionName = "SearchByText";
                ajaxModel.Value = [searchText, subjectType.toString()];
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            };
            this.btnCodeSearch_onClick = function (sender, args) {
                var searchCode = _this._txtCodeSearch.get_textBoxValue();
                var selectedSubjectType = $("#".concat(_this.chkSubjectTypeId).concat(" input:checked")).val();
                var subjectType = AUSSubjectType[selectedSubjectType];
                var ajaxModel = {};
                ajaxModel.ActionName = "SearchByCode";
                ajaxModel.Value = [searchCode, subjectType.toString()];
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            };
            this.btnConfirm_onClick = function (sender, args) {
                _this.addAUSContact(true);
            };
            this.btnConfirmAndNew_onClick = function (sender, args) {
                _this.addAUSContact(false);
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
            $(document).ready(function () {
            });
        }
        CommonSelAUSContact.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._uscNotification = $("#" + this.uscNotificationId).data();
            this._txtSearch = $find(this.txtSearchId);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicked(this.btnSearch_onClick);
            this._txtCodeSearch = $find(this.txtCodeSearchId);
            this._btnCodeSearch = $find(this.btnCodeSearchId);
            this._btnCodeSearch.add_clicked(this.btnCodeSearch_onClick);
            this._rtvAUSContacts = $find(this.rtvAUSContactsId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_onClick);
            this._btnConfirmAndNew = $find(this.btnConfirmAndNewId);
            this._btnConfirmAndNew.add_clicked(this.btnConfirmAndNew_onClick);
        };
        CommonSelAUSContact.prototype.contactsCallback = function (jsonResult) {
            var contacts = JSON.parse(jsonResult);
            this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().clear();
            for (var _i = 0, contacts_1 = contacts; _i < contacts_1.length; _i++) {
                var contact = contacts_1[_i];
                var node = new Telerik.Web.UI.RadTreeNode();
                if (contact.Email) {
                    node.set_text(contact.Name + " (" + contact.Code + " - " + contact.Email + ")");
                }
                else {
                    node.set_text(contact.Name + " (" + contact.Code + ")");
                }
                node.set_value(contact.Code);
                node.get_attributes().setAttribute(CommonSelAUSContact.AUS_CONTACT_SUBJECT_NAME_TYPE_NAME, contact.Name);
                node.get_attributes().setAttribute(CommonSelAUSContact.AUS_CONTACT_EMAIL_TYPE_NAME, contact.Email);
                this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().add(node);
            }
            this._rtvAUSContacts.get_nodes().getNode(0).expand();
            $("#" + this.rtvAUSContactsId).show();
            this._btnConfirm.set_enabled(true);
        };
        CommonSelAUSContact.prototype.contactsErrorCallback = function (errorMessage) {
            if (!errorMessage) {
                return;
            }
            this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().clear();
            var errorNode = new Telerik.Web.UI.RadTreeNode();
            errorNode.set_text(errorMessage);
            errorNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/error.png");
            errorNode.set_cssClass(CommonSelAUSContact.AUS_CONTACT_ERROR_NODE_CSS_CLASS_NAME);
            errorNode.get_attributes().setAttribute("IsErrorNode", true);
            this._rtvAUSContacts.get_nodes().getNode(0).get_nodes().add(errorNode);
        };
        CommonSelAUSContact.prototype.addAUSContact = function (closeWindow) {
            if (!this._rtvAUSContacts.get_selectedNode() || this._rtvAUSContacts.get_selectedNode().get_level() === 0) {
                this._uscNotification.showWarningMessage("Seleziona almeno un contatto");
                return;
            }
            if (this._rtvAUSContacts.get_selectedNode().get_attributes().getAttribute("IsErrorNode")) {
                this._uscNotification.showWarningMessage("Non è possibile selezionare un nodo di errore");
                return;
            }
            var code = this._rtvAUSContacts.get_selectedNode().get_value();
            var name = this._rtvAUSContacts.get_selectedNode().get_attributes().getAttribute(CommonSelAUSContact.AUS_CONTACT_SUBJECT_NAME_TYPE_NAME);
            var email = this._rtvAUSContacts.get_selectedNode().get_attributes().getAttribute(CommonSelAUSContact.AUS_CONTACT_EMAIL_TYPE_NAME);
            var selectedContact = {
                Code: code,
                Name: name,
                Email: email
            };
            var contact = {};
            contact.FiscalCode = selectedContact.Code.toString();
            contact.Code = selectedContact.Code.toString();
            contact.SearchCode = selectedContact.Code.toString();
            contact.Description = selectedContact.Name;
            contact.EmailAddress = selectedContact.Email;
            contact.ContactType = {};
            contact.ContactType.ContactTypeId = "I";
            var dataToReturn = {
                Action: "Ins",
                Contact: JSON.stringify(contact)
            };
            if (closeWindow) {
                this.closeWindow(dataToReturn);
            }
            else {
                var returnModel = { contactType: contact.ContactType.ContactTypeId, contact: JSON.stringify(contact) };
                this.getRadWindow().BrowserWindow[this.callerId + "_UpdateSmart"](returnModel.contactType, returnModel.contact);
            }
        };
        CommonSelAUSContact.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        CommonSelAUSContact.prototype.closeWindow = function (dataToReturn) {
            var wnd = this.getRadWindow();
            wnd.close(dataToReturn);
        };
        CommonSelAUSContact.AUS_CONTACT_SUBJECT_NAME_TYPE_NAME = "SubjectName";
        CommonSelAUSContact.AUS_CONTACT_EMAIL_TYPE_NAME = "Email";
        CommonSelAUSContact.AUS_CONTACT_ERROR_NODE_CSS_CLASS_NAME = "error-tree-node";
        return CommonSelAUSContact;
    }());
    return CommonSelAUSContact;
});
//# sourceMappingURL=CommonSelAUSContact.js.map