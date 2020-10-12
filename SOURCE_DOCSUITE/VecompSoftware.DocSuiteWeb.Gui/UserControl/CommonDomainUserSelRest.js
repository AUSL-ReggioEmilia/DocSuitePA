define(["require", "exports", "App/Helpers/EnumHelper", "App/Helpers/ServiceConfigurationHelper", "App/Services/Securities/DomainUserService", "App/DTOs/ExceptionDTO"], function (require, exports, EnumHelper, ServiceConfigurationHelper, DomainUserService, ExceptionDTO) {
    var CommonDomainUserSelRest = /** @class */ (function () {
        function CommonDomainUserSelRest(serviceConfigurations) {
            var _this = this;
            this.contactList = [];
            this.btnSearch_OnClick = function (sender, args) {
                args.set_cancel(true);
                var textFilterUserName = $("#" + _this.txtFilterId).val();
                if (textFilterUserName.length < 2) {
                    alert("Il filtro di ricerca deve essere di almeno 2 caratteri.");
                    return;
                }
                _this._domainUserService.userFinder(textFilterUserName, function (data) {
                    if (!data) {
                        return;
                    }
                    var domainUsersModel = data;
                    _this._tvwContactDomain.get_nodes().clear();
                    var rootNode = new Telerik.Web.UI.RadTreeNode();
                    rootNode.set_text("Contatti");
                    rootNode.set_cssClass("font_node");
                    rootNode.set_expanded(true);
                    _this._tvwContactDomain.get_nodes().add(rootNode);
                    _this._tvwContactDomain.get_nodes().getNode(0).get_nodes().clear();
                    for (var _i = 0, domainUsersModel_1 = domainUsersModel; _i < domainUsersModel_1.length; _i++) {
                        var domainUserModel = domainUsersModel_1[_i];
                        var node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(domainUserModel.Account + " - (" + domainUserModel.DisplayName + ")");
                        node.set_value(domainUserModel.EmailAddress);
                        _this._tvwContactDomain.get_nodes().getNode(0).get_nodes().add(node);
                        var newContact = {
                            IdContactType: "A",
                            Code: domainUserModel.Account,
                            Description: domainUserModel.DisplayName,
                            SearchCode: domainUserModel.Account.split("\\")[1],
                            EmailAddress: domainUserModel.EmailAddress,
                        };
                        _this.contactList.push(newContact);
                    }
                    $("#" + _this.tvwContactDomainId).show();
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.btnConfirm_OnClick = function (sender, args) {
                args.set_cancel(true);
                var selNode = _this._tvwContactDomain.get_selectedNode();
                var selectedContact = _this.contactList.filter(function (x) { return x.EmailAddress == selNode.get_value(); });
                var wnd = _this.getRadWindow();
                wnd.close(selectedContact);
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        CommonDomainUserSelRest.prototype.initialize = function () {
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUser");
            this._domainUserService = new DomainUserService(serviceConfiguration);
            this._tvwContactDomain = $find(this.tvwContactDomainId);
            this._tvwContactDomain.get_nodes().clear();
            this._txtFilter = $("#" + this.txtFilterId);
            $("#" + this.txtFilterId).val("");
            this.contactList = [];
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_OnClick);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
            //TODO add functionality for conferma Nuovo as in uscContattiSel
        };
        CommonDomainUserSelRest.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        CommonDomainUserSelRest.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#" + uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        CommonDomainUserSelRest.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        return CommonDomainUserSelRest;
    }());
    return CommonDomainUserSelRest;
});
//# sourceMappingURL=CommonDomainUserSelRest.js.map