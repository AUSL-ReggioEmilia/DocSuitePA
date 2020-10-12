define(["require", "exports", "App/Helpers/EnumHelper"], function (require, exports, EnumHelper) {
    var UscDomainUserSelRestEvent;
    (function (UscDomainUserSelRestEvent) {
        UscDomainUserSelRestEvent[UscDomainUserSelRestEvent["ContactsAdded"] = 0] = "ContactsAdded";
        UscDomainUserSelRestEvent[UscDomainUserSelRestEvent["ContactsRemoved"] = 1] = "ContactsRemoved";
    })(UscDomainUserSelRestEvent || (UscDomainUserSelRestEvent = {}));
    var uscDomainUserSelRest = /** @class */ (function () {
        function uscDomainUserSelRest(serviceConfigurations) {
            var _this = this;
            this.contactList = [];
            this.uscSelRestEvents = UscDomainUserSelRestEvent;
            this._parentPageEventHandlersDictionary = {};
            this.btnSelContactDomain_onClick = function (sender) {
                var url = "../UserControl/CommonDomainUserSelRest.aspx?Type=Fasc&ManagerID=" + _this.radWindowManagerId + "&Callback" + window.location.href + "&PageContentId=" + _this.pageContentId;
                return _this.openWindow(url, "windowSelContact", 630, 500, _this.closeWindowCallback);
            };
            this.btnDelContact_onClick = function (sender) {
                var removalNodes = [];
                var removalContacts = [];
                var _loop_1 = function (checkedNode) {
                    removalNodes.push(checkedNode);
                    removalContacts.push(_this.contactList.find(function (x) { return x.EmailAddress == checkedNode.get_value(); }));
                };
                //collect contacts to remove
                for (var _i = 0, _a = _this._radTreeContact.get_checkedNodes(); _i < _a.length; _i++) {
                    var checkedNode = _a[_i];
                    _loop_1(checkedNode);
                }
                if (removalNodes.length > 0) {
                    //removing contacts in the event.
                    _this.chainParentEventToAction(function () {
                        var _loop_2 = function (i) {
                            _this.contactList = _this.contactList.filter(function (x) { return x.EmailAddress != removalContacts[i].EmailAddress; });
                            _this._radTreeContact.get_nodes().getNode(0).get_nodes().remove(removalNodes[i]);
                        };
                        for (var i = 0; i < removalNodes.length; i++) {
                            _loop_2(i);
                        }
                    }, UscDomainUserSelRestEvent.ContactsRemoved, removalContacts);
                    return true;
                }
                return false;
            };
            this.closeWindowCallback = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var contacts = args.get_argument();
                _this.chainParentEventToAction(function () {
                    _this.createDomainUsersContactsTree(contacts);
                }, UscDomainUserSelRestEvent.ContactsAdded, contacts);
            };
            this.createDomainUsersContactsTree = function (contacts) {
                _this._radTreeContact.get_nodes().getNode(0).get_nodes().clear();
                for (var _i = 0, contacts_1 = contacts; _i < contacts_1.length; _i++) {
                    var contact = contacts_1[_i];
                    _this.contactList.push(contact);
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(contact.Description + " (" + (contact.Code.split("\\")[0] ? contact.Code.split("\\")[0] : contact.Code) + ")");
                    node.set_value(contact.EmailAddress);
                    node.set_cssClass("font_node");
                    node.set_imageUrl("../Comm/Images/Interop/Manuale.gif");
                    _this._radTreeContact.get_nodes().getNode(0).set_checkable(false);
                    _this._radTreeContact.get_nodes().getNode(0).set_expanded(true);
                    _this._radTreeContact.get_nodes().getNode(0).get_nodes().add(node);
                }
            };
            this.clearDomainUsersContactsTree = function () {
                _this._radTreeContact.get_nodes().getNode(0).get_nodes().clear();
            };
            this.registerEventHandlerContactsAdded = function (callback) {
                _this._parentPageEventHandlersDictionary[UscDomainUserSelRestEvent.ContactsAdded] = callback;
            };
            this.registerEventHandlerContactsDeleted = function (callback) {
                _this._parentPageEventHandlersDictionary[UscDomainUserSelRestEvent.ContactsRemoved] = callback;
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        uscDomainUserSelRest.prototype.initialize = function () {
            $("#" + this.pageContentId).data(this);
            this._radWindowManager = $find(this.radWindowManagerId);
            this._radTreeContact = $find(this.radTreeContactId);
            this._radTreeContact.get_nodes().getNode(0).set_checkable(false);
            this._btnSelContactDomain = $("#" + this.btnSelContactDomainId);
            this._btnSelContactDomain.click(this.btnSelContactDomain_onClick);
            this._btnDelContact = $("#" + this.btnDelContactId);
            this._btnDelContact.click(this.btnDelContact_onClick);
        };
        uscDomainUserSelRest.prototype.openWindow = function (url, name, width, height, onCloseCallback) {
            var manager = $find(this.radWindowManagerId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            if (onCloseCallback) {
                wnd.remove_close(onCloseCallback);
                wnd.add_close(onCloseCallback);
            }
            return false;
        };
        uscDomainUserSelRest.prototype.getContacts = function () {
            return this.contactList;
        };
        uscDomainUserSelRest.prototype.setImageButtonsVisibility = function (isVisible) {
            if (isVisible) {
                $("#" + this.btnSelContactDomainId).show();
                $("#" + this.btnDelContactId).show();
            }
            else {
                $("#" + this.btnSelContactDomainId).hide();
                $("#" + this.btnDelContactId).hide();
            }
        };
        uscDomainUserSelRest.prototype.chainParentEventToAction = function (action, eventType, eventData) {
            //if there is an external event registered to the event we will call it first
            if (this._parentPageEventHandlersDictionary[eventType]) {
                var parentUpdateCallback = void 0;
                if (eventData === null || eventData === undefined) {
                    parentUpdateCallback = this._parentPageEventHandlersDictionary[eventType]();
                }
                else {
                    //passing data to parent event
                    parentUpdateCallback = this._parentPageEventHandlersDictionary[eventType](eventData);
                }
                parentUpdateCallback.then(function (data) {
                    //executing in the then branch because parent may chose to prevent default action to be executed
                    action();
                });
            }
            else {
                action();
            }
        };
        return uscDomainUserSelRest;
    }());
    return uscDomainUserSelRest;
});
//# sourceMappingURL=uscDomainUserSelRest.js.map