/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Commons/CategoryService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Commons/CategoryModel", "./CommonSelCategoryRest", "App/Models/Processes/ProcessNodeType"], function (require, exports, CategoryService, ServiceConfigurationHelper, CategoryModel, CommonSelCategoryRest, ProcessNodeType) {
    var uscCategoryRest = /** @class */ (function () {
        function uscCategoryRest(serviceConfigurations, configuration, uscId) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            this.actionToolbar_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.toolbarActions().filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
                    .map(function (item) { return item[1]; })[0];
                currentAction();
            };
            this.windowSelCategory_onClose = function (sender, args) {
                var returnedData = args.get_argument();
                if (_this.showProcesses) {
                    var processFascicleTemplateParents = returnedData;
                    if (processFascicleTemplateParents) {
                        _this._treeCategory.get_nodes().clear();
                        _this.createProcessFascicleTemplateNode(processFascicleTemplateParents.reverse()).done(function (node) {
                        })
                            .fail(function (exception) { return _this.showNotificationException(exception); });
                    }
                }
                else {
                    var category_1 = returnedData;
                    if (category_1) {
                        _this._treeCategory.trackChanges();
                        _this._treeCategory.get_nodes().clear();
                        sessionStorage.removeItem(_this._selectedCategorySessionKey);
                        _this.createNode(category_1).
                            done(function (node) {
                            node.get_attributes().setAttribute("IsSelected", true);
                            node.set_selected(true);
                            _this._treeCategory.commitChanges();
                            _this.addToSelectedSource(category_1);
                            $("#" + _this.pnlMainContentId).triggerHandler(uscCategoryRest.ADDED_EVENT, category_1.IdCategory);
                            _this._ajaxManager.ajaxRequest("Add");
                        })
                            .fail(function (exception) { return _this.showNotificationException(exception); });
                    }
                }
            };
            var categoryServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryServiceConfiguration);
            this._selectedCategorySessionKey = uscId + "_selectedCategories";
            this._configurationCategorySessionKey = uscId + "_configuration";
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        }
        uscCategoryRest.prototype.toolbarActions = function () {
            var _this = this;
            var items = [
                ["add", function () { return _this.addCategories(); }],
                ["delete", function () { return _this.removeCategories(); }]
            ];
            return items;
        };
        /**
        *------------------------- Methods -----------------------------
        */
        uscCategoryRest.prototype.initialize = function () {
            var _this = this;
            this._windowManager = $find(this.windowManagerId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._actionToolbar = $find(this.actionToolbarId);
            this._treeCategory = $find(this.treeCategoryId);
            this._windowSelCategory = $find(this.windowSelCategoryId);
            this._windowSelCategory.add_close(this.windowSelCategory_onClose);
            this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
            sessionStorage.removeItem(this._selectedCategorySessionKey);
            this.initializeSources()
                .done(function () { return _this.bindLoaded(); })
                .fail(function (exception) { return _this.showNotificationException(exception); });
        };
        uscCategoryRest.prototype.registerAddedEventhandler = function (handler) {
            $("#" + this.pnlMainContentId).on(uscCategoryRest.ADDED_EVENT, handler);
        };
        uscCategoryRest.prototype.initializeSources = function () {
            var _this = this;
            var promise = $.Deferred();
            var configuration = this.getConfiguration();
            if (!this.idCategory) {
                return promise.resolve();
            }
            this._categoryService.findTreeCategory(this.idCategory, configuration.fascicleType, function (data) {
                if (!data) {
                    return promise.resolve();
                }
                _this._treeCategory.trackChanges();
                _this.createNode(data)
                    .done(function (node) {
                    node.get_attributes().setAttribute("IsSelected", true);
                    _this._treeCategory.commitChanges();
                    _this.addToSelectedSource(data);
                    $("#" + _this.pnlMainContentId).triggerHandler(uscCategoryRest.ADDED_EVENT, data.IdCategory);
                    setTimeout(function () { return _this._ajaxManager.ajaxRequest("Add"); }, 800);
                    promise.resolve();
                })
                    .fail(function (exception) { return promise.reject(exception); });
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscCategoryRest.prototype.bindLoaded = function () {
            $("#" + this.pnlMainContentId).data(this);
            $("#" + this.pnlMainContentId).triggerHandler(uscCategoryRest.LOADED_EVENT);
        };
        uscCategoryRest.prototype.addCategories = function () {
            var url = "../UserControl/CommonSelCategoryRest.aspx?Type=Comm";
            var configuration = this.getConfiguration();
            if (configuration.showAuthorizedFascicolable) {
                url = url.concat("&FascicleBehavioursEnabled=true");
            }
            if (configuration.showManagerFascicolable) {
                url = url.concat("&Manager=" + configuration.showManagerFascicolable);
            }
            if (configuration.showSecretaryFascicolable) {
                url = url.concat("&Secretary=" + configuration.showSecretaryFascicolable);
            }
            if (configuration.fascicleType) {
                url = url.concat("&FascicleType=" + configuration.fascicleType);
            }
            if (configuration.showRoleFascicolable) {
                url = url.concat("&Role=" + configuration.showRoleFascicolable);
            }
            if (configuration.showContainerFascicolable) {
                url = url.concat("&Container=" + configuration.showContainerFascicolable);
            }
            url = url.concat("&ShowProcesses=" + this.showProcesses);
            this._windowManager.open(url, "windowSelCategory", undefined);
        };
        uscCategoryRest.prototype.removeCategories = function () {
            var selectedNode = this._treeCategory.get_selectedNode();
            if (!selectedNode) {
                this.showNotificationException(null, "Selezionare un classificatore");
                return;
            }
            var idCategory = selectedNode.get_value();
            this._treeCategory.trackChanges();
            this._treeCategory.get_nodes().clear();
            this._treeCategory.commitChanges();
            sessionStorage[this._selectedCategorySessionKey] = JSON.stringify([]);
            $("#" + this.pnlMainContentId).triggerHandler(uscCategoryRest.REMOVED_EVENT, idCategory);
            this._ajaxManager.ajaxRequest("Remove");
        };
        uscCategoryRest.prototype.createNode = function (category) {
            var _this = this;
            var promise = $.Deferred();
            var currentNode = new Telerik.Web.UI.RadTreeNode();
            currentNode.set_text(category.Code + "." + category.Name);
            currentNode.set_value(category.IdCategory);
            if (category.HasChildren) {
                currentNode.set_imageUrl("../Comm/images/folderopen16.gif");
            }
            else {
                currentNode.set_imageUrl("../Comm/images/Classificatore.gif");
            }
            if (category.IdParent) {
                var configuration = this.getConfiguration();
                this._categoryService.findTreeCategory(category.IdParent, configuration.fascicleType, function (data) {
                    if (data.Code == 0) {
                        _this._treeCategory.get_nodes().add(currentNode);
                        return promise.resolve(currentNode);
                    }
                    _this.createNode(data)
                        .done(function (node) {
                        node.get_nodes().add(currentNode);
                        node.set_expanded(true);
                        promise.resolve(currentNode);
                    })
                        .fail(function (exception) { return promise.reject(exception); });
                }, function (exception) { return promise.reject(exception); });
            }
            else {
                this._treeCategory.get_nodes().add(currentNode);
                return promise.resolve(currentNode);
            }
            return promise.promise();
        };
        uscCategoryRest.prototype.createProcessFascicleTemplateNode = function (processFascicleTemplateParents) {
            var promise = $.Deferred();
            var nodeCollection = this._treeCategory.get_nodes();
            var leafNode = new Telerik.Web.UI.RadTreeNode();
            for (var _i = 0, processFascicleTemplateParents_1 = processFascicleTemplateParents; _i < processFascicleTemplateParents_1.length; _i++) {
                var treeNodeData = processFascicleTemplateParents_1[_i];
                var currentNode = new Telerik.Web.UI.RadTreeNode();
                currentNode.set_text("" + treeNodeData.text);
                currentNode.set_value(treeNodeData.value);
                currentNode.set_imageUrl(treeNodeData.icon);
                currentNode.set_cssClass(treeNodeData.cssClass);
                currentNode.get_attributes().setAttribute("NodeType", treeNodeData.nodeType);
                nodeCollection.add(currentNode);
                currentNode.expand();
                currentNode.select();
                nodeCollection = nodeCollection.getNode(0).get_nodes();
                leafNode = currentNode;
            }
            leafNode.get_attributes().setAttribute("IsSelected", true);
            $("#" + this.pnlMainContentId).triggerHandler(uscCategoryRest.ADDED_EVENT, processFascicleTemplateParents[processFascicleTemplateParents.length - 1].value);
            return promise.resolve(leafNode);
        };
        uscCategoryRest.prototype.addToSelectedSource = function (category) {
            var currentSource = this.getSelectedCategories();
            currentSource.push(category);
            sessionStorage[this._selectedCategorySessionKey] = JSON.stringify(currentSource);
        };
        uscCategoryRest.prototype.getSelectedCategories = function () {
            var source = [];
            if (sessionStorage[this._selectedCategorySessionKey]) {
                source = JSON.parse(sessionStorage[this._selectedCategorySessionKey]);
            }
            return source;
        };
        uscCategoryRest.prototype.getSelectedCategory = function () {
            var source = this.getSelectedCategories();
            if (source.length == 0) {
                return null;
            }
            var model = new CategoryModel();
            model.EntityShortId = source[0].IdCategory;
            model.Name = source[0].Name;
            return model;
        };
        uscCategoryRest.prototype.getProcessId = function () {
            return this._treeCategory.findNodeByText(CommonSelCategoryRest.PROCESSES_AND_FOLDERS_TEXT).get_nodes().getNode(0).get_value();
        };
        uscCategoryRest.prototype.getSelectedNode = function () {
            return this._treeCategory.get_selectedNode();
        };
        uscCategoryRest.prototype.getProcessFascicleTemplateFolderId = function () {
            return this.getSelectedNode().get_attributes().getAttribute("NodeType") === ProcessNodeType.ProcessFascicleTemplate
                ? this._treeCategory.get_selectedNode().get_parent().get_value()
                : this._treeCategory.get_selectedNode().get_value();
        };
        uscCategoryRest.prototype.getConfiguration = function () {
            var configuration = {};
            if (sessionStorage[this._configurationCategorySessionKey]) {
                configuration = JSON.parse(sessionStorage[this._configurationCategorySessionKey]);
            }
            return configuration;
        };
        uscCategoryRest.prototype.showNotificationException = function (exception, customMessage) {
            var uscNotification = $("#" + this.uscNotificationId).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception) {
                    uscNotification.showNotification(exception);
                    return;
                }
                uscNotification.showWarningMessage(customMessage);
            }
        };
        uscCategoryRest.prototype.setShowAuthorizedParam = function (value) {
            var configuration = this.getConfiguration();
            configuration.showAuthorizedFascicolable = value;
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        };
        uscCategoryRest.prototype.setShowManagerParam = function (value) {
            var configuration = this.getConfiguration();
            configuration.showManagerFascicolable = value;
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        };
        uscCategoryRest.prototype.setShowSecretaryParam = function (value) {
            var configuration = this.getConfiguration();
            configuration.showSecretaryFascicolable = value;
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        };
        uscCategoryRest.prototype.setShowRoleParam = function (value) {
            var configuration = this.getConfiguration();
            configuration.showRoleFascicolable = value;
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        };
        uscCategoryRest.prototype.setShowContainerParam = function (value) {
            var configuration = this.getConfiguration();
            configuration.showContainerFascicolable = value;
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        };
        uscCategoryRest.prototype.setFascicleTypeParam = function (value) {
            var configuration = this.getConfiguration();
            configuration.fascicleType = value;
            sessionStorage[this._configurationCategorySessionKey] = JSON.stringify(configuration);
        };
        uscCategoryRest.prototype.addDefaultCategory = function (idCategory, onlyFascicolable) {
            var _this = this;
            if (onlyFascicolable === void 0) { onlyFascicolable = false; }
            var promise = $.Deferred();
            this._treeCategory.get_nodes().clear();
            this._treeCategory.trackChanges();
            var finderAction = function (id, callback, error) { return _this._categoryService.findTreeCategory(id, null, callback, error); };
            if (onlyFascicolable) {
                finderAction = function (id, callback, error) { return _this._categoryService.findFascicolableCategory(id, callback, error); };
            }
            finderAction(idCategory, function (data) {
                if (!data || data.Code == 0) {
                    return promise.resolve();
                }
                _this.createNode(data)
                    .done(function (node) {
                    node.select();
                    node.get_attributes().setAttribute("IsSelected", true);
                    node.get_attributes().setAttribute("NodeType", ProcessNodeType.Category);
                    _this.addToSelectedSource(data);
                    _this._treeCategory.commitChanges();
                    $("#" + _this.pnlMainContentId).triggerHandler(uscCategoryRest.ADDED_EVENT, data.IdCategory);
                    setTimeout(function () { return _this._ajaxManager.ajaxRequest("Add"); }, 800);
                    promise.resolve();
                })
                    .fail(function (exception) {
                    _this._treeCategory.commitChanges();
                    promise.reject(exception);
                });
            }, function (exception) {
                _this._treeCategory.commitChanges();
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscCategoryRest.prototype.disableButtons = function () {
            this._actionToolbar.get_items().forEach(function (item) {
                item.set_enabled(false);
            });
        };
        uscCategoryRest.prototype.setToolbarVisibilityButtons = function () {
            $("#" + this.actionToolbarId).hide();
        };
        uscCategoryRest.prototype.enableButtons = function () {
            this._actionToolbar.get_items().forEach(function (item) {
                item.set_enabled(true);
            });
        };
        uscCategoryRest.prototype.updateSessionStorageSelectedCategory = function (category) {
            sessionStorage.setItem(this._selectedCategorySessionKey, JSON.stringify([category]));
        };
        uscCategoryRest.prototype.populateCategotyTree = function (category) {
            this._treeCategory.get_nodes().clear();
            var node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(category.Code + "." + category.Name);
            node.set_value(category.IdCategory);
            this._treeCategory.get_nodes().add(node);
        };
        uscCategoryRest.prototype.clearTree = function () {
            this._treeCategory.get_nodes().clear();
        };
        uscCategoryRest.prototype.getCategoryFascicles = function (categoryId) {
            var promise = $.Deferred();
            this._categoryService.getCategoriesByIds([categoryId], this.currentTenantAOOId, function (data) {
                if (!data)
                    return;
                var category = data[0];
                promise.resolve(category.CategoryFascicles);
            }, this.showNotificationException);
            return promise.promise();
        };
        uscCategoryRest.LOADED_EVENT = "onLoaded";
        uscCategoryRest.ADDED_EVENT = "onAdded";
        uscCategoryRest.REMOVED_EVENT = "onRemoved";
        return uscCategoryRest;
    }());
    return uscCategoryRest;
});
//# sourceMappingURL=uscCategoryRest.js.map