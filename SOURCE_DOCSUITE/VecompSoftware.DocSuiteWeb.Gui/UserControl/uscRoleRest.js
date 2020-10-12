/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Services/Commons/RoleService", "App/Helpers/ServiceConfigurationHelper", "App/Helpers/ImageHelper", "App/Models/Commons/UscRoleRestEventType", "App/Models/Fascicles/VisibilityType", "../App/Models/Workflows/WorkflowDSWEnvironmentType"], function (require, exports, RoleService, ServiceConfigurationHelper, ImageHelper, UscRoleRestEventType, VisibilityType, DSWEnvironmentType) {
    var uscRoleRest = /** @class */ (function () {
        function uscRoleRest(serviceConfigurations, configuration, uscId) {
            var _this = this;
            this.uscRoleRestEvents = UscRoleRestEventType;
            this._raciRoleCollection = [];
            this._parentPageEventHandlersDictionary = {};
            /**
             * Registers the given callback function as handler for the given event
             * @param eventType
             * @param callback
             */
            this.registerEventHandler = function (eventType, callback, instanceId) {
                _this._instanceId = instanceId;
                _this._parentPageEventHandlersDictionary[eventType] = callback;
            };
            /**
             * Builds the role tree view based on the given role model collection
             * @param roleCollection
             */
            this.renderRolesTree = function (roleCollection) {
                _this._initialRoleCollection = roleCollection ? __spreadArrays(roleCollection) : [];
                _this.enableValidators(roleCollection.length === 0 && _this._requiredValidationEnabled() ? true : false);
                _this.populateRolesTreeView(roleCollection, true, _this._expanded());
            };
            /**
             * Displays tree view validation error
             * */
            this.enableValidators = function (state) {
                var behaviourValidationConfiguration = sessionStorage.getItem(_this._roleValidationSessionKey);
                var behaviourValidationConfigurationValue = state;
                if (behaviourValidationConfiguration) {
                    behaviourValidationConfigurationValue = behaviourValidationConfiguration.toLowerCase() == "true";
                }
                ValidatorEnable($get(_this.validatorAnyNodeId), behaviourValidationConfigurationValue);
            };
            /**
             *  Triggers the event listener of the clicked toolbar button
             */
            this.actionToolbar_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.toolbarActions().filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
                    .map(function (item) { return item[1]; })[0];
                currentAction();
            };
            /**
             * Registers event handlers of UI controls
             * */
            this.bindControlsEvents = function () {
                if (_this._actionToolbar)
                    _this._actionToolbar.add_buttonClicked(_this.actionToolbar_ButtonClicked);
                if (_this._btnExpandRoles) {
                    _this._btnExpandRoles.addCssClass("dsw-arrow-down");
                    _this._btnExpandRoles.add_clicking(_this.btnExpandRoles_OnClick);
                }
                _this._windowSelRole.add_close(_this.bindNewRolesToTree);
            };
            /**
             * Adds new roles to role tree view
             * */
            this.bindNewRolesToTree = function (sender, args) {
                var allSelectedRoles = args.get_argument();
                if (!allSelectedRoles || !allSelectedRoles.length)
                    return;
                var currentTreeNodes = _this._rolesTree.get_allNodes();
                var newRoles = allSelectedRoles.filter(function (role) { return !currentTreeNodes.some(function (node) { return +node.get_value() === role.IdRole && node.get_contentCssClass() === uscRoleRest.BOLD_CSSCLASS; }); });
                if (!newRoles.length)
                    return;
                var parentUpdateCallback = _this._parentPageEventHandlersDictionary[_this.uscRoleRestEvents.NewRolesAdded](newRoles, _this._instanceId);
                parentUpdateCallback.then(function (existedRole, keepChanges) {
                    if (keepChanges === void 0) { keepChanges = false; }
                    if (_this._requiredValidationEnabled())
                        _this.enableValidators(false);
                    if (keepChanges && existedRole) {
                        return;
                    }
                    if (existedRole && _this._multipleRolesEnabled()) {
                        newRoles = newRoles.filter(function (x) { return x.IdRole !== existedRole.IdRole; });
                    }
                    _this._initialRoleCollection = _this._multipleRolesEnabled()
                        ? __spreadArrays(_this._initialRoleCollection, newRoles) : newRoles.length > 0
                        ? [newRoles[0]]
                        : [];
                    _this.populateRolesTreeView(newRoles, !_this._multipleRolesEnabled(), true);
                });
            };
            /**
             * Removes selected node from the tree view
             * */
            this.removeRole = function () {
                var selectedNodeToDelete = _this._rolesTree.get_selectedNode();
                if (!selectedNodeToDelete)
                    return;
                var parentDeleteCallback = _this._parentPageEventHandlersDictionary[_this.uscRoleRestEvents.RoleDeleted](+selectedNodeToDelete.get_value(), _this._instanceId);
                parentDeleteCallback.then(function (data) {
                    _this._initialRoleCollection = _this._initialRoleCollection.filter(function (roleModel) { return roleModel.IdRole !== +selectedNodeToDelete.get_value(); });
                    _this._rolesTree.trackChanges();
                    _this.removeNodeFromTree(selectedNodeToDelete, selectedNodeToDelete.get_parent());
                    _this._rolesTree.commitChanges();
                    if (_this._requiredValidationEnabled() && _this._rolesTree.get_allNodes().length === 0) {
                        _this.enableValidators(true);
                    }
                });
            };
            /*
             * Expands or hides the tree view
             * */
            this.btnExpandRoles_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this._isContentExpanded) {
                    _this._rowContent.hide();
                    _this._isContentExpanded = false;
                    _this._btnExpandRoles.removeCssClass("dsw-arrow-down");
                    _this._btnExpandRoles.addCssClass("dsw-arrow-up");
                }
                else {
                    _this._rowContent.show();
                    _this._isContentExpanded = true;
                    _this._btnExpandRoles.removeCssClass("dsw-arrow-up");
                    _this._btnExpandRoles.addCssClass("dsw-arrow-down");
                }
            };
            /**
             * Add all roles from the tree view
             * */
            this.addAllRoles = function () {
                _this._windowManager.radconfirm("Sei sicuro di voler aggiungere tutti i settori?", function (arg) {
                    if (arg) {
                        _this._parentPageEventHandlersDictionary[_this.uscRoleRestEvents.AllRolesAdded]();
                    }
                }, 400, 300);
            };
            /**
             * Removes all roles from the tree view
             * */
            this.removeAllRoles = function () {
                _this._windowManager.radconfirm("Sei sicuro di voler eliminare tutti i settori?", function (arg) {
                    if (arg) {
                        var deleteCallback = _this._parentPageEventHandlersDictionary[_this.uscRoleRestEvents.AllRolesDeleted]();
                        deleteCallback.then(function () { return _this._rolesTree.get_nodes().clear(); });
                    }
                }, 400, 300);
            };
            /**
             * Set the selected role from tree view as RACI role
             * */
            this.setRaciRole = function () {
                var selectedNode = _this._rolesTree.get_selectedNode();
                selectedNode.set_imageUrl(uscRoleRest.RACI_ROLE_ICON);
                _this.disableRaciRoleButton();
                var selectedRole = _this._initialRoleCollection.filter(function (x) { return x.IdRole === +selectedNode.get_value(); })[0];
                _this._raciRoleCollection.push(selectedRole);
            };
            this.setFascicleVisibilityType = function () {
                var btnFascicleVisibilityType = _this._actionToolbar
                    .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
                if (btnFascicleVisibilityType && btnFascicleVisibilityType.get_visible()) {
                    var checked = btnFascicleVisibilityType.get_checked();
                    var visibilityType = checked ? VisibilityType.Accessible : VisibilityType.Confidential;
                    _this._parentPageEventHandlersDictionary[_this.uscRoleRestEvents.SetFascicleVisibilityType](visibilityType);
                }
            };
            this.rolesTree_onNodeClick = function (sender, args) {
                if (args.get_node().get_contentCssClass() === uscRoleRest.DISABLED_CSSCLASS) {
                    _this.disableRaciRoleButton();
                    _this._actionToolbar.findItemByValue(uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME).disable();
                }
                else if (args.get_node().get_imageUrl() === uscRoleRest.RACI_ROLE_ICON) {
                    _this.disableRaciRoleButton();
                }
                else {
                    _this._actionToolbar.findItemByValue(uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME).enable();
                    _this._actionToolbar.findItemByValue(uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME).enable();
                }
            };
            this.disableRaciRoleButton = function () {
                _this._actionToolbar.findItemByValue(uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME).disable();
            };
            this.setRaciRoles = function (raciRoles) {
                _this._raciRoleCollection = raciRoles;
            };
            this.getRaciRoles = function () {
                return _this._raciRoleCollection;
            };
            this.setFascicleVisibilityTypeButtonCheck = function (fascicleVisibilityType) {
                var btnFascicleVisibilityType = _this._actionToolbar
                    .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
                if (btnFascicleVisibilityType && btnFascicleVisibilityType.get_visible()) {
                    btnFascicleVisibilityType.set_checked(fascicleVisibilityType === VisibilityType[VisibilityType.Accessible]);
                }
            };
            this.setVisibilityOnFascicleVisibilityTypeButton = function (fascicleVisibilityTypeButtonVisibility) {
                var btnFascicleVisibilityType = _this._actionToolbar
                    .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
                if (btnFascicleVisibilityType) {
                    btnFascicleVisibilityType.set_visible(fascicleVisibilityTypeButtonVisibility);
                }
            };
            var roleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
            this._roleService = new RoleService(roleServiceConfiguration);
            this._uscId = uscId;
            this.setConfiguration(configuration);
            this._roleValidationSessionKey = uscId + "_validationState";
            sessionStorage.removeItem(this._roleValidationSessionKey);
        }
        uscRoleRest.prototype._multipleRolesEnabled = function () {
            return JSON.parse(this.multipleRoles.toLowerCase());
        };
        uscRoleRest.prototype._requiredValidationEnabled = function () {
            return JSON.parse(this.requiredValidationEnabled.toLowerCase());
        };
        uscRoleRest.prototype._expanded = function () {
            return JSON.parse(this.expanded.toLowerCase());
        };
        uscRoleRest.prototype.toolbarActions = function () {
            var _this = this;
            var items = [
                [uscRoleRest.ADD_TOOLBAR_ACTION_KEYNAME, function () { return _this.addRoles(); }],
                [uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME, function () { return _this.removeRole(); }],
                [uscRoleRest.ADD_ALL_TOOLBAR_ACTION_KEYNAME, function () { return _this.addAllRoles(); }],
                [uscRoleRest.DELETE_ALL_TOOLBAR_ACTION_KEYNAME, function () { return _this.removeAllRoles(); }],
                [uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME, function () { return _this.setRaciRole(); }],
                [uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME, function () { return _this.setFascicleVisibilityType(); }]
            ];
            return items;
        };
        uscRoleRest.prototype.initialize = function () {
            var _this = this;
            this._actionToolbar = $find(this.actionToolbarId);
            this._rolesTree = $find(this.rolesTreeId);
            this._rolesTree.add_nodeClicked(this.rolesTree_onNodeClick);
            this._windowManager = $find(this.windowManagerId);
            this._windowSelRole = $find(this.windowSelRoleId);
            this._rowContent = $("#".concat(this.contentRowId));
            this._rowContent.show();
            this._isContentExpanded = true;
            this._btnExpandRoles = $find(this.btnExpandRolesId);
            this.bindControlsEvents();
            $("#" + this.pnlContentId).data(this);
            $("#" + this.pnlContentId).triggerHandler(uscRoleRest.LOADED_EVENT);
            var configuration = this.getConfiguration();
            if (!configuration.isReadOnlyMode && this._actionToolbar) {
                this._actionToolbar.findItemByValue(uscRoleRest.ADD_ALL_TOOLBAR_ACTION_KEYNAME)
                    .set_visible(this.allDataButtonEnabled.toLowerCase() === "true" ? true : false);
                this._actionToolbar.findItemByValue(uscRoleRest.DELETE_ALL_TOOLBAR_ACTION_KEYNAME)
                    .set_visible(this.removeAllDataButtonEnabled.toLowerCase() === "true" ? true : false);
                this._actionToolbar.findItemByValue(uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME)
                    .set_visible(this.raciButtonEnabled);
            }
            if (this._actionToolbar) {
                var btnFascicleVisibilityType = this._actionToolbar
                    .findItemByValue(uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME);
                if (btnFascicleVisibilityType) {
                    btnFascicleVisibilityType.set_visible(this.fascicleVisibilityTypeButtonEnabled);
                }
            }
            $("#" + this.pnlContentId).on(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, function (event, entityType, entityId) {
                if (entityType && entityId) {
                    _this.addExternalSource(entityType, entityId);
                }
            });
            $("#" + this.pnlContentId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE);
        };
        /**
         * Returns the configuration object from session storage
        */
        uscRoleRest.prototype.getConfiguration = function () {
            var configuration = {};
            if (sessionStorage[this._configurationRoleSessionKey]) {
                configuration = JSON.parse(sessionStorage[this._configurationRoleSessionKey]);
            }
            return configuration;
        };
        /**
         * Set the configuration object in session storage
        */
        uscRoleRest.prototype.setConfiguration = function (configuration) {
            this._configurationRoleSessionKey = this._uscId + "_configuration";
            sessionStorage[this._configurationRoleSessionKey] = JSON.stringify(configuration);
        };
        /**
         * Populates roles tree view with given roles
         * @param roleTreeModels
         */
        uscRoleRest.prototype.populateRolesTreeView = function (rolesCollection, clearTree, isExpanded) {
            var _this = this;
            var configuration = this.getConfiguration();
            var treeViewContainsNodes = this._rolesTree.get_allNodes().length > 0;
            if (clearTree && treeViewContainsNodes) {
                this._rolesTree.get_nodes().clear();
            }
            rolesCollection.forEach(function (roleModel) {
                _this.buildTreeViewRecursive(roleModel, configuration.isReadOnlyMode, isExpanded);
            });
        };
        uscRoleRest.prototype.clearRoleTreeView = function () {
            this._rolesTree.get_nodes().clear();
        };
        /**
         * Creates and returns a RadTreeNode based on the given role model
         * @param roleModel
         * @param isReadOnlyMode
         */
        uscRoleRest.prototype.createTreeNodeFromRoleModel = function (roleModel, isReadOnlyMode, isExpanded) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            var treeNodeDescription = isReadOnlyMode && roleModel.ActiveFrom ? roleModel.Name + " - autorizzato il " + roleModel.ActiveFrom : "" + roleModel.Name;
            var treeNodeImageUrl = this._raciRoleCollection && this._raciRoleCollection.some(function (x) { return x.IdRole === roleModel.IdRole; })
                ? uscRoleRest.RACI_ROLE_ICON
                : roleModel.IdRoleFather === null || roleModel.IdRoleFather === undefined
                    ? ImageHelper.roleRootNodeImageUrl
                    : ImageHelper.roleChildNodeImageUrl;
            treeNode.set_text(treeNodeDescription);
            treeNode.set_value("" + roleModel.IdRole);
            treeNode.set_imageUrl(treeNodeImageUrl);
            treeNode.set_contentCssClass(roleModel.IsActive ? uscRoleRest.BOLD_CSSCLASS : uscRoleRest.DISABLED_CSSCLASS);
            treeNode.set_expanded(isExpanded);
            return treeNode;
        };
        /**
         * Sets tree node style as bolded if included in initial role database collection
         * @param treeNode
         */
        uscRoleRest.prototype.setNodeAsBoldIfFromInitialCollection = function (treeNode) {
            var roleIsFromDatabase = this._initialRoleCollection.some(function (roleModel) { return roleModel.IdRole === +treeNode.get_value(); });
            treeNode.set_contentCssClass(roleIsFromDatabase ? uscRoleRest.BOLD_CSSCLASS : uscRoleRest.DISABLED_CSSCLASS);
        };
        /**
         * Creates the tree view, fetching missing role parents
         * @param roleModel
         * @param isReadOnlyMode
         */
        uscRoleRest.prototype.buildTreeViewRecursive = function (roleModel, isReadOnlyMode, isExpanded) {
            var _this = this;
            var promise = $.Deferred();
            // Checks if current role already exists in the tree view
            var currentNodeFromTree = this._rolesTree.get_allNodes().filter(function (node) { return +node.get_value() === roleModel.IdRole; })[0];
            if (currentNodeFromTree) {
                currentNodeFromTree.set_expanded(isExpanded);
                this.setNodeAsBoldIfFromInitialCollection(currentNodeFromTree);
                return promise.resolve(currentNodeFromTree);
            }
            var currentTreeNode = this.createTreeNodeFromRoleModel(roleModel, isReadOnlyMode, isExpanded);
            if (roleModel.IdRoleFather) {
                this._roleService.findRole(roleModel.IdRoleFather, function (parentRole) {
                    _this.buildTreeViewRecursive(parentRole, isReadOnlyMode, isExpanded)
                        .then(function (parentNode) {
                        var parentNodeFromTree = parentNode.get_allNodes().filter(function (node) { return +node.get_value() === +currentTreeNode.get_value(); })[0];
                        _this.setNodeAsBoldIfFromInitialCollection(parentNode);
                        if (parentNodeFromTree) {
                            return promise.resolve(parentNodeFromTree);
                        }
                        else {
                            parentNode.get_nodes().add(currentTreeNode);
                            parentNode.set_expanded(isExpanded);
                            return promise.resolve(currentTreeNode);
                        }
                    });
                });
            }
            else {
                this._rolesTree.get_nodes().add(currentTreeNode);
                promise.resolve(currentTreeNode);
            }
            return promise.promise();
        };
        /**
         * Removes the selected tree node and it's disabled parents from the tree view
         * @param selectedNodeToDelete
         * @param nodeToDeleteParent
         */
        uscRoleRest.prototype.removeNodeFromTree = function (selectedNodeToDelete, selectedNodeParent) {
            var nodeToDeleteIsRootNode = this._rolesTree.get_nodes().toArray().some(function (rootNode) { return rootNode.get_value() === selectedNodeToDelete.get_value(); });
            if (nodeToDeleteIsRootNode) {
                this._rolesTree.get_nodes().remove(selectedNodeToDelete);
                return;
            }
            var parentNodeChildren = selectedNodeParent.get_nodes();
            parentNodeChildren.remove(selectedNodeToDelete);
            var parentHasChildrenLeft = parentNodeChildren.get_count() > 0;
            var parentIsDisabled = selectedNodeParent.get_contentCssClass() === uscRoleRest.DISABLED_CSSCLASS;
            if (!parentHasChildrenLeft && parentIsDisabled) {
                this.removeNodeFromTree(selectedNodeParent, selectedNodeParent.get_parent());
            }
        };
        /**
         * Opens roles selection window
         * */
        uscRoleRest.prototype.addRoles = function () {
            var url = "../UserControl/CommonSelRoleRest.aspx?Type=Comm&MultipleRoles=" + this.multipleRoles + "&OnlyMyRoles=" + this.onlyMyRoles;
            if (this.entityType) {
                url = url + "&EntityType=" + this.entityType;
            }
            if (this.entityId) {
                url = url + "&EntityId=" + this.entityId;
            }
            if (this.loadAllRoles) {
                url = url + "&LoadAllRoles=" + this.loadAllRoles;
            }
            if (this.dswEnvironmentType == DSWEnvironmentType[DSWEnvironmentType.Document]) {
                url = url + "&DSWEnvironment=" + this.dswEnvironmentType;
            }
            localStorage.setItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST, JSON.stringify(this._initialRoleCollection));
            this._windowManager.open(url, "windowSelRole", undefined);
        };
        uscRoleRest.prototype.setToolbarVisibility = function (isVisible) {
            this._actionToolbar.get_items().forEach(function (item) {
                item.set_enabled(isVisible);
            });
        };
        uscRoleRest.prototype.disableButtons = function () {
            if (this._actionToolbar) {
                this._actionToolbar.get_items().forEach(function (item) {
                    item.set_enabled(false);
                });
            }
        };
        uscRoleRest.prototype.enableButtons = function () {
            if (this._actionToolbar) {
                this._actionToolbar.get_items().forEach(function (item) {
                    item.set_enabled(true);
                });
            }
        };
        uscRoleRest.prototype.existsRole = function (roles) {
            for (var _i = 0, roles_1 = roles; _i < roles_1.length; _i++) {
                var role = roles_1[_i];
                if (this.exists(role.IdRole)) {
                    return role;
                }
            }
            return null;
        };
        uscRoleRest.prototype.exists = function (id) {
            return this._rolesTree.get_allNodes()
                .filter(function (x) { return x.get_contentCssClass() !== uscRoleRest.DISABLED_CSSCLASS && +x.get_value() === id; }).length > 0;
        };
        uscRoleRest.prototype.addExternalSource = function (entityType, entityId) {
            this.entityType = entityType;
            this.entityId = entityId;
        };
        uscRoleRest.prototype.forceBehaviourValidationState = function (state) {
            sessionStorage[this._roleValidationSessionKey] = state;
        };
        uscRoleRest.prototype.setToolbarRoleVisibility = function (isVisible) {
            if (this._actionToolbar) {
                if (isVisible) {
                    $("#" + this.actionToolbarId).show();
                }
                else {
                    $("#" + this.actionToolbarId).hide();
                }
            }
        };
        uscRoleRest.DISABLED_CSSCLASS = "node-disabled";
        uscRoleRest.BOLD_CSSCLASS = "dsw-text-bold";
        uscRoleRest.RACI_ROLE_ICON = "../App_Themes/DocSuite2008/imgset16/Admin.png";
        uscRoleRest.LOADED_EVENT = "onLoaded";
        uscRoleRest.ADD_TOOLBAR_ACTION_KEYNAME = "add";
        uscRoleRest.DELETE_TOOLBAR_ACTION_KEYNAME = "delete";
        uscRoleRest.ADD_ALL_TOOLBAR_ACTION_KEYNAME = "addAll";
        uscRoleRest.DELETE_ALL_TOOLBAR_ACTION_KEYNAME = "deleteAll";
        uscRoleRest.SET_RACI_ROLE_TOOLBAR_ACTION_KEYNAME = "setRaciRole";
        uscRoleRest.SET_FASCICLE_VISIBILITY_TYPE_TOOLBAR_ACTION_KEYNAME = "setFascicleVisibilityType";
        uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE = "onNeedRolesFromExternalSource";
        uscRoleRest.LOCAL_STORAGE_ROLE_REST = "uscRoleRest_setInitialRoleCollection";
        return uscRoleRest;
    }());
    return uscRoleRest;
});
//# sourceMappingURL=uscRoleRest.js.map