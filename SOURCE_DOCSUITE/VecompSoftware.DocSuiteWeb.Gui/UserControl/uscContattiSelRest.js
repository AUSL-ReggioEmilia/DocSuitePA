define(["require", "exports", "../App/Services/Commons/ContactService", "App/Helpers/ServiceConfigurationHelper", "App/Services/Tenants/TenantService", "App/ViewModels/Tenants/TenantViewModel", "App/Models/UpdateActionType"], function (require, exports, ContactService, ServiceConfigurationHelper, TenantService, TenantViewModel, UpdateActionType) {
    var UscContattiSelRestEvent;
    (function (UscContattiSelRestEvent) {
        UscContattiSelRestEvent[UscContattiSelRestEvent["NewContactsAdded"] = 0] = "NewContactsAdded";
        UscContattiSelRestEvent[UscContattiSelRestEvent["ContactDeleted"] = 1] = "ContactDeleted";
        UscContattiSelRestEvent[UscContattiSelRestEvent["AllContactsAdded"] = 2] = "AllContactsAdded";
        UscContattiSelRestEvent[UscContattiSelRestEvent["AllContactsDeleted"] = 3] = "AllContactsDeleted";
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
        function uscContactSelRest(serviceConfigurations, uscId) {
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
                    case 2:
                        _this.addAllContacts();
                        break;
                    case 3:
                        _this.deleteAllContacts();
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
                            if (Boolean(_this.multiTenantEnabled.toLowerCase())) {
                                var tenant = new TenantViewModel();
                                tenant.UniqueId = _this.currentTenantId;
                                tenant.Contacts = [newlyAddedContact];
                                _this._tenantService.updateTenant(tenant, UpdateActionType.TenantContactAdd, function (data) { });
                            }
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
                        if (selectedParentToDelete && !selectedParentToDelete.get_allNodes().length && selectedParentToDelete.get_contentCssClass() == 'initial') {
                            _this._treeContact.get_nodes().remove(selectedParentToDelete);
                        }
                        var treeHasChildrenLeft = _this._treeContact.get_allNodes().length > 0;
                        if (!treeHasChildrenLeft && _this._requiredValidationEnabled()) {
                            _this.enableValidators(true);
                        }
                        _this._treeContact.commitChanges();
                    });
                }
                else {
                    alert("Selezionare un contatto");
                }
            };
            this.addAllContacts = function () {
                _this._manager.radconfirm("Sei sicuro di voler aggiungere tutti i contatti?", function (arg) {
                    if (arg) {
                        _this._parentPageEventHandlersDictionary[_this.uscContattiSelRestEvents.AllContactsAdded]();
                    }
                    document.getElementsByTagName("body")[0].setAttribute("class", "comm chrome");
                }, 400, 300);
            };
            this.deleteAllContacts = function () {
                _this._manager.radconfirm("Sei sicuro di voler eliminare tutti i contatti?", function (arg) {
                    if (arg) {
                        var deleteCallback = _this._parentPageEventHandlersDictionary[_this.uscContattiSelRestEvents.AllContactsDeleted]();
                        deleteCallback.then(function () { return _this._treeContact.get_nodes().clear(); });
                    }
                    document.getElementsByTagName("body")[0].setAttribute("class", "comm chrome");
                }, 400, 300);
            };
            this.enableValidators = function (state) {
                var behaviourValidationConfiguration = sessionStorage.getItem(_this._roleValidationSessionKey);
                var behaviourValidationConfigurationValue = state;
                if (behaviourValidationConfiguration) {
                    behaviourValidationConfigurationValue = behaviourValidationConfiguration.toLowerCase() == "true";
                }
                ValidatorEnable($get(_this.validatorAnyNodeId), behaviourValidationConfigurationValue);
            };
            var contactServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Contact");
            this._contactService = new ContactService(contactServiceConfiguration);
            var tenantServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Tenant");
            this._tenantService = new TenantService(tenantServiceConfiguration);
            this._roleValidationSessionKey = uscId + "_validationState";
            sessionStorage.removeItem(this._roleValidationSessionKey);
        }
        uscContactSelRest.prototype._requiredValidationEnabled = function () {
            return JSON.parse(this.requiredValidationEnabled.toLowerCase());
        };
        uscContactSelRest.prototype.initialize = function () {
            this._treeContact = $find(this.treeContactId);
            this._tbContactsControl = $find(this.tbContactsControlId);
            if (this._tbContactsControl) {
                this._tbContactsControl.add_buttonClicking(this.toolbarContacts_onClick);
                this._tbContactsControl.findItemByValue("ADDALL").set_visible(this.addAllDataButtonVisibility.toLowerCase() === "true" ? true : false);
                this._tbContactsControl.findItemByValue("REMOVEALL").set_visible(this.removeAllDataButtonVisibility.toLowerCase() === "true" ? true : false);
            }
            this._rwContactSelector = $find(this.rwContactSelectorId);
            this._rwContactSelector.add_show(this._rwContactSelector_show);
            this._btnContactConfirm = $find(this.btnContactConfirmId);
            this._btnContactConfirm.add_clicking(this.btnContactConfirm_onClick);
            this._btnContactConfirmAndNew = $find(this.btnContactConfirmAndNewId);
            this._btnContactConfirmAndNew.add_clicking(this.btnContactConfirmAndNew_onClick);
            this._btnContactConfirmAndNew.set_visible(this.confirmAndNewEnabled);
            this._manager = $find(this.managerId);
            this._uscContactRest = $("#" + this.uscContactRestId).data();
            $("#" + this.pnlContentId).data(this);
        };
        uscContactSelRest.prototype.renderContactsTree = function (contactCollection) {
            this.enableValidators(contactCollection.length === 0 && this._requiredValidationEnabled() ? true : false);
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
            if (this._requiredValidationEnabled())
                this.enableValidators(false);
            var promise = $.Deferred();
            var currentNode = this.populateNode(contactModel);
            var currentNodeFromTree = this._treeContact.get_allNodes().filter(function (node) { return +node.get_value() === +currentNode.get_value(); })[0];
            if (currentNodeFromTree) {
                return promise.resolve(currentNodeFromTree);
            }
            if (contactModel.IncrementalFather) {
                var parentNode_1 = this._treeContact.get_allNodes().filter(function (node) { return +node.get_value() === contactModel.IncrementalFather; })[0];
                if (!parentNode_1) {
                    this._contactService.getContactParents(contactModel.IncrementalFather, function (data) {
                        parentNode_1 = _this.populateParentNode(data);
                        parentNode_1.get_nodes().add(currentNode);
                        promise.resolve(currentNode);
                    });
                }
                else {
                    parentNode_1.get_nodes().add(currentNode);
                }
            }
            else {
                this._treeContact.get_nodes().add(currentNode);
                return promise.resolve(currentNode);
            }
            return promise.promise();
        };
        uscContactSelRest.prototype.populateNode = function (contactModel) {
            var currentNode = new Telerik.Web.UI.RadTreeNode();
            var currentNodeDescription = contactModel.Description;
            var currentNodeImageUrl = "" + (contactModel.IdContactType || contactModel.ContactType);
            currentNode.set_text(currentNodeDescription.replace('|', ' '));
            currentNode.set_value("" + (contactModel.EntityId || contactModel.Id));
            currentNode.set_imageUrl(contactTypeIcons[currentNodeImageUrl]);
            currentNode.set_expanded(true);
            currentNode.set_contentCssClass('dsw-text-bold');
            return currentNode;
        };
        uscContactSelRest.prototype.populateParentNode = function (contactModel) {
            var parentNode = null;
            var _loop_1 = function (contact) {
                var currentNode = this_1.populateNode(contact);
                currentNode.set_contentCssClass('initial');
                var existingParent = this_1._treeContact.get_allNodes().filter(function (node) { return +node.get_value() === +currentNode.get_value(); })[0];
                if (existingParent) {
                    parentNode = existingParent;
                    return "continue";
                }
                if (!parentNode) {
                    this_1._treeContact.get_nodes().add(currentNode);
                }
                else {
                    parentNode.get_nodes().add(currentNode);
                }
                parentNode = currentNode;
            };
            var this_1 = this;
            for (var _i = 0, contactModel_1 = contactModel; _i < contactModel_1.length; _i++) {
                var contact = contactModel_1[_i];
                _loop_1(contact);
            }
            return parentNode;
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
            if (this._tbContactsControl) {
                this._tbContactsControl.get_items().forEach(function (item) {
                    item.set_enabled(isVisible);
                });
            }
        };
        uscContactSelRest.prototype.forceBehaviourValidationState = function (state) {
            sessionStorage[this._roleValidationSessionKey] = state;
        };
        return uscContactSelRest;
    }());
    return uscContactSelRest;
});
//# sourceMappingURL=uscContattiSelRest.js.map