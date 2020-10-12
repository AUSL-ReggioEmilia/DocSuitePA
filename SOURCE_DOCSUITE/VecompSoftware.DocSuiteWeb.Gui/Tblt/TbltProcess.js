var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Helpers/EnumHelper", "App/Services/Processes/ProcessService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Processes/ProcessNodeType", "App/Services/Dossiers/DossierFolderService", "UserControl/uscProcessDetails", "App/Models/Dossiers/DossierFolderStatus", "UserControl/uscCategoryRest", "App/Models/Commons/CategoryModel", "App/Services/Dossiers/DossierService", "App/Services/Processes/ProcessFascicleTemplateService", "App/DTOs/ExceptionDTO", "UserControl/uscRoleRest", "App/Models/Processes/ProcessType", "App/Models/InsertActionType", "App/Models/UpdateActionType", "App/Services/Commons/CategoryService", "App/Helpers/ExternalSourceActionEnum", "UserControl/CommonSelCategoryRest", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, EnumHelper, ProcessService, ServiceConfigurationHelper, ProcessNodeType, DossierFolderService, uscProcessDetails, DossierFolderStatus, uscCategoryRest, CategoryModel, DossierService, ProcessFascicleTemplateService, ExceptionDTO, uscRoleRest, ProcessType, InsertActionType, UpdateActionType, CategoryService, ExternalSourceActionEnum, CommonSelCategoryRest, SessionStorageKeysHelper) {
    var TbltProcess = /** @class */ (function () {
        function TbltProcess(serviceConfigurations) {
            var _this = this;
            this.tempPFTModel = [];
            this.addCategoryEventHandler = function (data, args) {
                var categoryId = args;
                if (categoryId) {
                    _this._uscProcessRoleRest.clearRoleTreeView();
                    _this._categoryService.getRolesByCategoryId(categoryId, function (data) {
                        _this.categoryModel = data;
                        var categoryFascicleModel = _this.categoryModel.CategoryFascicles[0];
                        var categoryFascicleRightsModel;
                        if (categoryFascicleModel) {
                            categoryFascicleRightsModel = categoryFascicleModel.CategoryFascicleRights;
                            if (categoryFascicleRightsModel) {
                                var roleArray = [];
                                for (var _i = 0, categoryFascicleRightsModel_1 = categoryFascicleRightsModel; _i < categoryFascicleRightsModel_1.length; _i++) {
                                    var cfrm = categoryFascicleRightsModel_1[_i];
                                    roleArray.push(cfrm.Role);
                                }
                                if (roleArray) {
                                    //set popup roles source
                                    var needRolesFromExternalSource_eventArgs = [ExternalSourceActionEnum.Category.toString(), categoryId];
                                    $("#" + _this.uscProcessRoleRestId).triggerHandler(uscRoleRest.NEED_ROLES_FROM_EXTERNAL_SOURCE, needRolesFromExternalSource_eventArgs);
                                    _this._uscProcessRoleRest.renderRolesTree(roleArray);
                                    _this.processRoles = roleArray;
                                    _this._uscProcessRoleRest.enableButtons();
                                }
                                else {
                                    _this._uscProcessRoleRest.disableButtons();
                                }
                            }
                            else {
                                _this._uscProcessRoleRest.disableButtons();
                            }
                        }
                        else {
                            _this._uscProcessRoleRest.disableButtons();
                        }
                    }, function (exception) {
                        _this.showNotificationException(exception);
                    });
                }
            };
            this.deleteRolePromise = function (roleIdToDelete, senderId) {
                var promise = $.Deferred();
                if (!roleIdToDelete)
                    return promise.promise();
                _this.processRoles = _this.processRoles
                    .filter(function (role) { return role.IdRole !== roleIdToDelete || role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
                promise.resolve(_this.processRoles);
                return promise.promise();
            };
            this.updateRolesPromise = function (newAddedRoles, senderId) {
                var promise = $.Deferred();
                if (!newAddedRoles.length)
                    return promise.promise();
                _this.processRoles = __spreadArrays(_this.processRoles, newAddedRoles);
                promise.resolve(_this.processRoles);
                return promise.promise();
            };
            this.rtvProcess_onExpand = function (sender, args) {
                var expandedNode = args.get_node();
                if (expandedNode.get_level() === 0) {
                    expandedNode.get_nodes().clear();
                    _this._initializeProcessesTree();
                }
                else if (expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Category) {
                    _this.expandCategoryNode(expandedNode);
                }
                else if (expandedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
                    _this.loadProcesses(expandedNode.get_parent().get_value());
                }
                else if (expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Process) {
                    _this.expandNodeLogic(expandedNode);
                    _this.loadData(expandedNode.get_attributes().getAttribute("idDossier"), 0, expandedNode.get_value());
                }
                else if (expandedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) !== ProcessNodeType.Category) {
                    if (expandedNode.get_nodes().getNode(0).get_text() === "") {
                        expandedNode.get_nodes().clear();
                        _this.loadData(expandedNode.get_value(), 0);
                    }
                    else {
                        for (var index = 0; index < expandedNode.get_nodes().get_count(); index++) {
                            _this.expandNodeLogic(expandedNode.get_nodes().getNode(index));
                        }
                    }
                }
            };
            this.rtvProcesses_nodeClicked = function (sender, args) {
                var selectedNode = args.get_node();
                _this.showNodeDetails(selectedNode);
            };
            this.rbProcessInsert_onCLick = function (sender, args) {
                _this.hideInsertInputs();
                var selectedNode = _this._rtvProcesses.get_selectedNode();
                _this.clearInputs();
                if (selectedNode.get_level() === 0) {
                    _this.selectedProcessId = "";
                    $("#insertProcess").show();
                }
                else {
                    _this.selectedProcessFascicleTemplateId = "";
                    $("#insertFascicleTemplate").show();
                }
                _this.processRoles = [];
                _this._rwInsert.show();
            };
            this.folderToolBar_onClick = function (sender, args) {
                _this.hideInsertInputs();
                switch (args.get_item().get_value()) {
                    case TbltProcess.TOOLBAR_CREATE: {
                        _this.clearInputs();
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        if (selectedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
                            _this.selectedProcessId = "";
                            _this._uscCategoryRest.disableButtons();
                            var categoryNodeText = selectedNode.get_parent().get_text();
                            var categoryNodeValue = +selectedNode.get_parent().get_value();
                            var category = {};
                            category.Code = +categoryNodeText.split(".")[0];
                            category.Name = categoryNodeText.split(".")[1];
                            category.IdCategory = categoryNodeValue;
                            _this._uscCategoryRest.updateSessionStorageSelectedCategory(category);
                            _this._uscCategoryRest.populateCategotyTree(category);
                            $("#" + _this.uscCategoryRestId).triggerHandler(uscCategoryRest.ADDED_EVENT, category.IdCategory);
                            $("#insertProcess").show();
                            _this._rwInsert.set_title("Aggiungi serie");
                        }
                        switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                            case ProcessNodeType.Process:
                            case ProcessNodeType.DossierFolder: {
                                _this.selectedDossierFolderId = "";
                                $("#insertDossierFolder").show();
                                _this._rwInsert.set_title("Aggiungi volume");
                                break;
                            }
                        }
                        _this._rwInsert.show();
                        break;
                    }
                    case TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE: {
                        _this.selectedProcessFascicleTemplateId = "";
                        $("#insertFascicleTemplate").show();
                        _this._rwInsert.set_title("Aggiungi modello di fascicolo di processo");
                        _this._rwInsert.show();
                        break;
                    }
                    case TbltProcess.TOOLBAR_DELETE: {
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                            case ProcessNodeType.Process: {
                                var yesterdayDate = new Date();
                                yesterdayDate.setDate(new Date().getDate() - 1);
                                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                                _this.removeProcess(yesterdayDate);
                                break;
                            }
                            case ProcessNodeType.DossierFolder: {
                                if (uscProcessDetails.processFascicleWorkflowRepositories.length > 0) {
                                    alert("Impossibile eliminare il volume. Esiste un flusso di lavoro associato.");
                                    return;
                                }
                                var dossierFolder = {};
                                dossierFolder.UniqueId = _this._rtvProcesses.get_selectedNode().get_value();
                                if (_this._rtvProcesses.get_selectedNode().get_parent().get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Process) {
                                    dossierFolder.ParentInsertId = _this.getProcessNodeByChild(_this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute("idDossier");
                                }
                                else {
                                    dossierFolder.ParentInsertId = _this._rtvProcesses.get_selectedNode().get_parent().get_value();
                                }
                                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                                if (window.confirm("Vuoi eliminare il volume selezionato?")) {
                                    _this._dossierFolderService.deleteDossierFolder(dossierFolder, function (data) {
                                        _this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(_this._rtvProcesses.get_selectedNode());
                                        _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                    }, function (error) {
                                        _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                        _this.showNotificationException(error);
                                    });
                                }
                                else {
                                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                                }
                                break;
                            }
                            case ProcessNodeType.ProcessFascicleTemplate: {
                                var yesterdayDate = new Date();
                                _this._ajaxLoadingPanel.show(_this.rtvProcessesId);
                                _this.removeFascicleTemplate(yesterdayDate);
                                break;
                            }
                        }
                        break;
                    }
                    case TbltProcess.TOOLBAR_MODIFY: {
                        _this.hideInsertInputs();
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                            case ProcessNodeType.Process: {
                                _this.selectedProcessId = _this._rtvProcesses.get_selectedNode().get_value();
                                _this.populateProcessInputs(_this.selectedProcessId);
                                for (var _i = 0, _a = _this.processesModel; _i < _a.length; _i++) {
                                    var processToFind = _a[_i];
                                    if (processToFind.UniqueId === _this.selectedProcessId) {
                                        _this.processRoles = processToFind.Roles;
                                        _this._uscProcessRoleRest.renderRolesTree(processToFind.Roles);
                                        break;
                                    }
                                }
                                _this._uscCategoryRest.disableButtons();
                                $("#insertProcess").show();
                                _this._rwInsert.set_title("Modifica serie");
                                break;
                            }
                            case ProcessNodeType.ProcessFascicleTemplate: {
                                _this.selectedProcessFascicleTemplateId = _this._rtvProcesses.get_selectedNode().get_value();
                                _this._rtbFascicleTemplateName.set_value(_this._rtvProcesses.get_selectedNode().get_text());
                                $("#insertFascicleTemplate").show();
                                _this._rwInsert.set_title("Modifica modello di fascicolo");
                                break;
                            }
                            case ProcessNodeType.DossierFolder: {
                                _this.selectedDossierFolderId = _this._rtvProcesses.get_selectedNode().get_value();
                                _this._rtbDossierFolderName.set_value(_this._rtvProcesses.get_selectedNode().get_text());
                                $("#insertDossierFolder").show();
                                _this._rwInsert.set_title("Modifica volume");
                                break;
                            }
                        }
                        _this._rwInsert.show();
                        break;
                    }
                    case TbltProcess.TOOLBAR_CLONE: {
                        _this.clearInputs();
                        _this.selectedDossierFolderId = "";
                        _this._rtbCloneDossierFolderName.set_value("");
                        $("#cloneDossierFolder").show();
                        _this._rwInsert.set_title("Duplica volume");
                        _this._rwInsert.show();
                        break;
                    }
                    case TbltProcess.TOOLBAR_COPYPFT: {
                        var selectedNode_1 = _this._rtvProcesses.get_selectedNode();
                        var fascicleTemplate_1 = {};
                        fascicleTemplate_1.Name = selectedNode_1.get_text();
                        fascicleTemplate_1.StartDate = new Date();
                        fascicleTemplate_1.EndDate = null;
                        _this._uscProcessDetails.populateFascicleTemplateInfo().then(function (jsonModel) {
                            fascicleTemplate_1.JsonModel = jsonModel;
                            fascicleTemplate_1.Process = {};
                            fascicleTemplate_1.Process.UniqueId = _this.getProcessNodeByChild(selectedNode_1).get_value();
                            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL, JSON.stringify(fascicleTemplate_1));
                        });
                        break;
                    }
                    case TbltProcess.TOOLBAR_PASTEPFT: {
                        var selectedNode = _this._rtvProcesses.get_selectedNode();
                        var fascicleTemplate = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL));
                        fascicleTemplate.DossierFolder = {};
                        fascicleTemplate.DossierFolder.UniqueId = selectedNode.get_value();
                        _this.insertPFT(selectedNode, fascicleTemplate);
                        break;
                    }
                }
            };
            this.rbConfirmInsert_onCLick = function (sender, args) {
                var selectedNode = _this._rtvProcesses.get_selectedNode();
                if ($("#cloneDossierFolder").is(":visible")) {
                    return _this.cloneDossierFolder(selectedNode);
                }
                if ($("#insertDossierFolder").is(":visible")) {
                    return _this.insertOrUpdateDossierFolder(selectedNode);
                }
                if ($("#insertProcess").is(":visible")) {
                    return _this.insertOrUpdateProcess(selectedNode);
                }
                if ($("#insertFascicleTemplate").is(":visible")) {
                    return _this.insertOrUpdateFascicleTemplate(selectedNode);
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        TbltProcess.prototype.initialize = function () {
            this.initializeServices();
            this.initializeControls();
            this.initializeUserControls();
            this._ajaxLoadingPanel.show(this.processViewPaneId);
            this.enableFolderToolbarButtons(false);
            this._initializeProcessesTree();
            this.processFascicleTemplatesModel = [];
            this.processRoles = [];
            sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL);
        };
        TbltProcess.prototype.initializeServices = function () {
            var processConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Process_TYPE_NAME);
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.DossierFolder_TYPE_NAME);
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var dossierConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Dossier_TYPE_NAME);
            this._dossierService = new DossierService(dossierConfiguration);
            var fascicleTemplateConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.ProcessFascicleTemplate_TYPE_NAME);
            this._fascicleTemplateService = new ProcessFascicleTemplateService(fascicleTemplateConfiguration);
            var categoryServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltProcess.Category_TYPE_NAME);
            this._categoryService = new CategoryService(categoryServiceConfiguration);
        };
        TbltProcess.prototype.initializeControls = function () {
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this._rtvProcesses = $find(this.rtvProcessesId);
            this._rtvProcesses.add_nodeClicked(this.rtvProcesses_nodeClicked);
            this._rtvProcesses.get_nodes().getNode(0).get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, ProcessNodeType.Root);
            this._rtvProcesses.add_nodeExpanded(this.rtvProcess_onExpand);
            this._rtvProcesses.get_nodes().getNode(0).expand();
            this._rwInsert = $find(this.rwInsertId);
            this._folderToolBar = $find(this.folderToolBarId);
            this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
            this._rtbProcessName = $find(this.rtbProcessNameId);
            this._rtbDossierFolderName = $find(this.rtbDossierFolderNameId);
            this._rtbFascicleTemplateName = $find(this.rtbFascicleTemplateNameId);
            this._rtbCloneDossierFolderName = $find(this.rtbCloneDossierFolderNameId);
            this._rbConfirm = $find(this.rbConfirmId);
            this._rbConfirm.add_clicked(this.rbConfirmInsert_onCLick);
            this._rcbProcessNote = $find(this.rcbProcessNoteId);
            this._filterToolbar = $find(this.filterToolbarId);
            //TODO: feature will be completed in > 9.01
            //this._filterToolbar.add_buttonClicked(this.filterToolbar_onClick);
            this._rtbProcessSearchName = this._filterToolbar.findItemByValue("searchInput").findControl("txtSearch");
        };
        TbltProcess.prototype.initializeUserControls = function () {
            this._uscCategoryRest = $("#" + this.uscCategoryRestId).data();
            this._uscProcessRoleRest = $("#" + this.uscProcessRoleRestId).data();
            this._uscProcessRoleRest.renderRolesTree([]);
            this.registerUscRoleRestEventHandlers();
            this._uscProcessRoleRest.disableButtons();
            this._uscCategoryRest.registerAddedEventhandler(this.addCategoryEventHandler);
        };
        TbltProcess.prototype._initializeProcessesTree = function () {
            this.loadFascicolableCategories();
        };
        TbltProcess.prototype.registerUscRoleRestEventHandlers = function () {
            var uscRoleRestEventsDictionary = this._uscProcessRoleRest.uscRoleRestEvents;
            this._uscProcessRoleRest.registerEventHandler(uscRoleRestEventsDictionary.RoleDeleted, this.deleteRolePromise, this.uscProcessRoleRestId);
            this._uscProcessRoleRest.registerEventHandler(uscRoleRestEventsDictionary.NewRolesAdded, this.updateRolesPromise, this.uscProcessRoleRestId);
        };
        TbltProcess.prototype.loadFascicolableCategories = function () {
            var _this = this;
            this._categoryService.getOnlyFascicolableCategories(this.currentTenantAOOId, function (data) {
                if (!data) {
                    return;
                }
                _this._processCategories = data;
                _this._rtvProcesses.get_nodes().getNode(0).showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                var categoryIds = _this.getCategoryIds(0, null);
                _this.loadCategories(categoryIds, null);
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.loadCategories = function (categoryIds, parentId) {
            var _this = this;
            this._categoryService.getCategoriesByIds(categoryIds, this.currentTenantAOOId, function (data) {
                if (!data) {
                    return;
                }
                var categoryChildren = data;
                var parentNode = parentId !== null
                    ? _this._rtvProcesses.findNodeByValue(parentId.toString())
                    : _this._rtvProcesses.get_nodes().getNode(0);
                for (var _i = 0, categoryChildren_1 = categoryChildren; _i < categoryChildren_1.length; _i++) {
                    var categoryChild = categoryChildren_1[_i];
                    var categoryNode = _this.createTreeNodeFromCategoryModel(categoryChild, false);
                    parentNode.get_nodes().add(categoryNode);
                }
                if (parentId !== null) {
                    _this.createProcessesNode(parentNode);
                }
                parentNode.hideLoadingStatus();
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.expandCategoryNode = function (expandedNode) {
            expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            expandedNode.get_nodes().clear();
            var expandedCategoryId = +expandedNode.get_value();
            var categoryIds = this.getCategoryIds(expandedNode.get_level(), expandedCategoryId);
            this.loadCategories(categoryIds, expandedCategoryId);
        };
        TbltProcess.prototype.getCategoryIds = function (nodeLevel, parentCategoryId) {
            var expandedCategoryChildren = parentCategoryId !== null
                ? this._processCategories.filter(function (x) { return +x.FullIncrementalPath.split('|')[nodeLevel] === parentCategoryId && x.EntityShortId !== parentCategoryId; })
                : this._processCategories;
            var categoryIds = expandedCategoryChildren.map(function (x) { return +x.FullIncrementalPath.split('|')[nodeLevel + 1]; }).filter(function (x) { return !isNaN(x); });
            var distinctCategoryIds = [];
            for (var _i = 0, categoryIds_1 = categoryIds; _i < categoryIds_1.length; _i++) {
                var categoryId = categoryIds_1[_i];
                if (distinctCategoryIds.indexOf(categoryId) === -1) {
                    distinctCategoryIds.push(categoryId);
                }
            }
            return distinctCategoryIds;
        };
        TbltProcess.prototype.createTreeNodeFromCategoryModel = function (category, expanded) {
            if (expanded === void 0) { expanded = true; }
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            var treeNodeDescription = category.Code + "." + category.Name;
            var treeNodeImageUrl = "../Comm/images/Classificatore.gif";
            treeNode.set_text(treeNodeDescription);
            treeNode.set_value("" + category.EntityShortId);
            treeNode.set_imageUrl(treeNodeImageUrl);
            treeNode.set_contentCssClass((category.CategoryFascicles && category.CategoryFascicles.length > 0) ? "node-fascicle" : "node-no-fascicle");
            treeNode.get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, ProcessNodeType.Category);
            if (category.CategoryFascicles) {
                this.createProcessesNode(treeNode);
            }
            treeNode.set_expanded(expanded);
            return treeNode;
        };
        TbltProcess.prototype.createProcessesNode = function (parentNode) {
            var processesNode = new Telerik.Web.UI.RadTreeNode();
            processesNode.set_text(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT);
            this.createNoItemsFoundNode(processesNode);
            parentNode.get_nodes().add(processesNode);
        };
        TbltProcess.prototype.createNoItemsFoundNode = function (parentNode) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text(CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT);
            parentNode.get_nodes().add(emptyNode);
        };
        TbltProcess.prototype.loadProcesses = function (categoryId) {
            var _this = this;
            this._processService.getProcessesByCategoryId(categoryId, function (data) {
                if (!data)
                    return;
                _this.processesModel = data;
                if (_this.processesModel.length > 0) {
                    _this._rtvProcesses.findNodeByValue(categoryId.toString()).get_nodes().getNode(0).get_nodes().clear();
                }
                ;
                for (var _i = 0, _a = _this.processesModel; _i < _a.length; _i++) {
                    var process = _a[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    var isActive = process.EndDate === null || new Date(process.EndDate) > new Date();
                    _this.createNode(node, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png", ProcessNodeType.Process, false, null, isActive, process.Dossier.UniqueId, _this._rtvProcesses.findNodeByValue(categoryId.toString()).get_nodes().getNode(0));
                    node.get_attributes().setAttribute(TbltProcess.ProcessType_TYPE_NAME, process.ProcessType);
                    node.get_attributes().setAttribute(TbltProcess.Category_ID_TYPE, process.Category.EntityShortId);
                    if (ProcessType[process.ProcessType.toString()] === ProcessType.Defined) {
                        _this.createEmptyNode(node.get_nodes());
                    }
                    _this._rtvProcesses.commitChanges();
                }
                if (_this.defaultSelectedProcessId.length) {
                    var treeNode = _this._rtvProcesses.findNodeByValue(_this.defaultSelectedProcessId);
                    if (treeNode) {
                        treeNode.set_selected(true);
                        _this.showNodeDetails(treeNode);
                    }
                }
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.processViewPaneId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.createEmptyNode = function (nodes) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text("");
            nodes.add(emptyNode);
        };
        TbltProcess.prototype.expandNodeLogic = function (expandedNodeChild) {
            expandedNodeChild.collapse();
            expandedNodeChild.get_nodes().clear();
            var dossierFolderStatus = expandedNodeChild.get_attributes().getAttribute("DossierFolderStatus");
            if (dossierFolderStatus === DossierFolderStatus[DossierFolderStatus.Folder]) {
                this.createEmptyNode(expandedNodeChild.get_nodes());
            }
        };
        TbltProcess.prototype.showNodeDetails = function (selectedNode) {
            this.initializeNodeClicked(selectedNode);
            if (selectedNode.get_level() === 0
                || selectedNode.get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT) {
                $("#" + this._uscProcessDetails.pnlDetailsId).hide();
                this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
            }
            else if (selectedNode.get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
                this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE).set_enabled(true);
                $("#" + this._uscProcessDetails.pnlDetailsId).hide();
                this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
            }
            else {
                switch (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME)) {
                    case ProcessNodeType.Category: {
                        this.initializeCategoryNodeDetails(selectedNode);
                        break;
                    }
                    case ProcessNodeType.Process: {
                        this.initializeProcessNodeDetails(selectedNode);
                        break;
                    }
                    case ProcessNodeType.DossierFolder: {
                        this.initializeDossierFolderNodeDetails(selectedNode);
                        break;
                    }
                    case ProcessNodeType.ProcessFascicleTemplate: {
                        this.initializeProcessFascicleTemplateNodeDetails(selectedNode);
                        break;
                    }
                }
            }
        };
        TbltProcess.prototype.initializeNodeClicked = function (selectedNode) {
            this._uscProcessDetails = $("#" + this.uscProcessDetailsId).data();
            this._uscProcessDetails.clearProcessDetails();
            this.enableFolderToolbarButtons(false);
            $("#" + this._uscProcessDetails.pnlDetailsId).show();
            document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlTitle")).style.position = "absolute";
            document.getElementById(this._uscProcessDetails.uscFascicleFoldersId.replace("_pageContent", "").concat("_pnlFolderToolbar")).style.position = "absolute";
            this._uscProcessDetails.clearFascicleInputs();
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, true);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.InformationDetails_PanelName, true);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.CategoryInformationDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.WorkflowDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.FascicleDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.RoleDetails_PanelName, true);
        };
        TbltProcess.prototype.initializeCategoryNodeDetails = function (selectedNode) {
            uscProcessDetails.selectedCategoryId = +selectedNode.get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.Category;
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.InformationDetails_PanelName, false);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.CategoryInformationDetails_PanelName, true);
            this._uscProcessDetails.setCategoryDetails();
        };
        TbltProcess.prototype.initializeProcessNodeDetails = function (selectedNode) {
            uscProcessDetails.selectedProcessId = selectedNode.get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.Category;
            uscProcessDetails.selectedCategoryId = selectedNode.get_attributes().getAttribute(TbltProcess.Category_ID_TYPE);
            this._uscProcessDetails.setProcessDetails('', true);
            this.enableFolderToolbarButtons(true);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_DELETE).set_enabled(false);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE).set_enabled(false);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CLONE).set_enabled(false);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(false);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(false);
        };
        TbltProcess.prototype.initializeDossierFolderNodeDetails = function (selectedNode) {
            this._uscProcessDetails.setPanelLoading(uscProcessDetails.WorkflowDetails_PanelName, true);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.WorkflowDetails_PanelName, true);
            var processParentNode = this.getProcessNodeByChild(selectedNode);
            uscProcessDetails.selectedDossierFolderId = selectedNode.get_value();
            uscProcessDetails.selectedProcessId = processParentNode.get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.DossierFolder;
            this.enableFolderToolbarButtons(true);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CLONE).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(false);
            if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == null ||
                sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == "[]") {
                this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(false);
            }
            else {
                this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(true);
            }
            this._uscProcessDetails.setProcessDetails(selectedNode.get_text(), false);
            this._uscProcessDetails.setDossierFolderRoles();
            this._uscProcessDetails.setDossierFolderWorkflowRepositories();
        };
        TbltProcess.prototype.initializeProcessFascicleTemplateNodeDetails = function (selectedNode) {
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.FascicleDetails_PanelName, true);
            this._uscProcessDetails.setPanelVisibility(uscProcessDetails.RoleDetails_PanelName, false);
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_PROCESS_FASCICLE_TEMPLATE_NAME, selectedNode.get_text());
            document.getElementById("pnlMainFascicleFolder").style.position = "initial";
            this.enableFolderToolbarButtons(false);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_DELETE).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_MODIFY).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(true);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(false);
            uscProcessDetails.selectedProcessFascicleTemplateId = selectedNode.get_value();
            uscProcessDetails.selectedDossierFolderId = selectedNode.get_parent().get_value();
            uscProcessDetails.selectedProcessId = this.getProcessNodeByChild(selectedNode).get_value();
            uscProcessDetails.selectedEntityType = ProcessNodeType.ProcessFascicleTemplate;
            this._uscProcessDetails.setProcessDetails(selectedNode.get_parent().get_text(), false);
            this._uscProcessDetails.setFascicle();
        };
        TbltProcess.prototype.loadData = function (currentNodeValue, status, idProcess) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            this._dossierFolderService.getProcessFascicleChildren(currentNodeValue, status, function (data) {
                if (!data)
                    return;
                var dossierFolders = data;
                for (var _i = 0, dossierFolders_1 = dossierFolders; _i < dossierFolders_1.length; _i++) {
                    var child = dossierFolders_1[_i];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    _this.createNode(node, child.Name, child.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, true, DossierFolderStatus[child.Status], null, null, _this._rtvProcesses.findNodeByValue(idProcess ? idProcess : currentNodeValue));
                    if (child.Status === DossierFolderStatus[DossierFolderStatus.Folder]) {
                        _this.createEmptyNode(node.get_nodes());
                    }
                }
                _this.loadFascicleTemplates(currentNodeValue);
                _this._rtvProcesses.commitChanges();
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.loadFascicleTemplates = function (dossierFolderId) {
            var _this = this;
            this._dossierFolderService.getFascicleTemplatesByDossierFolderId(dossierFolderId, function (data) {
                if (!data)
                    return;
                _this.processFascicleTemplatesModel = [];
                for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                    var tpftm = data_1[_i];
                    if (tpftm) {
                        _this.tempPFTModel.push(tpftm);
                    }
                }
                _this.processFascicleTemplatesModel = _this.processFascicleTemplatesModel.concat(data);
                for (var _a = 0, _b = _this.processFascicleTemplatesModel; _a < _b.length; _a++) {
                    var fascicleTemplate = _b[_a];
                    var node = new Telerik.Web.UI.RadTreeNode();
                    var isActive = fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) > new Date();
                    var imageUrl = isActive
                        ? "../App_Themes/DocSuite2008/imgset16/fascicle_close.png"
                        : "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
                    _this.createNode(node, fascicleTemplate.Name, fascicleTemplate.UniqueId, imageUrl, ProcessNodeType.ProcessFascicleTemplate, false, null, isActive, null, _this._rtvProcesses.findNodeByValue(dossierFolderId));
                }
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.getProcessNodeByChild = function (node) {
            while (node.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) !== ProcessNodeType.Process) {
                node = node.get_parent();
            }
            return node;
        };
        TbltProcess.prototype.clearInputs = function () {
            //process inputs
            this.processRoles = [];
            this._rtbProcessName.clear();
            this._rcbProcessNote.clear();
            this._uscCategoryRest.clearTree();
            this._uscProcessRoleRest.renderRolesTree([]);
            //dossierFolder inputs
            this._rtbDossierFolderName.clear();
            //fascicleTemplate inputs
            this._rtbFascicleTemplateName.clear();
        };
        TbltProcess.prototype.hideInsertInputs = function () {
            $("#insertProcess").hide();
            $("#insertDossierFolder").hide();
            $("#insertFascicleTemplate").hide();
            $("#cloneDossierFolder").hide();
        };
        TbltProcess.prototype.enableFolderToolbarButtons = function (value) {
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE).set_enabled(value);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE).set_enabled(value);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_MODIFY).set_enabled(value);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_DELETE).set_enabled(value);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_CLONE).set_enabled(value);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_COPYPFT).set_enabled(value);
            this._folderToolBar.findItemByValue(TbltProcess.TOOLBAR_PASTEPFT).set_enabled(value);
        };
        TbltProcess.prototype.insertPFT = function (selectedNode, fascicleTemplate) {
            if (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) != ProcessNodeType.DossierFolder) {
                alert("Seleziona un nodo di volume");
                return;
            }
            var imageUrl;
            if (fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) < new Date()) {
                imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
            }
            else {
                imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            }
            this.insertProcessFascicleTemplate(selectedNode, fascicleTemplate, imageUrl);
        };
        ///Insert or Update Process Fascicle Template
        TbltProcess.prototype.insertOrUpdateFascicleTemplate = function (selectedNode) {
            var exists = this.selectedProcessFascicleTemplateId !== "";
            var fascicleTemplate = {};
            fascicleTemplate.Name = this._rtbFascicleTemplateName.get_value();
            fascicleTemplate.StartDate = new Date();
            fascicleTemplate.EndDate = null;
            fascicleTemplate.Process = {};
            fascicleTemplate.Process.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();
            fascicleTemplate.DossierFolder = {};
            fascicleTemplate.DossierFolder.UniqueId = selectedNode.get_value();
            var imageUrl;
            if (fascicleTemplate.EndDate === null || new Date(fascicleTemplate.EndDate) < new Date()) {
                imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
            }
            else {
                imageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            }
            if (exists) {
                this.updateProcessFascicleTemplate(selectedNode, fascicleTemplate, imageUrl);
            }
            else {
                this.insertProcessFascicleTemplate(selectedNode, fascicleTemplate, imageUrl);
            }
        };
        TbltProcess.prototype.updateProcessFascicleTemplate = function (selectedNode, fascicleTemplate, imageUrl) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            fascicleTemplate.UniqueId = this.selectedProcessFascicleTemplateId;
            this._uscProcessDetails.populateFascicleTemplateInfo().then(function (jsonModel) {
                fascicleTemplate.JsonModel = jsonModel;
                _this._fascicleTemplateService.update(fascicleTemplate, function (data) {
                    _this.createNode(selectedNode, data.Name, data.UniqueId, imageUrl, ProcessNodeType.ProcessFascicleTemplate, false, null, null, null, null);
                    _this._rwInsert.close();
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                }, function (error) {
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                    _this.showNotificationException(error);
                });
            });
        };
        TbltProcess.prototype.insertProcessFascicleTemplate = function (selectedNode, fascicleTemplate, imageUrl) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == null ||
                sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_TEMPLATE_MODEL) == "[]") {
                fascicleTemplate.JsonModel = "";
            }
            this._fascicleTemplateService.insert(fascicleTemplate, function (data) {
                var hasChildren = selectedNode.get_nodes().get_count();
                var isExpandable = hasChildren > 0 ? false : true;
                var node = new Telerik.Web.UI.RadTreeNode();
                _this.createNode(node, data.Name, data.UniqueId, imageUrl, ProcessNodeType.ProcessFascicleTemplate, true, null, null, null, selectedNode, isExpandable);
                _this._rwInsert.close();
                _this.processFascicleTemplatesModel.push(data);
                _this.tempPFTModel.push(data);
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        ///end Insert or Update Process Fascicle Template
        ///Insert or Update Process 
        TbltProcess.prototype.insertOrUpdateProcess = function (selectedNode) {
            var exists = this.selectedProcessId !== "";
            var process = {};
            var processCategory = this._uscCategoryRest.getSelectedCategory();
            if (!processCategory)
                return;
            process.Name = this._rtbProcessName.get_value();
            process.Dossier = {};
            if (selectedNode.get_text() !== CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT) {
                process.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_value();
            }
            process.Category = new CategoryModel();
            process.Category.EntityShortId = processCategory.EntityShortId;
            process.StartDate = new Date();
            process.EndDate = null;
            process.Dossier = {};
            process.Roles = this.processRoles;
            process.Note = this._rcbProcessNote.get_value();
            if (exists) {
                this.updateProcess(selectedNode, process);
            }
            else {
                this.insertProcess(selectedNode);
            }
        };
        TbltProcess.prototype.updateProcess = function (selectedNode, process) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            process.UniqueId = this.selectedProcessId;
            this._processService.update(process, function (data) {
                var isActive = data.EndDate === null || new Date(data.EndDate) > new Date();
                _this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png", ProcessNodeType.Process, false, null, isActive, data.Dossier.UniqueId, null, false);
                var processModelToUpdate = _this.processesModel.filter(function (x) { return x.UniqueId === data.UniqueId; })[0];
                var processModelToUpdateIdx = _this.processesModel.indexOf(processModelToUpdate);
                _this.processesModel[processModelToUpdateIdx] = data;
                _this.processesModel[processModelToUpdateIdx].Roles = _this.processRoles;
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, true);
                _this._uscProcessDetails = $("#" + _this.uscProcessDetailsId).data();
                _this._uscProcessDetails.clearProcessDetails();
                uscProcessDetails.selectedProcessId = data.UniqueId;
                _this._uscProcessDetails.setProcessDetails('', true);
                _this._uscProcessDetails.setPanelLoading(uscProcessDetails.InformationDetails_PanelName, false);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.insertProcess = function (selectedNode) {
            var _this = this;
            var process = {};
            process.Name = this._rtbProcessName.get_value();
            process.Category = new CategoryModel();
            process.Category.EntityShortId = this._uscCategoryRest.getSelectedCategory().EntityShortId;
            process.StartDate = new Date();
            process.Roles = this.processRoles;
            process.Note = this._rcbProcessNote.get_value();
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            this._processService.insert(process, function (data) {
                var isActive = data.EndDate === null || new Date(data.EndDate) > new Date();
                var node = new Telerik.Web.UI.RadTreeNode();
                if (selectedNode.get_expanded() && selectedNode.get_nodes().getNode(0).get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT) {
                    selectedNode.get_nodes().clear();
                }
                _this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png", ProcessNodeType.Process, true, null, isActive, data.Dossier.UniqueId, selectedNode, false);
                data.Roles = _this.processRoles;
                if (!_this.processesModel) {
                    _this.processesModel = [];
                }
                _this.processesModel.push(data);
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        ///end Insert or Update Process
        ///Insert or Update Dossier Folder
        TbltProcess.prototype.insertOrUpdateDossierFolder = function (selectedNode) {
            if (this._rtbDossierFolderName.get_value()) {
                var exists = this.selectedDossierFolderId !== "";
                var dossierFolder = {};
                dossierFolder.Name = this._rtbDossierFolderName.get_value();
                if (selectedNode.get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) !== ProcessNodeType.Process) {
                    dossierFolder.ParentInsertId = exists ? selectedNode.get_parent().get_value() : selectedNode.get_value();
                }
                else {
                    dossierFolder.ParentInsertId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
                }
                dossierFolder.Dossier = {};
                dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(selectedNode).get_attributes().getAttribute("idDossier");
                if (exists) {
                    this.updateDossierFolder(selectedNode, dossierFolder);
                }
                else {
                    this.insertDossierFolder(selectedNode, dossierFolder);
                }
            }
        };
        TbltProcess.prototype.updateDossierFolder = function (selectedNode, dossierFolder) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            dossierFolder.UniqueId = this.selectedDossierFolderId;
            this._dossierFolderService.updateDossierFolder(dossierFolder, null, function (data) {
                _this.createNode(selectedNode, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, false, data.Status, null, null, null);
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        TbltProcess.prototype.insertDossierFolder = function (selectedNode, dossierFolder) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            this._processService.getById(this.getProcessNodeByChild(selectedNode).get_value(), function (data) {
                dossierFolder.DossierFolderRoles = data.Roles
                    .map(function (role) { return ({
                    Role: role
                }); });
                _this._dossierFolderService.insertDossierFolder(dossierFolder, null, function (data) {
                    var node = new Telerik.Web.UI.RadTreeNode();
                    if (selectedNode.get_nodes().get_count() > 0 && selectedNode.get_nodes().getNode(0).get_text() === "") {
                        selectedNode.get_nodes().clear();
                    }
                    _this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, true, data.Status, null, null, selectedNode, false);
                    _this._rwInsert.close();
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                }, function (error) {
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                    _this.showNotificationException(error);
                });
            });
        };
        ///end Insert or Update Dossier FOlder
        ///Clone dossier folder
        TbltProcess.prototype.cloneDossierFolder = function (sourceNode) {
            var _this = this;
            this._ajaxLoadingPanel.show(this.rtvProcessesId);
            var dossierFolder = {};
            dossierFolder.Name = this._rtbCloneDossierFolderName.get_textBoxValue();
            if (dossierFolder.Name === undefined || dossierFolder.Name === "") {
                return;
            }
            if (this._rtvProcesses.get_selectedNode().get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.DossierFolder) {
                dossierFolder.ParentInsertId = this.getProcessNodeByChild(this._rtvProcesses.get_selectedNode()).get_attributes().getAttribute("idDossier");
            }
            else {
                dossierFolder.ParentInsertId = this._rtvProcesses.get_selectedNode().get_parent().get_value();
            }
            dossierFolder.JsonMetadata = sourceNode.get_value();
            dossierFolder.Dossier = {};
            dossierFolder.Dossier.UniqueId = this.getProcessNodeByChild(sourceNode).get_attributes().getAttribute("idDossier");
            //TODO: Remove ActionTypes in DocSuite 9.0X (move webapi logic in Store Procedure) 
            this._dossierFolderService.insertDossierFolder(dossierFolder, InsertActionType.CloneProcessFolder, function (data) {
                //insert node
                var node = new Telerik.Web.UI.RadTreeNode();
                if (sourceNode.get_nodes().get_count() > 0 && sourceNode.get_nodes().getNode(0).get_text() === "") {
                    sourceNode.get_nodes().clear();
                }
                //update node hierarchy
                _this._dossierFolderService.updateDossierFolder(data, UpdateActionType.CloneProcessFolder, function (data) {
                    _this.createNode(node, data.Name, data.UniqueId, "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, true, data.Status, null, null, _this.getProcessNodeByChild(sourceNode), false);
                    //to be able to see the plus sign
                    var dummyNode = new Telerik.Web.UI.RadTreeNode();
                    _this.createNode(dummyNode, "", "", "../App_Themes/DocSuite2008/imgset16/folder_closed.png", ProcessNodeType.DossierFolder, true, data.Status, null, null, node, false);
                    _this._rwInsert.close();
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                });
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        ///end Clone dossier folder
        TbltProcess.prototype.createNode = function (node, text, value, imagePath, nodeType, isExpanded, dossierFolderStatus, isActive, idDossier, parentNode, expandParent) {
            if (expandParent === void 0) { expandParent = true; }
            node.set_value(value);
            node.set_text(text);
            node.get_attributes().setAttribute(TbltProcess.NodeType_TYPE_NAME, nodeType);
            node.set_imageUrl(imagePath);
            switch (nodeType) {
                case ProcessNodeType.Process: {
                    node.get_attributes().setAttribute("IsActive", isActive);
                    node.get_attributes().setAttribute("idDossier", idDossier);
                    break;
                }
                case ProcessNodeType.DossierFolder: {
                    node.get_attributes().setAttribute("DossierFolderStatus", DossierFolderStatus[dossierFolderStatus]);
                    break;
                }
                case ProcessNodeType.ProcessFascicleTemplate: {
                    node.get_attributes().setAttribute("IsActive", isActive);
                    break;
                }
            }
            if (parentNode !== null) {
                parentNode.get_nodes().add(node);
            }
            if (expandParent && parentNode !== null) {
                parentNode.expand();
            }
        };
        TbltProcess.prototype.removeFascicleTemplate = function (endDate) {
            var _this = this;
            var fascicleTemplate = {};
            fascicleTemplate.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
            for (var _i = 0, _a = this.tempPFTModel; _i < _a.length; _i++) {
                var fascicleToFind = _a[_i];
                if (fascicleToFind.UniqueId == fascicleTemplate.UniqueId) {
                    fascicleTemplate = fascicleToFind;
                    break;
                }
            }
            fascicleTemplate.EndDate = endDate;
            if (window.confirm("Vuoi eliminare modello di fascicolo selezionato?")) {
                this._fascicleTemplateService.update(fascicleTemplate, function (data) {
                    var imgUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
                    _this._rtvProcesses.get_selectedNode().set_imageUrl(imgUrl);
                    _this._rtvProcesses.get_selectedNode().get_attributes().setAttribute("IsActive", false);
                    _this._rtvProcesses.commitChanges();
                    _this._rwInsert.close();
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                }, function (error) {
                    _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                    _this.showNotificationException(error);
                });
            }
            else {
                this._ajaxLoadingPanel.hide(this.rtvProcessesId);
            }
        };
        TbltProcess.prototype.removeProcess = function (endDate) {
            var _this = this;
            var process = {};
            process.UniqueId = this._rtvProcesses.get_selectedNode().get_value();
            for (var _i = 0, _a = this.processesModel; _i < _a.length; _i++) {
                var processToFind = _a[_i];
                if (processToFind.UniqueId == process.UniqueId) {
                    process = processToFind;
                    break;
                }
            }
            process.Roles = this.processRoles;
            process.EndDate = endDate;
            this._processService.delete(process, function (data) {
                var processActiveItem = _this._filterToolbar.findItemByValue("processActive");
                var processDisabledItem = _this._filterToolbar.findItemByValue("processDisabled");
                var nodeRemoveConditions = processActiveItem.get_checked() && !processDisabledItem.get_checked();
                if (nodeRemoveConditions || !nodeRemoveConditions) {
                    _this._rtvProcesses.get_selectedNode().get_parent().get_nodes().remove(_this._rtvProcesses.get_selectedNode());
                }
                _this._rwInsert.close();
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
            }, function (error) {
                _this._ajaxLoadingPanel.hide(_this.rtvProcessesId);
                _this.showNotificationException(error);
            });
        };
        //TODO: filtering feature will be completed in 9.0X
        //filterToolbar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        //    switch (args.get_item().get_value()) {
        //        case "search": {
        //            this._ajaxLoadingPanel.show(this.processViewPaneId);
        //            this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
        //            this._uscProcessDetails.clearProcessDetails();
        //            $(`#${this._uscProcessDetails.pnlDetailsId}`).hide();
        //            this._rtvProcesses.get_nodes().getNode(0).get_nodes().clear();
        //            let processSearchName: string = this._rtbProcessSearchName.get_value();
        //            let processActiveItem: any = this._filterToolbar.findItemByValue("processActive");
        //            let processDisabledItem: any = this._filterToolbar.findItemByValue("processDisabled");
        //            let callLoadProcesses: void = processActiveItem.get_checked()
        //                ? processDisabledItem.get_checked() ? this.loadProcesses(processSearchName, null) : this.loadProcesses(processSearchName, true)
        //                : processDisabledItem.get_checked() ? this.loadProcesses(processSearchName, false) : this.loadProcesses(processSearchName, null);
        //            break;
        //        }
        //    }
        //}
        TbltProcess.prototype.populateProcessInputs = function (processId) {
            var process = {};
            for (var _i = 0, _a = this.processesModel; _i < _a.length; _i++) {
                var processToFind = _a[_i];
                if (processToFind.UniqueId === processId) {
                    process = processToFind;
                    break;
                }
            }
            this._rtbProcessName.set_value(process.Name);
            this._rcbProcessNote.set_value(process.Note);
            var category = {};
            category.Code = process.Category.Code;
            category.Name = process.Category.Name;
            category.IdCategory = process.Category.EntityShortId;
            this._uscCategoryRest.updateSessionStorageSelectedCategory(category);
            this._uscCategoryRest.populateCategotyTree(category);
        };
        TbltProcess.prototype.showNotificationException = function (exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#" + this.uscNotificationId).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(customMessage);
            }
        };
        TbltProcess.prototype.showNotificationMessage = function (customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltProcess.Process_TYPE_NAME = "Process";
        TbltProcess.DossierFolder_TYPE_NAME = "DossierFolder";
        TbltProcess.Dossier_TYPE_NAME = "Dossier";
        TbltProcess.ProcessFascicleTemplate_TYPE_NAME = "ProcessFascicleTemplate";
        TbltProcess.Category_TYPE_NAME = "Category";
        TbltProcess.TOOLBAR_CREATE = "create";
        TbltProcess.TOOLBAR_CREATE_PROCESS_FASCICLE_TEMPLATE = "createProcessFascicleTemplate";
        TbltProcess.TOOLBAR_MODIFY = "modify";
        TbltProcess.TOOLBAR_DELETE = "delete";
        TbltProcess.TOOLBAR_CLONE = "clone";
        TbltProcess.TOOLBAR_COPYPFT = "copyPFT";
        TbltProcess.TOOLBAR_PASTEPFT = "pastePFT";
        TbltProcess.NodeType_TYPE_NAME = "NodeType";
        TbltProcess.ProcessType_TYPE_NAME = "ProcessType";
        TbltProcess.Category_ID_TYPE = "idCategory";
        return TbltProcess;
    }());
    return TbltProcess;
});
//# sourceMappingURL=TbltProcess.js.map