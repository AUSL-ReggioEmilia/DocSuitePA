/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/Commons/PrivacyLevelService", "App/Models/Commons/PrivacyLevelModel", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, PrivacyLevelService, PrivacyLevelModel, ServiceConfigurationHelper, ExceptionDTO) {
    var TbltPrivacyLevel = /** @class */ (function () {
        /**
       * Costruttore
       * @param serviceConfigurations
       */
        function TbltPrivacyLevel(serviceConfigurations) {
            var _this = this;
            /**
            * Inizializzazione classe
            */
            this.initialize = function () {
                _this._manager = $find(_this.managerId);
                _this._ajaxManager = $find(_this.ajaxManagerId);
                _this._rtvLevels = $find(_this.rtvLevelsId);
                _this._toolBarSearch = $find(_this.toolBarSearchId);
                _this._btnAdd = $find(_this.btnAddId);
                _this._btnEdit = $find(_this.btnEditId);
                //this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
                _this._windowAddPrivacyLevel = $find(_this.windowAddUDSTypologyId);
                _this._windowAddPrivacyLevel.add_close(_this.closeInsertWindow);
                _this._toolBarSearch.add_buttonClicked(_this.toolBarSearch_ButtonClicked);
                _this._btnAdd.add_clicked(_this.btnAdd_OnClick);
                _this._btnEdit.add_clicked(_this.btnEdit_OnClick);
                //this._btnDelete.add_clicked(this.btnDelete_OnClick);
                _this._btnEdit.set_enabled(false);
                //this._btnDelete.set_enabled(false);
                _this._rtvLevels.add_nodeClicked(_this.treeView_ClientNodeClicked);
                $('#'.concat(_this.pnlDetailsId)).hide();
                _this._loadingPanel = $find(_this.ajaxLoadingPanelId);
                _this._loadingPanel.show(_this.paneSelectionId);
                _this.loadLevels();
            };
            /**
            *------------------------- Events -----------------------------
            */
            this.toolBarSearch_ButtonClicked = function (sender, eventArgs) {
                //this._btnDelete.set_enabled(false);
                _this._btnEdit.set_enabled(false);
                _this._btnAdd.set_enabled(true);
                _this.loadLevels();
            };
            this.btnAdd_OnClick = function (sender, eventArgs) {
                var url = "../Tblt/TbltPrivacyLevelGes.aspx?Type=Comm&Action=Add";
                _this.openWindow(url, 500, 400, "Livelli di ".concat(_this.privacyLabel, " - Aggiungi"));
            };
            this.btnEdit_OnClick = function (sender, eventArgs) {
                var selectedNode = _this._currentSelectedNode;
                if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessun livello selezionato.');
                    return;
                }
                _this.setPrivacyLevelNode();
                var url = "../Tblt/TbltPrivacyLevelGes.aspx?Type=Comm&Action=Edit&IdPrivacyLevel=".concat(selectedNode.get_value());
                _this.openWindow(url, 500, 400, "Livelli di ".concat(_this.privacyLabel, " - Modifica"));
            };
            this.btnDelete_OnClick = function (sender, eventArgs) {
                var selectedNode = _this._currentSelectedNode;
                if (selectedNode == undefined || selectedNode.get_value() == null || selectedNode.get_value() == "") {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessun livello selezionato.');
                    return;
                }
                _this._manager.radconfirm("Sei sicuro di voler eliminare il livello di ".concat(_this.privacyLabel, "?"), function (arg) {
                    if (arg) {
                        var privacyLevel_1 = new PrivacyLevelModel();
                        privacyLevel_1.UniqueId = _this._currentSelectedNode.get_value();
                        privacyLevel_1.IsActive = false;
                        _this._privacyLevelService.updatePrivacyLevel(privacyLevel_1, function (data) {
                            _this._btnAdd.set_enabled(false);
                            _this._btnEdit.set_enabled(false);
                            //this._btnDelete.set_enabled(false);
                            _this.removeNode(privacyLevel_1.UniqueId);
                        }, function (exception) {
                            var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        });
                    }
                });
            };
            this.treeView_ClientNodeClicked = function (sender, args) {
                _this._currentSelectedNode = args.get_node();
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                $('#'.concat(_this.pnlDetailsId)).show();
                if (!_this._currentSelectedNode || !_this._currentSelectedNode.get_value()) {
                    $('#'.concat(_this.pnlDetailsId)).hide();
                    _this._btnAdd.set_enabled(true);
                    _this._btnEdit.set_enabled(false);
                    //this._btnDelete.set_enabled(false);
                    return;
                }
                //this._btnDelete.set_enabled(true);
                _this._btnEdit.set_enabled(true);
                _this._btnAdd.set_enabled(false);
                if (_this._currentSelectedNode == _this._rtvLevels.get_nodes().getNode(0)) {
                    _this._btnAdd.set_enabled(true);
                    _this._btnEdit.set_enabled(false);
                    //this._btnDelete.set_enabled(false);
                    $('#'.concat(_this.pnlDetailsId)).hide();
                    return;
                }
                _this._loadingPanel.show(_this.paneSelectionId);
                _this._loadingPanel.show(_this.pnlDetailsId);
                _this.loadPrivacyLevel();
            };
            /**
            * Crea e imposta i nodi nella RadTreeView di visualizzazione
            */
            this.setNodes = function (levels) {
                var rootNode = _this._rtvLevels.get_nodes().getNode(0);
                rootNode.get_nodes().clear();
                $.each(levels, function (index, item) {
                    //Verifico se il nodo già esiste nella treeview
                    if (_this._rtvLevels.findNodeByValue(item.UniqueId)) {
                        return;
                    }
                    var newNode = _this.decorateNode(item);
                    rootNode.get_nodes().add(newNode);
                    _this.setNodeColour(newNode, item.Colour);
                });
                rootNode.set_expanded(true);
                _this._rtvLevels.commitChanges();
            };
            this.closeInsertWindow = function (sender, args) {
                var result = args.get_argument();
                if (result) {
                    switch (result.ActionName) {
                        case "Add":
                            var newLevel = JSON.parse(result.Value[0]);
                            var newNode = _this.decorateNode(newLevel);
                            var rootNode = _this._rtvLevels.get_nodes().getNode(0);
                            rootNode.get_nodes().add(newNode);
                            _this.setNodeColour(newNode, newLevel.Colour);
                            _this.refreshLevels();
                            break;
                        case "Edit":
                            var updatedNode = _this._rtvLevels.get_selectedNode();
                            if (result && result.Value[0]) {
                                var level = JSON.parse(result.Value[0]);
                                _this.setNodeAttribute(updatedNode, level);
                                _this.setNodeColour(updatedNode, level.Colour);
                                _this._rtvLevels.commitChanges();
                                _this.loadPrivacyLevel();
                                _this.refreshLevels();
                            }
                            break;
                    }
                }
            };
            this.setNodeAttribute = function (node, level) {
                node.set_text(level.Description);
                node.set_value(level.UniqueId);
                node.get_attributes().setAttribute("Level", level.Level);
                node.get_attributes().setAttribute("IsActive", level.IsActive);
                node.get_attributes().setAttribute("Colour", level.Colour);
                node.set_cssClass(level.IsActive ? "" : "node-disabled");
                return node;
            };
            /**
            * Caricamento del nodo selezionato nella Session Storage
            */
            this.setPrivacyLevelNode = function () {
                var privacyLevelNode = new PrivacyLevelModel();
                privacyLevelNode.UniqueId = _this._currentSelectedNode.get_value();
                privacyLevelNode.Description = _this._currentSelectedNode.get_text();
                privacyLevelNode.Level = Number(_this._currentSelectedNode.get_attributes().getAttribute("Level"));
                privacyLevelNode.IsActive = _this._currentSelectedNode.get_attributes().getAttribute("IsActive");
                privacyLevelNode.Colour = _this._currentSelectedNode.get_attributes().getAttribute("Colour");
                sessionStorage[privacyLevelNode.UniqueId] = JSON.stringify(privacyLevelNode);
            };
            /**
            * Carica la tipologia selezionata
            */
            this.loadPrivacyLevel = function () {
                _this._privacyLevelService.getById(_this._currentSelectedNode.get_value(), function (data) {
                    if (data) {
                        _this._currentPrivacyLevel = data;
                        _this.loadDetails();
                        _this._btnEdit.set_enabled(true);
                        //this._btnDelete.set_enabled(true);                    
                        _this._loadingPanel.hide(_this.pnlDetailsId);
                        _this._loadingPanel.hide(_this.pnlInformationsId);
                        _this._loadingPanel.hide(_this.paneSelectionId);
                    }
                }, function (exception) {
                    _this._loadingPanel.hide(_this.paneSelectionId);
                    _this._loadingPanel.hide(_this.pnlInformationsId);
                    _this._loadingPanel.hide(_this.pnlDetailsId);
                    _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento del livello.");
                });
            };
            /**
            * Carica i dettagli della tipologia selezionata nella sezione dettagli
            */
            this.loadDetails = function () {
                $('#'.concat(_this.txtDescriptionId)).text(_this._currentPrivacyLevel.Description);
                $('#'.concat(_this.txtLevelId)).text(_this._currentPrivacyLevel.Level);
                $('#'.concat(_this.lblActiveFromId)).text(moment(_this._currentPrivacyLevel.RegistrationDate).format("DD/MM/YYYY"));
                $('#'.concat(_this.lblIsActiveId)).text(_this._currentPrivacyLevel.IsActive ? 'Attivo' : 'Non Attivo');
                var colour = _this._currentPrivacyLevel.Colour;
                if (!colour) {
                    colour = '#ffffff';
                }
                $('#'.concat(_this.colorBoxId)).css('background-color', colour);
            };
            this.removeNode = function (idPrivacyLevel) {
                var nodeToRemove = _this._rtvLevels.findNodeByValue(idPrivacyLevel);
                if (nodeToRemove) {
                    _this._rtvLevels.get_nodes().remove(nodeToRemove);
                    _this._rtvLevels.commitChanges();
                    $('#'.concat(_this.pnlDetailsId)).hide();
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "PrivacyLevel");
            this._privacyLevelService = new PrivacyLevelService(serviceConfiguration);
        }
        /**
         *------------------------- Methods -----------------------------
         */
        TbltPrivacyLevel.prototype.loadLevels = function () {
            var _this = this;
            var txtDescription = this._toolBarSearch.findItemByValue('searchDescription').findControl('txtDescription');
            var description = txtDescription ? txtDescription.get_value() : '';
            this._privacyLevelService.findPrivacyLevels(description, function (data) {
                if (data) {
                    _this.setNodes(data);
                    _this._loadingPanel.hide(_this.paneSelectionId);
                }
            }, function (exception) {
                _this._loadingPanel.hide(_this.paneSelectionId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        TbltPrivacyLevel.prototype.decorateNode = function (level) {
            var newNode = new Telerik.Web.UI.RadTreeNode();
            newNode = this.setNodeAttribute(newNode, level);
            newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/lock.png");
            newNode.set_selectedCssClass("selectedLevel");
            return newNode;
        };
        TbltPrivacyLevel.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltPrivacyLevel.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        TbltPrivacyLevel.prototype.openWindow = function (url, width, height, title) {
            if (title) {
                this._windowAddPrivacyLevel.set_title(title);
            }
            this._windowAddPrivacyLevel.setSize(width, height);
            this._windowAddPrivacyLevel.setUrl(url);
            this._windowAddPrivacyLevel.set_modal(true);
            this._windowAddPrivacyLevel.show();
        };
        TbltPrivacyLevel.prototype.refreshLevels = function () {
            var ajaxModel = {};
            ajaxModel.ActionName = 'Refresh';
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        TbltPrivacyLevel.prototype.setNodeColour = function (node, colour) {
            if (colour && colour.toLowerCase() != '#ffffff') {
                node.get_textElement().style.color = colour;
            }
        };
        return TbltPrivacyLevel;
    }());
    return TbltPrivacyLevel;
});
//# sourceMappingURL=TbltPrivacyLevel.js.map