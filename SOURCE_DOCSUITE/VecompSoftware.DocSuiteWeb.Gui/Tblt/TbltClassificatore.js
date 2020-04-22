/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "App/Helpers/WindowHelper", "App/Services/Commons/MetadataRepositoryService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Services/Commons/CategoryFascicleService", "App/Models/Commons/CategoryModel", "App/Models/Fascicles/FascicleType", "App/Services/Commons/CategoryFascicleRightsService", "../app/core/extensions/string"], function (require, exports, WindowHelper, MetadataRepositoryService, ServiceConfigurationHelper, ExceptionDTO, CategoryFascicleService, CategoryModel, FascicleType, CategoryFascicleRightsService) {
    var TbltClassificatore = /** @class */ (function () {
        /**
         * Costruttore
         */
        function TbltClassificatore(serviceConfigurations) {
            var _this = this;
            this.isCategoryManager = false;
            /**
             *------------------------- Events -----------------------------
             */
            this.treeView_ClientNodeExpanding = function (sender, args) {
                var node = args.get_node();
                if (node.get_nodes().get_count() > 0) {
                    var oldExpandMode = node.get_expandMode();
                    node.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                    node.expand();
                    node.set_expandMode(oldExpandMode);
                    args.set_cancel(true);
                }
            };
            this.treeView_ClientNodeExpanded = function (sender, args) {
                var node = args.get_node();
                if (node.get_nodes().get_count() == 0) {
                    var params = "ExpandNode|" + node.get_value() + "|" + node.toJsonString();
                    $find(_this.ajaxManagerId).ajaxRequest(params);
                }
            };
            /**
             * Evento scatenato al click di un nodo
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.treeView_ClientNodeClicked = function (sender, eventArgs) {
                var node = eventArgs.get_node();
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                _this.loadDetail(node);
            };
            /**
             * Evento che dispone la visibilità degli elementi del context menu
             * @method
             * @param sender
             * @param args
             * @returns
             */
            this.onContextMenuShowing = function (sender, args) {
                var treeNode = args.get_node();
                var menu = args.get_menu();
                treeNode.set_selected(true);
                var strNodeType = treeNode.get_attributes().getAttribute("NodeType");
                var active = treeNode.get_attributes().getAttribute("Active") == "True";
                var isLeaf = (treeNode.get_attributes().getAttribute("HasChildren") == "False");
                var isRecoverable = (treeNode.get_attributes().getAttribute("IsRecoverable") == "True");
                var mustHaveFascicle = (treeNode.get_attributes().getAttribute("MustHaveFascicle").toUpperCase() === 'true'.toUpperCase());
                var hasFascicle = (treeNode.get_attributes().getAttribute("HasFascicle").toUpperCase() === 'true'.toUpperCase());
                var hasProcedureFascicle = (treeNode.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
                var isSubFascicle = (treeNode.get_attributes().getAttribute("IsSubFascicle") == "True");
                switch (strNodeType) {
                    case "Category":
                    case "SubCategory":
                        menu.findItemByValue('Add').enable();
                        menu.findItemByValue('Rename').enable();
                        if (active) {
                            menu.findItemByValue('Recovery').disable();
                            if (isLeaf) {
                                menu.findItemByValue('Delete').enable();
                            }
                            else {
                                menu.findItemByValue('Delete').disable();
                            }
                        }
                        else {
                            menu.findItemByValue('Add').disable();
                            menu.findItemByValue('Rename').disable();
                            (isRecoverable) ? menu.findItemByValue('Recovery').enable() : menu.findItemByValue('Recovery').disable();
                            menu.findItemByValue('Delete').disable();
                        }
                        menu.findItemByValue('Log').enable();
                        break;
                    default:
                        menu.findItemByValue('Add').enable();
                        menu.findItemByValue('Rename').disable();
                        menu.findItemByValue('Recovery').disable();
                        menu.findItemByValue('Delete').disable();
                        menu.findItemByValue('Log').disable();
                        break;
                }
                _this.alignButtons(menu);
            };
            /**
             * Evento scatenato al click di un elemento del context menu
             * @method
             * @param sender
             * @param args
             * @returns
             */
            this.onContextMenuItemClicked = function (sender, args) {
                var menuItem = args.get_menuItem();
                switch (menuItem.get_value()) {
                    case "Add":
                    case "Rename":
                    case "Delete":
                    case "Recovery":
                        _this.openEditWindow('rwEdit', menuItem.get_value());
                        break;
                    case "Log":
                        _this.openLogWindow('rwLog');
                        break;
                }
            };
            /**
             * Evento scatenato dopo la chiusura della finestra di editazione di un nodo
             * @method
             * @param sender
             * @param args
             * @returns
             */
            this.onCloseFunctionCallback = function (sender, args) {
                if (args.get_argument()) {
                    _this.updateCategories(args.get_argument());
                }
            };
            /**
             * Evento scatenato dopo la chiusura della finestra di associazione metadati
             * @method
             * @param sender
             * @param args
             * @returns
             */
            this.onCloseMetadataCallback = function (sender, args) {
                var treeView = $find(_this.treeViewCategoryId);
                var params = "ReloadNodes|".concat(treeView.get_selectedNode().get_value());
                if (args.get_argument()) {
                    $find(_this.ajaxManagerId).ajaxRequest(params);
                }
            };
            /**
             * Callback per modifica nodo di classificatore
             * @method
             * @param existGroups
             * @param source
             * @returns
             */
            this.renameCallback = function (existGroups, source) {
                var treeView = $find(_this.treeViewCategoryId);
                var node = treeView.findNodeByValue(source.value);
                node._loadFromDictionary(source);
                node.get_attributes().setAttribute("Code", source.attributes.Code);
                var parentNode = treeView.get_selectedNode().get_parent();
                _this.orderPositionNode(node, parentNode);
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        /**
         * Initialize
         */
        TbltClassificatore.prototype.initialize = function () {
            var wndManager = $find(this.radWindowManagerId);
            wndManager.getWindowByName('rwEdit').add_close(this.onCloseFunctionCallback);
            wndManager.getWindowByName('rwLog').add_close(this.onCloseFunctionCallback);
            wndManager.getWindowByName('rwAddMassimario').add_close(this.onCloseMetadataCallback);
            wndManager.getWindowByName('rwMetadata').add_close(this.onCloseFunctionCallback);
            var treeView = $find(this.treeViewCategoryId);
            var categoryFascicleRightsService = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicleRight");
            this._categoryFascicleRightsService = new CategoryFascicleRightsService(categoryFascicleRightsService);
            var categoryFascicleService = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
            this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
            this._pnlFasciclePlan.hide();
            this.detailsVisible = false;
            if (this.metadataRepositoryEnabled) {
                this._metadataRepositoryService = new MetadataRepositoryService(ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository"));
            }
        };
        TbltClassificatore.prototype.loadDetail = function (node) {
            this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
            node.expand();
            this._pnlFasciclePlan.show();
            this.detailsVisible = true;
            var btnCreateFascicle = $get(this.btnCreateFascicleId);
            if (btnCreateFascicle) {
                btnCreateFascicle.innerText = "Piano di fascicolazione";
            }
            var strNodeType = node.get_attributes().getAttribute("NodeType");
            if (strNodeType != "Root") {
                var strActive = node.get_attributes().getAttribute("Active") == 'True';
                var isLeaf = (node.get_attributes().getAttribute("HasChildren") == "False");
                var mustHaveFascicle = (node.get_attributes().getAttribute("MustHaveFascicle").toUpperCase() === 'true'.toUpperCase());
                var hasFascicle = (node.get_attributes().getAttribute("HasFascicle").toUpperCase() === 'true'.toUpperCase());
                var hasProcedureFascicle = (node.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
                var isRecoverable = (node.get_attributes().getAttribute("IsRecoverable") == "True");
                var isSubFascicle = (node.get_attributes().getAttribute("IsSubFascicle") == "True");
                this.setEnabledButtons(strNodeType, strActive, isLeaf, hasProcedureFascicle, hasFascicle, isRecoverable, isSubFascicle);
                var uscFasciclePlan = $("#".concat(this.uscFasciclePlanId)).data();
                if (!jQuery.isEmptyObject(uscFasciclePlan)) {
                    uscFasciclePlan.loadNodes(node.get_value());
                }
            }
            else {
                this.setEnabledButtons(strNodeType, true, false, false, false, false, false);
            }
        };
        TbltClassificatore.prototype.loadDetailSelectedNode = function () {
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            this.loadDetail(selectedNode);
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Allinea i pulsanti con il contextMenu
         * @method
         * @param menu
         * @returns
         */
        TbltClassificatore.prototype.alignButtons = function (menu) {
            var btnAggiungi = $get(this.btnAggiungiId);
            var btnModifica = $get(this.btnModificaId);
            var btnElimina = $get(this.btnEliminaId);
            var btnRecovery = $get(this.btnRecoveryId);
            var btnLog = $get(this.btnLogId);
            var btnMassimarioScarto = $get(this.btnMassimarioScartoId);
            if (btnAggiungi)
                btnAggiungi.disabled = !menu.findItemByValue('Add').get_enabled();
            if (btnModifica)
                btnModifica.disabled = !menu.findItemByValue('Rename').get_enabled();
            if (btnLog)
                btnLog.disabled = !menu.findItemByValue('Log').get_enabled();
            if (btnRecovery)
                btnRecovery.disabled = !menu.findItemByValue('Recovery').get_enabled();
            if (btnElimina)
                btnElimina.disabled = !menu.findItemByValue('Delete').get_enabled();
            if (btnMassimarioScarto)
                btnMassimarioScarto.disabled = !menu.findItemByValue('Rename').get_enabled();
        };
        /**
         * Setta la visibilità dei pulsanti
         * @method
         * @param strNodeType
         * @param active
         * @param isLeaf
         * @param hasProcedureFascicle
         * @param mustHaveFascicle
         * @returns
         */
        TbltClassificatore.prototype.setEnabledButtons = function (strNodeType, active, isLeaf, hasProcedureFascicle, mustHasFascicle, isRecoverable, isSubFascicle) {
            var btnAggiungi = $get(this.btnAggiungiId);
            var btnModifica = $get(this.btnModificaId);
            var btnElimina = $get(this.btnEliminaId);
            var btnRecovery = $get(this.btnRecoveryId);
            var btnLog = $get(this.btnLogId);
            var btnMassimarioScarto = $get(this.btnMassimarioScartoId);
            var btnMetadata = $get(this.btnMetadataId);
            var btnRunFasciclePlan = $find(this.btnRunFasciclePlanId);
            var btnCloseFasciclePlan = $find(this.btnCloseFasciclePlanId);
            switch (strNodeType) {
                case "Category":
                case "SubCategory":
                    if (btnAggiungi)
                        btnAggiungi.disabled = !active;
                    if (btnModifica)
                        btnModifica.disabled = !active;
                    if (btnMassimarioScarto)
                        btnMassimarioScarto.disabled = !active;
                    if (btnLog)
                        btnLog.disabled = false;
                    if (active) {
                        if (btnRecovery)
                            btnRecovery.disabled = true;
                        if (btnElimina)
                            btnElimina.disabled = (isLeaf != true);
                    }
                    else {
                        if (btnRecovery)
                            btnRecovery.disabled = !isRecoverable;
                        if (btnElimina)
                            btnElimina.disabled = true;
                    }
                    if (btnMetadata)
                        btnMetadata.disabled = !active;
                    if (btnRunFasciclePlan)
                        btnRunFasciclePlan.set_enabled(!hasProcedureFascicle || isSubFascicle);
                    if (btnCloseFasciclePlan)
                        btnCloseFasciclePlan.set_enabled(hasProcedureFascicle && !isSubFascicle);
                    break;
                case "Group":
                    if (btnAggiungi)
                        btnAggiungi.disabled = true;
                    if (btnModifica)
                        btnModifica.disabled = true;
                    if (btnMassimarioScarto)
                        btnMassimarioScarto.disabled = true;
                    if (btnLog)
                        btnLog.disabled = true;
                    if (btnRecovery)
                        btnRecovery.disabled = true;
                    if (btnElimina)
                        btnElimina.disabled = true;
                    if (btnMetadata)
                        btnMetadata.disabled = true;
                    break;
                default:
                    if (btnAggiungi)
                        btnAggiungi.disabled = false;
                    if (btnModifica)
                        btnModifica.disabled = true;
                    if (btnMassimarioScarto)
                        btnMassimarioScarto.disabled = true;
                    if (btnLog)
                        btnLog.disabled = true;
                    if (btnRecovery)
                        btnRecovery.disabled = true;
                    if (btnElimina)
                        btnElimina.disabled = true;
                    if (btnMetadata)
                        btnMetadata.disabled = true;
                    if (btnRunFasciclePlan)
                        btnRunFasciclePlan.set_enabled(!hasProcedureFascicle);
                    if (btnCloseFasciclePlan)
                        btnCloseFasciclePlan.set_enabled(hasProcedureFascicle);
                    break;
            }
        };
        /**
         * Inserisce un nuovo nodo nella treeview e ne ordina la visualizzazione
         * @method
         * @param node
         * @param parentNode
         * @returns
         */
        TbltClassificatore.prototype.orderPositionNode = function (node, parentNode) {
            var position = 0;
            $.each(parentNode.get_allNodes(), function (index, pNode) {
                if ((node.get_attributes().getAttribute("Code")) > pNode.get_attributes().getAttribute("Code")) {
                    position = pNode.get_index() + 1;
                }
            });
            parentNode.get_nodes().insert(position, node);
        };
        /**
         * Metodo di gestione nodi di classificatore
         * @method
         * @param category
         * @returns
         */
        TbltClassificatore.prototype.updateCategories = function (operator) {
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            if (operator == 'Delete') {
                selectedNode = selectedNode.get_parent();
            }
            var params = "ReloadNodes";
            if (selectedNode && selectedNode.get_value()) {
                params = params.concat('|', selectedNode.get_value());
            }
            $find(this.ajaxManagerId).ajaxRequest(params);
        };
        /**
         * Aggiorno la visibilità dei pulsanti post reload dei nodi
         */
        TbltClassificatore.prototype.updateCategoriesCallback = function () {
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
            var uscFasciclePlan = $("#".concat(this.uscFasciclePlanId)).data();
            uscFasciclePlan.loadNodes(selectedNode.get_value());
            this.detailsVisible = true;
            if (selectedNode && selectedNode.get_attributes().getAttribute("NodeType") != 'Root') {
                var strNodeType = selectedNode.get_attributes().getAttribute("NodeType");
                var active = selectedNode.get_attributes().getAttribute("Active") == 'True';
                var isLeaf = (selectedNode.get_attributes().getAttribute("HasChildren") == "False");
                var isRecoverable = (selectedNode.get_attributes().getAttribute("IsRecoverable") == "True");
                var mustHaveFascicle = (selectedNode.get_attributes().getAttribute("MustHaveFascicle").toUpperCase() === 'true'.toUpperCase());
                var hasFascicle = (selectedNode.get_attributes().getAttribute("HasFascicle").toUpperCase() === 'true'.toUpperCase());
                var hasProcedureFascicle = (selectedNode.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
                var isSubFascicle = (selectedNode.get_attributes().getAttribute("IsSubFascicle") == "True");
                this.setEnabledButtons(strNodeType, active, isLeaf, hasProcedureFascicle, mustHaveFascicle, isRecoverable, isSubFascicle);
            }
            else {
                this.setEnabledButtons('Root', true, false, false, false, false, false);
            }
        };
        /**
         * Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
         * @method
         * @param name
         * @param operation
         * @returns
         */
        TbltClassificatore.prototype.openEditWindow = function (name, operation) {
            var parameters = "Action=".concat(operation);
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            var active = selectedNode.get_attributes().getAttribute("Active") == '1';
            switch (operation) {
                case "Add":
                case "Rename":
                case "Delete":
                    parameters = parameters.concat("&CategoryID=", selectedNode.get_value());
                    break;
                case "Recovery":
                    if (active) {
                        this.showWarningMessage(this.uscNotificationId, "Classificatore già attivo!");
                        return false;
                    }
                    parameters = parameters.concat("&CategoryID=", selectedNode.get_value());
                    break;
            }
            var url = "../Tblt/TbltClassificatoreGesNew.aspx?".concat(parameters);
            return this.openWindow(url, name, WindowHelper.WIDTH_EDIT_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
        };
        /**
         * Apre la finestra con i Log per il nodo selezione
         * @method
         * @param name
         * @returns
         */
        TbltClassificatore.prototype.openLogWindow = function (name) {
            var url = "../Tblt/TbltLog.aspx?Type=Comm&TableName=Category";
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            if (selectedNode) {
                var attributes = selectedNode.get_attributes();
                url += "&entityUniqueId=".concat(attributes.getAttribute("UniqueId"));
            }
            return this.openWindow(url, name, WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_LOG_WINDOW);
        };
        /**
         * Apre la finestra per la creazione di un fascicolo
         * @method
         * @param name
         * @returns
         */
        TbltClassificatore.prototype.loadEnvironmentNodesImage = function () {
            this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
            var btnCreateFascicle = $get(this.btnCreateFascicleId);
            if (this.detailsVisible) {
                this.detailsVisible = false;
                var treeView = $find(this.treeViewCategoryId);
                var selectedNode = treeView.get_selectedNode();
                this._pnlFasciclePlan.show();
                var uscFasciclePlan = $("#".concat(this.uscFasciclePlanId)).data();
                if (!jQuery.isEmptyObject(uscFasciclePlan)) {
                    //   uscFasciclePlan.setNodesAttributes(selectedNode.get_value());
                }
                if (btnCreateFascicle) {
                    btnCreateFascicle.innerText = "Dettagli";
                }
            }
            else {
                this.detailsVisible = true;
                this._pnlFasciclePlan.hide();
                if (btnCreateFascicle) {
                    btnCreateFascicle.innerText = "Piano di fascicolazione";
                }
            }
            return true;
        };
        /**
         * Apre la finestra per l'associazione di un massimario di scarto
         * @method
         * @param name
         * @returns
         */
        TbltClassificatore.prototype.openAddMassimarioScartoWindow = function (name) {
            var url = "../Tblt/TbltMassimariScarto.aspx?Type=Comm";
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            url = url.concat("&CategoryID=", selectedNode.get_value());
            return this.openWindow(url, name, WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
        };
        /**
         * Apre la finestra per l'associazione di metadati
         * @method
         * @param name
         * @returns
         */
        TbltClassificatore.prototype.openAddMetadataRepository = function () {
            var url = "../Tblt/TbltCategoryMetadata.aspx?Type=Comm";
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            url = url.concat("&CategoryID=", selectedNode.get_value());
            if (selectedNode.get_attributes().getAttribute('MetadataRepository')) {
                url = url.concat("&IdMetadata=", selectedNode.get_attributes().getAttribute('MetadataRepository'));
            }
            return this.openWindow(url, 'rwMetadata', WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
        };
        TbltClassificatore.prototype.closeFasciclePlan = function () {
            var _this = this;
            var wndManager = $find(this.radWindowManagerId);
            wndManager.radconfirm("Sicuro di voler procedere?", function (arg) {
                if (arg) {
                    var treeView = $find(_this.treeViewCategoryId);
                    var selectedNode_1 = treeView.get_selectedNode();
                    _this._categoryFascicleService.getAvailableProcedureCategoryFascicleByCategory(selectedNode_1.get_value(), function (data) {
                        if (data) {
                            if (data && data.length > 0) {
                                var categoryFascicle = data[0];
                                _this._categoryFascicleService.deleteCategoryFascicle(categoryFascicle, function (data) {
                                    var params = "ResetPeriodicCategoryFascicles|" + selectedNode_1.get_value();
                                    $find(_this.ajaxManagerId).ajaxRequest(params);
                                }, function (exception) {
                                    var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                    if (!jQuery.isEmptyObject(uscNotification)) {
                                        uscNotification.showNotification(exception);
                                    }
                                });
                            }
                        }
                    }, function (exception) {
                        var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    });
                }
            });
            return true;
        };
        TbltClassificatore.prototype.runFasciclePlan = function () {
            var _this = this;
            var wndManager = $find(this.radWindowManagerId);
            wndManager.radconfirm("Sei sicuro di avviare un piano di fascicolazione per tutte le tipologie documentarie?", function (arg) {
                var treeView = $find(_this.treeViewCategoryId);
                if (arg) {
                    var rootNode = treeView.get_selectedNode();
                    var item = {};
                    var ajaxModel = {};
                    ajaxModel.Value = new Array();
                    item.FascicleType = FascicleType['Procedure'];
                    item.DSWEnvironment = 0;
                    var category = new CategoryModel;
                    category.EntityShortId = rootNode.get_value();
                    category.Id = rootNode.get_value();
                    item.Category = category;
                    ajaxModel.Value.push(JSON.stringify(item));
                    ajaxModel.ActionName = "ProcedureExternalDataCallback";
                    $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                    treeView.commitChanges();
                }
            });
            return true;
        };
        /**
         * Apre una nuova radwindow con dati personalizzati
         * @method
         * @param url
         * @param name
         * @param width
         * @param height
         * @returns
         */
        TbltClassificatore.prototype.openWindow = function (url, name, width, height) {
            var wnd = $find(this.radWindowManagerId).open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        TbltClassificatore.prototype.showLoadingPanelByElement = function (elementID) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(elementID);
        };
        TbltClassificatore.prototype.closeLoadingPanelByElement = function (elementID) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(elementID);
        };
        TbltClassificatore.prototype.updateVisibility = function (visibility) {
            this.detailsVisible = visibility;
            var treeView = $find(this.treeViewCategoryId);
            var selectedNode = treeView.get_selectedNode();
            var strNodeType = selectedNode.get_attributes().getAttribute("NodeType");
            var btnCreateFascicle = $get(this.btnCreateFascicleId);
            if (btnCreateFascicle) {
                btnCreateFascicle.innerText = "Piano di fascicolazione";
                btnCreateFascicle.disabled = true;
            }
            this.setEnabledButtons(strNodeType, true, false, false, false, false, false);
        };
        TbltClassificatore.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltClassificatore.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        TbltClassificatore.prototype.loadMetadataName = function (id) {
            var _this = this;
            if (this.metadataRepositoryEnabled) {
                this._lblMetadata = $("#".concat(this.lblMetadataId));
                this._metadataRepositoryService.getNameById(id, function (data) {
                    if (data) {
                        _this._lblMetadata.html(data);
                    }
                }, function (exception) {
                    var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                });
            }
        };
        TbltClassificatore.prototype.setVisibilityPanel = function (number) {
            var numberOfSettori = parseInt(number, 10);
            this._pnlSettoriId = $("#".concat(this.pnlSettoriId));
            var _pnlDetailsId = $("#".concat(this.pnlDetailsId));
            if (numberOfSettori == 0) {
                _pnlDetailsId.hide();
            }
        };
        TbltClassificatore.prototype.sendAjaxRequest = function (action) {
            var ajaxModel = {};
            ajaxModel.Value.push(JSON.stringify("ReloadRoleUsers"));
            ajaxModel.ActionName = "ReloadRoleUsers";
            $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
        };
        TbltClassificatore.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception && exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(customMessage);
                }
            }
        };
        return TbltClassificatore;
    }());
    return TbltClassificatore;
});
//# sourceMappingURL=TbltClassificatore.js.map