var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/RoleService", "App/Helpers/ImageHelper", "App/Models/Environment", "App/Services/Processes/ProcessService", "App/Services/Dossiers/DossierFolderService", "App/Helpers/ExternalSourceActionEnum", "App/Services/Commons/CategoryService", "./uscRoleRest", "App/Helpers/GenericHelper", "App/Models/Workflows/WorkflowDSWEnvironmentType"], function (require, exports, ServiceConfigurationHelper, RoleService, ImageHelper, Environment, ProcessService, DossierFolderService, ExternalSourceActionEnum, CategoryService, uscRoleRest, GenericHelper, DSWEnvironmentType) {
    var CommonSelRoleRest = /** @class */ (function () {
        function CommonSelRoleRest(serviceConfigurations) {
            var _this = this;
            this._initialRoleCollection = [];
            this._getRolesFromExternalSourceActionsDictionary = {};
            this.btnSearch_OnClick = function (sender, args) {
                sender.preventDefault();
                if (_this.entityType) {
                    _this.searchExternalRolesRecursive(_this.descriptionFilterValue().val());
                }
                else {
                    _this.searchRolesByDescription(_this.descriptionFilterValue().val());
                }
            };
            this.btnSearchCode_OnClick = function (sender, args) {
                sender.preventDefault();
                _this.searchRolesByCode(_this.codeFilterValue().val());
            };
            this.btnConfirm_OnClick = function (sender, args) {
                var selectedTreeNodes = _this._multipleRolesEnabled
                    ? _this._rolesTree.get_checkedNodes()
                    : _this._rolesTree.get_selectedNode() ? [_this._rolesTree.get_selectedNode()] : [];
                if (!selectedTreeNodes.length) {
                    _this.showNotificationException(null, "Nessun settore selezionato");
                    return;
                }
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
                _this._btnConfirm.set_enabled(true);
            };
            this.uncheckAllNodes = function () {
                var rootNode = _this._rolesTree.get_nodes().getNode(0);
                var treeNodes = rootNode.get_allNodes();
                treeNodes.forEach(function (treeNode) {
                    var currentTreeNodeClass = treeNode.get_cssClass();
                    treeNode.set_checked(false);
                    treeNode.set_cssClass(currentTreeNodeClass.replace("dsw-text-bold", ""));
                });
                _this._btnConfirm.set_enabled(false);
            };
            this.renderRolesTree = function (roleCollection, isExpanded) {
                if (isExpanded === void 0) { isExpanded = false; }
                _this._initialRoleCollection = roleCollection ? __spreadArrays(roleCollection) : [];
                _this.populateRolesTreeView(roleCollection, true, isExpanded);
            };
            this.rolesTree_onNodeClicked = function (sender, args) {
                var clickedNode = args.get_node();
                _this._btnConfirm.set_enabled(clickedNode.get_value());
            };
            this.rolesTree_onNodeChecked = function (sender, args) {
                var checkedNodes = _this._rolesTree.get_checkedNodes();
                _this._btnConfirm.set_enabled(checkedNodes.length > 0);
            };
            this.rolesTree_onNodeClicking = function (sender, args) {
                if (args.get_node().get_attributes().getAttribute("isReadOnly")) {
                    args.set_cancel(true);
                }
                else {
                    args.get_node().check();
                }
            };
            var roleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Role");
            this._roleService = new RoleService(roleServiceConfiguration);
            var processConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var categoryServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryServiceConfiguration);
        }
        Object.defineProperty(CommonSelRoleRest.prototype, "multipleRolesEnabled", {
            set: function (value) {
                this._multipleRolesEnabled = JSON.parse(value);
            },
            enumerable: true,
            configurable: true
        });
        CommonSelRoleRest.prototype.descriptionFilterValue = function () {
            return $("#" + this.descriptionFilterInputId);
        };
        CommonSelRoleRest.prototype.codeFilterValue = function () {
            return $("#" + this.codeSearchFilterInputId);
        };
        CommonSelRoleRest.prototype.initialize = function () {
            this._rolesTree = $find(this.rolesTreeId);
            this._rolesTree.add_nodeClicking(this.rolesTree_onNodeClicking);
            this._rolesTree.add_nodeClicked(this.rolesTree_onNodeClicked);
            this._rolesTree.add_nodeChecked(this.rolesTree_onNodeChecked);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnConfirm = $find(this.confirmSelectionBtnId);
            this._btnConfirm.set_enabled(false);
            this.registerGetRolesFromExternalSourceActions();
            this.bindControlsEvents();
            this._savedRoles = [];
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
            var dswEnvironment = GenericHelper.getUrlParams(window.location.href, "DSWEnvironment");
            var defaultSearchRoleFinderModel = this.getDefaultFinderModel();
            if (dswEnvironment && dswEnvironment == DSWEnvironmentType[DSWEnvironmentType.Document]) {
                defaultSearchRoleFinderModel = this.getDossierDefaultFinderModel();
            }
            else {
                defaultSearchRoleFinderModel = this.getDefaultFinderModel();
            }
            if (localStorage.getItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST)) {
                this._savedRoles = JSON.parse(localStorage.getItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST));
                localStorage.removeItem(uscRoleRest.LOCAL_STORAGE_ROLE_REST);
            }
            if (this.entityType) {
                var externalSourceAction = this._getRolesFromExternalSourceActionsDictionary[this.entityType];
                if (externalSourceAction) {
                    externalSourceAction(this.entityId);
                }
            }
            else {
                this.searchRoles(defaultSearchRoleFinderModel);
            }
        };
        CommonSelRoleRest.prototype.buildRolesTreeNodes = function (roleModels) {
            var rootNode = this.createRootNode();
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
                Children: [],
                CategoryFascicleRights: []
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
                    var descriptionSearchAndContains = _this.descriptionFilterValue().val() !== "" && _this.lowerCaseContains(roleModel.Name, _this.descriptionFilterValue().val());
                    var codeFilterValueToLower = _this.codeFilterValue().val().toLowerCase();
                    if (descriptionSearchAndContains || (codeFilterValueToLower !== "" && roleModel.ServiceCode && roleModel.ServiceCode.toLowerCase() === codeFilterValueToLower)) {
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
            if (this.entityType) {
                var filteredRolesByCode = [];
                switch (+this.entityType) {
                    case ExternalSourceActionEnum.Process: {
                        filteredRolesByCode = this.allProcessRoles.filter(function (x) { return x.ServiceCode.toLowerCase() === codeSearchRoleFinderModel.ServiceCode.toLowerCase(); });
                        break;
                    }
                    case ExternalSourceActionEnum.DossierFolder: {
                        filteredRolesByCode = this.allDossierFolderRoles.filter(function (x) { return x.Role.ServiceCode.toLowerCase() === codeSearchRoleFinderModel.ServiceCode.toLowerCase(); }).map(function (x) { return x.Role; });
                        break;
                    }
                    case ExternalSourceActionEnum.Category: {
                        filteredRolesByCode = this.allCategoryRoles.filter(function (x) { return x.ServiceCode.toLowerCase() === codeSearchRoleFinderModel.ServiceCode.toLowerCase(); });
                        break;
                    }
                }
                this.addSelectedRoleByCode(filteredRolesByCode);
            }
            else {
                this._loadingPanel.show(this.pnlMainContentId);
                this._roleService.findRoles(codeSearchRoleFinderModel, function (successResult) {
                    _this._loadingPanel.hide(_this.pnlMainContentId);
                    _this.addSelectedRoleByCode(successResult);
                }, function (exception) {
                    _this.showNotificationException(exception);
                    _this._loadingPanel.hide(_this.pnlMainContentId);
                });
            }
        };
        CommonSelRoleRest.prototype.addSelectedRoleByCode = function (roles) {
            if (roles.length > 1) {
                this.showNotificationException(null, "Il codice cercato non è univoco");
                return;
            }
            if (roles.length === 0) {
                this.showNotificationException(null, "Il codice cercato è inesistente");
                return;
            }
            this.buildRolesTreeNodes(roles);
            var filteredRole = this._rolesTree.get_allNodes().filter(function (node) { return node.get_cssClass().indexOf("dsw-text-bold") !== -1; })[0];
            this.closeWindow([this.getRoleModelFromNode(filteredRole)]);
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
            return this.descriptionFilterValue().val().length !== 0 || this.codeFilterValue().val().length !== 0;
        };
        CommonSelRoleRest.prototype.lowerCaseContains = function (str1, str2) {
            return str1.toLowerCase().indexOf(str2.toLowerCase()) !== -1;
        };
        CommonSelRoleRest.prototype.bindControlsEvents = function () {
            $("#" + this.descriptionSearchBtnId).click(this.btnSearch_OnClick);
            $("#" + this.codeSearchBtnId).click(this.btnSearchCode_OnClick);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
            $("#" + this.selectAllBtnId).click(this.checkAllNodes);
            $("#" + this.unselectAllBtnId).click(this.uncheckAllNodes);
        };
        CommonSelRoleRest.prototype.getDefaultFinderModel = function () {
            return {
                Name: null,
                ParentId: null,
                ServiceCode: null,
                TenantId: this.loadAllRoles ? null : this.tenantId,
                Environment: Environment.Any,
                LoadOnlyRoot: false,
                LoadOnlyMy: this.onlyMyRoles,
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
        CommonSelRoleRest.prototype.showNotificationMessage = function (customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        CommonSelRoleRest.prototype.registerGetRolesFromExternalSourceActions = function () {
            var _this = this;
            this._getRolesFromExternalSourceActionsDictionary[ExternalSourceActionEnum.Process] = function (processId) {
                if (processId) {
                    _this._processService.getById(processId, function (data) {
                        var process = data;
                        _this.allProcessRoles = process.Roles;
                        _this.renderRolesTree(process.Roles);
                    }, function (exception) {
                        _this.showNotificationException(exception);
                    });
                }
            };
            this._getRolesFromExternalSourceActionsDictionary[ExternalSourceActionEnum.Category] = function (categoryId) {
                if (categoryId) {
                    _this._categoryService.getRolesByCategoryId(categoryId, function (data) {
                        _this.categoryModel = data;
                        var categoryFascicleModel = _this.categoryModel.CategoryFascicles[0];
                        if (categoryFascicleModel) {
                            var roleArray = [];
                            for (var _i = 0, _a = categoryFascicleModel.CategoryFascicleRights; _i < _a.length; _i++) {
                                var cfrm = _a[_i];
                                roleArray.push(cfrm.Role);
                            }
                            _this.allCategoryRoles = roleArray;
                            _this.renderRolesTree(roleArray);
                        }
                    }, function (exception) {
                        _this.showNotificationException(exception);
                    });
                }
            };
            this._getRolesFromExternalSourceActionsDictionary[ExternalSourceActionEnum.DossierFolder] = function (dossierFolderId) {
                _this._dossierFolderService.getDossierFolderById(dossierFolderId, function (data) {
                    var dossierFolder = data[0];
                    _this.allDossierFolderRoles = dossierFolder.DossierFolderRoles;
                    _this.renderRolesTree(dossierFolder.DossierFolderRoles.map(function (x) { return x.Role; }));
                });
            };
        };
        CommonSelRoleRest.prototype.populateRolesTreeView = function (rolesCollection, clearTree, isExpanded) {
            var _this = this;
            var rootNode = this.createRootNode();
            var treeViewContainsNodes = this._rolesTree.get_allNodes().length > 0;
            if (clearTree && treeViewContainsNodes) {
                this._rolesTree.get_nodes().getNode(0).get_nodes().clear();
            }
            rolesCollection.forEach(function (roleModel) {
                _this.buildTreeViewRecursive(roleModel, isExpanded);
            });
        };
        CommonSelRoleRest.prototype.buildTreeViewRecursive = function (roleModel, isExpanded) {
            var _this = this;
            var promise = $.Deferred();
            var currentNodeFromTree = this._rolesTree.get_allNodes().filter(function (node) { return +node.get_value() === roleModel.IdRole; })[0];
            if (currentNodeFromTree) {
                currentNodeFromTree.set_expanded(isExpanded);
                this.setNodeAsBoldIfFromInitialCollection(currentNodeFromTree);
                return promise.resolve(currentNodeFromTree);
            }
            var currentTreeNode = this.createTreeNodeFromRoleModel(roleModel, isExpanded);
            this.addNodeAttributes(currentTreeNode, roleModel);
            if (roleModel.IdRoleFather) {
                this._roleService.findRole(roleModel.IdRoleFather, function (parentRole) {
                    _this.buildTreeViewRecursive(parentRole, isExpanded)
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
                this._rolesTree.get_nodes().getNode(0).get_nodes().add(currentTreeNode);
                promise.resolve(currentTreeNode);
            }
            return promise.promise();
        };
        CommonSelRoleRest.prototype.setNodeAsBoldIfFromInitialCollection = function (treeNode) {
            var roleIsFromDatabase = this._initialRoleCollection.some(function (roleModel) { return roleModel.IdRole === +treeNode.get_value(); });
            treeNode.set_contentCssClass(roleIsFromDatabase ? "dsw-text-bold" : "node-disabled");
            treeNode.get_attributes().setAttribute("isReadOnly", !roleIsFromDatabase);
            if (!roleIsFromDatabase) {
                treeNode.set_checkable(false);
            }
        };
        CommonSelRoleRest.prototype.createTreeNodeFromRoleModel = function (roleModel, isExpanded) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            var treeNodeDescription = roleModel.ActiveFrom ? roleModel.Name + " - autorizzato il " + roleModel.ActiveFrom : "" + roleModel.Name;
            var treeNodeImageUrl = roleModel.IdRoleFather === null ? ImageHelper.roleRootNodeImageUrl : ImageHelper.roleChildNodeImageUrl;
            treeNode.set_text(treeNodeDescription);
            treeNode.set_value("" + roleModel.IdRole);
            treeNode.set_imageUrl(treeNodeImageUrl);
            treeNode.set_contentCssClass(roleModel.IsActive ? "dsw-text-bold" : "node-disabled");
            treeNode.set_expanded(isExpanded);
            treeNode.get_attributes().setAttribute("isReadOnly", roleModel.IsActive !== 1);
            if (!roleModel.IsActive) {
                treeNode.set_checkable(false);
            }
            if (this._savedRoles.some(function (x) { return x.IdRole === roleModel.IdRole; })) {
                treeNode.set_contentCssClass(CommonSelRoleRest.BLUE_NODE_CSSCLASS);
                treeNode.get_attributes().setAttribute("isReadOnly", true);
                treeNode.set_checkable(false);
            }
            return treeNode;
        };
        CommonSelRoleRest.prototype.getDossierDefaultFinderModel = function () {
            return {
                Name: null,
                ParentId: null,
                ServiceCode: null,
                TenantId: this.loadAllRoles ? null : this.tenantId,
                Environment: Environment.Document,
                LoadOnlyRoot: false,
                LoadOnlyMy: this.onlyMyRoles,
                LoadAlsoParent: true
            };
        };
        CommonSelRoleRest.prototype.createRootNode = function () {
            if (this._rolesTree.get_nodes().get_count() > 0)
                this._rolesTree.get_nodes().clear();
            // build and add tree root node
            var rootNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text("Settori");
            rootNode.set_expanded(true);
            rootNode.set_checkable(false);
            this._rolesTree.get_nodes().add(rootNode);
            return rootNode;
        };
        CommonSelRoleRest.prototype.searchExternalRolesRecursive = function (searchText) {
            var filteredRoles = [];
            switch (this.entityType) {
                case ExternalSourceActionEnum.Process.toString(): {
                    filteredRoles = this.allProcessRoles.filter(function (x) { return x.Name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1; });
                    break;
                }
                case ExternalSourceActionEnum.DossierFolder.toString(): {
                    filteredRoles = this.allDossierFolderRoles.filter(function (x) { return x.Role.Name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1; }).map(function (x) { return x.Role; });
                    break;
                }
                case ExternalSourceActionEnum.Category.toString(): {
                    filteredRoles = this.allCategoryRoles.filter(function (x) { return x.Name.toLowerCase().indexOf(searchText.toLowerCase()) !== -1; });
                    break;
                }
            }
            this.renderRolesTree(filteredRoles, true);
        };
        CommonSelRoleRest.BOLD_CSSCLASS = "dsw-text-bold";
        CommonSelRoleRest.BLUE_NODE_CSSCLASS = "node-tree-fascicle";
        return CommonSelRoleRest;
    }());
    return CommonSelRoleRest;
});
//# sourceMappingURL=CommonSelRoleRest.js.map