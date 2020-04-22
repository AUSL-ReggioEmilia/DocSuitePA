/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/helpers/stringhelper.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "App/Models/MassimariScarto/MassimarioScartoModel", "App/Models/MassimariScarto/MassimarioScartoStatusType", "App/Services/MassimariScarto/MassimarioScartoService", "App/Helpers/ServiceConfigurationHelper", "../app/core/extensions/string"], function (require, exports, MassimarioScartoModel, MassimarioScartoStatusType, MassimarioScartoService, ServiceConfigurationHelper) {
    var uscMassimarioScarto = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function uscMassimarioScarto(serviceConfigurations) {
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
            this.treeView_ClientNodeExpanding = function (sender, args) {
                var node = args.get_node();
                var strNodeType = node.get_attributes().getAttribute("NodeType");
                if (node.get_nodes().get_count() == 0 && !hasFilter) {
                    args.set_cancel(true);
                    node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    node.set_selected(true);
                    _this._service.getMassimariByParent(_this.getSearchIncludeCancel(), node.get_attributes().getAttribute("UniqueId"), function (data) {
                        _this.loadNodes(data);
                        $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString());
                    }, _this.errorEventCallback);
                }
                else {
                    $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString());
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
                if (node.get_value() != "0" && node.get_nodes().get_count() == 0 && !hasFilter) {
                    node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                    _this._service.getMassimariByParent(_this.getSearchIncludeCancel(), node.get_attributes().getAttribute("UniqueId"), function (data) {
                        _this.loadNodes(data);
                        $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString());
                    }, _this.errorEventCallback);
                }
                else {
                    $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, node.toJsonString());
                }
            };
            /**
             * Evento scatenato al click di un RadButton nella toolbar di ricerca
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.toolBarSearch_ButtonClicked = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                var rootNode = _this._treeMassimario.get_nodes().getNode(0);
                rootNode.get_nodes().clear();
                rootNode.select();
                var txtSearchDescription = sender.findItemByValue('searchDescription').findControl('txtSearchName');
                var txtSearchCode = sender.findItemByValue('searchCode').findControl('txtSearchCode');
                var description = txtSearchDescription.get_value();
                var code = null;
                if (!String.isNullOrEmpty(txtSearchCode.get_value()))
                    code = txtSearchCode.get_value();
                $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_START_LOAD_EVENT);
                if (String.isNullOrEmpty(description) && code == undefined) {
                    hasFilter = false;
                    _this._service.getMassimariByParent(_this.getSearchIncludeCancel(), null, _this.loadNodes, _this.errorEventCallback);
                }
                else {
                    hasFilter = true;
                    _this._service.findMassimari(description, _this.getSearchIncludeCancel(), code, _this.loadNodes, _this.errorEventCallback);
                }
            };
            /**
            * Lancio un evento alla pagina di una eccezione avvenuta tramite una richiesta ajax.
            * Gestisco comunque l'errore nella console.
            * @param XMLHttpRequest
            * @param textStatus
            * @param errorThrown
            */
            this.errorEventCallback = function (exception) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
                $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_ERROR_EVENT, XMLHttpRequest);
            };
            /**
             * Carica gli elementi nella treeview
             * @method
             * @param data
             * @returns
             */
            this.loadNodes = function (data, node) {
                var parent = _this._treeMassimario.get_selectedNode();
                if (node != undefined) {
                    parent = node;
                }
                $.each(data, function (index, massimario) {
                    //Verifico se il nodo già esiste nella treeview
                    if (_this._treeMassimario.findNodeByValue(massimario.MassimarioScartoPath) != undefined) {
                        return;
                    }
                    var newNode = new Telerik.Web.UI.RadTreeNode();
                    var massimarioName = massimario.Code.toString().concat(".", massimario.Name);
                    newNode.set_text(massimarioName);
                    newNode.set_value(massimario.MassimarioScartoPath);
                    if (!massimario.isActive()) {
                        newNode.set_cssClass('node-disabled');
                    }
                    _this.setNodeAttribute(newNode, massimario);
                    //Gestisco il fatto che per il massimario di scarto saranno presenti al massimo 2 livelli
                    if (massimario.MassimarioScartoLevel == 2) {
                        newNode.set_imageUrl("../Comm/images/Classificatore.gif");
                        var currentParent = _this._treeMassimario.findNodeByValue(massimario.MassimarioScartoParentPath);
                        currentParent.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                        currentParent.get_nodes().add(newNode);
                        currentParent.set_expanded(true);
                        return;
                    }
                    else {
                        newNode.set_imageUrl("../Comm/images/FolderOpen16.gif");
                        if (hasFilter) {
                            newNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                        }
                        else {
                            newNode.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ServerSideCallBack);
                        }
                    }
                    parent.get_nodes().add(newNode);
                });
                parent.hideLoadingStatus();
                parent.set_expandMode(Telerik.Web.UI.TreeNodeExpandMode.ClientSide);
                parent.set_expanded(true);
                _this._treeMassimario.commitChanges();
                //Spedisco l'evento di fine loading massimari
                $("#".concat(_this.treeMassimarioId)).triggerHandler(uscMassimarioScarto.ON_END_LOAD_EVENT);
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "MassimarioScarto");
            if (!serviceConfiguration) {
                this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Massimario di Scarto");
                return;
            }
            this._service = new MassimarioScartoService(serviceConfiguration);
        }
        /**
         *------------------------- Methods -----------------------------
         */
        /**
        * Metodo di inizializzazione
        */
        uscMassimarioScarto.prototype.initialize = function () {
            this._treeMassimario = $find(this.treeMassimarioId);
            this._toolBarSearch = $find(this.toolBarSearchId);
            this._toolBarSearch.add_buttonClicking(this.toolBarSearch_ButtonClicked);
            if (this.hideCanceledFilter) {
                this._toolBarSearch.findItemByValue('includeCancel').findControl('btnIncludeCancel').set_visible(false);
            }
            $("#".concat(this.treeMassimarioId)).data(this);
            hasFilter = false;
            //Caricamento dati iniziale
            this._service.getMassimariByParent(this.getSearchIncludeCancel(), null, this.loadNodes, this.errorEventCallback);
        };
        /**
         * Imposta gli attributi di un nodo
         * @param node
         * @param massimario
         */
        uscMassimarioScarto.prototype.setNodeAttribute = function (node, massimario) {
            node.get_attributes().setAttribute("UniqueId", massimario.UniqueId);
            node.get_attributes().setAttribute("MassimarioScartoLevel", massimario.MassimarioScartoLevel);
            node.get_attributes().setAttribute("Code", massimario.Code);
            node.get_attributes().setAttribute("Name", massimario.Name);
            node.get_attributes().setAttribute("Note", massimario.Note);
            node.get_attributes().setAttribute("ConservationPeriod", massimario.ConservationPeriod);
            node.get_attributes().setAttribute("StartDate", massimario.StartDate);
            node.get_attributes().setAttribute("EndDate", massimario.EndDate);
            node.get_attributes().setAttribute("Status", massimario.Status);
            node.get_attributes().setAttribute("IsActive", massimario.isActive());
            return node;
        };
        /**
         * Recupera il modello dal nodo selezionato nella treeview
         */
        uscMassimarioScarto.prototype.getSelectedMassimario = function () {
            var selectedNode = this._treeMassimario.get_selectedNode();
            var model = new MassimarioScartoModel();
            model.UniqueId = selectedNode.get_attributes().getAttribute("UniqueId");
            model.Code = selectedNode.get_attributes().getAttribute("Code");
            model.Name = selectedNode.get_attributes().getAttribute("Name");
            model.Note = selectedNode.get_attributes().getAttribute("Note");
            model.ConservationPeriod = selectedNode.get_attributes().getAttribute("ConservationPeriod");
            model.StartDate = selectedNode.get_attributes().getAttribute("StartDate");
            model.EndDate = selectedNode.get_attributes().getAttribute("EndDate");
            if (selectedNode.get_attributes().getAttribute("Status") != undefined) {
                model.Status = MassimarioScartoStatusType[selectedNode.get_attributes().getAttribute("Status")];
            }
            model.MassimarioScartoLevel = selectedNode.get_attributes().getAttribute("MassimarioScartoLevel");
            model.FakeInsertId = selectedNode.get_parent().get_attributes().getAttribute("UniqueId");
            return model;
        };
        /**
     * Aggiorna il nodo padre del nodo selezionato
     */
        uscMassimarioScarto.prototype.updateParentNode = function (callback) {
            var _this = this;
            var parentNode = this._treeMassimario.get_selectedNode().get_parent();
            parentNode.get_nodes().clear();
            parentNode.select();
            var idMassimarioScarto = null;
            if (parentNode.get_value() != 0) {
                idMassimarioScarto = parentNode.get_attributes().getAttribute("UniqueId");
            }
            this._service.getMassimariByParent(this.getSearchIncludeCancel(), idMassimarioScarto, function (data) {
                _this.loadNodes(data);
                if (callback)
                    callback();
            }, this.errorEventCallback);
        };
        /**
         * Aggiorna i nodi figli del nodo selezionato
         */
        uscMassimarioScarto.prototype.updateSelectedNodeChildren = function () {
            var selectedNode = this._treeMassimario.get_selectedNode();
            selectedNode.get_nodes().clear();
            var idMassimarioScarto = null;
            if (selectedNode.get_value() != 0) {
                idMassimarioScarto = selectedNode.get_attributes().getAttribute("UniqueId");
            }
            this._service.getMassimariByParent(this.getSearchIncludeCancel(), idMassimarioScarto, this.loadNodes, this.errorEventCallback);
        };
        /**
         * Verifica se tutti i nodi figli del nodo selezionato sono in stato annullato
         */
        uscMassimarioScarto.prototype.allSelectedChildrenIsCancel = function () {
            var selectedNode = this._treeMassimario.get_selectedNode();
            var nodes = selectedNode.get_nodes().toArray();
            return nodes.filter(function (node) { return MassimarioScartoStatusType[node.get_attributes().getAttribute("Status")] == MassimarioScartoStatusType.Active; }).length == 0;
        };
        /**
         * Ritorna se devono essere inclusi gli elementi annullati nella ricerca. Data dal valore del controllo.
         */
        uscMassimarioScarto.prototype.getSearchIncludeCancel = function () {
            if (this.hideCanceledFilter)
                return false;
            var includeCancel = this._toolBarSearch.findItemByValue('includeCancel').findControl('btnIncludeCancel').get_checked();
            return includeCancel;
        };
        uscMassimarioScarto.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscMassimarioScarto.ON_SELECTED_NODE_EVENT = "onSelectedNode";
        uscMassimarioScarto.ON_START_LOAD_EVENT = "onStartLoad";
        uscMassimarioScarto.ON_END_LOAD_EVENT = "onEndLoad";
        uscMassimarioScarto.ON_ERROR_EVENT = "onErrorEvent";
        return uscMassimarioScarto;
    }());
    return uscMassimarioScarto;
});
//# sourceMappingURL=uscMassimarioScarto.js.map