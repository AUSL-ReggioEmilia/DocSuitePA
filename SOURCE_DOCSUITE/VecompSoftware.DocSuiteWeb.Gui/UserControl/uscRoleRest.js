/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "../App/Services/Commons/RoleService", "../App/Helpers/ServiceConfigurationHelper", "../App/Helpers/ImageHelper", "../App/Models/Commons/UscRoleRestEventType"], function (require, exports, RoleService, ServiceConfigurationHelper, ImageHelper, UscRoleRestEventType) {
    var uscRoleRest = /** @class */ (function () {
        function uscRoleRest(serviceConfigurations, configuration, uscId) {
            var _this = this;
            this.uscRoleRestEvents = UscRoleRestEventType;
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
                _this.enableValidators(roleCollection.length === 0 && _this._requiredValidationEnabled ? true : false);
                _this.populateRolesTreeView(roleCollection, true, _this._expanded);
            };
            /**
             * Displays tree view validation error
             * */
            this.enableValidators = function (state) {
                ValidatorEnable($get(_this.validatorAnyNodeId), state);
            };
            /**
             *  Triggers the event listener of the clicked toolbar button
             */
            this.actionToolbar_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.toolbarActions.filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
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
                parentUpdateCallback.then(function (data) {
                    if (_this._requiredValidationEnabled)
                        _this.enableValidators(false);
                    _this._initialRoleCollection = _this._multipleRolesEnabled
                        ? __spreadArrays(_this._initialRoleCollection, newRoles) : [newRoles[0]];
                    _this.populateRolesTreeView(newRoles, !_this._multipleRolesEnabled, true);
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
                    if (_this._requiredValidationEnabled && _this._rolesTree.get_allNodes().length === 0) {
                        _this.enableValidators(true);
                    }
                });
            };
            /**
             * Opens roles selection window
             * */
            this.addRoles = function () {
                var url = "../UserControl/CommonSelRoleRest.aspx?Type=Comm&MultipleRoles=" + _this.multipleRoles;
                _this._windowManager.open(url, "windowSelRole", undefined);
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
            var roleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
            this._roleService = new RoleService(roleServiceConfiguration);
            this._configurationRoleSessionKey = uscId + "_configuration";
            sessionStorage[this._configurationRoleSessionKey] = JSON.stringify(configuration);
        }
        Object.defineProperty(uscRoleRest.prototype, "_multipleRolesEnabled", {
            get: function () {
                return JSON.parse(this.multipleRoles.toLowerCase());
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscRoleRest.prototype, "_requiredValidationEnabled", {
            get: function () {
                return JSON.parse(this.requiredValidationEnabled.toLowerCase());
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscRoleRest.prototype, "_expanded", {
            get: function () {
                return JSON.parse(this.expanded.toLowerCase());
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscRoleRest.prototype, "toolbarActions", {
            get: function () {
                var _this = this;
                var items = [
                    ["add", function () { return _this.addRoles(); }],
                    ["delete", function () { return _this.removeRole(); }]
                ];
                return items;
            },
            enumerable: true,
            configurable: true
        });
        uscRoleRest.prototype.initialize = function () {
            this._actionToolbar = $find(this.actionToolbarId);
            this._rolesTree = $find(this.rolesTreeId);
            this._windowManager = $find(this.windowManagerId);
            this._windowSelRole = $find(this.windowSelRoleId);
            this._rowContent = $("#".concat(this.contentRowId));
            this._rowContent.show();
            this._isContentExpanded = true;
            this._btnExpandRoles = $find(this.btnExpandRolesId);
            this.bindControlsEvents();
            $("#" + this.pnlContentId).data(this);
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
        /**
         * Creates and returns a RadTreeNode based on the given role model
         * @param roleModel
         * @param isReadOnlyMode
         */
        uscRoleRest.prototype.createTreeNodeFromRoleModel = function (roleModel, isReadOnlyMode, isExpanded) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            var treeNodeDescription = isReadOnlyMode && roleModel.ActiveFrom ? roleModel.Name + " - autorizzato il " + roleModel.ActiveFrom : "" + roleModel.Name;
            var treeNodeImageUrl = roleModel.IdRoleFather === null ? ImageHelper.roleRootNodeImageUrl : ImageHelper.roleChildNodeImageUrl;
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
        uscRoleRest.prototype.setToolbarVisibility = function (isVisible) {
            this._actionToolbar.get_items().forEach(function (item) {
                item.set_enabled(isVisible);
            });
        };
        uscRoleRest.DISABLED_CSSCLASS = "node-disabled";
        uscRoleRest.BOLD_CSSCLASS = "dsw-text-bold";
        return uscRoleRest;
    }());
    return uscRoleRest;
});
//# sourceMappingURL=uscRoleRest.js.map