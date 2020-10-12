/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/core/extensions/number.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Commons/CategoryService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleType", "App/Services/Processes/ProcessService", "App/Models/Processes/ProcessNodeType", "App/Services/Dossiers/DossierFolderService", "App/Mappers/Dossiers/DossierSummaryFolderViewModelMapper", "App/Services/Processes/ProcessFascicleTemplateService", "Tblt/TbltProcess", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, CategoryService, ServiceConfigurationHelper, FascicleType, ProcessService, ProcessNodeType, DossierFolderService, DossierSummaryFolderViewModelMapper, ProcessFascicleTemplateService, TbltProcess, SessionStorageKeysHelper) {
    var CommonSelCategoryRest = /** @class */ (function () {
        function CommonSelCategoryRest(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            /**
             * Evento scatenato al expand di un nodo
             * @method
             * @param sender
             * @param eventArgs
             * @return
             */
            this.treeViewCategory_ClientNodeExpanding = function (sender, args) {
                var node = args.get_node();
                var expandedNodeType = node.get_attributes().getAttribute(CommonSelCategoryRest.NODETYPE_ATTRNAME);
                var hasFascicleInsertRights = _this._btnSearchOnlyFascicolable.get_checked();
                if ((_this.showProcesses && hasFascicleInsertRights && expandedNodeType === ProcessNodeType.Category) || expandedNodeType === ProcessNodeType.TreeRootNode) {
                    return;
                }
                node.get_nodes().clear();
                node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                if (expandedNodeType === ProcessNodeType.Root) {
                    _this.findProcesses(node)
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () { return node.hideLoadingStatus(); });
                }
                else if (expandedNodeType === ProcessNodeType.Process) {
                    _this._loadProcessDossierFolders(node)
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () { return node.hideLoadingStatus(); });
                }
                else if (expandedNodeType === ProcessNodeType.DossierFolder) {
                    _this._loadDossierFoldersChildren(node)
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () { return node.hideLoadingStatus(); });
                }
                else if (node.get_nodes().get_count() == 0) {
                    _this.findCategories(node.get_value())
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () { return node.hideLoadingStatus(); });
                }
            };
            /**
             * Evento scatenato al click di un nodo
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.treeViewCategory_ClientNodeClicked = function (sender, eventArgs) {
                var node = eventArgs.get_node();
                if (node.get_value() && node.get_nodes().get_count() == 0 && !_this.hasFilters()) {
                    _this.findCategories(node.get_value())
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () { return node.hideLoadingStatus(); });
                }
            };
            this.btnSearch_OnClick = function (sender, args) {
                sender.preventDefault();
                _this._loadingPanel.show(_this.pnlMainContentId);
                _this.findCategories()
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._loadingPanel.hide(_this.pnlMainContentId); });
            };
            this.btnConfirm_OnClick = function (sender, args) {
                sender.preventDefault();
                var selectedNode = _this._treeViewCategory.get_selectedNode();
                if (_this.showProcesses) {
                    var processFascicleTemplateParents = [
                        {
                            text: selectedNode.get_text(),
                            value: selectedNode.get_value(),
                            icon: selectedNode.get_imageUrl(),
                            cssClass: selectedNode.get_cssClass(),
                            nodeType: selectedNode.get_attributes().getAttribute("NodeType")
                        }
                    ];
                    _this.getAllParents(selectedNode.get_parent(), processFascicleTemplateParents);
                    _this.closeWindow(processFascicleTemplateParents);
                }
                else {
                    if (!selectedNode || !selectedNode.get_value()) {
                        _this.showNotificationException(null, "Selezionare almeno un classificatore");
                        return;
                    }
                    var category = _this.getCategoryFromNode(selectedNode);
                    if (_this.fascicleBehavioursEnabled && !category.HasFascicleDefinition) {
                        _this.showNotificationException(null, "Non si dispongono i permessi per questa voce del piano di fascicolazione.");
                        return;
                    }
                    _this.closeWindow(category);
                }
            };
            this.btnSearchCode_OnClick = function (sender, args) {
                sender.preventDefault();
                var inputCodeIsValid = _this._validateSearchCode(_this.txtSearchCode.val());
                if (!inputCodeIsValid) {
                    _this.showNotificationException(null, "Il codice inserito non è formattato correttamente");
                    return;
                }
                _this._loadingPanel.show(_this.pnlMainContentId);
                _this.findCategories()
                    .done(function () {
                    var foundCategories = sessionStorage[SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES];
                    if (!foundCategories) {
                        _this.showNotificationException(null, "Il codice cercato è inesistente");
                        return;
                    }
                    var categories = JSON.parse(foundCategories);
                    if (categories.length > 1) {
                        _this.showNotificationException(null, "Il codice cercato non è univoco");
                        return;
                    }
                    if (_this.fascicleBehavioursEnabled && !categories[0].HasFascicleDefinition) {
                        _this.showNotificationException(null, "Non si dispongono i permessi per questa voce del piano di fascicolazione.");
                        return;
                    }
                    sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES);
                    _this.closeWindow(categories[0]);
                })
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._loadingPanel.hide(_this.pnlMainContentId); });
            };
            this.btnSearchOnlyFascicolable_OnClick = function (sender, args) {
                _this._loadingPanel.show(_this.pnlMainContentId);
                _this.findCategories()
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._loadingPanel.hide(_this.pnlMainContentId); });
            };
            this.treeViewCategory_nodeClicked = function (sender, args) {
                var buttonDisabled = args.get_node().get_text() === CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT
                    || args.get_node().get_text() === CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT
                    || args.get_node().get_level() === 0
                    || args.get_node().get_attributes().getAttribute(TbltProcess.NodeType_TYPE_NAME) === ProcessNodeType.Process;
                $("#" + _this.btnConfermaId).prop('disabled', buttonDisabled);
            };
            var categoryServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryServiceConfiguration);
            var processConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Process");
            this._processService = new ProcessService(processConfiguration);
            var dossierFolderConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DossierFolder");
            this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            var processFascicleTemplateConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "ProcessFascicleTemplate");
            this._processFascicleTemplateService = new ProcessFascicleTemplateService(processFascicleTemplateConfiguration);
        }
        Object.defineProperty(CommonSelCategoryRest.prototype, "rootNode", {
            get: function () {
                return this._treeViewCategory.get_nodes().getNode(0);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelCategoryRest.prototype, "txtSearch", {
            get: function () {
                return $("#" + this.txtSearchId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelCategoryRest.prototype, "txtSearchCode", {
            get: function () {
                return $("#" + this.txtSearchCodeId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelCategoryRest.prototype, "lblDescription", {
            get: function () {
                return $("#" + this.lblDescriptionId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(CommonSelCategoryRest.prototype, "cachedCategories", {
            get: function () {
                var categories = [];
                if (sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CACHE_CATEGORIES]) {
                    categories = JSON.parse(sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CACHE_CATEGORIES]);
                }
                return categories;
            },
            enumerable: true,
            configurable: true
        });
        CommonSelCategoryRest.prototype.getAllParents = function (node, treeNodeModel) {
            if (!(node instanceof Telerik.Web.UI.RadTreeView)) {
                treeNodeModel.push({
                    text: node.get_text(),
                    value: node.get_value(),
                    cssClass: node.get_cssClass(),
                    icon: node.get_imageUrl(),
                    nodeType: node.get_attributes().getAttribute("NodeType")
                });
                this.getAllParents(node.get_parent(), treeNodeModel);
            }
        };
        CommonSelCategoryRest.prototype._validateSearchCode = function (fullCode) {
            if (!fullCode) {
                return false;
            }
            var codeStringFragments = fullCode.split('.');
            var invalidLengthFragments = codeStringFragments.some(function (stringFragment) { return stringFragment.length > CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH; });
            var fragmentsNotNumeric = codeStringFragments.some(function (stringFragment) { return isNaN(Number(stringFragment)); });
            if (invalidLengthFragments || fragmentsNotNumeric) {
                return false;
            }
            return true;
        };
        /**
        *------------------------- Methods -----------------------------
        */
        CommonSelCategoryRest.prototype.initialize = function () {
            var _this = this;
            this._treeViewCategory = $find(this.treeViewCategoryId);
            this._treeViewCategory.add_nodeClicked(this.treeViewCategory_nodeClicked);
            this._btnSearchOnlyFascicolable = $find(this.btnSearchOnlyFascicolableId);
            this._btnSearchOnlyFascicolable.add_clicked(this.btnSearchOnlyFascicolable_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#" + this.btnSearchId).click(this.btnSearch_OnClick);
            $("#" + this.btnSearchCodeId).click(this.btnSearchCode_OnClick);
            $("#" + this.btnConfermaId).click(this.btnConfirm_OnClick);
            $("#" + this.btnConfermaId).prop('disabled', true);
            $("#" + this.rowOnlyFascicolableId).hide();
            if (this.fascicleBehavioursEnabled) {
                $("#" + this.rowOnlyFascicolableId).show();
                this._btnSearchOnlyFascicolable.set_checked(true);
            }
            this.rootNode.get_attributes().setAttribute(CommonSelCategoryRest.NODETYPE_ATTRNAME, ProcessNodeType.TreeRootNode);
            sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES);
            this._loadingPanel.show(this.pnlMainContentId);
            this.initializeDescription();
            this.initCategoriesCache()
                .done(function () {
                _this.findCategories()
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._loadingPanel.hide(_this.pnlMainContentId); });
            })
                .fail(function (exception) {
                _this._loadingPanel.hide(_this.pnlMainContentId);
                _this.showNotificationException(exception);
            });
        };
        CommonSelCategoryRest.prototype.initializeDescription = function () {
            $("#" + this.pnlDescriptionId).hide();
            if (this.fascicleBehavioursEnabled) {
                $("#" + this.pnlDescriptionId).show();
                if (this.manager) {
                    this.lblDescription.text("Visualizzazione filtrata in base al ruolo di funzione");
                    return;
                }
                if (this.role) {
                    this.lblDescription.text("Visualizzazione filtrata in base al Settore responsabile selezionato");
                    return;
                }
                if (this.fascicleBehavioursEnabled) {
                    this.lblDescription.text("Visualizzazione filtrata sulle proprie autorizzazioni dei diritti di fascicolazione");
                    return;
                }
            }
        };
        CommonSelCategoryRest.prototype.initCategoriesCache = function () {
            var promise = $.Deferred();
            var finder = {};
            finder.FascicleFilterEnabled = false;
            finder.IdTenantAOO = this.currentTenantAOOId;
            this._categoryService.findTreeCategories(finder, function (data) {
                sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CACHE_CATEGORIES] = JSON.stringify(data);
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype.findCategories = function (parentId) {
            var _this = this;
            var promise = $.Deferred();
            var finder = this.prepareFinder(parentId);
            finder.IdTenantAOO = this.currentTenantAOOId;
            this._categoryService.findTreeCategories(finder, function (data) {
                _this.populateTreeView(data, (!parentId || parentId == 0))
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype.prepareFinder = function (parentId) {
            var finder = {};
            parentId = parentId ? parentId : this.parentId;
            finder.LoadRoot = !this.hasFilters() && (!parentId || parentId == 0);
            finder.ParentId = parentId;
            finder.Name = this.txtSearch.val();
            finder.FullCode = this.formatFullCode(this.txtSearchCode.val());
            finder.ParentAllDescendants = this.includeParentDescendants;
            if (this.fascicleBehavioursEnabled) {
                finder.HasFascicleInsertRights = this._btnSearchOnlyFascicolable.get_checked() && this.fascicleType == FascicleType.Procedure;
                finder.IdRole = this.role;
                finder.IdContainer = this.container;
                if (this.fascicleType == FascicleType.Procedure) {
                    finder.Manager = this.manager;
                    finder.Secretary = this.secretary;
                }
                finder.FascicleType = this.fascicleType.toString();
                finder.FascicleFilterEnabled = this._btnSearchOnlyFascicolable.get_checked();
            }
            return finder;
        };
        CommonSelCategoryRest.prototype.formatFullCode = function (fullCode) {
            var inputCodeIsValid = this._validateSearchCode(this.txtSearchCode.val());
            if (!inputCodeIsValid) {
                return '';
            }
            var codeStringFragments = fullCode.split('.');
            var fullCodeFormatted = codeStringFragments.map(function (stringFragment) {
                var fragmentLength = stringFragment.length;
                return fragmentLength == CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH ? Number(stringFragment) : Number(stringFragment).padLeft(CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH);
            }).join('|');
            return fullCodeFormatted;
        };
        CommonSelCategoryRest.prototype.populateTreeView = function (categories, needClearItems) {
            var _this = this;
            if (needClearItems === void 0) { needClearItems = true; }
            var promise = $.Deferred();
            if (!categories || categories.length == 0) {
                if (this.hasFilters()) {
                    sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES);
                }
                if (needClearItems) {
                    this.rootNode.get_nodes().clear();
                }
                return promise.resolve();
            }
            var nodeSource = [];
            if (!needClearItems) {
                nodeSource = this._treeViewCategory.get_allNodes();
            }
            if (this.hasFilters()) {
                sessionStorage[SessionStorageKeysHelper.SESSION_KEY_FOUND_CATEGORIES] = JSON.stringify(categories);
            }
            var hasFascicleRights = this._btnSearchOnlyFascicolable.get_checked();
            this.createNodes(categories, nodeSource)
                .done(function () {
                if (needClearItems) {
                    _this.rootNode.get_nodes().clear();
                    for (var _i = 0, nodeSource_1 = nodeSource; _i < nodeSource_1.length; _i++) {
                        var node = nodeSource_1[_i];
                        _this.rootNode.get_nodes().add(node);
                        if (hasFascicleRights && _this.showProcesses && node.get_nodes().get_count() === 0) {
                            _this.createProcessesNode(node);
                        }
                    }
                }
                _this.rootNode.expand();
                _this._treeViewCategory.commitChanges();
                promise.resolve();
            })
                .fail(function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype.createNodes = function (categories, nodeSource) {
            var _this = this;
            var promise = $.Deferred();
            if (!categories || categories.length == 0) {
                return promise.resolve();
            }
            var hasFascicleRights = this._btnSearchOnlyFascicolable.get_checked();
            var hasFilters = this.hasFilters();
            var currentCategory = categories.shift();
            this.createNode(currentCategory, nodeSource)
                .done(function (node) {
                if (hasFilters) {
                    var toAppendClass = node.get_cssClass();
                    node.set_cssClass(toAppendClass + " dsw-text-bold");
                }
                if (_this.fascicleBehavioursEnabled) {
                    if (currentCategory.HasFascicleDefinition) {
                        node.get_attributes().setAttribute("HasFascicleDefinition", true);
                        node.set_cssClass("node-tree-fascicle");
                    }
                }
                //if (categories.length == 0) {
                //    return promise.resolve();
                //}
                _this.createNodes(categories, nodeSource)
                    .done(function () {
                    if (hasFascicleRights && _this.showProcesses) {
                        _this.createProcessesNode(node);
                    }
                    promise.resolve();
                })
                    .fail(function (exception) { return promise.reject(exception); });
            });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype.createNode = function (category, nodeSource) {
            var promise = $.Deferred();
            var currentNode = this.findNodeFromSource(nodeSource, category.IdCategory);
            if (currentNode) {
                return promise.resolve(currentNode);
            }
            currentNode = new Telerik.Web.UI.RadTreeNode();
            currentNode.set_text(category.Code + "." + category.Name);
            currentNode.set_value(category.IdCategory);
            this.setNodeAttributes(currentNode, category);
            if (category.HasChildren) {
                if (!this._btnSearchOnlyFascicolable.get_checked()) {
                    this.createEmptyNode(currentNode);
                }
                currentNode.set_imageUrl("../Comm/images/folderopen16.gif");
            }
            else {
                currentNode.set_imageUrl("../Comm/images/Classificatore.gif");
            }
            if (this.fascicleBehavioursEnabled) {
                if (!category.HasFascicleDefinition) {
                    currentNode.set_cssClass("node-disabled");
                }
            }
            currentNode.get_attributes().setAttribute("NodeType", ProcessNodeType.Category);
            if (category.IdParent) {
                var parentNode = this.findNodeFromSource(nodeSource, category.IdParent);
                if (parentNode) {
                    parentNode.get_nodes().add(currentNode);
                    parentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                    parentNode.set_expanded(true);
                    return promise.resolve(currentNode);
                }
                var parentFromCache = this.cachedCategories.filter(function (item) { return item.IdCategory == category.IdParent; })[0];
                if (!parentFromCache) {
                    nodeSource.push(currentNode);
                    return promise.resolve(currentNode);
                }
                this.createNode(parentFromCache, nodeSource)
                    .done(function (node) {
                    node.get_nodes().add(currentNode);
                    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                    node.set_expanded(true);
                    promise.resolve(currentNode);
                })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            else {
                nodeSource.push(currentNode);
                return promise.resolve(currentNode);
            }
            return promise.promise();
        };
        CommonSelCategoryRest.prototype.setNodeAttributes = function (node, category) {
            node.get_attributes().setAttribute("UniqueId", category.UniqueId);
            node.get_attributes().setAttribute("HasFascicleDefinition", category.HasFascicleDefinition);
            node.get_attributes().setAttribute("IdCategory", category.IdCategory);
            node.get_attributes().setAttribute("Name", category.Name);
            node.get_attributes().setAttribute("FullCode", category.FullCode);
            node.get_attributes().setAttribute("Code", category.Code);
            node.get_attributes().setAttribute("FullIncrementalPath", category.FullIncrementalPath);
            node.get_attributes().setAttribute("HasChildren", category.HasChildren);
            node.get_attributes().setAttribute("IdParent", category.IdParent);
        };
        CommonSelCategoryRest.prototype.getCategoryFromNode = function (node) {
            var category = {};
            category.UniqueId = node.get_attributes().getAttribute("UniqueId");
            category.HasFascicleDefinition = node.get_attributes().getAttribute("HasFascicleDefinition");
            category.IdCategory = node.get_attributes().getAttribute("IdCategory");
            category.Name = node.get_attributes().getAttribute("Name");
            category.FullCode = node.get_attributes().getAttribute("FullCode");
            category.Code = node.get_attributes().getAttribute("Code");
            category.FullIncrementalPath = node.get_attributes().getAttribute("FullIncrementalPath");
            category.HasChildren = node.get_attributes().getAttribute("HasChildren");
            category.IdParent = node.get_attributes().getAttribute("IdParent");
            return category;
        };
        CommonSelCategoryRest.prototype.getProcessFascicleTemplateFromNode = function (node) {
            var processFascicleTemplateModel = {};
            processFascicleTemplateModel.UniqueId = node.get_value();
            processFascicleTemplateModel.Name = node.get_text();
            return processFascicleTemplateModel;
        };
        CommonSelCategoryRest.prototype.findNodeFromSource = function (nodeSource, idCategory) {
            var foundNode = null;
            for (var _i = 0, nodeSource_2 = nodeSource; _i < nodeSource_2.length; _i++) {
                var sourceNode = nodeSource_2[_i];
                if (sourceNode.get_value() == idCategory) {
                    return sourceNode;
                }
                if (sourceNode.get_allNodes()) {
                    foundNode = this.findNodeFromSource(sourceNode.get_allNodes(), idCategory);
                }
                if (foundNode) {
                    return foundNode;
                }
            }
        };
        CommonSelCategoryRest.prototype.hasFilters = function () {
            return this.txtSearch.val() || this._validateSearchCode(this.txtSearchCode.val()) || this._btnSearchOnlyFascicolable.get_checked();
        };
        CommonSelCategoryRest.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        CommonSelCategoryRest.prototype.closeWindow = function (dataToReturn) {
            var wnd = this.getRadWindow();
            wnd.close(dataToReturn);
        };
        CommonSelCategoryRest.prototype.showNotificationException = function (exception, customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception) {
                    uscNotification.showNotification(exception);
                    return;
                }
                uscNotification.showWarningMessage(customMessage);
            }
        };
        CommonSelCategoryRest.prototype.createProcessesNode = function (parentNode) {
            var processesNode = new Telerik.Web.UI.RadTreeNode();
            processesNode.set_text(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT);
            processesNode.set_cssClass("dsw-text-bold");
            processesNode.get_attributes().setAttribute("NodeType", ProcessNodeType.Root);
            this.createEmptyNode(processesNode);
            parentNode.get_nodes().add(processesNode);
            parentNode.expand();
        };
        CommonSelCategoryRest.prototype.createEmptyNode = function (parentNode) {
            var emptyNode = new Telerik.Web.UI.RadTreeNode();
            emptyNode.set_text(CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT);
            parentNode.get_nodes().add(emptyNode);
        };
        CommonSelCategoryRest.prototype.findProcesses = function (node) {
            var _this = this;
            var promise = $.Deferred();
            this._processService.getAvailableProcesses(null, true, +node.get_parent().get_value(), null, function (categoryProcesses) {
                if (!categoryProcesses.length) {
                    _this.createEmptyNode(node);
                    promise.resolve();
                    return;
                }
                categoryProcesses.map(function (process) {
                    var currentProcessTreeNode = _this._createTreeNode(ProcessNodeType.Process, process.Name, process.UniqueId, "../App_Themes/DocSuite2008/imgset16/process.png");
                    _this.createEmptyNode(currentProcessTreeNode);
                    return currentProcessTreeNode;
                }).forEach(function (child) {
                    node.get_nodes().add(child);
                });
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype._loadProcessDossierFolders = function (parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._dossierFolderService.getProcessFolders(null, parentNode.get_value(), false, false, function (processDossierFolders) {
                _this.loadProcessFascicleTemplate(parentNode);
                if (!processDossierFolders.length) {
                    _this.createEmptyNode(parentNode);
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
        CommonSelCategoryRest.prototype._addDossierFolderNodesRecursive = function (dossierFolders, parentNode) {
            var _this = this;
            dossierFolders.forEach(function (dossierFolder) {
                var dossierFolderImageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                var dossierFolderExpandedImageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                var dossierFolderNodeValue = dossierFolder.idFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;
                var currentDossierFolderTreeNode = _this._createTreeNode(ProcessNodeType.DossierFolder, dossierFolder.Name, dossierFolderNodeValue, dossierFolderImageUrl, parentNode, null, dossierFolderExpandedImageUrl);
                if (dossierFolder.DossierFolders.length > 0) {
                    _this._addDossierFolderNodesRecursive(dossierFolder.DossierFolders, currentDossierFolderTreeNode);
                }
                else {
                    _this.createEmptyNode(currentDossierFolderTreeNode);
                }
            });
        };
        CommonSelCategoryRest.prototype._loadDossierFoldersChildren = function (parentNode) {
            var _this = this;
            var defferedRequest = $.Deferred();
            this._dossierFolderService.getProcessFascicleChildren(parentNode.get_value(), 0, function (dossierFolders) {
                _this.loadProcessFascicleTemplate(parentNode);
                if (!dossierFolders.length) {
                    _this.createEmptyNode(parentNode);
                    defferedRequest.resolve();
                    return;
                }
                dossierFolders.forEach(function (dossierFolder) {
                    var dossierFolderIsFascicle = !!dossierFolder.idFascicle;
                    var nodeType = ProcessNodeType.DossierFolder;
                    var nodeValue = dossierFolderIsFascicle ? dossierFolder.idFascicle : dossierFolder.UniqueId;
                    var dossierFolderImageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                    var dossierFolderExpandedImageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                    var currentDossierFolderTreeNode = _this._createTreeNode(nodeType, dossierFolder.Name, nodeValue, dossierFolderImageUrl, parentNode, null, dossierFolderExpandedImageUrl);
                    _this.createEmptyNode(currentDossierFolderTreeNode);
                });
                defferedRequest.resolve();
            }, function (exception) { return defferedRequest.reject(exception); });
            return defferedRequest.promise();
        };
        CommonSelCategoryRest.prototype.loadProcessFascicleTemplate = function (parentNode) {
            var _this = this;
            var promise = $.Deferred();
            this._dossierFolderService.getFascicleTemplatesByDossierFolderId(parentNode.get_value(), function (data) {
                if (!data.length) {
                    promise.resolve();
                    return;
                }
                if (parentNode.get_nodes().get_count() > 0 && parentNode.get_nodes().getNode(0).get_text() == CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT) {
                    parentNode.get_nodes().clear();
                }
                data.map(function (processFascicleTemplate) {
                    var currentProcessTreeNode = _this._createTreeNode(ProcessNodeType.ProcessFascicleTemplate, processFascicleTemplate.Name, processFascicleTemplate.UniqueId, "../App_Themes/DocSuite2008/imgset16/fascicle_close.png");
                    return currentProcessTreeNode;
                }).forEach(function (child) {
                    parentNode.get_nodes().add(child);
                });
                promise.resolve();
            });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype._createTreeNode = function (nodeType, nodeDescription, nodeValue, imageUrl, parentNode, tooltipText, expandedImageUrl) {
            var treeNode = new Telerik.Web.UI.RadTreeNode();
            treeNode.set_text(nodeDescription);
            treeNode.set_value(nodeValue);
            treeNode.get_attributes().setAttribute("NodeType", nodeType);
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
        CommonSelCategoryRest.CATEGORYFULLCODE_MAXLENGTH = 4;
        CommonSelCategoryRest.NODETYPE_ATTRNAME = "NodeType";
        CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT = "Serie e volumi";
        CommonSelCategoryRest.NO_ITEMS_FOUND_TEXT = "Nessun elemento trovato";
        return CommonSelCategoryRest;
    }());
    return CommonSelCategoryRest;
});
//# sourceMappingURL=CommonSelCategoryRest.js.map