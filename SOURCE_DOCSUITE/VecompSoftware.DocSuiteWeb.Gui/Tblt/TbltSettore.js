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
                    isNodeRecovery ? menu.findItemByValue("Delete").set_text("Recupera") : menu.findItemByValue('Delete').set_text("Elimina");
                }
                if (isNodeRecovery) {
                    menu.findItemByValue('Add').disable();
                    menu.findItemByValue('Rename').disable();
                    menu.findItemByValue('Print').disable();
                    menu.findItemByValue('Delete').enable();
                    menu.findItemByValue('Groups').disable();
                    menu.findItemByValue('ChildrenRoles').disable();
                    menu.findItemByValue('Log').disable();
                    menu.findItemByValue('Function').disable();
                    menu.findItemByValue('Move').enable();
                    menu.findItemByValue("Clone").disable();
                    _this.alignButtons(menu);
                    return;
                }
                var nodeType = treeNode.get_attributes().getAttribute("NodeType");
                //let isNodeFather: boolean 
                switch (nodeType) {
                    case "Role":
                    case "SubRole":
                        menu.findItemByValue('Add').enable();
                        menu.findItemByValue('Rename').enable();
                        menu.findItemByValue('Print').enable();
                        menu.findItemByValue('Delete').enable();
                        menu.findItemByValue('Groups').enable();
                        menu.findItemByValue('Clone').enable();
                        menu.findItemByValue('ChildrenRoles').enable();
                        for (var _i = 0, _a = treeNode.get_allNodes(); _i < _a.length; _i++) {
                            var childNode = _a[_i];
                            if (childNode.get_attributes().getAttribute("NodeType") == "SubRole") {
                                menu.findItemByValue('ChildrenRoles').disable();
                                break;
                            }
                        }
                        if (treeNode.get_attributes().getAttribute("HasChildren") == "False") {
                            menu.findItemByValue('ChildrenRoles').disable();
                        }
                        menu.findItemByValue('Log').enable();
                        menu.findItemByValue('Function').enable();
                        menu.findItemByValue('Move').enable();
                        break;
                    case "Group":
                        menu.findItemByValue('Add').disable();
                        menu.findItemByValue('Rename').disable();
                        menu.findItemByValue('Print').disable();
                        menu.findItemByValue('Delete').disable();
                        menu.findItemByValue('Groups').disable();
                        menu.findItemByValue('ChildrenRoles').disable();
                        menu.findItemByValue('Log').enable();
                        menu.findItemByValue('Function').disable();
                        menu.findItemByValue('Move').disable();
                        menu.findItemByValue("Clone").disable();
                        _this.disableButtons();
                        break;
                    case "Root":
                        menu.findItemByValue('Add').enable();
                        menu.findItemByValue('Rename').disable();
                        menu.findItemByValue('Print').disable();
                        menu.findItemByValue('Delete').disable();
                        menu.findItemByValue('Groups').disable();
                        menu.findItemByValue('ChildrenRoles').disable();
                        menu.findItemByValue('Log').disable();
                        menu.findItemByValue('Function').disable();
                        menu.findItemByValue('Move').disable();
                        menu.findItemByValue("Clone").disable();
                        break;
                    default:
                        menu.findItemByValue('Add').enable();
                        menu.findItemByValue('Rename').disable();
                        menu.findItemByValue('Print').disable();
                        menu.findItemByValue('Delete').enable();
                        menu.findItemByValue('Groups').disable();
                        menu.findItemByValue('ChildrenRoles').disable();
                        menu.findItemByValue('Log').enable();
                        menu.findItemByValue('Function').disable();
                        menu.findItemByValue('Move').disable();
                        menu.findItemByValue("Clone").disable();
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
            var wndManager = $find(this.radWindowManagerRolesId);
            wndManager.getWindowByName('windowEditRoles').add_close(this.onCloseWindowEdit);
            wndManager.getWindowByName('windowGroupRoles').add_close(this.onCloseWindowGroup);
            wndManager.getWindowByName('windowRoles').add_close(this.onCloseWindowRoles);
            wndManager.getWindowByName('windowAddUsers').add_close(this.onCloseWindowAddUserInGroup);
        };
        TbltSettore.prototype.disableButtons = function () {
            var btnAdd = $get(this.btnAddId);
            var btnModify = $get(this.btnModifyId);
            var btnPrint = $get(this.btnPrintId);
            var btnLog = $get(this.btnLogId);
            var btnFunction = $get(this.btnFunctionId);
            var btnDelete = $get(this.btnDeleteId);
            var btnGroups = $get(this.btnGroupsId);
            var btnChildrenRoles = $get(this.btnChildrenRolesId);
            var btnMove = $get(this.btnMoveId);
            var btnClone = $get(this.btnCloneId);
            if (btnAdd != null)
                btnAdd.disabled = true;
            if (btnModify != null)
                btnModify.disabled = true;
            if (btnPrint != null)
                btnPrint.disabled = true;
            if (btnLog != null)
                btnLog.disabled = false;
            if (btnFunction != null)
                btnFunction.disabled = true;
            if (btnDelete != null)
                btnDelete.disabled = true;
            if (btnGroups != null)
                btnGroups.disabled = true;
            if (btnChildrenRoles != null)
                btnChildrenRoles.disabled = true;
            if (btnMove != null)
                btnMove.disabled = true;
            if (btnClone != null)
                btnClone.disabled = true;
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
            var btnDelete = $get(this.btnDeleteId);
            node.get_nodes().forEach(function (item) {
                var nodeType = item.get_attributes().getAttribute("NodeType").toUpperCase();
                if (nodeType != "GROUP") {
                    var isNodeRecovery = !!JSON.parse(item.get_attributes().getAttribute("Recovery"));
                    if (nodeType == "SUBROLE" && !isNodeRecovery) {
                        btnDelete.disabled = true;
                        return false;
                    }
                }
            });
            btnDelete.disabled = false;
            return true;
        };
        /**
         * Metodo per allineare i pulsante al context menu
         * @param menu
         */
        TbltSettore.prototype.alignButtons = function (menu) {
            var btnAdd = $get(this.btnAddId);
            var btnModify = $get(this.btnModifyId);
            var btnMove = $get(this.btnMoveId);
            var btnPrint = $get(this.btnPrintId);
            var btnLog = $get(this.btnLogId);
            var btnFunction = $get(this.btnFunctionId);
            var btnDelete = $get(this.btnDeleteId);
            var btnGroups = $get(this.btnGroupsId);
            var btnChildrenRoles = $get(this.btnChildrenRolesId);
            var btnClone = $get(this.btnCloneId);
            if (btnAdd != null) {
                btnAdd.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Add]).get_enabled();
            }
            if (btnModify != null) {
                btnModify.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Rename]).get_enabled();
            }
            if (btnMove != null) {
                btnMove.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Move]).get_enabled();
            }
            if (btnPrint != null) {
                btnPrint.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Print]).get_enabled();
            }
            if (btnLog != null) {
                btnLog.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Log]).get_enabled();
            }
            if (btnFunction != null) {
                btnFunction.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Function]).get_enabled();
            }
            if (btnDelete != null) {
                btnDelete.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Delete]).get_enabled();
            }
            if (btnGroups != null) {
                btnGroups.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Groups]).get_enabled();
            }
            if (btnChildrenRoles != null) {
                btnChildrenRoles.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.ChildrenRoles]).get_enabled();
            }
            if (btnClone != null) {
                btnClone.disabled = !menu.findItemByValue(TbltSettoreOperation[TbltSettoreOperation.Clone]).get_enabled();
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
        return TbltSettore;
    }());
    return TbltSettore;
});
//# sourceMappingURL=TbltSettore.js.map