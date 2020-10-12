/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "App/Helpers/WindowHelper", "App/Services/Commons/MetadataRepositoryService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Services/Commons/CategoryFascicleService", "App/Models/Commons/CategoryModel", "App/Models/Fascicles/FascicleType", "App/Helpers/PageClassHelper", "../app/core/extensions/string"], function (require, exports, WindowHelper, MetadataRepositoryService, ServiceConfigurationHelper, ExceptionDTO, CategoryFascicleService, CategoryModel, FascicleType, PageClassHelper) {
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
            this.actionToolbar_ButtonClicked = function (sender, args) {
                var currentActionButtonItem = args.get_item();
                var currentAction = _this.toolbarActions().filter(function (item) { return item[0] == currentActionButtonItem.get_commandName(); })
                    .map(function (item) { return item[1]; })[0];
                currentAction();
            };
            this.btnUpdateCustomActions_onClick = function (sender, args) {
                PageClassHelper.callUserControlFunctionSafe(_this.uscCustomActionsRestId)
                    .done(function (instance) {
                    var ajaxModel = {
                        ActionName: "UpdateCustomActions",
                        Value: [JSON.stringify(instance.getCustomActions())]
                    };
                    $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                });
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        TbltClassificatore.prototype.toolbarActions = function () {
            var _this = this;
            var items = [
                [TbltClassificatore.ADD_COMMANDNAME, function () { return _this.openEditWindow('rwEdit', 'Add'); }],
                [TbltClassificatore.DELETE_COMMANDNAME, function () { return _this.openEditWindow('rwEdit', 'Delete'); }],
                [TbltClassificatore.EDIT_COMMANDNAME, function () { return _this.openEditWindow('rwEdit', 'Rename'); }],
                [TbltClassificatore.RECOVER_COMMANDNAME, function () { return _this.openEditWindow('rwEdit', 'Recovery'); }],
                [TbltClassificatore.LOG_COMMANDNAME, function () { return _this.openLogWindow('rwLog'); }],
                [TbltClassificatore.ADDMASSIMARIO_COMMANDNAME, function () { return _this.openAddMassimarioScartoWindow('rwAddMassimario'); }],
                [TbltClassificatore.ADDMETADATA_COMMANDNAME, function () { return _this.openAddMetadataRepository(); }],
                [TbltClassificatore.RUNFASCICLEPLAN_COMMANDNAME, function () { return _this.runFasciclePlan(); }],
                [TbltClassificatore.CLOSEFASCICLEPLAN_COMMANDNAME, function () { return _this.closeFasciclePlan(); }]
            ];
            return items;
        };
        /**
         * Initialize
         */
        TbltClassificatore.prototype.initialize = function () {
            var wndManager = $find(this.radWindowManagerId);
            wndManager.getWindowByName('rwEdit').add_close(this.onCloseFunctionCallback);
            wndManager.getWindowByName('rwLog').add_close(this.onCloseFunctionCallback);
            wndManager.getWindowByName('rwAddMassimario').add_close(this.onCloseMetadataCallback);
            wndManager.getWindowByName('rwMetadata').add_close(this.onCloseFunctionCallback);
            var categoryFascicleService = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
            this._pnlFasciclePlan = $("#".concat(this.pnlFasciclePlanId));
            this._pnlFasciclePlan.hide();
            this.detailsVisible = false;
            if (this.metadataRepositoryEnabled) {
                this._metadataRepositoryService = new MetadataRepositoryService(ServiceConfigurationHelper.getService(this._serviceConfigurations, "MetadataRepository"));
            }
            this._actionToolbar = $find(this.actionsToolbarId);
            this._actionToolbar.add_buttonClicked(this.actionToolbar_ButtonClicked);
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
                var hasProcedureFascicle = (node.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
                var isRecoverable = (node.get_attributes().getAttribute("IsRecoverable") == "True");
                var isSubFascicle = (node.get_attributes().getAttribute("IsSubFascicle") == "True");
                this.setEnabledButtons(strNodeType, strActive, isLeaf, hasProcedureFascicle, isRecoverable, isSubFascicle);
                var uscFasciclePlan = $("#".concat(this.uscFasciclePlanId)).data();
                if (!jQuery.isEmptyObject(uscFasciclePlan)) {
                    uscFasciclePlan.loadNodes(node.get_value());
                }
            }
            else {
                this.setEnabledButtons(strNodeType, true, false, false, false, false);
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
            var btnAggiungi = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADD_COMMANDNAME);
            var btnModifica = this._actionToolbar.findButtonByCommandName(TbltClassificatore.EDIT_COMMANDNAME);
            var btnElimina = this._actionToolbar.findButtonByCommandName(TbltClassificatore.DELETE_COMMANDNAME);
            var btnRecovery = this._actionToolbar.findButtonByCommandName(TbltClassificatore.RECOVER_COMMANDNAME);
            var btnLog = this._actionToolbar.findButtonByCommandName(TbltClassificatore.LOG_COMMANDNAME);
            var btnMassimarioScarto = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADDMASSIMARIO_COMMANDNAME);
            if (btnAggiungi)
                btnAggiungi.set_enabled(menu.findItemByValue('Add').get_enabled());
            if (btnModifica)
                btnAggiungi.set_enabled(menu.findItemByValue('Rename').get_enabled());
            if (btnLog)
                btnLog.set_enabled(menu.findItemByValue('Log').get_enabled());
            if (btnRecovery)
                btnRecovery.set_enabled(menu.findItemByValue('Recovery').get_enabled());
            if (btnElimina)
                btnElimina.set_enabled(menu.findItemByValue('Delete').get_enabled());
            if (btnMassimarioScarto)
                btnMassimarioScarto.set_enabled(menu.findItemByValue('Rename').get_enabled());
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
        TbltClassificatore.prototype.setEnabledButtons = function (strNodeType, active, isLeaf, hasProcedureFascicle, isRecoverable, isSubFascicle) {
            var btnAggiungi = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADD_COMMANDNAME);
            var btnModifica = this._actionToolbar.findButtonByCommandName(TbltClassificatore.EDIT_COMMANDNAME);
            var btnElimina = this._actionToolbar.findButtonByCommandName(TbltClassificatore.DELETE_COMMANDNAME);
            var btnRecovery = this._actionToolbar.findButtonByCommandName(TbltClassificatore.RECOVER_COMMANDNAME);
            var btnLog = this._actionToolbar.findButtonByCommandName(TbltClassificatore.LOG_COMMANDNAME);
            var btnMassimarioScarto = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADDMASSIMARIO_COMMANDNAME);
            var btnMetadata = this._actionToolbar.findButtonByCommandName(TbltClassificatore.ADDMETADATA_COMMANDNAME);
            var btnRunFasciclePlan = this._actionToolbar.findButtonByCommandName(TbltClassificatore.RUNFASCICLEPLAN_COMMANDNAME);
            var btnCloseFasciclePlan = this._actionToolbar.findButtonByCommandName(TbltClassificatore.CLOSEFASCICLEPLAN_COMMANDNAME);
            switch (strNodeType) {
                case "Category":
                case "SubCategory":
                    if (btnAggiungi)
                        btnAggiungi.set_enabled(active);
                    if (btnModifica)
                        btnModifica.set_enabled(active);
                    if (btnMassimarioScarto)
                        btnMassimarioScarto.set_enabled(active);
                    if (btnLog)
                        btnLog.set_enabled(true);
                    if (active) {
                        if (btnRecovery)
                            btnRecovery.set_enabled(false);
                        if (btnElimina)
                            btnElimina.set_enabled(isLeaf);
                    }
                    else {
                        if (btnRecovery)
                            btnRecovery.set_enabled(isRecoverable);
                        if (btnElimina)
                            btnElimina.set_enabled(false);
                    }
                    if (btnMetadata)
                        btnMetadata.set_enabled(active);
                    if (btnRunFasciclePlan)
                        btnRunFasciclePlan.set_enabled(!hasProcedureFascicle || isSubFascicle);
                    if (btnCloseFasciclePlan)
                        btnCloseFasciclePlan.set_enabled(hasProcedureFascicle && !isSubFascicle);
                    break;
                case "Group":
                    if (btnAggiungi)
                        btnAggiungi.set_enabled(false);
                    if (btnModifica)
                        btnModifica.set_enabled(false);
                    if (btnMassimarioScarto)
                        btnMassimarioScarto.set_enabled(false);
                    if (btnLog)
                        btnLog.set_enabled(false);
                    if (btnRecovery)
                        btnRecovery.set_enabled(false);
                    if (btnElimina)
                        btnElimina.set_enabled(false);
                    if (btnMetadata)
                        btnMetadata.set_enabled(false);
                    break;
                default:
                    if (btnAggiungi)
                        btnAggiungi.set_enabled(true);
                    if (btnModifica)
                        btnModifica.set_enabled(false);
                    if (btnMassimarioScarto)
                        btnMassimarioScarto.set_enabled(false);
                    if (btnLog)
                        btnLog.set_enabled(false);
                    if (btnRecovery)
                        btnRecovery.set_enabled(false);
                    if (btnElimina)
                        btnElimina.set_enabled(false);
                    if (btnMetadata)
                        btnMetadata.set_enabled(false);
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
                var hasProcedureFascicle = (selectedNode.get_attributes().getAttribute("HasProcedureFascicle").toUpperCase() === 'true'.toUpperCase());
                var isSubFascicle = (selectedNode.get_attributes().getAttribute("IsSubFascicle") == "True");
                this.setEnabledButtons(strNodeType, active, isLeaf, hasProcedureFascicle, isRecoverable, isSubFascicle);
            }
            else {
                this.setEnabledButtons('Root', true, false, false, false, false);
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
            this.setEnabledButtons(strNodeType, true, false, false, false, false);
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
        TbltClassificatore.prototype.loadCustomActions = function (isVisible, customActionsJson) {
            var _this = this;
            if (isVisible) {
                PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                    .done(function (instance) {
                    if (!jQuery.isEmptyObject(instance)) {
                        var customActions = customActionsJson
                            ? JSON.parse(customActionsJson)
                            : {
                                AutoClose: false,
                                AutoCloseAndClone: false
                            };
                        instance.loadItems(customActions);
                        _this._btnUpdateCustomActions = $find(_this.btnUpdateCustomActionsId);
                        _this._btnUpdateCustomActions.add_clicked(_this.btnUpdateCustomActions_onClick);
                        _this._btnUpdateCustomActions.set_visible(!instance.isSummary);
                    }
                });
            }
        };
        TbltClassificatore.ADD_COMMANDNAME = "AddCategory";
        TbltClassificatore.DELETE_COMMANDNAME = "DeleteCategory";
        TbltClassificatore.EDIT_COMMANDNAME = "EditCategory";
        TbltClassificatore.RECOVER_COMMANDNAME = "RecoverCategory";
        TbltClassificatore.LOG_COMMANDNAME = "LogCategory";
        TbltClassificatore.ADDMASSIMARIO_COMMANDNAME = "AddMassimario";
        TbltClassificatore.ADDMETADATA_COMMANDNAME = "AddMetadata";
        TbltClassificatore.RUNFASCICLEPLAN_COMMANDNAME = "RunFasciclePlan";
        TbltClassificatore.CLOSEFASCICLEPLAN_COMMANDNAME = "CloseFasciclePlan";
        return TbltClassificatore;
    }());
    return TbltClassificatore;
});
//# sourceMappingURL=TbltClassificatore.js.map