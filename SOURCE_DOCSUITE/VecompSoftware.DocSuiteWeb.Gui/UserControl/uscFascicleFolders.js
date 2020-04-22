/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleFolderStatus", "App/Models/Fascicles/FascicleFolderTypology", "App/Services/Fascicles/FascicleFolderService", "App/DTOs/ExceptionDTO", "App/Services/Fascicles/FascicleService", "Fasc/FascMoveItems"], function (require, exports, ServiceConfigurationHelper, FascicleFolderStatus, FascicleFolderTypology, FascicleFolderService, ExceptionDTO, FascicleService, FascMoveItems) {
    var uscFascicleFolders = /** @class */ (function () {
        /**
        * Costruttore
        * @param webApiConfiguration
        */
        function uscFascicleFolders(serviceConfigurations) {
            var _this = this;
            this.folder_close_path = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
            this.folder_open_path = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
            this.folder_internet_close_path = "../App_Themes/DocSuite2008/imgset16/folder_internet_closed.png";
            this.folder_internet_open_path = "../App_Themes/DocSuite2008/imgset16/folder_internet_open.png";
            this.folder_auto_close_path = "../App_Themes/DocSuite2008/imgset16/folder_auto_closed.png";
            this.folder_auto_open_path = "../App_Themes/DocSuite2008/imgset16/folder_auto_open.png";
            this.tooltip_folder = "Cartella con sottocartelle";
            this.tooltip_internet_folder = "Cartella pubblica";
            this.tooltip_auto_folder = "Cartella di sottofascicolo";
            /**
            *---------------------------- Events ---------------------------
            */
            /**
            * Evento scatenato al click della toolbar delle cartelle
            */
            this.folderToolBar_ButtonClicked = function (sender, args) {
                var item = args.get_item();
                if (item) {
                    var rootNode = _this._treeFascicleFolders.get_nodes().getNode(0);
                    var currentSelectedNode = _this._treeFascicleFolders.get_selectedNode();
                    switch (item.get_value()) {
                        case "createFolder": {
                            var selectedNodeId = currentSelectedNode.get_value();
                            if (!selectedNodeId) {
                                selectedNodeId = "";
                            }
                            if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                                _this.setFascicleFolder(selectedNodeId);
                            }
                            var url = "../Fasc/FascicleFolderInserimento.aspx?Type=Fasc&idFascicleFolder=" + selectedNodeId + "&SessionUniqueKey=" + _this.pageId + "_" + selectedNodeId + "&DoNotUpdateDatabase=" + _this.doNotUpdateDatabase;
                            _this.openWindow(url, "managerCreateFolder", 480, 300);
                            break;
                        }
                        case "modifyFolder": {
                            var selectedNodeId = currentSelectedNode.get_value();
                            if (currentSelectedNode.get_attributes()) {
                                _this.setFascicleFolder(selectedNodeId);
                                var url = "../Fasc/FascicleFolderModifica.aspx?Type=Fasc&idFascicleFolder=" + selectedNodeId + "&SessionUniqueKey=" + _this.pageId + "_" + selectedNodeId;
                                _this.openWindow(url, "managerModifyFolder", 480, 300);
                            }
                            break;
                        }
                        case "moveFolder": {
                            var selectedNodeId = currentSelectedNode.get_value();
                            if (currentSelectedNode.get_attributes()) {
                                _this.setFascicleFolder(selectedNodeId);
                                var idFascicle = _this._treeFascicleFolders.get_nodes().getNode(0).get_value();
                                var dto = {};
                                dto.name = currentSelectedNode.get_text();
                                dto.uniqueId = selectedNodeId;
                                sessionStorage.setItem(FascMoveItems.FASC_MOVE_ITEMS_Session_key, JSON.stringify([dto]));
                                var url = "FascMoveItems.aspx?Type=Fasc&idFascicle=" + idFascicle + "&ItemsType=FolderType&IdFascicleFolder=" + selectedNodeId;
                                _this.openWindow(url, "managerMoveFolder", 480, 300);
                            }
                            break;
                        }
                        case "deleteFolder": {
                            var hasChildren = currentSelectedNode.get_attributes().getAttribute("hasChildren");
                            var hasDocuments = currentSelectedNode.get_attributes().getAttribute("hasDocuments");
                            var level = currentSelectedNode.get_attributes().getAttribute("fascicleFolderLevel");
                            var hasAttribute = false;
                            if (currentSelectedNode.get_attributes().getAttribute("idCategory")) {
                                hasAttribute = true;
                            }
                            if (currentSelectedNode.get_attributes() && !hasAttribute && !hasChildren && !hasDocuments && level > 2) {
                                _this.removeFascicleFolder();
                            }
                            else {
                                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    var errorMessage = "Impossibile eliminare la cartella " + currentSelectedNode.get_text() + " per le seguenti motivazioni: ";
                                    if (hasChildren) {
                                        errorMessage = errorMessage.concat("La cartella contiene sotto strutture che necessariamente vanno rimosse, ");
                                    }
                                    if (hasDocuments) {
                                        errorMessage = errorMessage.concat("La cartella ha documenti associati che necessariamente vanno rimossi, ");
                                    }
                                    if (hasAttribute) {
                                        errorMessage = errorMessage.concat("La cartella 'sotto fascicolo' è stata configurata in automatico dal sistema durante la creazione del fascicolo, ");
                                    }
                                    if (level < 3) {
                                        errorMessage = errorMessage.concat("La cartella 'principale' è stata configurata in automatico dal sistema durante la creazione del fascicolo, ");
                                    }
                                    uscNotification.showWarningMessage(errorMessage);
                                }
                            }
                            break;
                        }
                    }
                }
            };
            /**
            * Evento scatenato all'espansione di un nodo
            * @param sender
            * @param args
            */
            this.treeView_ClientNodeExpanding = function (sender, args) {
                if (!args.get_node()) {
                    return;
                }
                var node = args.get_node();
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                if (_this.doNotUpdateDatabase === "False") {
                    _this._fascicleFolderService.getChildren(node.get_value(), function (data) {
                        _this.loadNodes(data, node);
                    }, function (exception) {
                        var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    });
                }
            };
            /**
        * Evento al click del pulsante per la espandere o comprimere la gliglia delle UD presenti nel fascicolo
        */
            this.btnExpandFascicleFolders_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this.changeVisibilityFascicleFolders(_this._isPnlFascicleFolderOpen);
                $("#".concat(_this.pageId)).triggerHandler(uscFascicleFolders.RESIZE_EVENT);
            };
            /**
            * Metodo chiamato in chiusura della radwindow di modifica di cartella
            * @param sender
            * @param args
            */
            this.closeModifyWindow = function (sender, args) {
                if (args.get_argument) {
                    var result = args.get_argument();
                    var updatedNode = _this._treeFascicleFolders.get_selectedNode();
                    if (result && result.Value[0]) {
                        var fascicleFolderModel = JSON.parse(result.Value[0]);
                        _this.setNodeAttribute(updatedNode, fascicleFolderModel);
                        _this.setVisibilityButtonsByStatus();
                        _this._treeFascicleFolders.commitChanges();
                    }
                }
            };
            /**
            * Metono chiamato in chiusura della radwindow di inserimento
            * @param sender
            * @param args
            */
            this.closeFolderInsertWindow = function (sender, args) {
                if (args.get_argument) {
                    var result = void 0;
                    if (_this.doNotUpdateDatabase === "True") {
                        result = JSON.parse(sessionStorage.getItem("InsertedFascicleFolder"));
                    }
                    else {
                        result = args.get_argument();
                    }
                    _this.addNewFolder(result);
                }
            };
            this.closeMoveWindow = function (sender, args) {
                if (args.get_argument()) {
                    var fascicleId = _this._treeFascicleFolders.get_nodes().getNode(0).get_value();
                    _this.setRootNode(fascicleId);
                    _this.loadFolders(fascicleId);
                    _this.setVisibilityButtonsByStatus();
                }
            };
            /**
            * Evento scatenato al click di un nodo
            * @param sender
            * @param eventArgs
            */
            this.treeView_ClientNodeClicked = function (sender, eventArgs) {
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                _this.setSelectedNode(eventArgs.get_node());
            };
            /*
            * Eliminazione di una cartellina
            */
            this.removeFascicleFolder = function () {
                _this._manager.radconfirm("Sei sicuro di voler eliminare la cartella?", function (arg) {
                    if (arg) {
                        var fascicleFolder_1 = {};
                        var currentSelectedNode = _this._treeFascicleFolders.get_selectedNode();
                        fascicleFolder_1.UniqueId = currentSelectedNode.get_value();
                        fascicleFolder_1.Name = currentSelectedNode.get_text();
                        var parentNode = currentSelectedNode.get_parent();
                        if (parentNode && parentNode.get_value() != _this.currentFascicleId) {
                            fascicleFolder_1.ParentInsertId = parentNode.get_value();
                        }
                        if (_this.doNotUpdateDatabase === "False") {
                            _this._fascicleFolderService.deleteFascicleFolder(fascicleFolder_1, function (data) {
                                _this.removeNode(fascicleFolder_1.UniqueId);
                                _this.hiddenInputs();
                            }, function (exception) {
                                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    uscNotification.showNotification(exception);
                                }
                            });
                        }
                        else {
                            _this.removeNode(fascicleFolder_1.UniqueId);
                            _this.hiddenInputs();
                        }
                    }
                    document.getElementsByTagName("body")[0].setAttribute("class", "comm chrome");
                }, 400, 300);
            };
            /**
             * Metodo che nasconde il loading
             */
            this.hideLoadingPanel = function () {
                _this._loadingPanel.hide(_this.pageId);
            };
            this.showLoadingPanel = function () {
                _this._loadingPanel.show(_this.pageId);
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
        *---------------------------- Methods ---------------------------
        */
        /**
       * Inizializzazione
       */
        uscFascicleFolders.prototype.initialize = function () {
            if (!this.isVisible) {
                return;
            }
            this._ajaxManager = $find(this.ajaxManagerId);
            this._treeFascicleFolders = $find(this.treeFascicleFoldersId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._folderToolBar = $find(this.folderToolBarId);
            this._folderToolBar.add_buttonClicked(this.folderToolBar_ButtonClicked);
            this._managerCreateFolder = $find(this.managerCreateFolderId);
            this._managerModifyFolder = $find(this.managerModifyFolderId);
            this._managerMoveFolder = $find(this.managerMoveFolderId);
            this._managerMoveFolder.add_close(this.closeMoveWindow);
            this._manager = $find(this.managerId);
            this._loadingPanel.show(this.pageId);
            this._btnCreateFolder = this._folderToolBar.findItemByValue("createFolder");
            this._btnDeleteFolder = this._folderToolBar.findItemByValue("deleteFolder");
            this._btnModifyFolder = this._folderToolBar.findItemByValue("modifyFolder");
            this._btnMoveFolder = this._folderToolBar.findItemByValue("moveFolder");
            this._managerModifyFolder.add_close(this.closeModifyWindow);
            this._checkedToolBarButtons = 0;
            this._managerCreateFolder.add_close(this.closeFolderInsertWindow);
            this._pnlFascicleFolder = $("#".concat(this.pnlFascicleFolderId));
            this._btnExpandFascicleFolders = $find(this.btnExpandFascicleFoldersId);
            this._btnExpandFascicleFolders.addCssClass("dsw-arrow-down");
            this._pnlFascicleFolder.show();
            this._btnExpandFascicleFolders.add_clicking(this.btnExpandFascicleFolders_OnClick);
            this._isPnlFascicleFolderOpen = true;
            if (this.viewOnlyFolders) {
                this._pnlFascicleFolder.css("margin-top", "0");
            }
            var fascicleFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleConfiguration);
            this.bindLoaded();
        };
        uscFascicleFolders.prototype.changeVisibilityFascicleFolders = function (param) {
            if (!param) {
                this._isPnlFascicleFolderOpen = false;
            }
            else {
                this._isPnlFascicleFolderOpen = true;
            }
        };
        /**
        * Carico le cartelle
        */
        uscFascicleFolders.prototype.loadFolders = function (idFascicle) {
            var _this = this;
            var promise = $.Deferred();
            this._fascicleFolderService.getChildren(idFascicle, function (data) {
                _this.loadNodes(data);
                _this._loadingPanel.hide(_this.pageId);
                promise.resolve();
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageId);
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
                promise.reject(exception);
            });
            return promise.promise();
        };
        uscFascicleFolders.prototype.setManageFascicleFolderVisibility = function (right) {
            this._treeFascicleFolders.get_attributes().setAttribute("treeViewReadOnly", right.toString());
            this._folderToolBar.set_enabled(right);
        };
        uscFascicleFolders.prototype.setCloseAttributeFascicleFolder = function () {
            this._treeFascicleFolders.get_attributes().setAttribute("isClosed", "false");
            this._folderToolBar.set_enabled(false);
        };
        /**
        * Imposta gli attributi di un nodo
        * @param node
        * @param dossierFolder
        */
        uscFascicleFolders.prototype.setNodeAttribute = function (node, fascicleFolder) {
            node.get_attributes().setAttribute("Typology", fascicleFolder.Typology);
            node.set_text(fascicleFolder.Name);
            node.set_value(fascicleFolder.UniqueId);
            node.get_attributes().setAttribute("idParent", null);
            if (node.get_parent()) {
                var parentNode = node.get_parent();
                node.get_attributes().setAttribute("idParent", parentNode.get_value());
            }
            node.get_attributes().setAttribute("idFascicle", null);
            if (fascicleFolder.idFascicle) {
                node.get_attributes().setAttribute("idFascicle", fascicleFolder.idFascicle);
            }
            node.get_attributes().setAttribute("idCategory", null);
            if (fascicleFolder.idCategory) {
                node.get_attributes().setAttribute("idCategory", fascicleFolder.idCategory);
            }
            node.get_attributes().setAttribute("hasDocuments", null);
            node.get_attributes().setAttribute("hasDocuments", fascicleFolder.hasDocuments);
            node.get_attributes().setAttribute("hasChildren", null);
            node.get_attributes().setAttribute("hasChildren", fascicleFolder.hasChildren);
            node.get_attributes().setAttribute("fascicleFolderLevel", null);
            node.get_attributes().setAttribute("fascicleFolderLevel", fascicleFolder.FascicleFolderLevel);
            node.set_imageUrl(this.folder_close_path);
            //qui scelgo l'immagine da visualizzare per il nodo
            if (FascicleFolderTypology[fascicleFolder.Typology] == FascicleFolderTypology.Fascicle) {
                node.set_toolTip("Fascicolo ");
                node.get_imageElement().title = "Fascicolo";
            }
            if (fascicleFolder.hasChildren) {
                node.set_imageUrl(this.folder_close_path);
                node.set_expandedImageUrl(this.folder_open_path);
                node.set_toolTip(this.tooltip_folder);
                node.get_imageElement().title = this.tooltip_folder;
                var nodeToAdd = new Telerik.Web.UI.RadTreeNode();
                node.get_nodes().add(nodeToAdd);
                node.set_expanded(false);
            }
            if (FascicleFolderStatus[fascicleFolder.Status] == FascicleFolderStatus.Internet) {
                node.set_imageUrl(this.folder_internet_close_path);
                node.set_expandedImageUrl(this.folder_internet_open_path);
                node.set_toolTip(this.tooltip_internet_folder);
                node.get_imageElement().title = this.tooltip_internet_folder;
            }
            if ((fascicleFolder.idCategory != null) && (FascicleFolderTypology[fascicleFolder.Typology] == FascicleFolderTypology.SubFascicle)) {
                node.set_imageUrl(this.folder_auto_close_path);
                node.set_expandedImageUrl(this.folder_auto_open_path);
                node.set_toolTip(this.tooltip_auto_folder);
                node.get_imageElement().title = this.tooltip_auto_folder;
            }
            return node;
        };
        uscFascicleFolders.prototype.selectFascicleNode = function () {
            var fascicleNode = this._treeFascicleFolders.get_nodes().getNode(0).get_nodes().getNode(0);
            this.setSelectedNode(fascicleNode);
        };
        uscFascicleFolders.prototype.setSelectedNode = function (node) {
            var idFascicle = this._treeFascicleFolders.get_nodes().getNode(0).get_value();
            this.currentFascicleId = node.get_value();
            node.set_selected(true);
            this.setFascicleFolder(idFascicle);
            this.setVisibilityButtonsByStatus();
        };
        uscFascicleFolders.prototype.setRootNode = function (fascicleId, nodeText) {
            var rootNode = this._treeFascicleFolders.get_nodes().getNode(0);
            if (nodeText) {
                rootNode.set_text(nodeText);
            }
            else {
                rootNode.set_text("Tutti i documenti");
            }
            rootNode.set_value(fascicleId);
            rootNode.set_expanded(true);
            rootNode.set_selected(true);
            this._treeFascicleFolders.commitChanges();
            this.hiddenInputs();
            if (fascicleId !== "") {
                this.currentFascicleId = fascicleId;
                this.setFascicleFolder(fascicleId);
            }
        };
        uscFascicleFolders.prototype.setButtonVisibility = function (isManager) {
            if (!isManager) {
                $(this._folderToolBar.get_element()).hide();
            }
        };
        /**
        * Carica i dati dello user control
        */
        uscFascicleFolders.prototype.loadNodes = function (fascicleFolders, node) {
            var _this = this;
            if (fascicleFolders == null)
                return;
            var parentSelectedNode;
            if (node) {
                parentSelectedNode = node;
            }
            else {
                parentSelectedNode = this._treeFascicleFolders.get_nodes().getNode(0);
            }
            parentSelectedNode.get_nodes().clear();
            var newNode;
            $.each(fascicleFolders, function (index, fascicleFolder) {
                if (_this._treeFascicleFolders.findNodeByValue(fascicleFolder.UniqueId) != undefined) {
                    return;
                }
                newNode = new Telerik.Web.UI.RadTreeNode();
                parentSelectedNode.get_nodes().add(newNode);
                _this.setNodeAttribute(newNode, fascicleFolder);
                if (_this.foldersToDisabled && _this.foldersToDisabled.some(function (s) { return s == fascicleFolder.UniqueId; })) {
                    newNode.set_cssClass(newNode.get_cssClass() + " rtDisabled");
                }
            });
            this._treeFascicleFolders.commitChanges();
            $("#".concat(this.treeFascicleFoldersId)).triggerHandler(uscFascicleFolders.ON_END_LOAD_EVENT);
            this._loadingPanel.hide(this.pageId);
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscFascicleFolders.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.LOADED_EVENT);
        };
        uscFascicleFolders.prototype.addNewFolder = function (ajaxModel) {
            var _this = this;
            var parentNode = this._treeFascicleFolders.get_selectedNode();
            if (ajaxModel) {
                var fascicleFolderModel = {};
                fascicleFolderModel = JSON.parse(ajaxModel.Value[0]);
                if (this.doNotUpdateDatabase === "True") {
                    this._btnCreateFolder.set_enabled(parentNode.get_level() > 0);
                }
                else {
                    this._btnCreateFolder.set_enabled(true);
                }
                this._btnDeleteFolder.set_enabled(false);
                this._btnModifyFolder.set_enabled(false);
                this._btnMoveFolder.set_enabled(false);
                if (parentNode != this._treeFascicleFolders.get_nodes().getNode(0)) {
                    this._btnModifyFolder.set_enabled(false);
                    var attributeStatus = parentNode.get_attributes().getAttribute("Typology");
                    this.typology = FascicleFolderTypology[attributeStatus];
                    var attributeChildren = parentNode.get_attributes().getAttribute("hasChildren");
                    if (!attributeChildren) {
                        parentNode.get_attributes().setAttribute("hasChildren", true);
                    }
                    parentNode.set_imageUrl(this.folder_close_path);
                    parentNode.set_expandedImageUrl(this.folder_open_path);
                    parentNode.set_toolTip(this.tooltip_folder);
                    parentNode.get_imageElement().title = this.tooltip_folder;
                    parentNode.set_selected(true);
                }
                if (parentNode.get_expanded() == false && this.doNotUpdateDatabase === "False") {
                    this._fascicleFolderService.getChildren(parentNode.get_value(), function (data) {
                        _this.loadNodes(data, parentNode);
                        parentNode.set_expanded(true);
                        _this._treeFascicleFolders.commitChanges();
                    }, function (exception) {
                        var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                        if (!jQuery.isEmptyObject(uscNotification)) {
                            uscNotification.showNotification(exception);
                        }
                    });
                }
                else {
                    var nodeToAdd = new Telerik.Web.UI.RadTreeNode();
                    parentNode.get_nodes().add(nodeToAdd);
                    this.setNodeAttribute(nodeToAdd, fascicleFolderModel);
                    parentNode.set_expanded(true);
                    this._treeFascicleFolders.commitChanges();
                }
            }
        };
        uscFascicleFolders.prototype.hiddenInputs = function () {
            this._btnCreateFolder.set_enabled(false);
            this._btnDeleteFolder.set_enabled(false);
            this._btnModifyFolder.set_enabled(false);
            this._btnMoveFolder.set_enabled(false);
        };
        uscFascicleFolders.prototype.removeNode = function (idFascicleFolder) {
            var nodeToRemove = this._treeFascicleFolders.findNodeByValue(idFascicleFolder);
            if (nodeToRemove) {
                var parentNode = nodeToRemove.get_parent();
                if (parentNode && parentNode.get_nodes()) {
                    parentNode.get_nodes().remove(nodeToRemove);
                    if (parentNode != this._treeFascicleFolders.get_nodes().getNode(0) && parentNode.get_nodes().get_count() == 0) {
                        parentNode.get_attributes().setAttribute("hasChildren", false);
                        parentNode.set_imageUrl(this.folder_open_path);
                    }
                    this._treeFascicleFolders.commitChanges();
                }
            }
        };
        /**
        * Caricamento della cartella selezionata nella Session Storage
        * @param currentFascicleId
        */
        uscFascicleFolders.prototype.setFascicleFolder = function (currentFascicleId) {
            var currentSelectedNode = this._treeFascicleFolders.get_selectedNode();
            if (!currentSelectedNode || !currentSelectedNode.get_value()) {
                sessionStorage.removeItem(this.pageId + "_" + currentFascicleId);
                return;
            }
            var fascicleFolder = {};
            fascicleFolder.UniqueId = currentSelectedNode.get_value();
            fascicleFolder.Name = currentSelectedNode.get_text();
            if (currentSelectedNode.get_attributes().getAttribute("idCategory")) {
                fascicleFolder.idCategory = currentSelectedNode.get_attributes().getAttribute("idCategory");
            }
            if (currentSelectedNode.get_attributes().getAttribute("Typology")) {
                fascicleFolder.Typology = currentSelectedNode.get_attributes().getAttribute("Typology");
            }
            if (currentSelectedNode.get_attributes().getAttribute("idParent")) {
                fascicleFolder.idParent = currentSelectedNode.get_attributes().getAttribute("idParent");
            }
            if (currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                fascicleFolder.idFascicle = currentSelectedNode.get_attributes().getAttribute("idFascicle");
            }
            sessionStorage[this.pageId + "_" + currentFascicleId] = JSON.stringify(fascicleFolder);
        };
        uscFascicleFolders.prototype.getSelectedFascicleFolder = function (currentFascicleId) {
            var selectedFolder = sessionStorage.getItem(this.pageId + "_" + currentFascicleId);
            if (!selectedFolder) {
                return undefined;
            }
            return JSON.parse(selectedFolder);
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param url
        * @param name
        * @param width
        * @param height
        */
        uscFascicleFolders.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        uscFascicleFolders.prototype.setVisibilityButtonsByStatus = function () {
            var isClosed = false;
            var currentSelectedNode = this._treeFascicleFolders.get_selectedNode();
            if (this._treeFascicleFolders.get_attributes().getAttribute("isClosed")) {
                isClosed = (this._treeFascicleFolders.get_attributes().getAttribute("isClosed").toLowerCase() === "true");
            }
            if (isClosed || currentSelectedNode == this._treeFascicleFolders.get_nodes().getNode(0)) {
                this.hiddenInputs();
                $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.ROOT_NODE_CLICK);
                return;
            }
            var attributeStatus = currentSelectedNode.get_attributes().getAttribute("Typology");
            this.typology = FascicleFolderTypology[attributeStatus];
            var hasChildren = currentSelectedNode.get_attributes().getAttribute("hasChildren");
            var hasDocuments = currentSelectedNode.get_attributes().getAttribute("hasDocuments");
            var hasCategory = currentSelectedNode.get_attributes().getAttribute("idCategory");
            var level = currentSelectedNode.get_attributes().getAttribute("fascicleFolderLevel");
            if (this.typology && isNaN(this.typology)) {
                this.typology = FascicleFolderTypology[this.typology.toString()];
            }
            var manageable = false;
            if (this._treeFascicleFolders.get_attributes().getAttribute("treeViewReadOnly")) {
                manageable = (this._treeFascicleFolders.get_attributes().getAttribute("treeViewReadOnly").toLowerCase() === "true");
            }
            switch (this.typology) {
                case FascicleFolderTypology.Fascicle: {
                    this._btnCreateFolder.set_enabled(manageable);
                    this._btnDeleteFolder.set_enabled(!(hasCategory || hasDocuments || hasChildren || level < 3) && manageable);
                    this._btnModifyFolder.set_enabled(false);
                    this._btnMoveFolder.set_enabled(false);
                    $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.FASCICLE_TREE_NODE_CLICK);
                    break;
                }
                case FascicleFolderTypology.SubFascicle: {
                    this._btnCreateFolder.set_enabled(manageable);
                    this._btnDeleteFolder.set_enabled(!(hasCategory || hasDocuments || hasChildren || level < 3) && manageable);
                    this._btnModifyFolder.set_enabled(!(hasCategory || level < 3) && manageable);
                    this._btnMoveFolder.set_enabled(manageable);
                    $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK);
                    break;
                }
            }
        };
        uscFascicleFolders.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        uscFascicleFolders.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscFascicleFolders.prototype.getFascicleFolderTree = function () {
            return this._treeFascicleFolders;
        };
        uscFascicleFolders.ON_END_LOAD_EVENT = "onEndLoad";
        uscFascicleFolders.LOADED_EVENT = "onLoaded";
        uscFascicleFolders.FASCICLE_TREE_NODE_CLICK = "onFascicleTreeNodeClick";
        uscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK = "onSubFascicleTreeNodeClick";
        uscFascicleFolders.ROOT_NODE_CLICK = "onRootTreeNodeClick";
        uscFascicleFolders.RESIZE_EVENT = "onResize";
        return uscFascicleFolders;
    }());
    return uscFascicleFolders;
});
//# sourceMappingURL=uscFascicleFolders.js.map