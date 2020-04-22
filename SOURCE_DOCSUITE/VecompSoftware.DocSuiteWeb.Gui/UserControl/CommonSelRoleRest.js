define(["require", "exports", "../App/Helpers/ServiceConfigurationHelper", "../App/Services/Commons/RoleService", "../App/Helpers/ImageHelper", "../App/Models/Environment"], function (require, exports, ServiceConfigurationHelper, RoleService, ImageHelper, Environment) {
    var CommonSelRoleRest = /** @class */ (function () {
        function CommonSelRoleRest(serviceConfigurations) {
            var _this = this;
            this.btnSearch_OnClick = function (sender, args) {
                sender.preventDefault();
                _this.searchRolesByDescription(_this.descriptionFilterValue.val());
            };
            this.btnSearchCode_OnClick = function (sender, args) {
                sender.preventDefault();
                _this.searchRolesByCode(_this.codeFilterValue.val());
            };
            this.btnConfirm_OnClick = function (sender, args) {
                sender.preventDefault();
                var selectedTreeNodes = _this._multipleRolesEnabled ? _this._rolesTree.get_checkedNodes() : [_this._rolesTree.get_selectedNode()];
                var selectedRoles = selectedTreeNodes.map(function (roleNode) { return _this.getRoleModelFromNode(roleNode); });
                _this.closeWindow(selectedRoles);
            };
            this.checkAllNodes = function () {
                var rootNode = _this._rolesTree.get_nodes().getNode(0);
                var treeNodes = rootNode.get_allNodes();
                treeNodes.forEach(function (treeNode) {
                    var currentTreeNodeClass = treeNode.get_cssClass();
                    treeNode.set_checked(true);
                    treeNode.set_cssClass(currentTreeNodeClass + " " + CommonSelRoleRest.BOLD_CSSCLASS);
                    treeNode.set_expanded(true);
                });
            };
            this.uncheckAllNodes = function () {
                var rootNode = _this._rolesTree.get_nodes().getNode(0);
                var treeNodes = rootNode.get_allNodes();
                treeNodes.forEach(function (treeNode) {
                    var currentTreeNodeClass = treeNode.get_cssClass();
                    treeNode.set_checked(false);
                    treeNode.set_cssClass(currentTreeNodeClass.replace("dsw-text-bold", ""));
                });
            };
            var roleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
            this._roleService = new RoleService(roleServiceConfiguration);
        }
        Object.defineProperty(CommonSelRoleRest.prototype, "multipleRolesEnabled", {
            set: function (value) {
                this._multipleRolesEnabled = JSON.parse(value);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelRoleRest.prototype, "multiTenantEnabled", {
            set: function (value) {
                this._multitenantEnabled = JSON.parse(value);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelRoleRest.prototype, "descriptionFilterValue", {
            get: function () {
                return $("#" + this.descriptionFilterInputId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelRoleRest.prototype, "codeFilterValue", {
            get: function () {
                return $("#" + this.codeSearchFilterInputId);
            },
            enumerable: true,
            configurable: true
        });
        CommonSelRoleRest.prototype.initialize = function () {
            this._rolesTree = $find(this.rolesTreeId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this.bindControlsEvents();
            this.initializeRolesTree();
            $("#" + this.pnlMainContentId).data(this);
        };
        CommonSelRoleRest.prototype.searchRoles = function (finderModel) {
            var _this = this;
            this._loadingPanel.show(this.pnlMainContentId);
            this._roleService.findRoles(finderModel, function (successResult) {
                _this.buildRolesTreeNodes(successResult);
                _this._loadingPanel.hide(_this.pnlMainContentId);
            }, function (exception) {
                _this.showNotificationException(exception);
                _this._loadingPanel.hide(_this.pnlMainContentId);
            });
        };
        CommonSelRoleRest.prototype.initializeRolesTree = function () {
            var defaultSearchRoleFinderModel = this.getDefaultFinderModel();
            this.searchRoles(defaultSearchRoleFinderModel);
        };
        CommonSelRoleRest.prototype.buildRolesTreeNodes = function (roleModels) {
            if (this._rolesTree.get_nodes().get_count() > 0)
                this._rolesTree.get_nodes().clear();
            // build and add tree root node
            var rootNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text("Settori");
            rootNode.set_expanded(true);
            rootNode.set_checkable(false);
            this._rolesTree.get_nodes().add(rootNode);
            // recursively build and add other tree nodes
            this.buildNodesRecursive(roleModels, rootNode);
        };
        CommonSelRoleRest.prototype.addNodeAttributes = function (roleNode, roleModel) {
            roleNode.get_attributes().setAttribute("IdRole", roleModel.IdRole);
            roleNode.get_attributes().setAttribute("IdRoleFather", roleModel.IdRoleFather);
            roleNode.get_attributes().setAttribute("UniqueId", roleModel.UniqueId);
            roleNode.get_attributes().setAttribute("EntityShortId", roleModel.EntityShortId ? roleModel.EntityShortId : roleModel.IdRole);
            roleNode.get_attributes().setAttribute("Name", roleModel.Name);
            roleNode.get_attributes().setAttribute("IdRoleTenant", roleModel.IdRoleTenant);
            roleNode.get_attributes().setAttribute("TenantId", roleModel.TenantId);
            roleNode.get_attributes().setAttribute("IsActive", roleModel.IsActive);
            roleNode.get_attributes().setAttribute("ServiceCode", roleModel.ServiceCode);
            roleNode.get_attributes().setAttribute("ActiveFrom", roleModel.ActiveFrom);
            roleNode.get_attributes().setAttribute("FullIncrementalPath", roleModel.FullIncrementalPath);
        };
        CommonSelRoleRest.prototype.getRoleModelFromNode = function (roleNode) {
            var roleModel = {
                IdRole: roleNode.get_attributes().getAttribute("IdRole"),
                IdRoleFather: roleNode.get_attributes().getAttribute("IdRoleFather"),
                UniqueId: roleNode.get_attributes().getAttribute("UniqueId"),
                EntityShortId: roleNode.get_attributes().getAttribute("EntityShortId"),
                Name: roleNode.get_attributes().getAttribute("Name"),
                IdRoleTenant: roleNode.get_attributes().getAttribute("IdRoleTenant"),
                TenantId: roleNode.get_attributes().getAttribute("TenantId"),
                IsActive: roleNode.get_attributes().getAttribute("IsActive"),
                ServiceCode: roleNode.get_attributes().getAttribute("ServiceCode"),
                ActiveFrom: roleNode.get_attributes().getAttribute("ActiveFrom"),
                FullIncrementalPath: roleNode.get_attributes().getAttribute("FullIncrementalPath"),
                Children: []
            };
            return roleModel;
        };
        CommonSelRoleRest.prototype.buildNodesRecursive = function (roleModels, parentNode) {
            var _this = this;
            roleModels.forEach(function (roleModel) {
                var currentNode = new Telerik.Web.UI.RadTreeNode();
                _this.addNodeAttributes(currentNode, roleModel);
                var currentNodeImageUrl = roleModel.IdRoleFather === null ? ImageHelper.roleRootNodeImageUrl : ImageHelper.roleChildNodeImageUrl;
                currentNode.set_text(roleModel.ServiceCode ? roleModel.Name + " (" + roleModel.ServiceCode + ")" : "" + roleModel.Name);
                currentNode.set_value("" + roleModel.IdRole);
                currentNode.set_imageUrl(currentNodeImageUrl);
                parentNode.get_nodes().add(currentNode);
                if (_this.hasFilters()) {
                    parentNode.set_expanded(true);
                    var descriptionSearchAndContains = _this.descriptionFilterValue.val() !== "" && _this.lowerCaseContains(roleModel.Name, _this.descriptionFilterValue.val());
                    if (descriptionSearchAndContains || (_this.codeFilterValue.val() !== "" && roleModel.ServiceCode === _this.codeFilterValue.val())) {
                        var currentNodeClass = currentNode.get_cssClass();
                        currentNode.set_cssClass(currentNodeClass + " " + CommonSelRoleRest.BOLD_CSSCLASS);
                    }
                    if (roleModel.Children.length)
                        currentNode.set_expanded(true);
                }
                if (roleModel.Children.length)
                    _this.buildNodesRecursive(roleModel.Children, currentNode);
            });
        };
        CommonSelRoleRest.prototype.searchRolesByDescription = function (descriptionFilterValue) {
            var descriptionSearchRoleFinderModel = this.getDefaultFinderModel();
            descriptionSearchRoleFinderModel.Name = descriptionFilterValue;
            this.searchRoles(descriptionSearchRoleFinderModel);
        };
        CommonSelRoleRest.prototype.searchRolesByCode = function (codeFilterValue) {
            var _this = this;
            var codeSearchRoleFinderModel = this.getDefaultFinderModel();
            codeSearchRoleFinderModel.ServiceCode = codeFilterValue;
            this._loadingPanel.show(this.pnlMainContentId);
            this._roleService.findRoles(codeSearchRoleFinderModel, function (successResult) {
                _this.buildRolesTreeNodes(successResult);
                var filteredRole = _this._rolesTree.get_allNodes().filter(function (node) { return node.get_cssClass().indexOf("dsw-text-bold") !== -1; })[0];
                _this.closeWindow([_this.getRoleModelFromNode(filteredRole)]);
            }, function (exception) {
                _this.showNotificationException(exception);
                _this._loadingPanel.hide(_this.pnlMainContentId);
            });
        };
        CommonSelRoleRest.prototype.closeWindow = function (selectedRoles) {
            var wnd = this.getRadWindow();
            wnd.close(selectedRoles);
        };
        CommonSelRoleRest.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        CommonSelRoleRest.prototype.filterRoles = function (roleCollection, name) {
            var _this = this;
            var filteredRoles = roleCollection.filter(function (role) {
                if (role.Children.length)
                    role.Children = _this.filterRoles(role.Children, name);
                var predicate = _this.lowerCaseContains(role.Name, name);
                return predicate;
            });
            return filteredRoles;
        };
        CommonSelRoleRest.prototype.hasFilters = function () {
            return this.descriptionFilterValue.val().length !== 0 || this.codeFilterValue.val().length !== 0;
        };
        CommonSelRoleRest.prototype.lowerCaseContains = function (str1, str2) {
            return str1.toLowerCase().indexOf(str2.toLowerCase()) !== -1;
        };
        CommonSelRoleRest.prototype.bindControlsEvents = function () {
            $("#" + this.descriptionSearchBtnId).click(this.btnSearch_OnClick);
            $("#" + this.codeSearchBtnId).click(this.btnSearchCode_OnClick);
            $("#" + this.confirmSelectionBtnId).click(this.btnConfirm_OnClick);
            $("#" + this.selectAllBtnId).click(this.checkAllNodes);
            $("#" + this.unselectAllBtnId).click(this.uncheckAllNodes);
        };
        CommonSelRoleRest.prototype.getDefaultFinderModel = function () {
            return {
                Name: null,
                ParentId: null,
                ServiceCode: null,
                TenantId: null,
                Environment: Environment.Any,
                LoadOnlyRoot: false,
                LoadOnlyMy: this._multitenantEnabled,
                LoadAlsoParent: true
            };
        };
        CommonSelRoleRest.prototype.showNotificationException = function (exception, customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception) {
                    uscNotification.showNotification(exception);
                    return;
                }
                uscNotification.showWarningMessage(customMessage);
            }
        };
        CommonSelRoleRest.BOLD_CSSCLASS = "dsw-text-bold";
        return CommonSelRoleRest;
    }());
    return CommonSelRoleRest;
});
//# sourceMappingURL=CommonSelRoleRest.js.map