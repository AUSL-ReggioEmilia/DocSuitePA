/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Environment", "App/Helpers/ServiceConfigurationHelper", "App/Services/UDS/UDSRepositoryService", "App/Services/Commons/CategoryFascicleService", "App/Models/Fascicles/FascicleType", "App/Models/Commons/CategoryModel", "App/Mappers/Commons/CategoryFascicleViewModelMapper"], function (require, exports, DSWEnvironment, ServiceConfigurationHelper, UDSRepositoryService, CategoryFascicleService, FascicleType, CategoryModel, CategoryFascicleViewModelMapper) {
    var uscFasciclePlan = /** @class */ (function () {
        /**
        * Costruttore
        * @param webApiConfiguration
        */
        function uscFasciclePlan(serviceConfigurations) {
            var _this = this;
            /**
            * ---------------------- Events -------------------------
            */
            /**
            * Evento alla chiusura della radWindow
            * @param sender
            * @param args
            */
            this.managerCreatePeriod_OnClose = function (sender, args) {
                if (args.get_argument()) {
                    var item = {};
                    item = JSON.parse(args.get_argument());
                    //let node: Telerik.Web.UI.RadTreeNode = this._rtvEnvironments.get_selectedNode();
                    //node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/history.png");
                    //node.get_attributes().setAttribute("Type", "Periodic");
                    //node.set_toolTip("Periodico");
                    //node.get_attributes().setAttribute("Period", item.FasciclePeriod.PeriodDays);
                    //node.set_value(item.UniqueId);
                    _this._btnRemovePeriodicPlan.set_enabled(false);
                    var params = "ReloadNodes|".concat(item.Category.EntityShortId.toString());
                    $find(_this.ajaxManagerId).ajaxRequest(params);
                }
            };
            /**
             * Evento chiamato al click del bottone Procedimento Amministrativo
             * @param sender
             * @param args
             */
            this.btnProcedure_onClientClick = function (sender, args) {
                _this._manager.radconfirm("Sei sicuro di avviare un piano di fascicolazione per tutte le tipologie documentarie?", function (arg) {
                    if (arg && _this._rtvEnvironments.get_selectedNode() == _this._rtvEnvironments.get_nodes().getNode(0)) {
                        var rootNode = _this._rtvEnvironments.get_nodes().getNode(0);
                        _this._loadingPanel.show(_this.pnlFasciclePlanId);
                        var item = {};
                        var ajaxModel = {};
                        ajaxModel.Value = new Array();
                        item.FascicleType = FascicleType['Procedure'];
                        item.DSWEnvironment = 0;
                        var category = new CategoryModel;
                        category.EntityShortId = rootNode.get_attributes().getAttribute("IdCategory");
                        category.Id = rootNode.get_attributes().getAttribute("IdCategory");
                        item.Category = category;
                        ajaxModel.Value.push(JSON.stringify(item));
                        ajaxModel.ActionName = "ProcedureExternalDataCallback";
                        $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                        _this._rtvEnvironments.commitChanges();
                    }
                });
            };
            /**
             * Evento chiamato al click del bottone Procedimento Periodico
             * @param sender
             * @param args
             */
            this.btnAddPeriodicPlan_onClientClick = function (sender, args) {
                var selectedNode = _this._rtvEnvironments.get_selectedNode();
                var rootNode;
                rootNode = _this._rtvEnvironments.get_nodes().getNode(0);
                var url = '../Tblt/TbltCreaFascicolo.aspx?Type=Comm&Environment='.concat(selectedNode.get_attributes().getAttribute("Environment"), '&IdCategory=', rootNode.get_attributes().getAttribute("IdCategory"));
                _this.openWindow(url, "managerCreatePeriod", 750, 600);
            };
            /**
            * Evento scatenato al click di svuota
            * @param sender
            * @param args
            */
            this.btnRemovePeriodicPlan_onClientClick = function (sender, args) {
                _this._manager.radconfirm("Sicuro di procedere?", function (arg) {
                    if (arg) {
                        _this._loadingPanel.show(_this.pnlFasciclePlanId);
                        var selectedNode_1 = _this._rtvEnvironments.get_selectedNode();
                        var rootNode = _this._rtvEnvironments.get_nodes().getNode(0);
                        _this._categoryFascicleService.getByIdCategory(rootNode.get_attributes().getAttribute("IdCategory"), function (data) {
                            if (data) {
                                var categoryFascicles_1 = data;
                                var categoryFascicleToDelete = data.filter(function (item) { return item.UniqueId == selectedNode_1.get_value(); });
                                if (categoryFascicleToDelete && categoryFascicleToDelete.length > 0) {
                                    var item_1 = categoryFascicleToDelete[0];
                                    _this._categoryFascicleService.deleteCategoryFascicle(item_1, function (data) {
                                        if (categoryFascicles_1.some(function (item) {
                                            return (item.FascicleType == FascicleType[FascicleType.Procedure.toString()] || item.FascicleType == FascicleType[FascicleType.SubFascicle.toString()]) && item.DSWEnvironment == 0;
                                        })) {
                                            selectedNode_1.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png");
                                            selectedNode_1.get_attributes().setAttribute("Type", "Procedure");
                                            selectedNode_1.set_toolTip("Procedimento");
                                            selectedNode_1.set_visible(false);
                                        }
                                        else {
                                            selectedNode_1.set_visible(false);
                                        }
                                        _this._btnAddPeriodicPlan.set_enabled(false);
                                        _this._btnRemovePeriodicPlan.set_enabled(true);
                                        _this._rtvEnvironments.commitChanges();
                                        var params = "ReloadNodes|".concat(item_1.Category.EntityShortId.toString());
                                        $find(_this.ajaxManagerId).ajaxRequest(params);
                                        _this._loadingPanel.hide(_this.pnlFasciclePlanId);
                                    }, function (exception) {
                                        var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                        if (!jQuery.isEmptyObject(uscNotification)) {
                                            uscNotification.showNotification(exception);
                                        }
                                        _this._loadingPanel.hide(_this.pnlFasciclePlanId);
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
            };
            /**
            * Evento al click della Treeview
            * @param sender
            * @param eventArgs
            */
            this.environmentTreeView_ClientNodeClicked = function (sender, eventArgs) {
                var selectedNode = eventArgs.get_node();
                switch (selectedNode.get_attributes().getAttribute("Type")) {
                    case "Periodic": {
                        _this._btnAddPeriodicPlan.set_enabled(false);
                        _this._btnRemovePeriodicPlan.set_enabled(true);
                        break;
                    }
                    case "Procedure": {
                        _this._btnAddPeriodicPlan.set_enabled(false);
                        _this._btnRemovePeriodicPlan.set_enabled(true);
                        break;
                    }
                    case "ToManage": {
                        _this._btnAddPeriodicPlan.set_enabled(false);
                        _this._btnRemovePeriodicPlan.set_enabled(true);
                        break;
                    }
                    case "SubFascicle": {
                        _this._btnAddPeriodicPlan.set_enabled(false);
                        _this._btnRemovePeriodicPlan.set_enabled(true);
                        break;
                    }
                    default: {
                        _this._btnAddPeriodicPlan.set_enabled(true);
                        _this._btnRemovePeriodicPlan.set_enabled(false);
                        break;
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
         * Inizializzazione
         */
        uscFasciclePlan.prototype.initialize = function () {
            this._btnRemovePeriodicPlan = $find(this.btnRemovePeriodicPlan);
            this._btnRemovePeriodicPlan.add_clicking(this.btnRemovePeriodicPlan_onClientClick);
            this._btnRemovePeriodicPlan.set_enabled(false);
            this._btnAddPeriodicPlan = $find(this.btnAddPeriodicPlan);
            this._btnAddPeriodicPlan.add_clicking(this.btnAddPeriodicPlan_onClientClick);
            this._rtvEnvironments = $find(this.rtvEnvironmentsId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._managerCreatePeriod = $find(this.managerCreatePeriodId);
            this._managerCreatePeriod.add_close(this.managerCreatePeriod_OnClose);
            this._manager = $find(this.managerId);
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSRepository");
            this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
            var categoryFascicleService = ServiceConfigurationHelper.getService(this._serviceConfigurations, "CategoryFascicle");
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleService);
            if (this._rtvEnvironments.get_nodes().get_count() == 1) {
                this.loadNodes("0");
            }
            this.bindLoaded();
        };
        /**
        * ---------------------- Methods -------------------------
        */
        /**
        * Imposto gli attributi dei nodi
        */
        uscFasciclePlan.prototype.setNodesAttributes = function (idCategory) {
            var _this = this;
            this.currentCategoryId = idCategory;
            this._loadingPanel.show(this.pnlFasciclePlanId);
            this._categoryFascicleService.getByIdCategory(idCategory, function (data) {
                if (data) {
                    var rootNode = void 0;
                    rootNode = _this._rtvEnvironments.get_nodes().getNode(0);
                    rootNode.get_attributes().setAttribute("IdCategory", idCategory);
                    var mapper = new CategoryFascicleViewModelMapper();
                    var categoryFascicles_2 = mapper.MapCollection(data);
                    $.each(rootNode.get_allNodes(), function (index, node) {
                        var env = node.get_attributes().getAttribute("Environment");
                        if (categoryFascicles_2.some(function (item) {
                            return item.Environment == env || item.Environment == 0;
                        })) {
                            $.each(categoryFascicles_2, function (index, categoryFascicle) {
                                switch (categoryFascicle.FascicleType) {
                                    case "Period": {
                                        if (categoryFascicle.Environment == env) {
                                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/history.png");
                                            node.get_attributes().setAttribute("Type", "Periodic");
                                            node.set_toolTip("Periodico");
                                            node.get_attributes().setAttribute("Period", categoryFascicle.PeriodName);
                                            node.set_value(categoryFascicle.UniqueId);
                                        }
                                        break;
                                    }
                                    case "Procedure": {
                                        if (categoryFascicle.Environment == 0 &&
                                            (!node.get_attributes().getAttribute("Type") || node.get_attributes().getAttribute("Type") != "Periodic")) {
                                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png");
                                            node.get_attributes().setAttribute("Type", "Procedure");
                                            node.set_toolTip("Procedimento");
                                            node.set_value(categoryFascicle.UniqueId);
                                        }
                                        break;
                                    }
                                    case "SubFascicle": {
                                        if (categoryFascicle.Environment == 0 &&
                                            (!node.get_attributes().getAttribute("Type") || node.get_attributes().getAttribute("Type") != "Periodic")) {
                                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png");
                                            node.get_attributes().setAttribute("Type", "SubFascicle");
                                            node.set_toolTip("SottoFascicolo");
                                            node.set_value(categoryFascicle.UniqueId);
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
                            node.get_attributes().setAttribute("Type", "ToManage");
                            node.set_toolTip("Da gestire");
                        }
                    });
                    _this._rtvEnvironments.commitChanges();
                    _this._loadingPanel.hide(_this.pnlFasciclePlanId);
                }
            }, function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            });
        };
        /**
         * Funzione che carica i nodi
         */
        uscFasciclePlan.prototype.loadNodes = function (idCategory) {
            var _this = this;
            //recupera fasci category
            this.currentCategoryId = idCategory;
            this.pnlFasciclePlanId;
            this._loadingPanel.show(this.pnlFasciclePlanId);
            var selectedNode = this._rtvEnvironments.get_selectedNode();
            var rootNode = this._rtvEnvironments.get_nodes().getNode(0);
            var newNode;
            rootNode = this._rtvEnvironments.get_nodes().getNode(0);
            rootNode.get_attributes().setAttribute("IdCategory", idCategory);
            rootNode = this._rtvEnvironments.get_nodes().getNode(0);
            rootNode.set_expanded(true);
            rootNode.get_nodes().clear();
            this._categoryFascicleService.getByIdCategory(idCategory, function (data) {
                var mapper = new CategoryFascicleViewModelMapper();
                var categoryFascicles = mapper.MapCollection(data);
                $.each(categoryFascicles, function (index, categoryFascicle) {
                    switch (categoryFascicle.Environment) {
                        case DSWEnvironment.Protocol: {
                            newNode = new Telerik.Web.UI.RadTreeNode();
                            newNode.set_text("Protocollo".concat(" (", categoryFascicle.PeriodName, ")"));
                            newNode.get_attributes().setAttribute("Environment", DSWEnvironment.Protocol);
                            newNode.set_value(categoryFascicle.UniqueId);
                            rootNode.get_nodes().add(_this.setNodePeriodic(newNode));
                            break;
                        }
                        case DSWEnvironment.Resolution: {
                            newNode = new Telerik.Web.UI.RadTreeNode();
                            newNode.set_text(_this.currentResolutionName.concat(" (", categoryFascicle.PeriodName, ")"));
                            newNode.get_attributes().setAttribute("Environment", DSWEnvironment.Resolution);
                            newNode.set_value(categoryFascicle.UniqueId);
                            rootNode.get_nodes().add(_this.setNodePeriodic(newNode));
                            break;
                        }
                        case DSWEnvironment.DocumentSeries: {
                            newNode = new Telerik.Web.UI.RadTreeNode();
                            newNode.set_text(_this.currentDocumentSeriesName.concat(" (", categoryFascicle.PeriodName, ")"));
                            newNode.get_attributes().setAttribute("Environment", DSWEnvironment.DocumentSeries);
                            newNode.set_value(categoryFascicle.UniqueId);
                            rootNode.get_nodes().add(_this.setNodePeriodic(newNode));
                            break;
                        }
                        default: {
                            if (categoryFascicle.Environment >= 100) {
                                _this._udsRepositoryService.getUDSRepositoryByDSWEnvironment(categoryFascicle.Environment.toString(), function (dataRep) {
                                    if (dataRep) {
                                        newNode = new Telerik.Web.UI.RadTreeNode();
                                        newNode.set_text(dataRep.Name.concat(" (", categoryFascicle.PeriodName, ")"));
                                        newNode.get_attributes().setAttribute("Environment", dataRep.DSWEnvironment);
                                        newNode.set_value(categoryFascicle.UniqueId);
                                        rootNode.get_nodes().add(_this.setNodePeriodic(newNode));
                                    }
                                }, function (exception) {
                                    var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                    if (!jQuery.isEmptyObject(uscNotification)) {
                                        uscNotification.showNotification(exception);
                                    }
                                });
                            }
                            break;
                        }
                    }
                });
            });
            this._rtvEnvironments.commitChanges();
            this._loadingPanel.hide(this.pnlFasciclePlanId);
            this.bindLoaded();
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscFasciclePlan.prototype.bindLoaded = function () {
            $("#".concat(this.pnlFasciclePlanId)).data(this);
        };
        /**
         * Imposto come nodo selezionato quello root e resetto la visibilità dei bottoni
         */
        uscFasciclePlan.prototype.setRootNode = function () {
            var rootNode;
            rootNode = this._rtvEnvironments.get_nodes().getNode(0);
            rootNode.set_selected(true);
            this._btnAddPeriodicPlan.set_enabled(true);
            this._btnRemovePeriodicPlan.set_enabled(false);
        };
        /**
         * Apre una nuova nuova RadWindow
         * @param url
         * @param name
         * @param width
         * @param height
         */
        uscFasciclePlan.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.managerWindowId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        /**
         * metodo che setta il nodo root
         * @param node
         */
        uscFasciclePlan.prototype.setNodeDefault = function (node) {
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
            node.get_attributes().setAttribute("Type", "ToManage");
            node.set_toolTip("Da gestire");
            return node;
        };
        uscFasciclePlan.prototype.setNodePeriodic = function (node) {
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/history.png");
            node.get_attributes().setAttribute("Type", "Periodic");
            node.set_toolTip("Periodico");
            return node;
        };
        /**
         * Nascondo il loading
         */
        uscFasciclePlan.prototype.hideLoadingPanel = function () {
            this._loadingPanel.hide(this.pnlFasciclePlanId);
        };
        return uscFasciclePlan;
    }());
    return uscFasciclePlan;
});
//# sourceMappingURL=UscFasciclePlan.js.map