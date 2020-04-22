/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/core/extensions/number.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Commons/CategoryService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleType"], function (require, exports, CategoryService, ServiceConfigurationHelper, FascicleType) {
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
                if (node.get_nodes().get_count() == 0 && !_this.hasFilters()) {
                    args.set_cancel(true);
                    node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    node.set_selected(true);
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
                    node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
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
            };
            this.btnSearchCode_OnClick = function (sender, args) {
                sender.preventDefault();
                _this._loadingPanel.show(_this.pnlMainContentId);
                _this.findCategories()
                    .done(function () {
                    var foundCategories = sessionStorage[CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY];
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
                    sessionStorage.removeItem(CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY);
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
            var categoryServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryServiceConfiguration);
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
                if (sessionStorage[CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY]) {
                    categories = JSON.parse(sessionStorage[CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY]);
                }
                return categories;
            },
            enumerable: true,
            configurable: true
        });
        /**
        *------------------------- Methods -----------------------------
        */
        CommonSelCategoryRest.prototype.initialize = function () {
            var _this = this;
            this._treeViewCategory = $find(this.treeViewCategoryId);
            this._btnSearchOnlyFascicolable = $find(this.btnSearchOnlyFascicolableId);
            this._btnSearchOnlyFascicolable.add_clicked(this.btnSearchOnlyFascicolable_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            $("#" + this.btnSearchId).click(this.btnSearch_OnClick);
            $("#" + this.btnSearchCodeId).click(this.btnSearchCode_OnClick);
            $("#" + this.btnConfermaId).click(this.btnConfirm_OnClick);
            $("#" + this.rowOnlyFascicolableId).hide();
            if (this.fascicleBehavioursEnabled) {
                $("#" + this.rowOnlyFascicolableId).show();
                this._btnSearchOnlyFascicolable.set_checked(true);
            }
            sessionStorage.removeItem(CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY);
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
                    this.lblDescription.text("Visualizzazione filtrata in base al settore responsabile selezionato");
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
            this._categoryService.findTreeCategories(finder, function (data) {
                sessionStorage[CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY] = JSON.stringify(data);
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        CommonSelCategoryRest.prototype.findCategories = function (parentId) {
            var _this = this;
            var promise = $.Deferred();
            var finder = this.prepareFinder(parentId);
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
            if (!fullCode) {
                return '';
            }
            var splittedCode = fullCode.split('.').filter(function (code) { return Number(code); }).map(function (code) { return Number(code).padLeft(4); });
            var fullCodeFormatted = splittedCode.join('|');
            return fullCodeFormatted;
        };
        CommonSelCategoryRest.prototype.populateTreeView = function (categories, needClearItems) {
            var _this = this;
            if (needClearItems === void 0) { needClearItems = true; }
            var promise = $.Deferred();
            if (!categories || categories.length == 0) {
                if (this.hasFilters()) {
                    sessionStorage.removeItem(CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY);
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
                sessionStorage[CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY] = JSON.stringify(categories);
            }
            this.createNodes(categories, nodeSource)
                .done(function () {
                if (needClearItems) {
                    _this.rootNode.get_nodes().clear();
                    for (var _i = 0, nodeSource_1 = nodeSource; _i < nodeSource_1.length; _i++) {
                        var node = nodeSource_1[_i];
                        _this.rootNode.get_nodes().add(node);
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
            var currentCategory = categories.shift();
            this.createNode(currentCategory, nodeSource)
                .done(function (node) {
                if (_this.hasFilters()) {
                    var toAppendClass = node.get_cssClass();
                    node.set_cssClass(toAppendClass + " dsw-text-bold");
                }
                if (_this.fascicleBehavioursEnabled) {
                    if (currentCategory.HasFascicleDefinition) {
                        node.get_attributes().setAttribute("HasFascicleDefinition", true);
                        node.set_cssClass("node-tree-fascicle");
                    }
                }
                if (categories.length == 0) {
                    return promise.resolve();
                }
                _this.createNodes(categories, nodeSource)
                    .done(function () { return promise.resolve(); })
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
                if (!this.hasFilters()) {
                    currentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ServerSideCallBack);
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
            if (category.IdParent) {
                var parentNode = this.findNodeFromSource(nodeSource, category.IdParent);
                if (parentNode) {
                    parentNode.get_nodes().add(currentNode);
                    parentNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                    parentNode.set_expanded(true);
                    return promise.resolve(currentNode);
                }
                var parentFromCache = this.cachedCategories.filter(function (item) { return item.IdCategory == category.IdParent; })[0];
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
            return this.txtSearch.val() || this.formatFullCode(this.txtSearchCode.val()) || this._btnSearchOnlyFascicolable.get_checked();
        };
        CommonSelCategoryRest.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        CommonSelCategoryRest.prototype.closeWindow = function (selectedCategory) {
            var wnd = this.getRadWindow();
            wnd.close(selectedCategory);
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
        CommonSelCategoryRest.FOUND_CATEGORIES_SESSION_KEY = "FoundCategories";
        CommonSelCategoryRest.CACHE_CATEGORIES_SESSION_KEY = "CacheCategories";
        return CommonSelCategoryRest;
    }());
    return CommonSelCategoryRest;
});
//# sourceMappingURL=CommonSelCategoryRest.js.map