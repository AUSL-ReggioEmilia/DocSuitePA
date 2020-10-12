/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/WindowHelper", "Tblt/TbltSettoreOperation"], function (require, exports, WindowHelper, TbltSettoreOperation) {
    var TbltSettore = /** @class */ (function () {
        /**
         * Costruttore
         */
        function TbltSettore() {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento alla chiusura della finestra di modifica
             * @param sender
             * @param args
             */
            this.onCloseWindowEdit = function (sender, args) {
                if (args.get_argument() !== null) {
                    _this.updateGroups(args.get_argument());
                }
            };
            /**
             * Evento alla chiusura della finestra di gestione gruppi
             * @param sender
             * @param args
             */
            this.onCloseWindowGroup = function (sender, args) {
                _this.updateGroups();
            };
            /**
             * Evento alla chiusura della finestra di move settori
             * @param sender
             * @param args
             */
            this.onCloseWindowRoles = function (sender, args) {
                var ajaxManager = $find(_this.ajaxManagerId);
                if (args.get_argument() !== null) {
                    ajaxManager.ajaxRequest('move' + '|' + args.get_argument());
                }
            };
            /**
           * Evento alla chiusura della finestra di aggiunta utente
           * @param sender
           * @param args
           */
            this.onCloseWindowAddUserInGroup = function (sender, args) {
                var ajaxManager = $find(_this.ajaxManagerId);
                if (args.get_argument() !== null) {
                    ajaxManager.ajaxRequest('AddUser' + '|' + args.get_argument());
                }
            };
            /**
             * Evento scatenato al click di un elemento nel context menu
             * @param sender
             * @param args
             */
            this.onContextMenuItemClicked = function (sender, args) {
                var menuItem = args.get_menuItem();
                var operation = TbltSettoreOperation[menuItem.get_value()];
                switch (operation) {
                    case TbltSettoreOperation.Add:
                    case TbltSettoreOperation.Rename:
                    case TbltSettoreOperation.Delete:
                    case TbltSettoreOperation.Clone:
                        _this.openEditWindow('windowEditRoles', menuItem.get_value());
                        break;
                    case TbltSettoreOperation.Move:
                        _this.openRolesWindow();
                        break;
                    case TbltSettoreOperation.Print:
                        _this.openPrintWindow('windowPrintRoles');
                        break;
                    case TbltSettoreOperation.Groups:
                        _this.openGroupsWindow();
                        break;
                    case TbltSettoreOperation.Log:
                        _this.openLogWindow('windowLogRoles');
                        break;
                    case TbltSettoreOperation.Function:
                        _this.viewFunctionUsers();
                        break;
                    case TbltSettoreOperation.ChildrenRoles:
                        _this.loadChildrenRoles();
                        break;
                }
            };
            /**
             * Evento scatenato in visualizzazione del context menu
             * @param sender
             * @param args
             */
            this.onContextMenuShowing = function (sender, args) {
                var treeNode = args.get_node();
                var menu = args.get_menu();
                treeNode.set_selected(true);
                var attributeRecovery = treeNode.get_attributes().getAttribute("Recovery");
                var isNodeRecovery;
                if (attributeRecovery != null) {
                    isNodeRecovery = !!JSON.parse(treeNode.get_attributes().getAttribute("Recovery"));
                    isNodeRecovery ? menu.findItemByValue(TbltSettore.DELETE_OPTION).set_text("Recupera") : menu.findItemByValue('Delete').set_text("Elimina");
                }
                if (isNodeRecovery) {
                    menu.findItemByValue(TbltSettore.CREATE_OPTION).disable();
                    menu.findItemByValue(TbltSettore.MODIFY_OPTION).disable();
                    menu.findItemByValue(TbltSettore.PRINT_OPTION).disable();
                    menu.findItemByValue(TbltSettore.DELETE_OPTION).enable();
                    menu.findItemByValue(TbltSettore.GROUPS_OPTION).disable();
                    menu.findItemByValue(TbltSettore.LOG_OPTION).disable();
                    menu.findItemByValue(TbltSettore.FUNCTION_OPTION).disable();
                    menu.findItemByValue(TbltSettore.MOVE_OPTION).enable();
                    menu.findItemByValue(TbltSettore.CLONE_OPTION).disable();
                    _this.alignButtons(menu);
                    return;
                }
                var nodeType = treeNode.get_attributes().getAttribute("NodeType");
                //let isNodeFather: boolean 
                switch (nodeType) {
                    case "Role":
                    case "SubRole":
                        menu.findItemByValue(TbltSettore.CREATE_OPTION).enable();
                        menu.findItemByValue(TbltSettore.MODIFY_OPTION).enable();
                        menu.findItemByValue(TbltSettore.PRINT_OPTION).enable();
                        menu.findItemByValue(TbltSettore.DELETE_OPTION).enable();
                        menu.findItemByValue(TbltSettore.GROUPS_OPTION).enable();
                        menu.findItemByValue(TbltSettore.CLONE_OPTION).enable();
                        menu.findItemByValue(TbltSettore.LOG_OPTION).enable();
                        menu.findItemByValue(TbltSettore.FUNCTION_OPTION).enable();
                        menu.findItemByValue(TbltSettore.MOVE_OPTION).enable();
                        break;
                    case "Group":
                        menu.findItemByValue(TbltSettore.CREATE_OPTION).disable();
                        menu.findItemByValue(TbltSettore.MODIFY_OPTION).disable();
                        menu.findItemByValue(TbltSettore.PRINT_OPTION).disable();
                        menu.findItemByValue(TbltSettore.DELETE_OPTION).disable();
                        menu.findItemByValue(TbltSettore.GROUPS_OPTION).disable();
                        menu.findItemByValue(TbltSettore.LOG_OPTION).enable();
                        menu.findItemByValue(TbltSettore.FUNCTION_OPTION).disable();
                        menu.findItemByValue(TbltSettore.MOVE_OPTION).disable();
                        menu.findItemByValue(TbltSettore.CLONE_OPTION).disable();
                        _this.disableButtons();
                        break;
                    case "Root":
                        menu.findItemByValue(TbltSettore.CREATE_OPTION).enable();
                        menu.findItemByValue(TbltSettore.MODIFY_OPTION).disable();
                        menu.findItemByValue(TbltSettore.PRINT_OPTION).disable();
                        menu.findItemByValue(TbltSettore.DELETE_OPTION).disable();
                        menu.findItemByValue(TbltSettore.LOG_OPTION).disable();
                        menu.findItemByValue(TbltSettore.LOG_OPTION).disable();
                        menu.findItemByValue(TbltSettore.FUNCTION_OPTION).disable();
                        menu.findItemByValue(TbltSettore.MOVE_OPTION).disable();
                        menu.findItemByValue(TbltSettore.CLONE_OPTION).disable();
                        break;
                    default:
                        menu.findItemByValue(TbltSettore.CREATE_OPTION).enable();
                        menu.findItemByValue(TbltSettore.MODIFY_OPTION).disable();
                        menu.findItemByValue(TbltSettore.PRINT_OPTION).disable();
                        menu.findItemByValue(TbltSettore.DELETE_OPTION).enable();
                        menu.findItemByValue(TbltSettore.GROUPS_OPTION).disable();
                        menu.findItemByValue(TbltSettore.LOG_OPTION).enable();
                        menu.findItemByValue(TbltSettore.FUNCTION_OPTION).disable();
                        menu.findItemByValue(TbltSettore.MOVE_OPTION).disable();
                        menu.findItemByValue(TbltSettore.CLONE_OPTION).disable();
                        break;
                }
                _this.alignButtons(menu);
            };
            $(document).ready(function () {
            });
        }
        /**
         * Initialize
         */
        TbltSettore.prototype.initialize = function () {
            this._folderToolBar = $find(this.folderToolBarId);
            var wndManager = $find(this.radWindowManagerRolesId);
            wndManager.getWindowByName('windowEditRoles').add_close(this.onCloseWindowEdit);
            wndManager.getWindowByName('windowGroupRoles').add_close(this.onCloseWindowGroup);
            wndManager.getWindowByName('windowRoles').add_close(this.onCloseWindowRoles);
            wndManager.getWindowByName('windowAddUsers').add_close(this.onCloseWindowAddUserInGroup);
        };
        TbltSettore.prototype.disableButtons = function () {
            var btnAdd = this._folderToolBar.findItemByValue(TbltSettore.CREATE_OPTION);
            var btnModify = this._folderToolBar.findItemByValue(TbltSettore.MODIFY_OPTION);
            var btnPrint = this._folderToolBar.findItemByValue(TbltSettore.PRINT_OPTION);
            var btnLog = this._folderToolBar.findItemByValue(TbltSettore.LOG_OPTION);
            var btnFunction = this._folderToolBar.findItemByValue(TbltSettore.FUNCTION_OPTION);
            var btnDelete = this._folderToolBar.findItemByValue(TbltSettore.DELETE_OPTION);
            var btnGroups = this._folderToolBar.findItemByValue(TbltSettore.GROUPS_OPTION);
            var btnMove = this._folderToolBar.findItemByValue(TbltSettore.MOVE_OPTION);
            var btnClone = this._folderToolBar.findItemByValue(TbltSettore.CLONE_OPTION);
            if (btnAdd != null)
                btnAdd.set_enabled(false);
            if (btnModify != null)
                btnModify.set_enabled(false);
            if (btnPrint != null)
                btnPrint.set_enabled(false);
            if (btnLog != null)
                btnLog.set_enabled(false);
            if (btnFunction != null)
                btnFunction.set_enabled(false);
            if (btnDelete != null)
                btnDelete.set_enabled(false);
            if (btnGroups != null)
                btnGroups.set_enabled(false);
            if (btnMove != null)
                btnMove.set_enabled(false);
            if (btnClone != null)
                btnClone.set_enabled(false);
        };
        TbltSettore.prototype.selectNode = function (nodeValue) {
            var treeView = $find(this.radTreeViewRolesId);
            var node = treeView.findNodeByValue(nodeValue);
            node.select();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Metodo di apertura finestra di modifica settore
         * @param name
         * @param operation
         */
        TbltSettore.prototype.openEditWindow = function (name, operation) {
            var parameters = "Action=".concat(operation.toString());
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            switch (TbltSettoreOperation[operation]) {
                case TbltSettoreOperation.Add:
                    if (selectedNode != null) {
                        if (selectedNode.get_value() != "Root") {
                            parameters = parameters.concat("&ParentRoleID=", selectedNode.get_value());
                        }
                    }
                    break;
                case TbltSettoreOperation.Rename:
                    parameters = parameters.concat("&RoleID=", selectedNode.get_value());
                    break;
                case TbltSettoreOperation.Delete:
                    if (selectedNode.get_attributes().getAttribute("Recovery") == "true") {
                        parameters = "Action=Recovery";
                    }
                    parameters = parameters.concat("&RoleID=", selectedNode.get_value());
                    break;
                case TbltSettoreOperation.Modify:
                case TbltSettoreOperation.Clone:
                    parameters = parameters.concat("&RoleID=", selectedNode.get_value());
                    break;
            }
            var url = "../Tblt/TbltSettoreGes.aspx?Type=Comm&".concat(parameters);
            return this.openWindow(url, name, WindowHelper.WIDTH_EDIT_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
        };
        /**
         * Metodo di apertura finestra di visualizzazione log settore
         * @param name
         */
        TbltSettore.prototype.openLogWindow = function (name) {
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            var url = "../Tblt/TbltLog.aspx?Type=Comm&TableName=RoleGroup";
            if (selectedNode != null) {
                var attributes = selectedNode.get_attributes();
                url += "&entityUniqueId=".concat(attributes.getAttribute("UniqueId"));
            }
            return this.openWindow(url, name, WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_LOG_WINDOW);
        };
        /**
         * Metodo di apertura finestra di stampa settore
         * @param name
         */
        TbltSettore.prototype.openPrintWindow = function (name) {
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            var url = "../Comm/CommPrint.aspx?Type=Comm&PrintName=SingleRolePrint&IdRef=".concat(selectedNode.get_value());
            return this.openWindow(url, name, WindowHelper.WIDTH_PRINT_WINDOW, WindowHelper.HEIGHT_PRINT_WINDOW);
        };
        /**
         * Metodo di apertura finestra di gestione gruppi AD settore
         */
        TbltSettore.prototype.openRolesWindow = function () {
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            var nodeType = selectedNode.get_attributes().getAttribute("NodeType");
            var url = "../UserControl/CommonSelSettori.aspx?Type=Comm&MultiSelect=False&RoleRestiction=None&Rights=&TenantEnabled=False&RightEnabled=false&RootSelectable=true&ConfirmSelection=true&isActive=".concat(this.showDisabled);
            return this.openWindow(url, "windowRoles", WindowHelper.WIDTH_GROUP_WINDOW, WindowHelper.HEIGHT_GROUP_WINDOW - 200);
        };
        /**
         * Metodo di apertura finestra di gestione gruppi AD settore
         */
        TbltSettore.prototype.openGroupsWindow = function () {
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            var nodeType = selectedNode.get_attributes().getAttribute("NodeType");
            var url = "../Tblt/TbltSettoreGesGruppi.aspx?Type=Comm&IdRole=".concat(selectedNode.get_value());
            if (nodeType == "Group") {
                url = url.concat("&GroupName=", selectedNode.get_text());
            }
            return this.openWindow(url, "windowGroupRoles", WindowHelper.WIDTH_GROUP_WINDOW + 200, WindowHelper.HEIGHT_GROUP_WINDOW - 100);
        };
        /**
      * Apre la finestra di propagazione massiva
      * TODO: da rivedere
      * @param name
      */
        TbltSettore.prototype.openPropagationWindow = function (name) {
            var url = "../Tblt/TbltSettorePropagation.aspx?Type=ProtDB";
            return this.openWindow(url, name, WindowHelper.WIDTH_PRINT_WINDOW, WindowHelper.HEIGHT_PRINT_WINDOW);
        };
        /**
         * Metodo di apertura finestra di visualizzazione history settore
         */
        TbltSettore.prototype.openHistoryWindow = function () {
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            var nodeType = selectedNode.get_attributes().getAttribute("NodeType");
            var url = "../Tblt/TbltRoleHistory.aspx?Type=ProtDB&IdRole=".concat(selectedNode.get_value());
            return this.openWindow(url, "windowHistory", WindowHelper.WIDTH_GROUP_WINDOW, WindowHelper.HEIGHT_GROUP_WINDOW);
        };
        /**
         * Metodo di apertura nuova finestra
         * @param url
         * @param name
         * @param width
         * @param height
         */
        TbltSettore.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.radWindowManagerRolesId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.center();
            return false;
        };
        /**
         * Chiamata per la visualizzazione della lista utenti cofigurati nel disegno di funzione
         */
        TbltSettore.prototype.viewFunctionUsers = function () {
            var ajaxManager = $find(this.ajaxManagerId);
            ajaxManager.ajaxRequest('showcollaboration');
        };
        /**
         * Metodo per settare la visibilità del pulsante di cancellazione
         * @param node
         * @param menu
         */
        TbltSettore.prototype.checkDeleteItem = function (node, menu) {
            var btnDelete = this._folderToolBar.findItemByValue(TbltSettore.DELETE_OPTION);
            node.get_nodes().forEach(function (item) {
                var nodeType = item.get_attributes().getAttribute("NodeType").toUpperCase();
                if (nodeType != "GROUP") {
                    var isNodeRecovery = !!JSON.parse(item.get_attributes().getAttribute("Recovery"));
                    if (nodeType == "SUBROLE" && !isNodeRecovery) {
                        btnDelete.disable();
                        return false;
                    }
                }
            });
            btnDelete.enable();
            return true;
        };
        /**
         * Metodo per allineare i pulsante al context menu
         * @param menu
         */
        TbltSettore.prototype.alignButtons = function (menu) {
            var btnAdd = this._folderToolBar.findItemByValue(TbltSettore.CREATE_OPTION);
            var btnModify = this._folderToolBar.findItemByValue(TbltSettore.MODIFY_OPTION);
            var btnPrint = this._folderToolBar.findItemByValue(TbltSettore.PRINT_OPTION);
            var btnLog = this._folderToolBar.findItemByValue(TbltSettore.LOG_OPTION);
            var btnFunction = this._folderToolBar.findItemByValue(TbltSettore.FUNCTION_OPTION);
            var btnDelete = this._folderToolBar.findItemByValue(TbltSettore.DELETE_OPTION);
            var btnGroups = this._folderToolBar.findItemByValue(TbltSettore.GROUPS_OPTION);
            var btnMove = this._folderToolBar.findItemByValue(TbltSettore.MOVE_OPTION);
            var btnClone = this._folderToolBar.findItemByValue(TbltSettore.CLONE_OPTION);
            if (btnAdd != null) {
                btnAdd.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Add]).get_enabled());
            }
            if (btnModify != null) {
                btnModify.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Rename]).get_enabled());
            }
            if (btnMove != null) {
                btnMove.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Move]).get_enabled());
            }
            if (btnPrint != null) {
                btnPrint.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Print]).get_enabled());
            }
            if (btnLog != null) {
                btnLog.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Log]).get_enabled());
            }
            if (btnFunction != null) {
                btnFunction.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Function]).get_enabled());
            }
            if (btnDelete != null) {
                btnDelete.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Delete]).get_enabled());
            }
            if (btnGroups != null) {
                btnGroups.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Groups]).get_enabled());
            }
            if (btnClone != null) {
                btnClone.set_enabled(!menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Clone]).get_enabled());
            }
        };
        /**
        * Chiamata Ajax per caricamento Settori figli del settore selezionato
        */
        TbltSettore.prototype.loadChildrenRoles = function () {
            var treeView = $find(this.radTreeViewRolesId);
            var selectedNode = treeView.get_selectedNode();
            if (selectedNode != null && selectedNode.get_value() != "Root") {
                var nodeTypeId = selectedNode.get_value();
                var ajaxManager = $find(this.ajaxManagerId);
                ajaxManager.ajaxRequest('loadChildrenRoles');
            }
        };
        /**
         * Chiamata Ajax per la modifica degli utenti di un settore
         * @param source
         */
        TbltSettore.prototype.updateGroups = function (source) {
            var ajaxManager = $find(this.ajaxManagerId);
            if (source != null) {
                ajaxManager.ajaxRequest('Update' + '|' + source.Operation + '|' + source.ID);
            }
            else {
                ajaxManager.ajaxRequest('Update');
            }
        };
        TbltSettore.CREATE_OPTION = "create";
        TbltSettore.MODIFY_OPTION = "modify";
        TbltSettore.DELETE_OPTION = "delete";
        TbltSettore.MOVE_OPTION = "move";
        TbltSettore.CLONE_OPTION = "clone";
        TbltSettore.PRINT_OPTION = "print";
        TbltSettore.GROUPS_OPTION = "groups";
        TbltSettore.HISTORY_OPTION = "history";
        TbltSettore.LOG_OPTION = "log";
        TbltSettore.FUNCTION_OPTION = "function";
        TbltSettore.PROPAGATION_OPTION = "propagation";
        return TbltSettore;
    }());
    return TbltSettore;
});
//# sourceMappingURL=TbltSettore.js.map