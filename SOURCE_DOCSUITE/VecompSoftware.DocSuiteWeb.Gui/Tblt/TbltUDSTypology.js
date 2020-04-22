/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/UDS/UDSTypologyService", "App/Services/UDS/UDSRepositoryService", "App/Models/UDS/UDSTypologyModel", "App/Models/UDS/UDSTypologyStatus", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Helpers/EnumHelper"], function (require, exports, UDSTypologyService, UDSRepositoryService, UDSTypologyModel, UDSTypologyStatus, ServiceConfigurationHelper, ExceptionDTO, EnumHelper) {
    var TbltUDSTypology = /** @class */ (function () {
        /**
       * Costruttore
       * @param serviceConfigurations
       */
        function TbltUDSTypology(serviceConfigurations) {
            var _this = this;
            /**
            * Inizializzazione classe
            */
            this.initialize = function () {
                _this._rtvTypology = $find(_this.rtvTypologyId);
                _this._toolBarSearch = $find(_this.toolBarSearchId);
                _this._toolBarStatus = $find(_this.toolBarStatusId);
                _this._btnAdd = $find(_this.btnAddId);
                _this._btnEdit = $find(_this.btnEditId);
                _this._btnAggiungi = $find(_this.btnAggiungiId);
                _this._btnRimuovi = $find(_this.btnRimuoviId);
                _this._windowAddUDSTypology = $find(_this.windowAddUDSTypologyId);
                _this._windowAddUDSTypology.add_close(_this.closeInsertWindow);
                _this._toolBarSearch.add_buttonClicked(_this.toolBarSearch_ButtonClicked);
                _this._toolBarStatus.add_buttonClicked(_this.toolBarStatus_ButtonClicked);
                _this._btnAdd.add_clicked(_this.btnAdd_OnClick);
                _this._btnEdit.add_clicked(_this.btnEdit_OnClick);
                _this._btnEdit.set_enabled(false);
                _this._btnAggiungi.add_clicked(_this.btnAggiungi_OnClick);
                _this._btnRimuovi.add_clicked(_this.btnRimuovi_OnClick);
                _this._rtvTypology.add_nodeClicked(_this.treeView_ClientNodeClicked);
                _this._grdUDSRepositories = $find(_this.grdUDSRepositoriesId);
                $('#'.concat(_this.pnlDetailsId)).hide();
                $('#'.concat(_this.pnlButtonsId)).hide();
                _this._loadingPanel = $find(_this.ajaxLoadingPanelId);
                _this._loadingPanel.show(_this.paneSelectionId);
                _this.loadTypologies();
            };
            /**
            *------------------------- Events -----------------------------
            */
            this.toolBarSearch_ButtonClicked = function (sender, eventArgs) {
                _this.loadTypologies();
            };
            this.toolBarStatus_ButtonClicked = function (sender, eventArgs) {
                _this.loadTypologies();
            };
            this.btnAdd_OnClick = function (sender, eventArgs) {
                var url = "../Tblt/TbltUDSTypologyGes.aspx?Type=Comm&Action=Add";
                _this.openWindow(url, 450, 250, "Tipologie - Aggiungi");
            };
            this.btnEdit_OnClick = function (sender, eventArgs) {
                var selectedNode = _this._currentSelectedNode;
                if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessuna tipologia selezionata');
                    return;
                }
                _this.setUDSTypologyNode();
                var url = "../Tblt/TbltUDSTypologyGes.aspx?Type=Comm&Action=Edit&IdUDSTypology=".concat(selectedNode.get_value());
                _this.openWindow(url, 450, 250, "Tipologie - Modifica");
            };
            this.btnAggiungi_OnClick = function (sender, eventArgs) {
                var selectedNode = _this._currentSelectedNode;
                if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessuna tipologia selezionata');
                    return;
                }
                _this.setUDSTypologyNode();
                var url = "../Tblt/TbltUDSRepositoriesTypologyGes.aspx?Type=Comm&IdUDSTypology=".concat(selectedNode.get_value());
                _this.openWindow(url, 600, 450, "Tipologie - Aggiungi Archivio");
            };
            this.btnRimuovi_OnClick = function (sender, eventArgs) {
                var selectedNode = _this._currentSelectedNode;
                if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessuna tipologia selezionata');
                    return;
                }
                _this.setUDSTypologyNode();
                if (!_this._grdUDSRepositories.get_selectedItems() || _this._grdUDSRepositories.get_selectedItems().length < 1) {
                    _this.showNotificationException(_this.uscNotificationId, null, "Selezionare un archivio da rimuovere");
                    return;
                }
                _this._loadingPanel.show(_this.pnlDetailsId);
                var currentUDSTypologyModel = JSON.parse(sessionStorage[selectedNode.get_value()]);
                var udsRepositories = _this._grdUDSRepositories.get_selectedItems();
                var notDeletedUDSReposiotries = new Array();
                var _loop_1 = function (item) {
                    var selectedItems = udsRepositories.filter(function (rep) { if (rep._dataItem.UniqueId == item.UniqueId)
                        return rep._dataItem; });
                    if (!selectedItems || selectedItems.length < 1) {
                        notDeletedUDSReposiotries.push(item);
                    }
                };
                for (var _i = 0, _a = currentUDSTypologyModel.UDSRepositories; _i < _a.length; _i++) {
                    var item = _a[_i];
                    _loop_1(item);
                }
                currentUDSTypologyModel.UDSRepositories = notDeletedUDSReposiotries;
                _this._udsTypologyService.updateUDSTypology(currentUDSTypologyModel, function (data) {
                    if (data) {
                        _this.loadDetailsTypology();
                        _this._loadingPanel.hide(_this.pnlDetailsId);
                    }
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pnlDetailsId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.treeView_ClientNodeClicked = function (sender, args) {
                _this._currentSelectedNode = args.get_node();
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                $('#'.concat(_this.pnlDetailsId)).show();
                if (!_this._currentSelectedNode || !_this._currentSelectedNode.get_value()) {
                    $('#'.concat(_this.pnlDetailsId)).hide();
                    $('#'.concat(_this.pnlButtonsId)).hide();
                    _this._btnAdd.set_enabled(true);
                    _this._btnEdit.set_enabled(false);
                    return;
                }
                _this._btnEdit.set_enabled(true);
                _this._btnAdd.set_enabled(false);
                if (_this._currentSelectedNode == _this._rtvTypology.get_nodes().getNode(0)) {
                    _this._btnAdd.set_enabled(true);
                    _this._btnEdit.set_enabled(false);
                    _this._btnAggiungi.set_enabled(false);
                    _this._btnRimuovi.set_enabled(false);
                    $('#'.concat(_this.pnlDetailsId)).hide();
                    $('#'.concat(_this.pnlButtonsId)).hide();
                    return;
                }
                _this._loadingPanel.show(_this.paneSelectionId);
                _this._loadingPanel.show(_this.pnlDetailsId);
                $.when(_this.loadUDSTypology(), _this.loadUDSRepositories()).done(function () {
                    var inactive = (parseInt(UDSTypologyStatus[_this._currentUDSTypology.Status]) == UDSTypologyStatus.Inactive);
                    _this._btnEdit.set_enabled(!inactive);
                    _this._btnAggiungi.set_enabled(!inactive);
                    _this._btnRimuovi.set_enabled(!inactive);
                    _this.loadDetails();
                    _this.fillDetailsTable(_this._currentUDSTypologyUDSRepositories);
                    $('#'.concat(_this.pnlButtonsId)).show();
                    _this._loadingPanel.hide(_this.paneSelectionId);
                    _this._loadingPanel.hide(_this.pnlDetailsId);
                }).fail(function (exception) {
                    $('#'.concat(_this.pnlButtonsId)).show();
                    _this._loadingPanel.hide(_this.paneSelectionId);
                    _this._loadingPanel.hide(_this.pnlDetailsId);
                    _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento della Tipologia.");
                });
            };
            /**
            * Crea e imposta i nodi nella RadTreeView di visualizzazione
            */
            this.setNodes = function (typologies) {
                var rootNode = _this._rtvTypology.get_nodes().getNode(0);
                rootNode.get_nodes().clear();
                $.each(typologies, function (index, item) {
                    //Verifico se il nodo già esiste nella treeview
                    if (_this._rtvTypology.findNodeByValue(item.UniqueId) != undefined) {
                        return;
                    }
                    var newNode = _this.decorateNode(item);
                    rootNode.get_nodes().add(newNode);
                });
                rootNode.set_expanded(true);
                _this._rtvTypology.commitChanges();
            };
            this.closeInsertWindow = function (sender, args) {
                var result = args.get_argument();
                if (result) {
                    switch (result.ActionName) {
                        case "Add":
                            var checkedButtons = _this._toolBarStatus.get_items().toArray().filter(function (i) { return i.get_checked(); });
                            if (!checkedButtons || checkedButtons.filter(function (c) { return c.get_value() == UDSTypologyStatus[UDSTypologyStatus.Active]; }).length > 0) {
                                var newTypology = JSON.parse(result.Value[0]);
                                var newNode = _this.decorateNode(newTypology);
                                var rootNode = _this._rtvTypology.get_nodes().getNode(0);
                                rootNode.get_nodes().add(newNode);
                            }
                            break;
                        case "Edit":
                            var updatedNode = _this._rtvTypology.get_selectedNode();
                            if (result && result.Value[0]) {
                                var typology = JSON.parse(result.Value[0]);
                                _this.setNodeAttribute(updatedNode, typology);
                                _this._rtvTypology.commitChanges();
                            }
                            break;
                        case "AddUDSRepositories":
                            _this.loadDetailsTypology();
                            break;
                    }
                }
            };
            this.setNodeAttribute = function (node, typology) {
                node.get_attributes().setAttribute("Status", typology.Status);
                node.set_text(typology.Name);
                node.set_value(typology.UniqueId);
                return node;
            };
            /**
            * Caricamento del nodo selezionato nella Session Storage
            */
            this.setUDSTypologyNode = function () {
                var udsTypologyNode = new UDSTypologyModel();
                udsTypologyNode.UniqueId = _this._currentSelectedNode.get_value();
                udsTypologyNode.Name = _this._currentSelectedNode.get_text();
                udsTypologyNode.Status = _this._currentSelectedNode.get_attributes().getAttribute("Status");
                udsTypologyNode.UDSRepositories = _this._currentUDSTypologyUDSRepositories;
                sessionStorage[udsTypologyNode.UniqueId] = JSON.stringify(udsTypologyNode);
            };
            /**
            * Carica la tipologia selezionata
            */
            this.loadUDSTypology = function () {
                var promise = $.Deferred();
                try {
                    _this._udsTypologyService.getUDSTypologyById(_this._currentSelectedNode.get_value(), function (data) {
                        if (data == null) {
                            promise.resolve();
                            return;
                        }
                        _this._currentUDSTypology = data;
                        promise.resolve();
                    }, function (exception) {
                        promise.reject(exception);
                    });
                }
                catch (error) {
                    console.log(error.stack);
                    promise.reject(error);
                }
                return promise.promise();
            };
            /**
            * Carica i dettagli della tipologia selezionata nella sezione dettagli
            */
            this.loadDetails = function () {
                $('#'.concat(_this.lblStatusId)).text(_this._enumHelper.getUDSTypologyStatusDescription(_this._currentUDSTypology.Status));
                $('#'.concat(_this.lblActiveFromId)).text(moment(_this._currentUDSTypology.RegistrationDate).format("DD/MM/YYYY"));
            };
            /**
             * Imposta gli archivi della tipologia selezionata
             */
            this.loadUDSRepositories = function () {
                var promise = $.Deferred();
                try {
                    _this._udsRepositoryService.getUDSRepositoryByUDSTypology(_this._currentSelectedNode.get_value(), function (response) {
                        if (response == null) {
                            promise.resolve();
                            return;
                        }
                        _this._currentUDSTypologyUDSRepositories = response;
                        promise.resolve();
                    }, function (exception) {
                        promise.reject(exception);
                    });
                }
                catch (error) {
                    console.log(error.stack);
                    promise.reject(error);
                }
                return promise.promise();
            };
            /**
            * Carica gli archivi della tipologia selezionata nella griglia
            */
            this.fillDetailsTable = function (udsRepositories) {
                _this._grdUDSRepositories = $find(_this.grdUDSRepositoriesId);
                var grdUDSRepositoriesMasterTableView = _this._grdUDSRepositories.get_masterTableView();
                grdUDSRepositoriesMasterTableView.set_dataSource(udsRepositories);
                grdUDSRepositoriesMasterTableView.clearSelectedItems();
                grdUDSRepositoriesMasterTableView.dataBind();
            };
            /**
            * Carica il pannello dei detttagli
            */
            this.loadDetailsTypology = function () {
                $.when(_this.loadUDSTypology(), _this.loadUDSRepositories()).done(function () {
                    _this.loadDetails();
                    _this._loadingPanel.hide(_this.pnlInformationsId);
                    _this.fillDetailsTable(_this._currentUDSTypologyUDSRepositories);
                    _this._loadingPanel.hide(_this.pnlUDSRepositoriesId);
                    $('#'.concat(_this.pnlButtonsId)).show();
                    _this._loadingPanel.hide(_this.paneSelectionId);
                }).fail(function (exception) {
                    $('#'.concat(_this.pnlButtonsId)).show();
                    _this._loadingPanel.hide(_this.paneSelectionId);
                    _this._loadingPanel.hide(_this.pnlInformationsId);
                    _this._loadingPanel.hide(_this.pnlUDSRepositoriesId);
                    _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento della Tipologia.");
                });
            };
            this._serviceConfigurations = serviceConfigurations;
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "UDSTypology");
            this._udsTypologyService = new UDSTypologyService(serviceConfiguration);
            var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, TbltUDSTypology.UDSREPOSITORY_TYPE_NAME);
            this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
            this._enumHelper = new EnumHelper();
        }
        /**
         *------------------------- Methods -----------------------------
         */
        TbltUDSTypology.prototype.loadTypologies = function () {
            var _this = this;
            var txtDescription = this._toolBarSearch.findItemByValue('searchDescription').findControl('txtDescription');
            var checkedButtons = this._toolBarStatus.get_items().toArray().filter(function (i) { return i.get_checked(); });
            var description = txtDescription ? txtDescription.get_value() : '';
            var status = null;
            if (checkedButtons && checkedButtons.length == 1) {
                status = UDSTypologyStatus[checkedButtons[0].get_value()];
            }
            this._udsTypologyService.getUDSTypologyByName(description, status, function (data) {
                if (data) {
                    _this.setNodes(data);
                    _this._loadingPanel.hide(_this.paneSelectionId);
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.paneSelectionId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltUDSTypology.prototype.decorateNode = function (typology) {
            var newNode = new Telerik.Web.UI.RadTreeNode();
            newNode = this.setNodeAttribute(newNode, typology);
            newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/document_copies.png");
            if (parseInt(UDSTypologyStatus[typology.Status]) == UDSTypologyStatus.Inactive) {
                newNode.set_cssClass('node-disabled');
            }
            return newNode;
        };
        TbltUDSTypology.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltUDSTypology.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        TbltUDSTypology.prototype.openWindow = function (url, width, height, title) {
            if (title) {
                this._windowAddUDSTypology.set_title(title);
            }
            this._windowAddUDSTypology.setSize(width, height);
            this._windowAddUDSTypology.setUrl(url);
            this._windowAddUDSTypology.set_modal(true);
            this._windowAddUDSTypology.show();
        };
        TbltUDSTypology.UDSREPOSITORY_TYPE_NAME = "UDSRepository";
        return TbltUDSTypology;
    }());
    return TbltUDSTypology;
});
//# sourceMappingURL=TbltUDSTypology.js.map