define(["require", "exports", "../App/Services/Commons/ContactService", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, ContactService, ServiceConfigurationHelper) {
    var UscContattiSelRestEvent;
    (function (UscContattiSelRestEvent) {
        UscContattiSelRestEvent[UscContattiSelRestEvent["NewContactsAdded"] = 0] = "NewContactsAdded";
        UscContattiSelRestEvent[UscContattiSelRestEvent["ContactDeleted"] = 1] = "ContactDeleted";
    })(UscContattiSelRestEvent || (UscContattiSelRestEvent = {}));
    var contactTypeIcons;
    (function (contactTypeIcons) {
        contactTypeIcons["Administration"] = "../comm/images/interop/Amministrazione.gif";
        contactTypeIcons["AOO"] = "../comm/images/interop/Aoo.gif";
        contactTypeIcons["AO"] = "../comm/images/interop/Uo.gif";
        contactTypeIcons["Role"] = "../comm/images/interop/Ruolo.gif";
        contactTypeIcons["Citizen"] = "../App_Themes/DocSuite2008/imgset16/user.png";
        contactTypeIcons["Group"] = "../comm/images/interop/Gruppo.gif";
        contactTypeIcons["Sector"] = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png";
        contactTypeIcons["M"] = "../comm/images/interop/Amministrazione.gif";
        contactTypeIcons["A"] = "../comm/images/interop/Aoo.gif";
        contactTypeIcons["U"] = "../comm/images/interop/Uo.gif";
        contactTypeIcons["R"] = "../comm/images/interop/Ruolo.gif";
        contactTypeIcons["P"] = "../App_Themes/DocSuite2008/imgset16/user.png";
        contactTypeIcons["G"] = "../comm/images/interop/Gruppo.gif";
        contactTypeIcons["S"] = "../App_Themes/DocSuite2008/imgset16/GroupMembers.png";
    })(contactTypeIcons || (contactTypeIcons = {}));
    var uscContactSelRest = /** @class */ (function () {
        function uscContactSelRest(serviceConfigurations) {
            var _this = this;
            this.uscContattiSelRestEvents = UscContattiSelRestEvent;
            this._parentPageEventHandlersDictionary = {};
            this.registerEventHandler = function (eventType, callback) {
                _this._parentPageEventHandlersDictionary[eventType] = callback;
            };
            this.toolbarContacts_onClick = function (sender, args) {
                var btn = args.get_item();
                switch (btn.get_index()) {
                    case 0:
                        _this._rwContactSelector.show();
                        break;
                    case 1:
                        _this.deleteContacts();
                        break;
                }
            };
            this.btnContactConfirm_onClick = function (sender, args) {
                var contact = _this._uscContactRest.getLastSearchedContactModel();
                if (contact) {
                    _this._rwContactSelector.close();
                    var parentUpdateCallback = _this._parentPageEventHandlersDictionary[_this.uscContattiSelRestEvents.NewContactsAdded](contact);
                    parentUpdateCallback.then(function (data) {
                        _this.createNode(contact);
                    });
                }
            };
            this.btnContactConfirmAndNew_onClick = function (sender, args) {
                _this.createContact().then(function (_a) {
                    var rubrica = _a[0], newlyAddedContact = _a[1];
                    if (newlyAddedContact) {
                        var parentUpdateCallback = _this._parentPageEventHandlersDictionary[_this.uscContattiSelRestEvents.NewContactsAdded](newlyAddedContact);
                        parentUpdateCallback.then(function (data) {
                            _this.createNode(newlyAddedContact);
                        });
                    }
                    _this._uscContactRest.clear();
                });
            };
            this._rwContactSelector_show = function (sender, args) {
                _this._uscContactRest.clear();
            };
            this.deleteContacts = function () {
                var selectedNodeToDelete = _this._treeContact.get_selectedNode();
                if (selectedNodeToDelete) {
                    var parentDeleteCallback = _this._parentPageEventHandlersDictionary[_this.uscContattiSelRestEvents.ContactDeleted](+selectedNodeToDelete.get_value());
                    parentDeleteCallback.then(function (contactParentId) {
                        _this._treeContact.trackChanges();
                        selectedNodeToDelete.get_parent().get_nodes().removeAt(selectedNodeToDelete.get_index());
                        var selectedParentToDelete = _this._treeContact.get_allNodes().filter(function (node) { return +node.get_value() === contactParentId; })[0];
                        if (!selectedParentToDelete.get_allNodes().length && selectedParentToDelete.get_contentCssClass() == 'initial') {
                            _this._treeContact.get_nodes().remove(selectedParentToDelete);
                        }
                        var treeHasChildrenLeft = _this._treeContact.get_allNodes().length > 0;
                        if (!treeHasChildrenLeft && _this._requiredValidationEnabled) {
                            _this.enableValidators(true);
                        }
                        _this._treeContact.commitChanges();
                    });
                }
                else {
                    alert("Selezionare un contatto");
                }
            };
            this.enableValidators = function (state) {
                ValidatorEnable($get(_this.validatorAnyNodeId), state);
            };
            var contactServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
            this._contactService = new ContactService(contactServiceConfiguration);
        }
        Object.defineProperty(uscContactSelRest.prototype, "_requiredValidationEnabled", {
            get: function () {
                return JSON.parse(this.requiredValidationEnabled.toLowerCase());
            },
            enumerable: true,
            configurable: true
        });
        uscContactSelRest.prototype.initialize = function () {
            this._treeContact = $find(this.treeContactId);
            this._tbContactsControl = $find(this.tbContactsControlId);
            if (this._tbContactsControl) {
                this._tbContactsControl.add_buttonClicking(this.toolbarContacts_onClick);
            }
            this._rwContactSelector = $find(this.rwContactSelectorId);
            this._rwContactSelector.add_show(this._rwContactSelector_show);
            this._btnContactConfirm = $find(this.btnContactConfirmId);
            this._btnContactConfirm.add_clicking(this.btnContactConfirm_onClick);
            this._btnContactConfirmAndNew = $find(this.btnContactConfirmAndNewId);
            this._btnContactConfirmAndNew.add_clicking(this.btnContactConfirmAndNew_onClick);
            this._uscContactRest = $("#" + this.uscContactRestId).data();
            $("#" + this.pnlContentId).data(this);
        };
        uscContactSelRest.prototype.renderContactsTree = function (contactCollection) {
            this.enableValidators(contactCollection.length === 0 && this._requiredValidationEnabled ? true : false);
            this.populateContactsTreeView(contactCollection);
        };
        uscContactSelRest.prototype.createContact = function () {
            var promise = $.Deferred();
            if (!jQuery.isEmptyObject(this._uscContactRest)) {
                this._uscContactRest.createContact()
                    .done(function (data) {
                    promise.resolve(data);
                })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            return promise.promise();
        };
        uscContactSelRest.prototype.createNode = function (contactModel) {
            var _this = this;
            if (this._requiredValidationEnabled)
                this.enableValidators(false);
            var promise = $.Deferred();
            var currentNode = new Telerik.Web.UI.RadTreeNode();
            var currentNodeDescription = contactModel.Description;
            var currentNodeImageUrl = "" + (contactModel.IdContactType || contactModel.ContactType);
            currentNode.set_text(currentNodeDescription);
            currentNode.set_value("" + (contactModel.EntityId || contactModel.Id));
            currentNode.set_imageUrl(contactTypeIcons[currentNodeImageUrl]);
            currentNode.set_expanded(true);
            currentNode.set_contentCssClass('dsw-text-bold');
            var currentNodeFromTree = this._treeContact.get_allNodes().filter(function (node) { return +node.get_value() === +currentNode.get_value(); })[0];
            if (currentNodeFromTree) {
                return promise.resolve(currentNodeFromTree);
            }
            if (contactModel.IncrementalFather) {
                var parentNode = this._treeContact.get_allNodes().filter(function (node) { return +node.get_value() === contactModel.IncrementalFather; })[0];
                if (!parentNode) {
                    this._contactService.getContactParents(contactModel.IncrementalFather, function (data) {
                        _this.createNode(data[0])
                            .done(function (node) {
                            node.set_contentCssClass('initial');
                            node.get_nodes().add(currentNode);
                            node.set_expanded(true);
                            promise.resolve(currentNode);
                        });
                    });
                }
                else {
                    parentNode.get_nodes().add(currentNode);
                }
            }
            else {
                this._treeContact.get_nodes().add(currentNode);
                return promise.resolve(currentNode);
            }
            return promise.promise();
        };
        uscContactSelRest.prototype.populateContactsTreeView = function (contactTreeModels) {
            var _this = this;
            if (this._treeContact.get_allNodes().length > 0)
                this._treeContact.get_nodes().clear();
            //build and add other tree nodes
            contactTreeModels.forEach(function (contactModel) {
                _this.createNode(contactModel);
            });
        };
        uscContactSelRest.prototype.setToolbarVisibility = function (isVisible) {
            this._tbContactsControl.get_items().forEach(function (item) {
                item.set_enabled(isVisible);
            });
        };
        return uscContactSelRest;
    }());
    return uscContactSelRest;
});
//# sourceMappingURL=uscContattiSelRest.js.map