/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/core/extensions/number.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/CategoryService", "App/Services/DocumentUnits/DocumentUnitService", "App/Services/Processes/ProcessService", "App/Services/Dossiers/DossierFolderService", "App/Models/Fascicles/FascicleModel", "App/Models/Dossiers/DossierFolderStatus", "App/Services/Fascicles/FascicleFolderService", "App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper", "App/Models/Fascicles/FascicleFolderStatus", "App/Models/Fascicles/FascicleFolderTypology", "App/Models/Environment", "App/Mappers/DocumentUnits/DocumentUnitModelMapper", "App/Models/Commons/TreeNodeType", "App/Services/Fascicles/FascicleService"], function (require, exports, ServiceConfigurationHelper, CategoryService, DocumentUnitService, ProcessService, DossierFolderService, FascicleModel, DossierFolderStatus, FascicleFolderService, DossierSummaryFolderViewModelMapper, FascicleFolderStatus, FascicleFolderTypology, Environment, DocumentUnitModelMapper, TreeNodeType, FascicleService) {
    var ProcessesTreeView = /** @class */ (function () {
        function ProcessesTreeView(serviceConfigurations) {
            var _this = this;
            this.treeView_LoadNodeChildrenOnExpand = function (sender, args) {
                var expandedNode = args.get_node();
                expandedNode.get_nodes().clear();
                var nodeType = expandedNode.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
                var expandedNodeAction = _this._nodeTypeExpandingActionsDictionary[nodeType];
                if (expandedNodeAction) {
                    expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    expandedNodeAction(expandedNode)
                        .done(function () {
                        expandedNode.hideLoadingStatus();
                        expandedNode.set_expanded(true);
                    })
                        .fail(function (exception) {
                        _this._showNotificationException(exception);
                    });
                }
            };
            this.treeView_LoadNodeTypeDetailsOnClick = function (sender, args) {
                var node = args.get_node();
                var nodeType = node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
                _this._moveFascicleToolbarBtn.set_enabled(nodeType === TreeNodeType.Fascicle);
                _this._editProcessToolbarBtn.set_enabled(nodeType === TreeNodeType.Process);
                var nodeTypeClickEventHandler = _this._nodeTypeClickActionsDictionary[nodeType];
                if (nodeTypeClickEventHandler) {
                    nodeTypeClickEventHandler(node);
                }
            };
            this.actionToolbar_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.toolbarActions().filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
                    .map(function (item) { return item[1]; })[0];
                currentAction();
            };
            this._updateProcessesTreeView = function (sender, args) {
                var fascMoveResponseModel = args.get_argument();
                if (!fascMoveResponseModel) {
                    return;
                }
                // Get selected fascicle category in order to optimize the search in the tree, of the new dossier folder parent
                var categoryParent = _this._categoryTreeView.findNodeByValue(fascMoveResponseModel.categoryId.toString());
                var newDossierFolderParentId = fascMoveResponseModel.newDossierFolderParentId;
                var newDossierFolderParent = categoryParent.get_allNodes().filter(function (node) { return node.get_value() === newDossierFolderParentId; })[0];
                // Remove selected fascicle from the initial parent node collection
                var currentSelectedFascicle = _this._categoryTreeView.get_selectedNode();
                var currentSelectedFascicleParent = currentSelectedFascicle.get_parent();
                currentSelectedFascicle.get_parent().get_nodes().remove(currentSelectedFascicle);
                // If source dossierfolder has no more children, collapse and change node icon
                if (currentSelectedFascicleParent.get_nodes().get_count() === 0) {
                    currentSelectedFascicleParent.set_expanded(false);
                    currentSelectedFascicleParent.set_imageUrl(_this._dossierFolderStatusImageDictionary[DossierFolderStatus.InProgress]);
                }
                // If the new dossierfolder parent is visible in the tree, expand it and update node image
                if (newDossierFolderParent) {
                    newDossierFolderParent.set_imageUrl(_this._expandedFolderImageDictionary[DossierFolderStatus.Folder]);
                    newDossierFolderParent.get_nodes().clear();
                    newDossierFolderParent.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    _this._loadDossierFoldersChildren(newDossierFolderParent.get_value(), newDossierFolderParent)
                        .done(function () {
                        newDossierFolderParent.hideLoadingStatus();
                        newDossierFolderParent.set_expanded(true);
                    })
                        .fail(function (exception) {
                        _this._showNotificationException(exception);
                    });
                }
            };
            this._redirectToProcessEditPage = function () {
                var selectedProcess = _this._categoryTreeView.get_selectedNode();
                var processId = selectedProcess.get_value();
                var processCategoryId = selectedProcess.get_attributes().getAttribute(ProcessesTreeView.CATEGORYID_ATTRNAME);
                _this._loadingPanel.show(_this.splitterMainId);
                var editPageUrl = "../Tblt/TbltProcess.aspx?Type=Comm&IdProcess=" + processId + "&IdCategory=" + processCategoryId;
                window.open(editPageUrl, "main");
            };
            var categoryServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryServiceConfiguration);
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DocumentUnit");
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            var processConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var fascicleFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "FascicleFolder");
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
            var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleServiceConfiguration);
        }
        ProcessesTreeView.prototype._showOnlyMyProcesses = function () {
            return JSON.parse(this.showOnlyMyProcesses.toLowerCase());
        };
        ProcessesTreeView.prototype._categoryTreeViewRootNode = function () {
            return this._categoryTreeView.get_nodes().getNode(0);
        };
        ProcessesTreeView.prototype.toolbarActions = function () {
            var _this = this;
            var items = [
                [ProcessesTreeView.COMMANDNAME_MOVEFASCICLE, function () { return _this._moveFascicle(); }],
                [ProcessesTreeView.COMMANDNAME_EDITPROCESS, function () { return _this._redirectToProcessEditPage(); }]
            ];
            return items;
        };
        /// region [ PUBLIC METHODS ]
        ProcessesTreeView.prototype.initialize = function () {
            this._initializeDictionaries();
            this._initializeControls();
            this._initializeCategoryTreeView();
        };
        /// endregion [ PUBLIC METHODS ]
        /// region [ LOAD TREE NODE ITEMS METHODS]
        ProcessesTreeView.prototype._initializeCategoryTreeView = function () {
            if (this._categoryTreeView.get_nodes().get_count() > 0)
                this._categoryTreeView.get_nodes().clear();
            var rootNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text(ProcessesTreeView.ROOTNODE_DESCRIPTION);
            rootNode.get_attributes().setAttribute(ProcessesTreeView.NODETYPE_ATTRNAME, TreeNodeType.Root);
            this._categoryTreeView.get_nodes().add(rootNode);
            rootNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
            this._loadCategories(TreeNodeType.Category, this._categoryTreeViewRootNode())
                .done(function () {
                rootNode.hideLoadingStatus();
                rootNode.set_expanded(true);
            });
        };
        ProcessesTreeView.prototype._loadCategories = function (nodeType, parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            var categoryFinderModel = {};
            categoryFinderModel.IdTenantAOO = this.currentTenantAOOId;
            if (!parentNode.get_value()) {
                categoryFinderModel.LoadRoot = true;
            }
            else {
                categoryFinderModel.ParentId = parentNode.get_value();
            }
            this._categoryService.findTreeCategories(categoryFinderModel, function (categories) {
                var parentNodeIsTreeRootNode = parentNode === _this._categoryTreeViewRootNode();
                var categoryTreeNodeCollection = categories.map(function (categoryModel) {
                    var categoryNodeDescription = categoryModel.Code + "." + categoryModel.Name;
                    var categoryNodeImageUrl = "../Comm/images/folderopen16.gif";
                    var currentCategoryTreeNode = _this._createTreeNode(nodeType, categoryNodeDescription, categoryModel.IdCategory, categoryNodeImageUrl, parentNodeIsTreeRootNode ? parentNode : undefined);
                    _this._appendEmptyNode(currentCategoryTreeNode);
                    return currentCategoryTreeNode;
                });
                defferedRequest.resolve(categoryTreeNodeCollection);
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        ProcessesTreeView.prototype._loadCategoryProcesses = function (nodeType, parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            var categoryId = parentNode.get_value();
            this._processService.getAvailableProcesses(null, this._showOnlyMyProcesses(), parentNode.get_value(), null, function (categoryProcesses) {
                var processTreeNodeCollection = categoryProcesses.map(function (process) {
                    var currentProcessTreeNode = _this._createTreeNode(nodeType, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png");
                    currentProcessTreeNode.get_attributes().setAttribute(ProcessesTreeView.CATEGORYID_ATTRNAME, categoryId);
                    _this._appendEmptyNode(currentProcessTreeNode);
                    return currentProcessTreeNode;
                });
                defferedRequest.resolve(processTreeNodeCollection);
            }, function (exception) {
                defferedRequest.reject(exception);
            });
            return defferedRequest.promise();
        };
        ProcessesTreeView.prototype._loadCategoryFascicles = function (nodeType, parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._fascicleService.getFascicleByCategory(parentNode.get_value(), '', false, function (odataResponseModel) {
                var categoryFascicles = odataResponseModel.value;
                var fascicleTreeNodeCollection = categoryFascicles.map(function (fasc) {
                    var fascicleImageUrl = _this._dossierFolderStatusImageDictionary[DossierFolderStatus.Fascicle];
                    var fascicleTooltip = _this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Fascicle];
                    var fascicleExpandedImageUrl = _this._expandedFolderImageDictionary[DossierFolderStatus.Fascicle];
                    var nodeDescription = fasc.Title + "-" + fasc.FascicleObject;
                    var currentFascTreeNode = _this._createTreeNode(nodeType, nodeDescription, fasc.UniqueId, fascicleImageUrl, undefined, fascicleTooltip, fascicleExpandedImageUrl);
                    currentFascTreeNode.get_attributes().setAttribute(ProcessesTreeView.FASCISACTIVE_ATTRNAME, !fasc.EndDate);
                    _this._appendEmptyNode(currentFascTreeNode);
                    return currentFascTreeNode;
                });
                defferedRequest.resolve(fascicleTreeNodeCollection);
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        ProcessesTreeView.prototype._loadProcessDossierFolders = function (processId, parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._dossierFolderService.getProcessFolders(null, processId, false, false, function (processDossierFolders) {
                if (!processDossierFolders.length) {
                    _this._appendEmptyNode(parentNode);
                    defferedRequest.resolve();
                    return;
                }
                var dossierSummaryFolderViewModelMapper = new DossierSummaryFolderViewModelMapper();
                var processDossierFoldersViewModels = dossierSummaryFolderViewModelMapper.MapCollection(processDossierFolders);
                _this._addDossierFolderNodesRecursive(processDossierFoldersViewModels, parentNode);
                defferedRequest.resolve();
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        ProcessesTreeView.prototype._loadDossierFoldersChildren = function (parentId, parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._dossierFolderService.getChildren(parentId, 0, function (dossierFolders) {
                if (!dossierFolders.length) {
                    _this._appendEmptyNode(parentNode);
                    defferedRequest.resolve();
                    return;
                }
                dossierFolders.forEach(function (dossierFolder) {
                    var dossierFolderIsFascicle = !!dossierFolder.idFascicle;
                    var nodeType = dossierFolderIsFascicle ? TreeNodeType.Fascicle : TreeNodeType.DossierFolder;
                    var nodeValue = dossierFolderIsFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;
                    var dossierFolderImageUrl = _this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
                    var dossierFolderTooltip = _this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
                    var dossierFolderExpandedImageUrl = _this._expandedFolderImageDictionary[DossierFolderStatus[dossierFolder.Status]];
                    var currentDossierFolderTreeNode = _this._createTreeNode(nodeType, dossierFolder.Name, nodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);
                    if (dossierFolderIsFascicle) {
                        currentDossierFolderTreeNode.get_attributes().setAttribute(ProcessesTreeView.DOSSIERFOLDERID_ATTRNAME, dossierFolder.UniqueId);
                    }
                    _this._appendEmptyNode(currentDossierFolderTreeNode);
                });
                defferedRequest.resolve();
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        ProcessesTreeView.prototype._loadFascicleFolders = function (parentFascicleId, nodeType) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._fascicleFolderService.getChildren(parentFascicleId, function (fascicleFolders) {
                if (!fascicleFolders.length) {
                    defferedRequest.resolve([]);
                    return;
                }
                var fascFoldersTreeNodeCollection = fascicleFolders.map(function (fascicleFolder) {
                    var fascicleFolderImageUrl = _this._dossierFolderStatusImageDictionary[DossierFolderStatus.Folder];
                    var fascicleFolderTooltip = _this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus[fascicleFolder.Status]];
                    var fascicleFolderExpandedImageUrl = _this._expandedFolderImageDictionary[DossierFolderStatus.Folder];
                    var currentFascicleFolderTreeNode = _this._createTreeNode(nodeType, fascicleFolder.Name, fascicleFolder.UniqueId, fascicleFolderImageUrl, undefined, fascicleFolderTooltip, fascicleFolderExpandedImageUrl);
                    currentFascicleFolderTreeNode.get_attributes().setAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME, fascicleFolder.idFascicle);
                    _this._appendEmptyNode(currentFascicleFolderTreeNode);
                    return currentFascicleFolderTreeNode;
                });
                defferedRequest.resolve(fascFoldersTreeNodeCollection);
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        ProcessesTreeView.prototype._loadFascicleFolderDocumentUnits = function (fascicleFolderFascicleId, fascicleFolderId, nodeType) {
            var _this = this;
            var defferedRequest = $.Deferred();
            var fascicleModel = new FascicleModel();
            fascicleModel.UniqueId = fascicleFolderFascicleId;
            this._documentUnitService.getFascicleDocumentUnits(fascicleModel, null, this.currentTenantAOOId, fascicleFolderId, function (fascicleFolderDocUnits) {
                var docUnitMapper = new DocumentUnitModelMapper();
                fascicleFolderDocUnits = docUnitMapper.MapCollection(fascicleFolderDocUnits);
                var fascFolderDocUnitTreeNodeCollection = fascicleFolderDocUnits.map(function (fascicleFolderDocUnit) { return _this._createDocumentUnitNode(fascicleFolderDocUnit, nodeType); });
                defferedRequest.resolve(fascFolderDocUnitTreeNodeCollection);
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        /// endregion [ LOAD TREE NODE ITEMS METHODS]
        /// region [ ** HELPER METHODS ** ]
        ProcessesTreeView.prototype._registerNodeTypesExpandingActions = function () {
            var _this = this;
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.Root] = function (expandedNode) {
                var categoryLoadingDefferedRequest = $.Deferred();
                _this._loadCategories(TreeNodeType.Category, expandedNode)
                    .done(function () { return categoryLoadingDefferedRequest.resolve(); })
                    .fail(function (exception) { return categoryLoadingDefferedRequest.reject(exception); });
                return categoryLoadingDefferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.Category] = function (expandedNode) {
                var categoryChildrenLoadingDefferedRequest = $.Deferred();
                var childrenLoadingPromises = [
                    _this._loadCategories(TreeNodeType.Category, expandedNode),
                    _this._loadCategoryProcesses(TreeNodeType.Process, expandedNode),
                    _this._loadCategoryFascicles(TreeNodeType.Fascicle, expandedNode)
                ];
                $.when.apply($, childrenLoadingPromises).done(function () {
                    var categoryChildrenNodeGroups = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        categoryChildrenNodeGroups[_i] = arguments[_i];
                    }
                    var subCategories = categoryChildrenNodeGroups[0], processes = categoryChildrenNodeGroups[1], fascicles = categoryChildrenNodeGroups[2];
                    if (subCategories.length === 0 && processes.length === 0 && fascicles.length === 0) {
                        _this._appendEmptyNode(expandedNode);
                        categoryChildrenLoadingDefferedRequest.resolve();
                        return;
                    }
                    var currentParentNodeCollection = expandedNode.get_nodes();
                    categoryChildrenNodeGroups.forEach(function (childrenGroup) {
                        childrenGroup.forEach(function (node) { return currentParentNodeCollection.add(node); });
                    });
                    categoryChildrenLoadingDefferedRequest.resolve();
                })
                    .fail(function (exception) { return categoryChildrenLoadingDefferedRequest.reject(exception); });
                return categoryChildrenLoadingDefferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.Process] = function (parentNode) {
                var processDossierFoldersLoadingDefferedRequest = $.Deferred();
                _this._loadProcessDossierFolders(parentNode.get_value(), parentNode)
                    .done(function () { return processDossierFoldersLoadingDefferedRequest.resolve(); })
                    .fail(function (exception) { return processDossierFoldersLoadingDefferedRequest.reject(exception); });
                return processDossierFoldersLoadingDefferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.DossierFolder] = function (parentNode) {
                var dossierFolderChildrenLoadingDefferedRequest = $.Deferred();
                _this._loadDossierFoldersChildren(parentNode.get_value(), parentNode)
                    .done(function () { return dossierFolderChildrenLoadingDefferedRequest.resolve(); })
                    .fail(function (exception) { return dossierFolderChildrenLoadingDefferedRequest.reject(exception); });
                return dossierFolderChildrenLoadingDefferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.Fascicle] = function (parentNode) {
                var fascicleFoldersLoadingDefferedRequest = $.Deferred();
                var fascicleId = parentNode.get_value();
                _this._loadFascicleFolders(fascicleId, TreeNodeType.FascicleFolder)
                    .done(function (fascFolders) {
                    if (fascFolders.length === 0) {
                        _this._appendEmptyNode(parentNode);
                        fascicleFoldersLoadingDefferedRequest.resolve();
                        return;
                    }
                    fascFolders.forEach(function (fascFolderNode) {
                        parentNode.get_nodes().add(fascFolderNode);
                    });
                    fascicleFoldersLoadingDefferedRequest.resolve();
                })
                    .fail(function (exception) { return fascicleFoldersLoadingDefferedRequest.reject(exception); });
                return fascicleFoldersLoadingDefferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.FascicleFolder] = function (parentNode) {
                var fascFolderChildrenLoadingDefferedRequest = $.Deferred();
                var parentFascicleFolderId = parentNode.get_value();
                var parentFascileFolderFascicleId = parentNode.get_attributes().getAttribute(ProcessesTreeView.FASCICLEID_ATTRNAME);
                var childrenLoadingPromises = [
                    _this._loadFascicleFolders(parentFascicleFolderId, TreeNodeType.FascicleFolder),
                    _this._loadFascicleFolderDocumentUnits(parentFascileFolderFascicleId, parentFascicleFolderId, TreeNodeType.DocumentUnit)
                ];
                $.when.apply($, childrenLoadingPromises).done(function () {
                    var fascFolderChildrenNodeGroups = [];
                    for (var _i = 0; _i < arguments.length; _i++) {
                        fascFolderChildrenNodeGroups[_i] = arguments[_i];
                    }
                    var subFolders = fascFolderChildrenNodeGroups[0], docUnits = fascFolderChildrenNodeGroups[1];
                    if (subFolders.length === 0 && docUnits.length === 0) {
                        _this._appendEmptyNode(parentNode);
                        fascFolderChildrenLoadingDefferedRequest.resolve();
                        return;
                    }
                    var currentParentNodeCollection = parentNode.get_nodes();
                    fascFolderChildrenNodeGroups.forEach(function (childrenGroup) {
                        childrenGroup.forEach(function (node) { return currentParentNodeCollection.add(node); });
                    });
                    fascFolderChildrenLoadingDefferedRequest.resolve();
                })
                    .fail(function (exception) { return fascFolderChildrenLoadingDefferedRequest.reject(exception); });
                return fascFolderChildrenLoadingDefferedRequest.promise();
            };
        };
        ProcessesTreeView.prototype._registerNodeTypesClickActions = function () {
            var _this = this;
            this._nodeTypeClickActionsDictionary[TreeNodeType.Fascicle] = function (clickedFascicleNode) {
                var fascicleId = clickedFascicleNode.get_value();
                if (fascicleId) {
                    var url = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=" + fascicleId;
                    _this._detailsPane.set_contentUrl(url);
                }
            };
            this._nodeTypeClickActionsDictionary[TreeNodeType.DocumentUnit] = function (clickedDocUnitNode) {
                var detailsPageUrl = _this._createEnvironmentDetailsPageUrl(clickedDocUnitNode);
                if (detailsPageUrl) {
                    _this._detailsPane.set_contentUrl(detailsPageUrl);
                }
            };
            this._nodeTypeClickActionsDictionary[TreeNodeType.Process] = function (clickedProcessNode) {
                var processId = clickedProcessNode.get_value();
                if (processId) {
                    var url = "../Processes/ProcessVisualizza.aspx?Type=Comm&IdProcess=" + processId + "&ReadOnlyRoles=" + true;
                    _this._detailsPane.set_contentUrl(url);
                }
            };
        };
        ProcessesTreeView.prototype._initializeDictionaries = function () {
            /// Nodes expanding event handlers registration
            this._nodeTypeExpandingActionsDictionary = {};
            this._registerNodeTypesExpandingActions();
            /// Nodes click event handlers registration
            this._nodeTypeClickActionsDictionary = {};
            this._registerNodeTypesClickActions();
            /// Dossier folders images and tooltips
            this._dossierFolderStatusImageDictionary = {};
            this._dossierFolderStatusTooltipDictionary = {};
            this._registerDossierFoldersStatusImages();
            this._registerDossierFoldersStatusTooltips();
            /// Fascicle folders images and tooltips
            this._fascicleFolderStatusImageDictionary = {};
            this._fascicleFolderStatusTooltipDictionary = {};
            this._registerFascicleFoldersStatusImages();
            this._registerFascicleFoldersStatusTooltips();
            /// Expanded folders images
            this._expandedFolderImageDictionary = {};
            this._registerExpandedFoldersImages();
            /// Document units environment images
            this._environmentImageDictionary = {};
            this._registerEnvironmentImages();
        };
        ProcessesTreeView.prototype._registerDossierFoldersStatusImages = function () {
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.DoAction] = "../App_Themes/DocSuite2008/imgset16/folder_hidden.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.InProgress] = "../App_Themes/DocSuite2008/imgset16/folder_hidden.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.Fascicle] = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.FascicleClose] = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
        };
        ProcessesTreeView.prototype._registerDossierFoldersStatusTooltips = function () {
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.DoAction] = "Da gestire";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.InProgress] = "Da gestire";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Fascicle] = "Fascicolo";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.FascicleClose] = "Fascicolo chiuso";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Folder] = "Cartella con sottocartelle";
        };
        ProcessesTreeView.prototype._registerFascicleFoldersStatusImages = function () {
            this._fascicleFolderStatusImageDictionary[FascicleFolderStatus.Active] = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            this._fascicleFolderStatusImageDictionary[FascicleFolderStatus.Internet] = "../App_Themes/DocSuite2008/imgset16/folder_internet_closed.png";
        };
        ProcessesTreeView.prototype._registerFascicleFoldersStatusTooltips = function () {
            this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus.Active] = "Cartella attiva";
            this._fascicleFolderStatusTooltipDictionary[FascicleFolderStatus.Internet] = "Cartella pubblica via internet";
        };
        ProcessesTreeView.prototype._registerExpandedFoldersImages = function () {
            this._expandedFolderImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
            this._expandedFolderImageDictionary[FascicleFolderTypology.SubFascicle] = "../App_Themes/DocSuite2008/imgset16/folder_internet_open.png";
        };
        ProcessesTreeView.prototype._registerEnvironmentImages = function () {
            this._environmentImageDictionary[Environment.Protocol] = "../Comm/Images/DocSuite/Protocollo16.gif";
            this._environmentImageDictionary[Environment.Resolution] = "../Comm/Images/DocSuite/Atti16.gif";
            this._environmentImageDictionary[Environment.DocumentSeries] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
            this._environmentImageDictionary[Environment.UDS] = "../App_Themes/DocSuite2008/imgset16/document_copies.png";
        };
        /**
         * Creates a RadTreeNode object with the given details.
         * If the parent node is passed, the new created node is added to the parent node collection.
         * @param nodeType
         * @param nodeDescription
         * @param nodeValue
         * @param imageUrl
         * @param parentNode
         * @param tooltipText
         * @param expandedImageUrl
         */
        ProcessesTreeView.prototype._createTreeNode = function (nodeType, nodeDescription, nodeValue, imageUrl, parentNode, tooltipText, expandedImageUrl) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            treeNode.set_text(nodeDescription);
            treeNode.set_value(nodeValue);
            treeNode.get_attributes().setAttribute(ProcessesTreeView.NODETYPE_ATTRNAME, nodeType);
            if (imageUrl) {
                treeNode.set_imageUrl(imageUrl);
            }
            if (tooltipText) {
                treeNode.set_toolTip(tooltipText);
            }
            if (expandedImageUrl) {
                treeNode.set_expandedImageUrl(expandedImageUrl);
            }
            if (parentNode) {
                parentNode.get_nodes().add(treeNode);
            }
            return treeNode;
        };
        ProcessesTreeView.prototype._initializeControls = function () {
            this._uscNotification = $("#" + this.uscNotificationId).data();
            this._actionToolbar = $find(this.actionToolbarId);
            this._categoryTreeView = $find(this.categoryTreeViewId);
            this._detailsPane = $find(this.detailsPaneId);
            this._windowManager = $find(this.windowManagerId);
            this._windowMoveFasc = $find(this.windowMoveFascId);
            this._windowMoveFasc.add_close(this._updateProcessesTreeView);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            if (this._actionToolbar) {
                this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
                this._moveFascicleToolbarBtn = this._actionToolbar.findButtonByCommandName(ProcessesTreeView.COMMANDNAME_MOVEFASCICLE);
                this._moveFascicleToolbarBtn.set_enabled(false);
                this._editProcessToolbarBtn = this._actionToolbar.findButtonByCommandName(ProcessesTreeView.COMMANDNAME_EDITPROCESS);
                this._editProcessToolbarBtn.set_enabled(false);
            }
        };
        ProcessesTreeView.prototype._moveFascicle = function () {
            var _this = this;
            var selectedFascicleNode = this._categoryTreeView.get_selectedNode();
            var selectedFascicleId = selectedFascicleNode.get_value();
            if (!selectedFascicleId) {
                alert("Fascicolo non valido");
                return;
            }
            var parentNode = selectedFascicleNode.get_parent();
            var parentNodeType = parentNode.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
            var fascicleToMoveId = parentNodeType === TreeNodeType.Category
                ? selectedFascicleId
                : selectedFascicleNode.get_attributes().getAttribute(ProcessesTreeView.DOSSIERFOLDERID_ATTRNAME);
            if (parentNodeType === TreeNodeType.Category) {
                var parentCategoryHasProcesses = parentNode.get_allNodes().some(function (node) {
                    var nodeType = node.get_attributes().getAttribute(ProcessesTreeView.NODETYPE_ATTRNAME);
                    return nodeType === TreeNodeType.Process;
                });
                if (!parentCategoryHasProcesses) {
                    this._showWarningMessage("La categoria selezionata non contiene nessun processo, quindi non e' possibile spostare il fascicolo.");
                    return;
                }
                var fascicleIsActive = selectedFascicleNode.get_attributes().getAttribute(ProcessesTreeView.FASCISACTIVE_ATTRNAME);
                if (!fascicleIsActive) {
                    this._showWarningMessage("Il fascicolo selezionato non è attivo");
                    return;
                }
                this._openMoveFascicleWindow(fascicleToMoveId);
            }
            else {
                this._dossierFolderService.checkIfDossierFolderFascicleIsActive(fascicleToMoveId, function (fascicleIsActive) {
                    if (!fascicleIsActive) {
                        _this._showWarningMessage("Il fascicolo selezionato non è attivo");
                        return;
                    }
                    _this._openMoveFascicleWindow(fascicleToMoveId, parentNode.get_value());
                }, function (exception) { return _this._showNotificationException(exception); });
            }
        };
        ProcessesTreeView.prototype._openMoveFascicleWindow = function (fascicleToMoveId, parentFolderId) {
            var url = "../Fasc/FascProcessMove.aspx?Type=Comm&FascicleId=" + fascicleToMoveId;
            if (parentFolderId) {
                url += "&ParentFolderId=" + parentFolderId;
            }
            this._windowManager.open(url, "windowMoveFasc", undefined);
        };
        ProcessesTreeView.prototype._addDossierFolderNodesRecursive = function (dossierFolders, parentNode) {
            var _this = this;
            dossierFolders.forEach(function (dossierFolder) {
                var dossierFolderImageUrl = _this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
                var dossierFolderExpandedImageUrl = _this._expandedFolderImageDictionary[DossierFolderStatus.Folder];
                var dossierFolderTooltip = _this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
                var dossierFolderNodeType = dossierFolder.idFascicle ? TreeNodeType.Fascicle : TreeNodeType.DossierFolder;
                var dossierFolderNodeValue = dossierFolder.idFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;
                var currentDossierFolderTreeNode = _this._createTreeNode(dossierFolderNodeType, dossierFolder.Name, dossierFolderNodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);
                if (dossierFolder.DossierFolders.length > 0) {
                    _this._addDossierFolderNodesRecursive(dossierFolder.DossierFolders, currentDossierFolderTreeNode);
                }
                else {
                    _this._appendEmptyNode(currentDossierFolderTreeNode);
                }
            });
        };
        ProcessesTreeView.prototype._appendEmptyNode = function (treeNode) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text("Nessun elemento trovato");
            treeNode.get_nodes().add(emptyNode);
        };
        ProcessesTreeView.prototype._addDocumentUnitNodeAttributes = function (docUnitNode, docUnitModel) {
            docUnitNode.get_attributes().setAttribute(ProcessesTreeView.DOCUNITTYPE_ATTRNAME, docUnitModel.Environment);
            docUnitNode.get_attributes().setAttribute(ProcessesTreeView.YEAR_ATTRNAME, docUnitModel.Year);
            docUnitNode.get_attributes().setAttribute(ProcessesTreeView.NUMBER_ATTRNAME, docUnitModel.Number);
            docUnitNode.get_attributes().setAttribute(ProcessesTreeView.ENTITYID_ATTRNAME, docUnitModel.EntityId);
            if (docUnitModel.UDSRepository) {
                docUnitNode.get_attributes().setAttribute(ProcessesTreeView.UDSREPOID_ATTRNAME, docUnitModel.UDSRepository.UniqueId);
            }
        };
        ProcessesTreeView.prototype._createDocumentUnitNode = function (documentUnitModel, nodeType) {
            var docUnitEnv = documentUnitModel.Environment < 100 ? documentUnitModel.Environment : Environment.UDS;
            var docUnitImageUrl = this._environmentImageDictionary[docUnitEnv] || ProcessesTreeView.DEFAULT_DOCUMENT_IMAGEURL;
            var docUnitDescription = documentUnitModel.Title + " - " + documentUnitModel.Subject;
            var docUnitTooltip = documentUnitModel.DocumentUnitName;
            var docUnitTreeNode = this._createTreeNode(nodeType, docUnitDescription, documentUnitModel.UniqueId, docUnitImageUrl, undefined, docUnitTooltip);
            this._addDocumentUnitNodeAttributes(docUnitTreeNode, documentUnitModel);
            return docUnitTreeNode;
        };
        ProcessesTreeView.prototype._showNotificationException = function (exception, customMessage) {
            if (!jQuery.isEmptyObject(this._uscNotification)) {
                if (exception) {
                    this._uscNotification.showNotification(exception);
                    return;
                }
                this._uscNotification.showWarningMessage(customMessage);
            }
        };
        ProcessesTreeView.prototype._showWarningMessage = function (customMessage) {
            if (!jQuery.isEmptyObject(this._uscNotification)) {
                this._uscNotification.showWarningMessage(customMessage);
            }
        };
        ProcessesTreeView.prototype._createEnvironmentDetailsPageUrl = function (clickedDocUnitNode) {
            var docUnitEnvironmentNumber = clickedDocUnitNode.get_attributes().getAttribute(ProcessesTreeView.DOCUNITTYPE_ATTRNAME);
            var docUnitEnv = docUnitEnvironmentNumber < 100 ? docUnitEnvironmentNumber : Environment.UDS;
            var docUnitEntityId = clickedDocUnitNode.get_attributes().getAttribute(ProcessesTreeView.ENTITYID_ATTRNAME);
            var docUnitUniqueId = clickedDocUnitNode.get_value();
            var detailsPageUrl;
            switch (docUnitEnv) {
                case Environment.Protocol: {
                    detailsPageUrl = "../Prot/ProtVisualizza.aspx?Type=Prot&UniqueId=" + docUnitUniqueId;
                    break;
                }
                case Environment.UDS: {
                    var idUdsRepository = clickedDocUnitNode.get_attributes().getAttribute(ProcessesTreeView.UDSREPOID_ATTRNAME);
                    detailsPageUrl = "../UDS/UDSView.aspx?Type=UDS&IdUDS=" + docUnitUniqueId + "&IdUDSRepository=" + idUdsRepository;
                    break;
                }
                case Environment.Resolution: {
                    detailsPageUrl = "../Resl/ReslVisualizza.aspx?Type=Resl&IdResolution=" + docUnitEntityId;
                    break;
                }
                case Environment.DocumentSeries: {
                    detailsPageUrl = "../Series/Item.aspx?IdDocumentSeriesItem=" + docUnitEntityId + "&Action=View&Type=Series";
                    break;
                }
            }
            return detailsPageUrl;
        };
        ProcessesTreeView.ROOTNODE_DESCRIPTION = "Classificatore";
        ProcessesTreeView.NODETYPE_ATTRNAME = "NodeType";
        ProcessesTreeView.FASCICLEID_ATTRNAME = "FascicleId";
        ProcessesTreeView.DOCUNITTYPE_ATTRNAME = "DocUnitType";
        ProcessesTreeView.YEAR_ATTRNAME = "Year";
        ProcessesTreeView.NUMBER_ATTRNAME = "Number";
        ProcessesTreeView.ENTITYID_ATTRNAME = "EntityId";
        ProcessesTreeView.UDSREPOID_ATTRNAME = "UdsRepoUniqueId";
        ProcessesTreeView.DOSSIERFOLDERID_ATTRNAME = "DossierFolderId";
        ProcessesTreeView.FASCISACTIVE_ATTRNAME = "FascicleIsActive";
        ProcessesTreeView.COMMANDNAME_MOVEFASCICLE = "MoveFascicle";
        ProcessesTreeView.COMMANDNAME_EDITPROCESS = "EditProcess";
        ProcessesTreeView.CATEGORYID_ATTRNAME = "CategoryId";
        ProcessesTreeView.DEFAULT_DOCUMENT_IMAGEURL = "../App_Themes/DocSuite2008/imgset16/document.png";
        return ProcessesTreeView;
    }());
    return ProcessesTreeView;
});
//# sourceMappingURL=ProcessesTreeView.js.map