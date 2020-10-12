/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleFolderStatus", "App/Models/Fascicles/FascicleFolderTypology", "App/Services/Fascicles/FascicleFolderService", "App/DTOs/ExceptionDTO", "App/Services/Fascicles/FascicleService", "App/Services/Fascicles/FascicleDocumentService", "App/Models/DocumentUnits/ChainType", "App/Helpers/GuidHelper", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, ServiceConfigurationHelper, FascicleFolderStatus, FascicleFolderTypology, FascicleFolderService, ExceptionDTO, FascicleService, FascicleDocumentService, ChainType, Guid, SessionStorageKeysHelper) {
    var uscFascicleFolders = /** @class */ (function () {
        /*
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
            this.SESSION_FascicleId = "FascicleId_Tree";
            this.TYPOLOGY_ATTRIBUTE = "Typology";
            this.HASCHILDREN_ATTRIBUTE = "hasChildren";
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
                                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASC_MOVE_ITEMS, JSON.stringify([dto]));
                                var url = "FascMoveItems.aspx?Type=Fasc&idFascicle=" + idFascicle + "&ItemsType=FolderType&IdFascicleFolder=" + selectedNodeId;
                                _this.openWindow(url, "managerMoveFolder", 480, 300);
                            }
                            break;
                        }
                        case "deleteFolder": {
                            var hasChildren = currentSelectedNode.get_attributes().getAttribute(_this.HASCHILDREN_ATTRIBUTE);
                            var hasDocuments = currentSelectedNode.get_attributes().getAttribute("hasDocuments");
                            var level = currentSelectedNode.get_level() + 1;
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
                        case uscFascicleFolders.REFRESH_FolderHierarchy: {
                            sessionStorage.removeItem(_this.SESSION_FascicleHierarchy);
                            _this.setRootNode(sessionStorage.getItem(_this.SESSION_FascicleId));
                            _this.loadFolders(sessionStorage.getItem(_this.SESSION_FascicleId))
                                .done(function () { return _this.selectFascicleNode(); });
                            break;
                        }
                        case uscFascicleFolders.UPLOAD_file: {
                            var url = "../UserControl/CommonUploadDocument.aspx";
                            _this.openWindow(url, "managerUploadFile", 480, 300, _this.closeDocumentWnd);
                            break;
                        }
                        case uscFascicleFolders.SCANNER: {
                            sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_COMPONENT_SCANNER);
                            var url = "";
                            if (_this.scannerLightRestEnabled.toLocaleLowerCase() == "true") {
                                url = "../UserControl/ScannerRest.aspx?&multipleEnabled=True";
                            }
                            _this.openWindow(url, "managerScannerDocument", 800, 500, _this.closeScannerWnd);
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
                node.showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
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
                if (args.get_argument()) {
                    var result = args.get_argument();
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
            this.treeView_ClientNodeClicking = function (sender, eventArgs) {
                var isSelectable = eventArgs.get_node().get_attributes().getAttribute("selectable");
                if (isSelectable !== undefined && isSelectable === false) {
                    eventArgs.set_cancel(true);
                    return;
                }
            };
            /**
            * Evento scatenato al click di un nodo
            * @param sender
            * @param eventArgs
            */
            this.treeView_ClientNodeClicked = function (sender, eventArgs) {
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                if (sessionStorage.getItem(_this.SESSION_FascicleHierarchy) == null ||
                    sessionStorage.getItem(_this.SESSION_FascicleHierarchy) == "[]") {
                    _this.setSelectedNode(eventArgs.get_node());
                }
                else {
                    _this.createModelInSession();
                    var idFascicle = _this._treeFascicleFolders.get_nodes().getNode(0).get_value();
                    _this.setFascicleFolder(idFascicle);
                    _this.setVisibilityButtonsByStatus();
                }
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
                        _this.setSelectedNode(parentNode);
                    }
                }, 400, 300);
            };
            this.closeDocumentWnd = function (sender, args) {
                sender.remove_close(_this.closeDocumentWnd);
                _this.chainId = _this._treeFascicleFolders.get_selectedNode().get_attributes().getAttribute(uscFascicleFolders.CHAIN_ID);
                var model = {};
                model.ActionName = "Upload_document";
                if (args.get_argument() !== null) {
                    var argument = args.get_argument();
                    model.Value = [argument, _this.chainId];
                    _this._ajaxManager.ajaxRequest(JSON.stringify(model));
                }
            };
            this.closeScannerWnd = function (sender, args) {
                sender.remove_close(_this.closeScannerWnd);
                _this.chainId = _this._treeFascicleFolders.get_selectedNode().get_attributes().getAttribute(uscFascicleFolders.CHAIN_ID);
                var documents = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_COMPONENT_SCANNER);
                var model = {};
                model.ActionName = uscFascicleFolders.SCAN_DOCUMENT;
                if (documents) {
                    model.Value = [documents, _this.chainId];
                    _this._ajaxManager.ajaxRequest(JSON.stringify(model));
                }
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
            var fascicleDocumentConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
            this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);
            this.SESSION_FascicleHierarchy = "FascicleHierarchy_" + this.pageId;
            var fascicleFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleFolder");
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleConfiguration);
            this.clearSessionStorageCache();
            this.bindLoaded();
        };
        uscFascicleFolders.prototype.clearSessionStorageCache = function () {
            if (sessionStorage.getItem(this.SESSION_FascicleHierarchy)) {
                sessionStorage.removeItem(this.SESSION_FascicleHierarchy);
            }
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
        uscFascicleFolders.prototype.fileManagementButtonsVisibility = function (right) {
            if (right && this.scannerLightRestEnabled.toLocaleLowerCase() == "true") {
                this._folderToolBar.findItemByValue(uscFascicleFolders.SCANNER).set_visible(right);
            }
            else {
                this._folderToolBar.findItemByValue(uscFascicleFolders.SCANNER).set_visible(false);
            }
            this._folderToolBar.findItemByValue(uscFascicleFolders.UPLOAD_file).set_visible(right);
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
            node.get_attributes().setAttribute(this.TYPOLOGY_ATTRIBUTE, fascicleFolder.Typology);
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
            node.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, null);
            node.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, fascicleFolder.hasChildren);
            node.get_attributes().setAttribute("fascicleFolderLevel", null);
            node.get_attributes().setAttribute("fascicleFolderLevel", fascicleFolder.FascicleFolderLevel);
            if (!fascicleFolder.FascicleDocuments) {
                node.get_attributes().setAttribute(uscFascicleFolders.CHAIN_ID, Guid.empty);
            }
            else {
                var inserts = $.grep(fascicleFolder.FascicleDocuments, function (x) { return x.ChainType.toString() == ChainType[ChainType.Miscellanea]; })[0];
                if (inserts != undefined) {
                    node.get_attributes().setAttribute(uscFascicleFolders.CHAIN_ID, inserts.IdArchiveChain);
                }
            }
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
        uscFascicleFolders.prototype.selectFascicleNode = function (triggerHandler) {
            if (triggerHandler === void 0) { triggerHandler = true; }
            var fascicleNode = this._treeFascicleFolders.get_nodes().getNode(0).get_nodes().getNode(0);
            this.setSelectedNode(fascicleNode, triggerHandler);
        };
        uscFascicleFolders.prototype.setSelectedNode = function (node, triggerHandler) {
            if (triggerHandler === void 0) { triggerHandler = true; }
            var idFascicle = this._treeFascicleFolders.get_nodes().getNode(0).get_value();
            this.currentFascicleId = node.get_value();
            node.set_selected(true);
            this.setFascicleFolder(idFascicle);
            this.setVisibilityButtonsByStatus(triggerHandler);
            this.createModelInSession();
        };
        uscFascicleFolders.prototype.createTreeFromSession = function () {
            this.fascicleFoldersModel = JSON.parse(sessionStorage.getItem(this.SESSION_FascicleHierarchy));
            this.fascicleFoldersModel = this.fascicleFoldersModel.reverse();
            this._treeFascicleFolders.get_nodes().getNode(0).expand();
        };
        uscFascicleFolders.prototype.createModelInSession = function () {
            var fascicleFolderParent = [];
            this.getParentsById(this._treeFascicleFolders.get_selectedNode(), fascicleFolderParent);
            sessionStorage.setItem(this.SESSION_FascicleHierarchy, JSON.stringify(fascicleFolderParent));
        };
        uscFascicleFolders.prototype.getParentsById = function (node, parents) {
            if (node.get_level() > 0) {
                var fascFolder = {
                    Name: node.get_text(),
                    UniqueId: node.get_value(),
                    Typology: node.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE),
                    hasChildren: node.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE)
                };
                parents.push(fascFolder);
                this.getParentsById(node.get_parent(), parents);
            }
        };
        uscFascicleFolders.prototype.setRootNode = function (fascicleId, nodeText, setAsSelected) {
            if (setAsSelected === void 0) { setAsSelected = true; }
            var rootNode = this._treeFascicleFolders.get_nodes().getNode(0);
            if (nodeText) {
                rootNode.set_text(nodeText);
            }
            else {
                rootNode.set_text("Tutti i documenti");
            }
            rootNode.set_value(fascicleId);
            rootNode.set_expanded(true);
            rootNode.set_selected(setAsSelected);
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
            var idFascicle = parentSelectedNode.get_value();
            var lastSelectedFolder = this.getSelectedFascicleFolder(idFascicle);
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
                if (lastSelectedFolder && lastSelectedFolder.UniqueId === fascicleFolder.UniqueId) {
                    newNode.set_selected(true);
                }
            });
            this._treeFascicleFolders.commitChanges();
            $("#".concat(this.treeFascicleFoldersId)).triggerHandler(uscFascicleFolders.ON_END_LOAD_EVENT);
            this._loadingPanel.hide(this.pageId);
            parentSelectedNode.set_expanded(true);
            parentSelectedNode.hideLoadingStatus();
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
                    var attributeStatus = parentNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE);
                    this.typology = FascicleFolderTypology[attributeStatus];
                    var attributeChildren = parentNode.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE);
                    if (!attributeChildren) {
                        parentNode.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, true);
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
                        parentNode.get_attributes().setAttribute(this.HASCHILDREN_ATTRIBUTE, false);
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
            if (currentSelectedNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE)) {
                fascicleFolder.Typology = currentSelectedNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE);
            }
            if (currentSelectedNode.get_attributes().getAttribute("idParent")) {
                fascicleFolder.idParent = currentSelectedNode.get_attributes().getAttribute("idParent");
            }
            if (currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                fascicleFolder.idFascicle = currentSelectedNode.get_attributes().getAttribute("idFascicle");
            }
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_SELECTED_FASCICLE_FOLDER_ID, JSON.stringify(fascicleFolder));
            sessionStorage[this.pageId + "_" + currentFascicleId] = JSON.stringify(fascicleFolder);
        };
        uscFascicleFolders.prototype.getSelectedFascicleFolder = function (currentFascicleId) {
            if (sessionStorage.getItem(this.SESSION_FascicleHierarchy) == null ||
                sessionStorage.getItem(this.SESSION_FascicleHierarchy) == "[]") {
                var selectedFolder = sessionStorage.getItem(this.pageId + "_" + currentFascicleId);
                if (!selectedFolder) {
                    return undefined;
                }
                return JSON.parse(selectedFolder);
            }
            else {
                var selectedFolderFromSession = JSON.parse(sessionStorage.getItem(this.SESSION_FascicleHierarchy));
                return selectedFolderFromSession[0];
            }
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param url
        * @param name
        * @param width
        * @param height
        */
        uscFascicleFolders.prototype.openWindow = function (url, name, width, height, oncloseCallback) {
            if (oncloseCallback === void 0) { oncloseCallback = null; }
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            if (oncloseCallback) {
                wnd.add_close(oncloseCallback);
            }
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        uscFascicleFolders.prototype.bindMiscellanea = function (chainId) {
            var _this = this;
            if (this.chainId == Guid.empty || this.chainId == undefined) {
                var fascicleDocumentModel = {};
                fascicleDocumentModel.ChainType = ChainType.Miscellanea;
                fascicleDocumentModel.IdArchiveChain = chainId;
                fascicleDocumentModel.Fascicle = { UniqueId: this._treeFascicleFolders.get_nodes().getNode(0).get_value() };
                fascicleDocumentModel.FascicleFolder = {};
                fascicleDocumentModel.FascicleFolder.UniqueId = this._treeFascicleFolders.get_selectedNode().get_value();
                this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel, function (data) {
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            $("#" + this.pageId).triggerHandler(uscFascicleFolders.REFRESH_GRID_UPLOAD_DOCUMENTS);
        };
        uscFascicleFolders.prototype.setVisibilityButtonsByStatus = function (triggerHandler) {
            if (triggerHandler === void 0) { triggerHandler = true; }
            var isClosed = false;
            var currentSelectedNode = this._treeFascicleFolders.get_selectedNode();
            if (this._treeFascicleFolders.get_attributes().getAttribute("isClosed")) {
                isClosed = (this._treeFascicleFolders.get_attributes().getAttribute("isClosed").toLowerCase() === "true");
            }
            if (isClosed || currentSelectedNode == this._treeFascicleFolders.get_nodes().getNode(0)) {
                this.hiddenInputs();
                if (triggerHandler) {
                    $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.ROOT_NODE_CLICK);
                }
                return;
            }
            var attributeStatus = currentSelectedNode.get_attributes().getAttribute(this.TYPOLOGY_ATTRIBUTE);
            this.typology = FascicleFolderTypology[attributeStatus];
            var hasChildren = currentSelectedNode.get_attributes().getAttribute(this.HASCHILDREN_ATTRIBUTE);
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
                    if (triggerHandler) {
                        $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.FASCICLE_TREE_NODE_CLICK);
                    }
                    break;
                }
                case FascicleFolderTypology.SubFascicle: {
                    this._btnCreateFolder.set_enabled(manageable);
                    this._btnDeleteFolder.set_enabled(!(hasCategory || hasDocuments || hasChildren || level < 3) && manageable);
                    this._btnModifyFolder.set_enabled(!(hasCategory || level < 3) && manageable);
                    this._btnMoveFolder.set_enabled(manageable);
                    if (triggerHandler) {
                        $("#".concat(this.pageId)).triggerHandler(uscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK);
                    }
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
        uscFascicleFolders.prototype.rebuildTreeFromSession = function (fascicleId) {
            var _this = this;
            var promise = $.Deferred();
            sessionStorage.setItem(this.SESSION_FascicleId, fascicleId);
            this.createTreeFromSession();
            var _loop_1 = function (fascicleFolder) {
                $(document).queue(function (next) {
                    _this.populateTree(fascicleFolder)
                        .done(function () {
                        if (fascicleFolder.UniqueId == _this.fascicleFoldersModel[_this.fascicleFoldersModel.length - 1].UniqueId) {
                            var toSelectNode = _this._treeFascicleFolders.findNodeByValue(_this.fascicleFoldersModel[_this.fascicleFoldersModel.length - 1].UniqueId);
                            if (toSelectNode) {
                                _this.setSelectedNode(toSelectNode, false);
                            }
                            else {
                                _this.selectFascicleNode(false);
                            }
                        }
                        next();
                    })
                        .fail(function (exception) {
                        $(document).clearQueue();
                        promise.reject(exception);
                        return;
                    });
                });
            };
            for (var _i = 0, _a = this.fascicleFoldersModel; _i < _a.length; _i++) {
                var fascicleFolder = _a[_i];
                _loop_1(fascicleFolder);
            }
            $(document).queue(function (next) {
                _this._loadingPanel.hide(_this.pageId);
                promise.resolve();
                next();
            });
            return promise.promise();
        };
        uscFascicleFolders.prototype.populateTree = function (fascicleFolder) {
            var _this = this;
            var promise = $.Deferred();
            this._fascicleFolderService.getChildren(fascicleFolder.UniqueId, function (data) {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }
                var fascicleFolders = data;
                var parentNode = _this._treeFascicleFolders.findNodeByValue(fascicleFolder.UniqueId);
                parentNode.get_nodes().clear();
                _this.buildFascicleFolder(fascicleFolders, parentNode);
                parentNode.set_expanded(true);
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscFascicleFolders.prototype.buildFascicleFolder = function (data, parentNode) {
            this._treeFascicleFolders.trackChanges();
            var _loop_2 = function (i) {
                var node = new Telerik.Web.UI.RadTreeNode;
                node.set_text(data[i].Name);
                node.set_value(data[i].UniqueId);
                node.set_imageUrl(this_1.folder_close_path);
                parentNode.get_nodes().add(node);
                this_1.setNodeAttribute(node, data[i]);
                if (data[i].hasChildren && this_1.fascicleFoldersModel.some(function (f) { return f.Name == data[i].Name; })) {
                    node.set_expanded(true);
                    if (node.get_nodes().getNode(0).get_text() == "") {
                        node.get_nodes().remove(node.get_nodes().getNode(0));
                    }
                }
                else if (!data[i].hasChildren) {
                    node.set_expanded(false);
                }
                else {
                    node.set_expanded(false);
                }
            };
            var this_1 = this;
            for (var i = 0; i < data.length; i++) {
                _loop_2(i);
            }
            this._treeFascicleFolders.commitChanges();
        };
        uscFascicleFolders.ON_END_LOAD_EVENT = "onEndLoad";
        uscFascicleFolders.LOADED_EVENT = "onLoaded";
        uscFascicleFolders.FASCICLE_TREE_NODE_CLICK = "onFascicleTreeNodeClick";
        uscFascicleFolders.SUBFASCICLE_TREE_NODE_CLICK = "onSubFascicleTreeNodeClick";
        uscFascicleFolders.ROOT_NODE_CLICK = "onRootTreeNodeClick";
        uscFascicleFolders.RESIZE_EVENT = "onResize";
        uscFascicleFolders.REFRESH_GRID_EVENT = "onRefresh";
        uscFascicleFolders.REFRESH_GRID_UPLOAD_DOCUMENTS = "onGridUpdateDocument";
        uscFascicleFolders.REFRESH_FolderHierarchy = "refreshFolder";
        uscFascicleFolders.UPLOAD_file = "uploadFile";
        uscFascicleFolders.SCANNER = "scanner";
        uscFascicleFolders.SCAN_DOCUMENT = "Scan_document";
        uscFascicleFolders.CHAIN_ID = "chainId";
        return uscFascicleFolders;
    }());
    return uscFascicleFolders;
});
//# sourceMappingURL=uscFascicleFolders.js.map