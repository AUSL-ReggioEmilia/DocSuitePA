var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Fascicles/FascicleService", "App/Services/Processes/ProcessService", "App/Models/Commons/TreeNodeType", "App/Services/Dossiers/DossierFolderService", "App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper", "App/Models/Dossiers/DossierFolderStatus", "App/Models/Fascicles/FascicleFolderTypology", "App/Models/UpdateActionType", "Fasc/FascBase"], function (require, exports, ServiceConfigurationHelper, FascicleService, ProcessService, TreeNodeType, DossierFolderService, DossierSummaryFolderViewModelMapper, DossierFolderStatus, FascicleFolderTypology, UpdateActionType, FascBase) {
    var FascProcessMove = /** @class */ (function (_super) {
        __extends(FascProcessMove, _super);
        function FascProcessMove(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME)) || this;
            _this._moveFascicleResponseModel = {};
            _this._moveFascicleToDossierFolder = function () {
                var selectedDossierFolderNode = _this._processesTreeView.get_selectedNode();
                if (selectedDossierFolderNode.get_value() === _this.fascicleParentFolderId) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare un volume di destinazione differente dal volume di origine");
                    return;
                }
                _this._windowManager.radconfirm("Sei sicuro di voler spostare il fascicolo selezionato nel volume " + selectedDossierFolderNode.get_text() + "?", function (arg) {
                    if (!arg) {
                        return;
                    }
                    _this._loadingPanel.show(_this.pnlMainContentId);
                    _this._dossierFolderService.getDossierFolderById(selectedDossierFolderNode.get_value(), function (res) {
                        var targetDossierFolder = res[0];
                        var dossierFolderToInsert = _this.fascicleParentFolderId
                            ? _this._createSourceDossierFolderCopy(targetDossierFolder)
                            : _this._createNewDossierFolder(targetDossierFolder);
                        _this._moveFascicleResponseModel.newDossierFolderParentId = targetDossierFolder.UniqueId;
                        _this._moveFascicleResponseModel.categoryId = _this.fascicleParentFolderId
                            ? _this._sourceDossierFolderModel.Category.EntityShortId
                            : _this._sourceFascicleModel.Category.EntityShortId;
                        _this._dossierFolderService.insertDossierFolder(dossierFolderToInsert, null, function (data) {
                            if (!_this.fascicleParentFolderId) {
                                _this.closeWindow(_this._moveFascicleResponseModel);
                            }
                            else {
                                var sourceDossierFolder = {};
                                sourceDossierFolder.Name = _this._sourceDossierFolderModel.Name;
                                sourceDossierFolder.UniqueId = _this.sourceFascicleId;
                                _this._deleteDossierFolderFascicle(sourceDossierFolder);
                            }
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.pnlMainContentId);
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.pnlMainContentId);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                });
            };
            _this._treeView_ClearSelectionOnCollapse = function (sender, args) {
                var selectedNode = _this._processesTreeView.get_selectedNode();
                if (!selectedNode) {
                    return;
                }
                selectedNode.set_selected(false);
                _this._moveFascicleConfirmBtn.set_enabled(false);
            };
            _this._treeView_SetConfirmBtnStateOnNodeClick = function (sender, args) {
                var selectedNode = args.get_node();
                var nodeType = selectedNode.get_attributes().getAttribute(FascProcessMove.NODETYPE_ATTRNAME);
                var confirmBtnEnableState = (nodeType === TreeNodeType.DossierFolder) && (selectedNode.get_value() !== _this.fascicleParentFolderId);
                _this._moveFascicleConfirmBtn.set_enabled(confirmBtnEnableState);
            };
            _this._treeView_LoadNodeChildrenOnExpand = function (sender, args) {
                var expandedNode = args.get_node();
                expandedNode.get_nodes().clear();
                var nodeType = expandedNode.get_attributes().getAttribute(FascProcessMove.NODETYPE_ATTRNAME);
                var expandedNodeAction = _this._nodeTypeExpandingActionsDictionary[nodeType];
                if (expandedNodeAction) {
                    expandedNode.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    expandedNodeAction(expandedNode)
                        .done(function () {
                        expandedNode.hideLoadingStatus();
                        expandedNode.set_expanded(true);
                    })
                        .fail(function (exception) {
                        _this.showNotificationException(_this.uscNotificationId, exception);
                        expandedNode.hideLoadingStatus();
                    });
                }
            };
            _this._createSourceDossierFolderCopy = function (targetDossierFolder) {
                var dossierFolderCopy = {};
                dossierFolderCopy.ParentInsertId = targetDossierFolder.UniqueId;
                dossierFolderCopy.Category = {};
                dossierFolderCopy.Category.EntityShortId = _this._sourceDossierFolderModel.Category.EntityShortId;
                dossierFolderCopy.Dossier = {};
                dossierFolderCopy.Dossier.UniqueId = targetDossierFolder.Dossier.UniqueId;
                dossierFolderCopy.Fascicle = {};
                dossierFolderCopy.Fascicle.UniqueId = _this._sourceDossierFolderModel.Fascicle.UniqueId;
                dossierFolderCopy.JsonMetadata = _this._sourceDossierFolderModel.JsonMetadata;
                dossierFolderCopy.DossierFolders = _this._sourceDossierFolderModel.DossierFolders;
                dossierFolderCopy.FascicleTemplates = _this._sourceDossierFolderModel.FascicleTemplates;
                dossierFolderCopy.DossierFolderRoles = _this._sourceDossierFolderModel.DossierFolderRoles;
                return dossierFolderCopy;
            };
            _this._createNewDossierFolder = function (targetDossierFolder) {
                var newDossierFolder = {};
                newDossierFolder.ParentInsertId = targetDossierFolder.UniqueId;
                newDossierFolder.Category = {};
                newDossierFolder.Category.EntityShortId = _this._sourceFascicleModel.Category.EntityShortId;
                newDossierFolder.Dossier = {};
                newDossierFolder.Dossier.UniqueId = targetDossierFolder.Dossier.UniqueId;
                newDossierFolder.Fascicle = {};
                newDossierFolder.Fascicle.UniqueId = _this._sourceFascicleModel.UniqueId;
                return newDossierFolder;
            };
            var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME);
            _this._fascicleService = new FascicleService(fascicleServiceConfiguration);
            var processConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
            _this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
            _this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            return _this;
        }
        FascProcessMove.prototype._treeRootNode = function () {
            return this._processesTreeView.get_nodes().getNode(0);
        };
        FascProcessMove.prototype._sourceFascicleParentIsFolder = function () {
            return !!this.fascicleParentFolderId;
        };
        FascProcessMove.prototype.initialize = function () {
            this._initializeControls();
            this._initializeDictionaries();
            if (this._sourceFascicleParentIsFolder()) {
                this._initializeSourceDossierFolder();
            }
            else {
                this._initializeSourceFascicle();
            }
        };
        FascProcessMove.prototype._initializeSourceFascicle = function () {
            var _this = this;
            this._fascicleService.getFascicle(this.sourceFascicleId, function (fascicleModel) {
                _this._sourceFascicleModel = fascicleModel;
                var fascicleLabelText = fascicleModel.Title + "-" + fascicleModel.FascicleObject;
                var rootNodeText = fascicleModel.Category.Code + "." + fascicleModel.Category.Name;
                _this._initializeFascNameLabelAndTreeView(fascicleLabelText, rootNodeText, fascicleModel.Category.EntityShortId);
                _this._loadCategoryProcesses(fascicleModel.Category.EntityShortId);
            }, function (exception) { return _this.showNotificationException(_this.uscNotificationId, exception); });
        };
        FascProcessMove.prototype._initializeSourceDossierFolder = function () {
            var _this = this;
            this._dossierFolderService.getFullDossierFolder(this.sourceFascicleId, function (dossierFolder) {
                _this._sourceDossierFolderModel = dossierFolder;
                var fascicleLabelText = dossierFolder.Fascicle.Title + "-" + dossierFolder.Fascicle.FascicleObject;
                var rootNodeText = dossierFolder.Category.Code + "." + dossierFolder.Category.Name;
                _this._initializeFascNameLabelAndTreeView(fascicleLabelText, rootNodeText, dossierFolder.Category.EntityShortId);
                _this._loadCategoryProcesses(dossierFolder.Category.EntityShortId);
            });
        };
        FascProcessMove.prototype._loadCategoryProcesses = function (categoryId) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._processService.getAvailableProcesses(null, true, categoryId, null, function (categoryProcesses) {
                categoryProcesses.forEach(function (categoryProcess) {
                    var currentProcessTreeNode = _this._createTreeNode(TreeNodeType.Process, categoryProcess.Name, categoryProcess.UniqueId, FascProcessMove.PROCESS_IMGURL, _this._treeRootNode());
                    _this._appendEmptyNode(currentProcessTreeNode);
                });
                _this._treeRootNode().hideLoadingStatus();
                defferedRequest.resolve();
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        FascProcessMove.prototype._loadProcessDossierFolders = function (processId, parentNode) {
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
                _this._renderDossierFolders(processDossierFoldersViewModels, parentNode);
                defferedRequest.resolve();
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        FascProcessMove.prototype._loadDossierFoldersChildren = function (parentId, parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._dossierFolderService.getChildren(parentId, 0, function (dossierFolders) {
                var childrenFolders = dossierFolders.filter(function (dossierFolder) { return !dossierFolder.idFascicle; });
                if (!childrenFolders.length) {
                    _this._appendEmptyNode(parentNode);
                    defferedRequest.resolve();
                    return;
                }
                _this._renderDossierFolders(dossierFolders, parentNode);
                defferedRequest.resolve();
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        FascProcessMove.prototype._deleteDossierFolderFascicle = function (dossierFolder) {
            var _this = this;
            this._dossierFolderService.updateDossierFolder(dossierFolder, UpdateActionType.RemoveFascicleFromDossierFolder, function (data) {
                dossierFolder.ParentInsertId = _this.fascicleParentFolderId;
                _this._deleteDossierFolder(dossierFolder);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pnlMainContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascProcessMove.prototype._deleteDossierFolder = function (dossierFolder) {
            var _this = this;
            this._dossierFolderService.deleteDossierFolder(dossierFolder, function (data) {
                _this._loadingPanel.hide(_this.pnlMainContentId);
                _this.closeWindow(_this._moveFascicleResponseModel);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pnlMainContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascProcessMove.prototype._initializeControls = function () {
            this._windowManager = $find(this.radWindowManagerId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._processesTreeView = $find(this.processesTreeViewId);
            this._processesTreeView.add_nodeExpanding(this._treeView_LoadNodeChildrenOnExpand);
            this._processesTreeView.add_nodeCollapsed(this._treeView_ClearSelectionOnCollapse);
            this._processesTreeView.add_nodeClicked(this._treeView_SetConfirmBtnStateOnNodeClick);
            this._fascicleNameLabel = document.getElementById(this.lblFascNameId);
            this._moveFascicleConfirmBtn = $find(this.moveFascicleConfirmBtnId);
            this._moveFascicleConfirmBtn.add_clicked(this._moveFascicleToDossierFolder);
            this._moveFascicleConfirmBtn.set_enabled(false);
        };
        FascProcessMove.prototype._initializeFascNameLabelAndTreeView = function (fascicleLabelValue, categoryName, categoryId) {
            this._fascicleNameLabel.innerText = fascicleLabelValue;
            this._addTreeRootNode(categoryName, categoryId);
        };
        FascProcessMove.prototype._initializeDictionaries = function () {
            /// Nodes expanding event handlers registration
            this._nodeTypeExpandingActionsDictionary = {};
            this._registerNodeTypesExpandingActions();
            /// Dossier folders images and tooltips
            this._dossierFolderStatusImageDictionary = {};
            this._dossierFolderStatusTooltipDictionary = {};
            this._registerDossierFoldersStatusImages();
            this._registerDossierFoldersStatusTooltips();
            /// Expanded folders images
            this._expandedFolderImageDictionary = {};
            this._registerExpandedFoldersImages();
        };
        FascProcessMove.prototype._registerNodeTypesExpandingActions = function () {
            var _this = this;
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.Root] = function (expandedNode) {
                var defferedRequest = $.Deferred();
                _this._loadCategoryProcesses(expandedNode.get_value())
                    .done(function () { return defferedRequest.resolve(); })
                    .fail(function (exception) { return defferedRequest.reject(exception); });
                return defferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.Process] = function (parentNode) {
                var defferedRequest = $.Deferred();
                _this._loadProcessDossierFolders(parentNode.get_value(), parentNode)
                    .done(function () { return defferedRequest.resolve(); })
                    .fail(function (exception) { return defferedRequest.reject(exception); });
                return defferedRequest.promise();
            };
            this._nodeTypeExpandingActionsDictionary[TreeNodeType.DossierFolder] = function (parentNode) {
                var defferedRequest = $.Deferred();
                _this._loadDossierFoldersChildren(parentNode.get_value(), parentNode)
                    .done(function () { return defferedRequest.resolve(); })
                    .fail(function (exception) { return defferedRequest.reject(exception); });
                return defferedRequest.promise();
            };
        };
        FascProcessMove.prototype._appendEmptyNode = function (treeNode) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text("Nessun elemento trovato");
            treeNode.get_nodes().add(emptyNode);
        };
        FascProcessMove.prototype._addTreeRootNode = function (rootNodeText, rootNodeValue) {
            var rootNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text(rootNodeText);
            rootNode.set_value(rootNodeValue);
            rootNode.set_imageUrl(FascProcessMove.ROOTNODE_IMGURL);
            rootNode.get_attributes().setAttribute(FascProcessMove.NODETYPE_ATTRNAME, TreeNodeType.Root);
            rootNode.set_expanded(true);
            this._processesTreeView.get_nodes().add(rootNode);
        };
        FascProcessMove.prototype._renderDossierFolders = function (dossierFolders, parentNode) {
            var _this = this;
            dossierFolders.forEach(function (dossierFolder) {
                var dossierFolderIsFascicle = !!dossierFolder.idFascicle;
                if (!dossierFolderIsFascicle) {
                    var nodeValue = dossierFolderIsFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;
                    var dossierFolderImageUrl = _this._dossierFolderStatusImageDictionary[DossierFolderStatus[dossierFolder.Status]];
                    var dossierFolderTooltip = _this._dossierFolderStatusTooltipDictionary[DossierFolderStatus[dossierFolder.Status]];
                    var dossierFolderExpandedImageUrl = FascProcessMove.EXPANDEDFOLDER_IMGURL;
                    var currentDossierFolderTreeNode = _this._createTreeNode(TreeNodeType.DossierFolder, dossierFolder.Name, nodeValue, dossierFolderImageUrl, parentNode, dossierFolderTooltip, dossierFolderExpandedImageUrl);
                    if (dossierFolder.UniqueId === _this.fascicleParentFolderId) {
                        currentDossierFolderTreeNode.disable();
                    }
                    _this._appendEmptyNode(currentDossierFolderTreeNode);
                }
            });
        };
        FascProcessMove.prototype._registerDossierFoldersStatusImages = function () {
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.DoAction] = "../App_Themes/DocSuite2008/imgset16/folder_hidden.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.InProgress] = "../App_Themes/DocSuite2008/imgset16/folder_hidden.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.Fascicle] = "../App_Themes/DocSuite2008/imgset16/fascicle_open.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.FascicleClose] = "../App_Themes/DocSuite2008/imgset16/fascicle_close.png";
            this._dossierFolderStatusImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
        };
        FascProcessMove.prototype._registerDossierFoldersStatusTooltips = function () {
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.DoAction] = "Da gestire";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.InProgress] = "Da gestire";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Fascicle] = "Fascicolo";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.FascicleClose] = "Fascicolo chiuso";
            this._dossierFolderStatusTooltipDictionary[DossierFolderStatus.Folder] = "Cartella con sottocartelle";
        };
        FascProcessMove.prototype._registerExpandedFoldersImages = function () {
            this._expandedFolderImageDictionary[DossierFolderStatus.Folder] = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
            this._expandedFolderImageDictionary[FascicleFolderTypology.SubFascicle] = "../App_Themes/DocSuite2008/imgset16/folder_internet_open.png";
        };
        FascProcessMove.prototype.closeWindow = function (fascMoveResponseModel) {
            var wnd = this.getRadWindow();
            wnd.close(fascMoveResponseModel);
        };
        FascProcessMove.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
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
        FascProcessMove.prototype._createTreeNode = function (nodeType, nodeDescription, nodeValue, imageUrl, parentNode, tooltipText, expandedImageUrl) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            treeNode.set_text(nodeDescription);
            treeNode.set_value(nodeValue);
            treeNode.get_attributes().setAttribute(FascProcessMove.NODETYPE_ATTRNAME, nodeType);
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
        FascProcessMove.ROOTNODE_IMGURL = "../Comm/images/folderopen16.gif";
        FascProcessMove.PROCESS_IMGURL = "../App_Themes/DocSuite2008/imgset16/process.png";
        FascProcessMove.EXPANDEDFOLDER_IMGURL = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
        FascProcessMove.NODETYPE_ATTRNAME = "NodeType";
        return FascProcessMove;
    }(FascBase));
    return FascProcessMove;
});
//# sourceMappingURL=FascProcessMove.js.map