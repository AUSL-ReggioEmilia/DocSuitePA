/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Models/Dossiers/DossierFolderStatus", "App/Services/Dossiers/DossierFolderService", "App/Services/Dossiers/DossierFolderLocalService", "App/DTOs/ExceptionDTO", "App/Mappers/Dossiers/DossierFolderSummaryModelMapper", "App/Models/UpdateActionType", "App/Services/Commons/RoleService", "App/Services/Commons/ContainerPropertyService", "App/Models/Commons/BuildActionType", "App/Services/Fascicles/FascicleService", "App/Models/Commons/CategoryModel", "App/Services/Commons/RoleLocalService", "App/Models/Commons/AuthorizationRoleType", "App/Models/Fascicles/FascicleRoleModel", "App/Services/Fascicles/FascicleRoleService"], function (require, exports, ServiceConfigurationHelper, DossierFolderStatus, DossierFolderService, DossierFolderLocalService, ExceptionDTO, DossierFolderSummaryModelMapper, UpdateActionType, RoleService, ContainerPropertyService, BuildActionType, FascicleService, CategoryModel, RoleLocalService, AuthorizationRoleType, FascicleRoleModel, FascicleRoleService) {
    var uscDossierFolders = /** @class */ (function () {
        /**
        * Costruttore
        * @param webApiConfiguration
        */
        function uscDossierFolders(serviceConfigurations) {
            var _this = this;
            this._dossierFolders = [];
            /**
            *---------------------------- Events ---------------------------
            */
            /**
            * Evento scatenato al click della toolbar delle cartelle
            */
            this.folderToolBar_ButtonClicked = function (sender, args) {
                var item = args.get_item();
                if (item) {
                    var rootNode = _this._treeDossierFolders.get_nodes().getNode(0);
                    switch (item.get_value()) {
                        case uscDossierFolders.CREATE_FOLDER: {
                            var selectedNodeId = _this._currentSelectedNode.get_value();
                            if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                                _this.setDossierFolder(rootNode.get_value());
                            }
                            var url = '../Dossiers/DossierFolderInserimento.aspx?Type=Dossier&idDossier='.concat(rootNode.get_value(), '&PersistanceDisabled=', _this.persistanceDisabled.toString());
                            _this.openWindow(url, "managerCreateFolder", 750, 600);
                            break;
                        }
                        case uscDossierFolders.CREATE_FASCICLE: {
                            var selectedNodeId = _this._currentSelectedNode.get_value();
                            if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                                _this.setDossierFolder(rootNode.get_value());
                            }
                            var url = '../Dossiers/DossierFascicleFolderInserimento.aspx?Type=Dossier&idDossier='.concat(rootNode.get_value(), '&PersistanceDisabled=', _this.persistanceDisabled.toString());
                            _this.openWindow(url, "managerCreateFascicleFolder", 750, 650);
                            break;
                        }
                        case uscDossierFolders.DELETE_FOLDER: {
                            if (_this._currentSelectedNode.get_attributes() && !_this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                                _this.removeDossierFolder();
                            }
                            else {
                                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    uscNotification.showWarningMessage("Impossibile eliminare una cartella con associato un fascicolo.");
                                }
                            }
                            break;
                        }
                        case uscDossierFolders.REMOVE_FASCICLE: {
                            if (_this._currentSelectedNode.get_attributes() && _this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                                _this.removeFascicleFromfolder();
                            }
                            else {
                                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    uscNotification.showWarningMessage("La cartella selezionata non ha alcun fascicolo associato.");
                                }
                            }
                            break;
                        }
                        case uscDossierFolders.ADD_FASCICLE: {
                            if (_this._currentSelectedNode.get_attributes() && !_this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                                _this.setDossierFolder(rootNode.get_value());
                                var url = '../Dossiers/DossierFolderLinkFascicle.aspx?Type=Dossier&idDossier='.concat(rootNode.get_value());
                                _this.openWindow(url, "managerFascicleLink", 750, 600);
                            }
                            else {
                                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    uscNotification.showWarningMessage("Impossibile associare un fascicolo ad una cartella che già contiene un fascicolo");
                                }
                            }
                            break;
                        }
                        case uscDossierFolders.MODIFY_FOLDER: {
                            if (_this._currentSelectedNode.get_attributes()) {
                                _this.setDossierFolder(rootNode.get_value());
                                var url = '../Dossiers/DossierFolderModifica.aspx?Type=Dossier&idDossier='.concat(rootNode.get_value(), '&PersistanceDisabled=', _this.persistanceDisabled.toString());
                                _this.openWindow(url, "managerModifyFolder", 750, 600);
                            }
                            break;
                        }
                        case uscDossierFolders.MODIFY_FASCICLE: {
                            var selectedNodeId = _this._currentSelectedNode.get_value();
                            if (selectedNodeId != "00000000-0000-0000-0000-000000000000") {
                                _this.setDossierFolder(rootNode.get_value());
                            }
                            var selectedNodeIdFascicle = _this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                            var url = "../Dossiers/DossierFascicleFolderInserimento.aspx?Type=Dossier&ActionType=Update&idDossier=" + rootNode.get_value() + "&idFascicle=" + selectedNodeIdFascicle + "&PersistanceDisabled=" + _this.persistanceDisabled.toString();
                            _this.openWindow(url, "managerModifyFascicle", 750, 650);
                            break;
                        }
                        case uscDossierFolders.REFRESH_DOSSIER_FOLDERS: {
                            _this._treeDossierFolders.get_nodes().getNode(0).collapse();
                            _this._treeDossierFolders.get_nodes().getNode(0).select();
                            $("#" + _this.pageId).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                        }
                        case uscDossierFolders.CREATE_FASCICLE_FROM_TEMPLATE: {
                            _this.createAndLinkFascicle(_this._templateDossierFolder.UniqueId, _this._treeDossierFolders.get_selectedNode().get_parent().get_value());
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
                _this._currentSelectedNode = node;
                if (_this.isWindowPopupEnable) {
                    _this._setToolbarButtonsEnabled(false);
                }
                else {
                    _this._btnCreateFolder.set_enabled(true);
                    _this._btnRemoveFascicle.set_enabled(false);
                    _this._btnModifyFascicle.set_enabled(false);
                    _this._btnDeleteFolder.set_enabled(false);
                    _this._btnAddFascicle.set_enabled(true);
                    _this._btnModifyFolder.set_enabled(true);
                    _this._btnCreateFascicle.set_enabled(true);
                }
                _this._dossierFolderService.getChildren(node.get_value(), uscDossierFolders.defaultFilterStatus != 0 ? uscDossierFolders.defaultFilterStatus : _this._checkedToolBarButtons, function (data) {
                    _this.loadNodes(data, node);
                }, function (exception) {
                    var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(uscNotification)) {
                        uscNotification.showNotification(exception);
                    }
                });
            };
            /**
            * Evento scatenato al click dei filtri
            */
            this.statusToolBar_ButtonClicked = function (sender, args) {
                _this._loadingPanel.show(_this.pageId);
                _this._checkedToolBarButtons = 0;
                for (var _i = 0, _a = _this._statusToolBar.get_items().toArray(); _i < _a.length; _i++) {
                    var item = _a[_i];
                    if ((item.get_checked())) {
                        _this._checkedToolBarButtons = _this._checkedToolBarButtons + DossierFolderStatus[item.get_value()];
                    }
                }
                //Se l'utente seleziona dei filtri, carico sempre anche le cartelle che hanno delle sottocartelle figlie
                if (_this._checkedToolBarButtons != 0) {
                    _this._checkedToolBarButtons += DossierFolderStatus.Folder;
                }
                var rootNode = _this._treeDossierFolders.get_nodes().getNode(0);
                _this.loadFolders(rootNode.get_value(), _this._checkedToolBarButtons);
            };
            /**
            * Metono chiamato in chiusura della radwindow di inserimento
            * @param sender
            * @param args
            */
            this.closeFolderInsertWindow = function (sender, args) {
                if (args.get_argument) {
                    var result = args.get_argument();
                    var parentNode_1 = _this._treeDossierFolders.get_selectedNode();
                    if (result) {
                        var dossierFolderModel = {};
                        dossierFolderModel = JSON.parse(result.Value[0]);
                        _this._btnCreateFolder.set_enabled(true);
                        _this._btnRemoveFascicle.set_enabled(false);
                        _this._btnDeleteFolder.set_enabled(false);
                        _this._btnModifyFascicle.set_enabled(false);
                        _this._btnCreateFascicle.set_enabled(true);
                        _this._btnAddFascicle.set_enabled(false);
                        _this._btnModifyFolder.set_enabled(false);
                        if (parentNode_1 != _this._treeDossierFolders.get_nodes().getNode(0)) {
                            _this._btnModifyFolder.set_enabled(true);
                            var attributeStatus = parentNode_1.get_attributes().getAttribute("Status");
                            _this.status = DossierFolderStatus[attributeStatus];
                            if (_this.status == DossierFolderStatus.Folder && !parentNode_1.get_nodes().getNode(0).get_value()) {
                                parentNode_1.get_nodes().remove(parentNode_1.get_nodes().getNode(0));
                            }
                            parentNode_1.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                            parentNode_1.set_expandedImageUrl("../App_Themes/DocSuite2008/imgset16/folder_open.png");
                            parentNode_1.set_toolTip("Cartella con sottocartelle");
                            parentNode_1.get_attributes().setAttribute("Status", "Folder");
                            parentNode_1.set_selected(true);
                        }
                        if (parentNode_1.get_expanded() == false) {
                            _this._dossierFolderService.getChildren(parentNode_1.get_value(), _this._checkedToolBarButtons, function (data) {
                                _this.loadNodes(data, parentNode_1);
                                parentNode_1.set_expanded(true);
                                _this._treeDossierFolders.commitChanges();
                            }, function (exception) {
                                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(uscNotification)) {
                                    uscNotification.showNotification(exception);
                                }
                            });
                        }
                        else {
                            var nodeToAdd = new Telerik.Web.UI.RadTreeNode();
                            parentNode_1.get_nodes().add(nodeToAdd);
                            _this.setNodeAttribute(nodeToAdd, dossierFolderModel);
                            parentNode_1.set_expanded(true);
                            _this._treeDossierFolders.commitChanges();
                        }
                    }
                }
            };
            /**
            * Metodo chiamato in chiusura della radwindow di modifica di cartella
            * @param sender
            * @param args
            */
            this.closeModifyWindow = function (sender, args) {
                if (args.get_argument) {
                    var result = args.get_argument();
                    var updatedNode = _this._treeDossierFolders.get_selectedNode();
                    if (result && result.Value[0]) {
                        var dossierFolderModel = JSON.parse(result.Value[0]);
                        _this.setNodeAttribute(updatedNode, dossierFolderModel);
                        _this._currentSelectedNode = updatedNode;
                        _this.setVisibilityButtonsByStatus();
                        _this._treeDossierFolders.commitChanges();
                    }
                }
            };
            /**
            * Evento scatenato al click di un nodo
            * @param sender
            * @param eventArgs
            */
            this.treeView_ClientNodeClicked = function (sender, eventArgs) {
                _this._currentSelectedNode = eventArgs.get_node();
                sender.set_loadingStatusPosition(Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
                if (_this.isWindowPopupEnable) {
                    _this._setToolbarButtonsEnabled(false);
                    var btnSelectDossierFolder = $find(_this.btnSelectDossierFolderId);
                    btnSelectDossierFolder.set_enabled(_this._currentSelectedNode.get_level() !== 0);
                    btnSelectDossierFolder.remove_clicked(_this.btnSelectDossierFolder_onClick);
                    btnSelectDossierFolder.add_clicked(_this.btnSelectDossierFolder_onClick);
                }
                else {
                    _this.setVisibilityButtonsByStatus();
                }
                var selectedDossierFolder = _this._dossierFolders.filter(function (x) { return x.UniqueId === _this._currentSelectedNode.get_value(); })[0];
                _this._btnCreateFascicleFromTemplate.set_enabled(false);
                if (_this._currentSelectedNode.get_level() > 1) {
                    _this._enableBtnCreateFascicleFromTemplate(selectedDossierFolder);
                }
            };
            /*
            * Eliminazione di una cartellina
            */
            this.removeDossierFolder = function () {
                _this._manager.radconfirm("Sei sicuro di voler eliminare la cartella?", function (arg) {
                    if (arg) {
                        var dossierFolder_1 = {};
                        dossierFolder_1.UniqueId = _this._currentSelectedNode.get_value();
                        dossierFolder_1.Name = _this._currentSelectedNode.get_text();
                        var parentNode = _this._currentSelectedNode.get_parent();
                        if (parentNode && parentNode.get_value() != _this.currentDossierId) {
                            dossierFolder_1.ParentInsertId = parentNode.get_value();
                        }
                        var idRole = _this._currentSelectedNode.get_attributes().getAttribute("idRole");
                        if (idRole) {
                            var dossierFolderRole = {};
                            var role = {};
                            role.EntityShortId = idRole;
                            dossierFolderRole.Role = role;
                            dossierFolder_1.DossierFolderRoles = [];
                            dossierFolder_1.DossierFolderRoles.push(dossierFolderRole);
                        }
                        _this._dossierFolderService.deleteDossierFolder(dossierFolder_1, function (data) {
                            _this.removeNode(dossierFolder_1.UniqueId);
                            _this._btnCreateFolder.set_enabled(false);
                            _this._btnRemoveFascicle.set_enabled(false);
                            _this._btnModifyFascicle.set_enabled(false);
                            _this._btnDeleteFolder.set_enabled(false);
                            _this._btnCreateFascicle.set_enabled(false);
                            _this._btnAddFascicle.set_enabled(false);
                            _this._btnModifyFolder.set_enabled(false);
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
            * Caricamento della cartella selezionata nella Session Storage
            * @param currentDossierId
            */
            this.setDossierFolder = function (currentDossierId) {
                var dossierFolder = {};
                dossierFolder.UniqueId = _this._currentSelectedNode.get_value();
                dossierFolder.Name = _this._currentSelectedNode.get_text();
                if (_this._currentSelectedNode.get_attributes().getAttribute("idCategory")) {
                    dossierFolder.idCategory = _this._currentSelectedNode.get_attributes().getAttribute("idCategory");
                }
                if (_this._currentSelectedNode.get_attributes().getAttribute("Status")) {
                    dossierFolder.Status = _this._currentSelectedNode.get_attributes().getAttribute("Status");
                }
                if (_this._currentSelectedNode.get_attributes().getAttribute("idRole")) {
                    dossierFolder.idRole = _this._currentSelectedNode.get_attributes().getAttribute("idRole");
                }
                if (_this._currentSelectedNode.get_attributes().getAttribute("idParent")) {
                    dossierFolder.idParent = _this._currentSelectedNode.get_attributes().getAttribute("idParent");
                }
                if (_this._currentSelectedNode.get_attributes().getAttribute("idFascicle")) {
                    dossierFolder.idFascicle = _this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                }
                sessionStorage[currentDossierId] = JSON.stringify(dossierFolder);
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
            this.setVisibilityButtonsByStatus = function () {
                if (_this._currentSelectedNode == _this._treeDossierFolders.get_nodes().getNode(0)) {
                    _this._btnCreateFolder.set_enabled(true);
                    _this._btnRemoveFascicle.set_enabled(false);
                    _this._btnModifyFascicle.set_enabled(false);
                    _this._btnDeleteFolder.set_enabled(false);
                    _this._btnCreateFascicle.set_enabled(true);
                    _this._btnAddFascicle.set_enabled(true);
                    _this._btnModifyFolder.set_enabled(false);
                    $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                    return;
                }
                var attributeStatus = _this._currentSelectedNode.get_attributes().getAttribute("Status");
                _this.status = DossierFolderStatus[attributeStatus];
                switch (_this.status) {
                    case DossierFolderStatus.InProgress: {
                        _this._btnCreateFolder.set_enabled(true);
                        _this._btnRemoveFascicle.set_enabled(false);
                        _this._btnModifyFascicle.set_enabled(false);
                        _this._btnDeleteFolder.set_enabled(true);
                        _this._btnCreateFascicle.set_enabled(true);
                        _this._btnAddFascicle.set_enabled(true);
                        _this._btnModifyFolder.set_enabled(true);
                        $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                        break;
                    }
                    case DossierFolderStatus.Folder: {
                        _this._btnCreateFolder.set_enabled(true);
                        _this._btnRemoveFascicle.set_enabled(false);
                        _this._btnModifyFascicle.set_enabled(false);
                        _this._btnDeleteFolder.set_enabled(false);
                        _this._btnCreateFascicle.set_enabled(true);
                        _this._btnAddFascicle.set_enabled(true);
                        _this._btnModifyFolder.set_enabled(true);
                        $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                        break;
                    }
                    case DossierFolderStatus.Fascicle: {
                        _this.fascicleId = _this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                        if (_this.fascicleId) {
                            _this._btnCreateFolder.set_enabled(false);
                            _this._btnRemoveFascicle.set_enabled(true);
                            _this._btnModifyFascicle.set_enabled(!_this.isWindowPopupEnable && _this.fascicleModifyButtonEnable);
                            _this._btnDeleteFolder.set_enabled(false);
                            _this._btnCreateFascicle.set_enabled(false);
                            _this._btnAddFascicle.set_enabled(false);
                            _this._btnModifyFolder.set_enabled(false);
                            $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.FASCICLE_TREE_NODE_CLICK, _this.fascicleId);
                        }
                        break;
                    }
                    case DossierFolderStatus.FascicleClose: {
                        _this.fascicleId = _this._currentSelectedNode.get_attributes().getAttribute("idFascicle");
                        if (_this.fascicleId) {
                            _this._btnCreateFolder.set_enabled(false);
                            _this._btnRemoveFascicle.set_enabled(false);
                            _this._btnModifyFascicle.set_enabled(false);
                            _this._btnDeleteFolder.set_enabled(false);
                            _this._btnCreateFascicle.set_enabled(false);
                            _this._btnAddFascicle.set_enabled(false);
                            _this._btnModifyFolder.set_enabled(false);
                            $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.FASCICLE_TREE_NODE_CLICK, _this.fascicleId);
                        }
                        break;
                    }
                    case DossierFolderStatus.DoAction: {
                        _this._btnCreateFolder.set_enabled(false);
                        _this._btnRemoveFascicle.set_enabled(false);
                        _this._btnModifyFascicle.set_enabled(false);
                        _this._btnDeleteFolder.set_enabled(false);
                        _this._btnCreateFascicle.set_enabled(false);
                        _this._btnAddFascicle.set_enabled(false);
                        _this._btnModifyFolder.set_enabled(false);
                        var message = "E' stata selezionata una cartellina con definizione di fascicolo associata. Procedere con la creazione del fascicolo?";
                        _this._manager.radconfirm(message, function (arg) {
                            if (arg) {
                                _this.createAndLinkFascicle(_this._currentSelectedNode.get_value());
                            }
                        }, 300, 160);
                        break;
                    }
                }
            };
            this.getFolderRoles = function (uniqueId) {
                var promise = $.Deferred();
                try {
                    _this._roleService.getDossierFolderRole(uniqueId, function (data) {
                        if (data == null) {
                            promise.resolve();
                            return;
                        }
                        var dossierFolderRoles = new Array();
                        for (var _i = 0, data_1 = data; _i < data_1.length; _i++) {
                            var role = data_1[_i];
                            var dossierFolderRoleModel = {};
                            var r = {};
                            r.EntityShortId = role.EntityShortId;
                            r.IdRole = role.EntityShortId;
                            r.Name = role.Name;
                            r.TenantId = role.TenantId;
                            r.IdRoleTenant = role.IdRoleTenant;
                            dossierFolderRoleModel.UniqueId = role.DossierFolderRoles[0].UniqueId;
                            dossierFolderRoleModel.Role = r;
                            dossierFolderRoles.push(dossierFolderRoleModel);
                        }
                        _this._dossierFolderRoles = dossierFolderRoles;
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
            this.loadFascicleDossierFolders = function (dossierId, fascicleId, dossierFoldersModel) {
                _this._dossierFolderService.getAllParentsOfFascicle(dossierId, fascicleId, function (data) {
                    if (!data)
                        return;
                    var dossierFolders = data.reverse();
                    _this.setRootNode(dossierFolders[0].Name, dossierFolders[0].UniqueId);
                    var parentNode = _this._treeDossierFolders.get_nodes().getNode(0);
                    for (var _i = 0, _a = dossierFolders.slice(1); _i < _a.length; _i++) {
                        var dossierFolder = _a[_i];
                        parentNode = _this._createFascicleDossierFolderParent(parentNode, dossierFolder);
                    }
                    parentNode.select();
                    _this._currentSelectedNode = parentNode;
                    _this.setVisibilityButtonsByStatus();
                    _this._loadingPanel.hide(_this.pageId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            this.btnSelectDossierFolder_onClick = function (sender, args) {
                var idDossier = _this._treeDossierFolders.get_nodes().getNode(0).get_value();
                var idDossierFolder = _this._currentSelectedNode.get_value();
                _this.closeGridWindow(idDossier, idDossierFolder);
            };
            this.getSelectedDossierFolderNode = function () {
                return _this._currentSelectedNode;
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
        uscDossierFolders.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._treeDossierFolders = $find(this.treeDossierFoldersId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._statusToolBar = $find(this.statusToolBarId);
            this._folderToolBar = $find(this.folderToolBarId);
            this._folderToolBar.add_buttonClicked(this.folderToolBar_ButtonClicked);
            this._statusToolBar.add_buttonClicked(this.statusToolBar_ButtonClicked);
            this._managerCreateFolder = $find(this.managerCreateFolderId);
            this._managerAddFascLink = $find(this.managerFascicleLinkId);
            this._managerModifyFolder = $find(this.managerModifyFolderId);
            this._managerCreateFascicleFolder = $find(this.managerCreateFascicleFolderId);
            this._managerModifyFascicle = $find(this.managerModifyFascicle);
            this._manager = $find(this.managerId);
            this._loadingPanel.show(this.pageId);
            this._btnCreateFolder = this._folderToolBar.findItemByValue(uscDossierFolders.CREATE_FOLDER);
            this._btnDeleteFolder = this._folderToolBar.findItemByValue(uscDossierFolders.DELETE_FOLDER);
            this._btnRemoveFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.REMOVE_FASCICLE);
            this._btnModifyFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.MODIFY_FASCICLE);
            this._btnAddFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.ADD_FASCICLE);
            this._btnModifyFolder = this._folderToolBar.findItemByValue(uscDossierFolders.MODIFY_FOLDER);
            this._btnCreateFascicle = this._folderToolBar.findItemByValue(uscDossierFolders.CREATE_FASCICLE);
            this._btnCreateFascicleFromTemplate = this._folderToolBar.findItemByValue(uscDossierFolders.CREATE_FASCICLE_FROM_TEMPLATE);
            this._checkedToolBarButtons = 0;
            this._managerCreateFolder.add_close(this.closeFolderInsertWindow);
            this._managerCreateFascicleFolder.add_close(this.closeFolderInsertWindow);
            this._managerAddFascLink.add_close(this.closeFolderInsertWindow);
            this._managerModifyFolder.add_close(this.closeModifyWindow);
            this._managerModifyFascicle.add_close(this.closeModifyWindow);
            if (this.hideFascicleAssociateButton) {
                this._btnAddFascicle.set_visible(false);
            }
            if (this.hideStatusToolbar) {
                $("#".concat(this.statusToolBarId)).hide();
            }
            if (this.persistanceDisabled) {
                this._dossierFolderService = new DossierFolderLocalService();
            }
            else {
                var dossierFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
                this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);
            }
            var containerPropertyConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ContainerProperty");
            this._containerPropertyService = new ContainerPropertyService(containerPropertyConfiguration);
            var fascicleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleConfiguration);
            var fascicleRoleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleRole");
            this._fascicleRoleService = new FascicleRoleService(fascicleRoleConfiguration);
            var roleConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Role");
            if (this.persistanceDisabled) {
                this._roleService = new RoleLocalService(roleConfiguration);
            }
            else {
                this._roleService = new RoleService(roleConfiguration);
            }
            this._currentSelectedNode = this._treeDossierFolders.get_nodes().getNode(0);
            if (this.isWindowPopupEnable) {
                var btnSelectDossierFolder = $find(this.btnSelectDossierFolderId);
                btnSelectDossierFolder.set_enabled(false);
            }
            this._btnCreateFascicleFromTemplate.set_enabled(false);
            this.bindLoaded();
        };
        /**
        * Carico le cartelle
        */
        uscDossierFolders.prototype.loadFolders = function (idDossier, status) {
            var _this = this;
            this._dossierFolderService.getChildren(idDossier, status, function (data) {
                _this.loadNodes(data);
                _this._loadingPanel.hide(_this.pageId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageId);
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            });
        };
        uscDossierFolders.prototype.removeNode = function (idDossierFolder) {
            var nodeToRemove = this._treeDossierFolders.findNodeByValue(idDossierFolder);
            if (nodeToRemove) {
                var parentNode = nodeToRemove.get_parent();
                if (parentNode && parentNode.get_nodes()) {
                    parentNode.get_nodes().remove(nodeToRemove);
                    if (parentNode != this._treeDossierFolders.get_nodes().getNode(0) && parentNode.get_nodes().get_count() == 0) {
                        parentNode.get_attributes().setAttribute("Status", "InProgress");
                        parentNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
                        parentNode.set_toolTip("Da gestire");
                    }
                    this._treeDossierFolders.commitChanges();
                }
            }
        };
        /**
        * Imposta gli attributi di un nodo
        * @param node
        * @param dossierFolder
        */
        uscDossierFolders.prototype.setNodeAttribute = function (node, dossierFolder) {
            node.get_attributes().setAttribute("Status", dossierFolder.Status);
            node.set_text(dossierFolder.Name);
            node.set_value(dossierFolder.UniqueId);
            node.get_attributes().setAttribute("idParent", null);
            if (node.get_parent()) {
                var parentNode = node.get_parent();
                node.get_attributes().setAttribute("idParent", parentNode.get_value());
            }
            node.get_attributes().setAttribute("idFascicle", null);
            if (dossierFolder.idFascicle) {
                node.get_attributes().setAttribute("idFascicle", dossierFolder.idFascicle);
            }
            node.get_attributes().setAttribute("idCategory", null);
            if (dossierFolder.idCategory) {
                node.get_attributes().setAttribute("idCategory", dossierFolder.idCategory);
            }
            node.get_attributes().setAttribute("idRole", null);
            if (dossierFolder.idRole) {
                node.get_attributes().setAttribute("idRole", dossierFolder.idRole);
            }
            //qui scelgo l'immagine da visualizzare per il nodo
            switch (DossierFolderStatus[dossierFolder.Status]) {
                case DossierFolderStatus.DoAction:
                case DossierFolderStatus.InProgress: {
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_hidden.png");
                    node.set_toolTip("Da gestire");
                    break;
                }
                case DossierFolderStatus.Fascicle: {
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
                    node.set_toolTip("Fascicolo ");
                    break;
                }
                case DossierFolderStatus.FascicleClose: {
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_close.png");
                    node.set_toolTip("Fascicolo chiuso");
                    break;
                }
                case DossierFolderStatus.Folder: {
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                    node.set_expandedImageUrl("../App_Themes/DocSuite2008/imgset16/folder_open.png");
                    node.set_toolTip("Cartella con sottocartelle");
                    var nodeToAdd = new Telerik.Web.UI.RadTreeNode();
                    node.get_nodes().add(nodeToAdd);
                    node.set_expanded(false);
                    break;
                }
            }
            return node;
        };
        /*
        * Imposto il valore del nodo Root
        * @param dossierName
        */
        uscDossierFolders.prototype.setRootNode = function (dossierTitle, dossierId) {
            var rootNode = this._treeDossierFolders.get_nodes().getNode(0);
            rootNode.set_text("Dossier: ".concat(dossierTitle));
            rootNode.set_value(dossierId);
            rootNode.set_expanded(true);
            rootNode.set_selected(true);
            this._treeDossierFolders.commitChanges();
            if (this.isWindowPopupEnable) {
                this._setToolbarButtonsEnabled(false);
            }
            else {
                this._btnCreateFolder.set_enabled(true);
                this._btnRemoveFascicle.set_enabled(false);
                this._btnModifyFascicle.set_enabled(false);
                this._btnDeleteFolder.set_enabled(false);
                this._btnCreateFascicle.set_enabled(true);
                this._btnAddFascicle.set_enabled(true);
                this._btnModifyFolder.set_enabled(false);
                this.currentDossierId = dossierId;
            }
        };
        uscDossierFolders.prototype.setButtonVisibility = function (isManager) {
            if (!isManager) {
                $(this._folderToolBar.get_element()).hide();
            }
        };
        uscDossierFolders.prototype.setStatusVisibility = function (isManager) {
            if (!isManager) {
                $("#".concat(this.statusToolBarId)).hide();
            }
        };
        /**
        * Carica i dati dello user control
        */
        uscDossierFolders.prototype.loadNodes = function (dossierFolders, node, appendToParentNode) {
            var _this = this;
            if (appendToParentNode === void 0) { appendToParentNode = false; }
            if (dossierFolders == null)
                return;
            var parentSelectedNode;
            if (node) {
                parentSelectedNode = node;
            }
            else {
                parentSelectedNode = this._treeDossierFolders.get_nodes().getNode(0);
            }
            this._btnCreateFascicleFromTemplate.set_enabled(false);
            if (!appendToParentNode) {
                parentSelectedNode.get_nodes().clear();
                parentSelectedNode.select();
            }
            var newNode;
            $.each(dossierFolders, function (index, dossierFolder) {
                _this._dossierFolders.push(dossierFolder);
                if (_this._treeDossierFolders.findNodeByValue(dossierFolder.UniqueId) != undefined) {
                    return;
                }
                if (!_this.isWindowPopupEnable || (_this.isWindowPopupEnable && DossierFolderStatus[dossierFolder.Status] !== DossierFolderStatus.Fascicle)) {
                    newNode = new Telerik.Web.UI.RadTreeNode();
                    parentSelectedNode.get_nodes().add(newNode);
                    _this.setNodeAttribute(newNode, dossierFolder);
                }
            });
            this._treeDossierFolders.commitChanges();
            $("#".concat(this.treeDossierFoldersId)).triggerHandler(uscDossierFolders.ON_END_LOAD_EVENT);
            this._loadingPanel.hide(this.pageId);
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscDossierFolders.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscDossierFolders.LOADED_EVENT);
        };
        /**
         * Rimuovo un fascicolo dalla cartella
         */
        uscDossierFolders.prototype.removeFascicleFromfolder = function () {
            var _this = this;
            this._manager.radconfirm("Sei sicuro di voler rimuovere il fascicolo dalla cartella?", function (arg) {
                if (arg) {
                    var dossierFolder_2 = {};
                    dossierFolder_2.UniqueId = _this._currentSelectedNode.get_value();
                    dossierFolder_2.Name = _this._currentSelectedNode.get_text();
                    dossierFolder_2.Status = DossierFolderStatus.InProgress;
                    $.when(_this.getFolderRoles(dossierFolder_2.UniqueId)).done(function () {
                        dossierFolder_2.DossierFolderRoles = _this._dossierFolderRoles;
                        _this._dossierFolderService.updateDossierFolder(dossierFolder_2, UpdateActionType.RemoveFascicleFromDossierFolder, function (data) {
                            var mapper = new DossierFolderSummaryModelMapper();
                            _this.setNodeAttribute(_this._currentSelectedNode, mapper.Map(data));
                            _this._btnCreateFolder.set_enabled(true);
                            _this._btnRemoveFascicle.set_enabled(false);
                            _this._btnModifyFascicle.set_enabled(false);
                            _this._btnDeleteFolder.set_enabled(true);
                            _this._btnCreateFascicle.set_enabled(true);
                            _this._btnAddFascicle.set_enabled(true);
                            _this._btnModifyFolder.set_enabled(true);
                            $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.ROOT_NODE_CLICK);
                        }, function (exception) {
                            var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                            if (!jQuery.isEmptyObject(uscNotification)) {
                                uscNotification.showNotification(exception);
                            }
                        });
                    }).fail(function (exception) {
                        _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento dei settori autorizzati alla cartella.");
                    });
                }
            }, 300, 160);
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param url
        * @param name
        * @param width
        * @param height
        */
        uscDossierFolders.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        uscDossierFolders.prototype.createAndLinkFascicle = function (dossierFolderId, dossierFolderParentId) {
            var _this = this;
            if (dossierFolderParentId === void 0) { dossierFolderParentId = null; }
            this._loadingPanel.show(this.pageId);
            this._dossierFolderService.getFullDossierFolder(dossierFolderId, function (dossierFolder) {
                var folder = dossierFolder;
                if (!folder.JsonMetadata) {
                    _this._loadingPanel.hide(_this.pageId);
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessun modello di fascicolo associato alla cartellina selezionata.");
                    return;
                }
                var actionModels = JSON.parse(folder.JsonMetadata);
                var filteredItems = actionModels.filter(function (item, index) { return item.BuildType == BuildActionType.Build; });
                if (filteredItems.length == 0) {
                    _this._loadingPanel.hide(_this.pageId);
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessun modello di fascicolo associato alla cartellina selezionata.");
                    return;
                }
                var insertFascicleAction = filteredItems[0];
                var fascicleToInsert = JSON.parse(insertFascicleAction.Model);
                if (fascicleToInsert.MetadataRepository && folder.Dossier.MetadataRepository && fascicleToInsert.MetadataRepository.UniqueId == folder.Dossier.MetadataRepository.UniqueId) {
                    fascicleToInsert.MetadataDesigner = folder.Dossier.MetadataDesigner;
                    fascicleToInsert.MetadataValues = folder.Dossier.MetadataValues;
                }
                _this._fascicleService.insertFascicle(fascicleToInsert, null, function (fascicle) {
                    folder.Fascicle = fascicle;
                    folder.Status = DossierFolderStatus.Fascicle;
                    var category = new CategoryModel();
                    category.EntityShortId = fascicle.Category.EntityShortId;
                    folder.Category = category;
                    fascicleToInsert.UniqueId = fascicle.UniqueId;
                    $.when(_this.setFascicleRoles(fascicleToInsert, folder.DossierFolderRoles))
                        .done(function () {
                        if (dossierFolderParentId) {
                            folder.ParentInsertId = dossierFolderParentId;
                            folder.UniqueId = null;
                            _this._dossierFolderService.insertDossierFolder(folder, null, function (data) {
                                var mapper = new DossierFolderSummaryModelMapper();
                                var insertedDossierFolder = mapper.Map(data);
                                insertedDossierFolder.idFascicle = fascicleToInsert.UniqueId;
                                var parentNode = _this._treeDossierFolders.findNodeByValue(dossierFolderParentId);
                                _this.loadNodes([insertedDossierFolder], parentNode, true);
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.pageId);
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                        else {
                            _this._dossierFolderService.updateDossierFolder(folder, null, function (data) {
                                var node = _this._treeDossierFolders.findNodeByValue(dossierFolderId);
                                data.Fascicle = fascicle;
                                var mapper = new DossierFolderSummaryModelMapper();
                                var updatedDossierFolder = mapper.Map(data);
                                _this._dossierFolders = _this._dossierFolders.filter(function (x) { return x.UniqueId !== updatedDossierFolder.UniqueId; });
                                _this._dossierFolders.push(updatedDossierFolder);
                                _this._enableBtnCreateFascicleFromTemplate(updatedDossierFolder);
                                _this.setNodeAttribute(node, updatedDossierFolder);
                                $("#".concat(_this.pageId)).triggerHandler(uscDossierFolders.FASCICLE_TREE_NODE_CLICK, fascicle.UniqueId);
                                _this._loadingPanel.hide(_this.pageId);
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.pageId);
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                    })
                        .fail(function () {
                        _this._loadingPanel.hide(_this.pageId);
                        _this.showNotificationMessage(_this.uscNotificationId, "Errore in autorizzazione del fascicolo");
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscDossierFolders.prototype.setFascicleRoles = function (fascicle, roles) {
            var _this = this;
            var promise = $.Deferred();
            if (!roles || roles.length == 0) {
                promise.resolve();
                return;
            }
            var role = roles.shift();
            var existFascicleAccountedRole = fascicle.FascicleRoles &&
                fascicle.FascicleRoles.filter(function (x) { return x.AuthorizationRoleType == AuthorizationRoleType.Accounted && x.Role.EntityShortId == role.Role.EntityShortId; }).length > 0;
            if (existFascicleAccountedRole) {
                if (roles.length > 0) {
                    $.when(this.setFascicleRoles(fascicle, roles))
                        .done(function () { return promise.resolve(); })
                        .fail(function () { return promise.reject(); });
                }
                else {
                    promise.resolve();
                    return;
                }
            }
            var fascicleRole = new FascicleRoleModel();
            fascicleRole.Fascicle = fascicle;
            fascicleRole.Role = role.Role;
            fascicleRole.AuthorizationRoleType = AuthorizationRoleType.Accounted;
            this._fascicleRoleService.insertFascicleRole(fascicleRole, function (data) {
                if (roles.length == 0) {
                    promise.resolve();
                    return;
                }
                $.when(_this.setFascicleRoles(fascicle, roles))
                    .done(function () { return promise.resolve(); })
                    .fail(function () { return promise.reject(); });
            }, function (exception) {
                promise.reject();
            });
            return promise.promise();
        };
        uscDossierFolders.prototype._createFascicleDossierFolderParent = function (parentNode, dossierFolder) {
            var childNode = new Telerik.Web.UI.RadTreeNode();
            parentNode.get_nodes().clear();
            parentNode.get_nodes().add(childNode);
            this.setNodeAttribute(childNode, dossierFolder);
            parentNode.expand();
            return childNode;
        };
        uscDossierFolders.prototype._setToolbarButtonsEnabled = function (isEnabled) {
            this._btnCreateFolder.set_enabled(isEnabled);
            this._btnRemoveFascicle.set_enabled(isEnabled);
            this._btnDeleteFolder.set_enabled(isEnabled);
            this._btnCreateFascicle.set_enabled(isEnabled);
            this._btnAddFascicle.set_enabled(isEnabled);
            this._btnModifyFolder.set_enabled(isEnabled);
            this._btnModifyFascicle.set_enabled(isEnabled);
        };
        uscDossierFolders.prototype.setToolbarButtonsVisibility = function (isVisible) {
            this._btnCreateFolder.set_visible(isVisible);
            this._btnRemoveFascicle.set_visible(isVisible);
            this._btnDeleteFolder.set_visible(isVisible);
            this._btnCreateFascicle.set_visible(isVisible);
            this._btnAddFascicle.set_visible(isVisible);
            this._btnModifyFolder.set_visible(isVisible);
            this._btnModifyFascicle.set_visible(isVisible);
            this._btnCreateFascicleFromTemplate.set_visible(isVisible);
        };
        uscDossierFolders.prototype.closeGridWindow = function (idDossier, idDossierFolder) {
            var wnd = this._getRadWindow();
            wnd.close(idDossier + "|" + idDossierFolder);
        };
        uscDossierFolders.prototype._getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        uscDossierFolders.prototype._enableBtnCreateFascicleFromTemplate = function (dossierFolder) {
            if (dossierFolder.Status === DossierFolderStatus[DossierFolderStatus.Fascicle] && dossierFolder.JsonMetadata) {
                var actionModels = JSON.parse(dossierFolder.JsonMetadata);
                if (actionModels.filter(function (x) { return x.BuildType === BuildActionType.Build; }).length > 0) {
                    this._btnCreateFascicleFromTemplate.set_enabled(true);
                    this._templateDossierFolder = dossierFolder;
                }
            }
        };
        uscDossierFolders.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        uscDossierFolders.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscDossierFolders.ON_END_LOAD_EVENT = "onEndLoad";
        uscDossierFolders.LOADED_EVENT = "onLoaded";
        uscDossierFolders.FASCICLE_TREE_NODE_CLICK = "onFascicleTreeNodeClick";
        uscDossierFolders.ROOT_NODE_CLICK = "onRootTreeNodeClick";
        uscDossierFolders.CREATE_FOLDER = "createFolder";
        uscDossierFolders.CREATE_FASCICLE = "createFascicle";
        uscDossierFolders.DELETE_FOLDER = "deleteFolder";
        uscDossierFolders.REMOVE_FASCICLE = "removeFascicle";
        uscDossierFolders.ADD_FASCICLE = "addFascicle";
        uscDossierFolders.MODIFY_FOLDER = "modifyFolder";
        uscDossierFolders.MODIFY_FASCICLE = "modifyFascicle";
        uscDossierFolders.REFRESH_DOSSIER_FOLDERS = "refreshDossierFolders";
        uscDossierFolders.CREATE_FASCICLE_FROM_TEMPLATE = "createFascicleFromTemplate";
        uscDossierFolders.defaultFilterStatus = 0;
        return uscDossierFolders;
    }());
    return uscDossierFolders;
});
//# sourceMappingURL=uscDossierFolders.js.map