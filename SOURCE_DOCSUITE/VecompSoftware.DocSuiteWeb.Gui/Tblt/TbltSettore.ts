/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import WindowHelper = require('App/Helpers/WindowHelper');
import TbltSettoreOperation = require('Tblt/TbltSettoreOperation');

class TbltSettore {
    radTreeViewRolesId: string;
    radWindowManagerRolesId: string;
    ajaxManagerId: string;
    folderToolBarId: string;
    showDisabled: string;


    private _folderToolBar: Telerik.Web.UI.RadToolBar;

    private static CREATE_OPTION: string = "create"
    private static MODIFY_OPTION: string = "modify"
    private static DELETE_OPTION: string = "delete"
    private static MOVE_OPTION: string = "move"
    private static CLONE_OPTION: string = "clone"
    private static PRINT_OPTION: string = "print"
    private static GROUPS_OPTION: string = "groups"
    private static LOG_OPTION: string = "log"
    private static FUNCTION_OPTION: string = "function"
    private static PROPAGATION_OPTION: string = "propagation"

    /**
     * Costruttore
     */
    constructor() {
        $(document).ready(() => {

        });
    }

    /**
     * Initialize
     */
    initialize(): void {
        this._folderToolBar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolBarId);
        let wndManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerRolesId);
        wndManager.getWindowByName('windowEditRoles').add_close(this.onCloseWindowEdit);
        wndManager.getWindowByName('windowGroupRoles').add_close(this.onCloseWindowGroup);
        wndManager.getWindowByName('windowRoles').add_close(this.onCloseWindowRoles);
        wndManager.getWindowByName('windowAddUsers').add_close(this.onCloseWindowAddUserInGroup);
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento alla chiusura della finestra di modifica
     * @param sender
     * @param args
     */
    onCloseWindowEdit = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument() !== null) {
            this.updateGroups(args.get_argument());
        }
    }

    /**
     * Evento alla chiusura della finestra di gestione gruppi
     * @param sender
     * @param args
     */
    onCloseWindowGroup = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        this.updateGroups();
    }
    /**
     * Evento alla chiusura della finestra di move settori
     * @param sender
     * @param args
     */
    onCloseWindowRoles = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        let ajaxManager: Telerik.Web.UI.RadAjaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        if (args.get_argument() !== null) {
            ajaxManager.ajaxRequest('move' + '|' + args.get_argument());
        }
    }

    /**
   * Evento alla chiusura della finestra di aggiunta utente
   * @param sender
   * @param args
   */
    onCloseWindowAddUserInGroup = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        let ajaxManager: Telerik.Web.UI.RadAjaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        if (args.get_argument() !== null) {
            ajaxManager.ajaxRequest('AddUser' + '|' + args.get_argument());
        }
    }
    /**
     * Evento scatenato al click di un elemento nel context menu
     * @param sender
     * @param args
     */
    onContextMenuItemClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeViewContextMenuEventArgs) => {
        let menuItem: Telerik.Web.UI.RadMenuItem = args.get_menuItem();
        let operation: TbltSettoreOperation = TbltSettoreOperation[menuItem.get_value()];
        switch (operation) {
            case TbltSettoreOperation.Add:
            case TbltSettoreOperation.Rename:
            case TbltSettoreOperation.Delete:
            case TbltSettoreOperation.Clone:
                this.openEditWindow('windowEditRoles', menuItem.get_value());
                break;
            case TbltSettoreOperation.Move:
                this.openRolesWindow();
                break;
            case TbltSettoreOperation.Print:
                this.openPrintWindow('windowPrintRoles');
                break;
            case TbltSettoreOperation.Groups:
                this.openGroupsWindow();
                break;
            case TbltSettoreOperation.Log:
                this.openLogWindow('windowLogRoles');
                break;
            case TbltSettoreOperation.Function:
                this.viewFunctionUsers();
                break;
            case TbltSettoreOperation.ChildrenRoles:
                this.loadChildrenRoles();
                break;
        }
    }

    /**
     * Evento scatenato in visualizzazione del context menu
     * @param sender
     * @param args
     */
    onContextMenuShowing = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeViewContextMenuEventArgs) => {

        let treeNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        let menu: Telerik.Web.UI.RadMenu = args.get_menu();
        treeNode.set_selected(true);
        let attributeRecovery: any = treeNode.get_attributes().getAttribute("Recovery");
        let isNodeRecovery: boolean
        if (attributeRecovery != null) {
            isNodeRecovery = !!JSON.parse(treeNode.get_attributes().getAttribute("Recovery"));
            isNodeRecovery ? (<any>menu.findItemByValue(TbltSettore.DELETE_OPTION)).set_text("Recupera") : (<any>menu.findItemByValue('Delete')).set_text("Elimina");
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
            this.alignButtons(menu);
            return;
        }

        let nodeType: string = treeNode.get_attributes().getAttribute("NodeType");
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

                this.disableButtons();
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
        this.alignButtons(menu);
    }

    disableButtons() {
        var btnAdd = this._folderToolBar.findItemByValue(TbltSettore.CREATE_OPTION);
        var btnModify = this._folderToolBar.findItemByValue(TbltSettore.MODIFY_OPTION);
        var btnPrint = this._folderToolBar.findItemByValue(TbltSettore.PRINT_OPTION);
        var btnLog = this._folderToolBar.findItemByValue(TbltSettore.LOG_OPTION);
        var btnFunction = this._folderToolBar.findItemByValue(TbltSettore.FUNCTION_OPTION);
        var btnDelete = this._folderToolBar.findItemByValue(TbltSettore.DELETE_OPTION);
        var btnGroups = this._folderToolBar.findItemByValue(TbltSettore.GROUPS_OPTION);
        var btnMove = this._folderToolBar.findItemByValue(TbltSettore.MOVE_OPTION);
        var btnClone = this._folderToolBar.findItemByValue(TbltSettore.CLONE_OPTION);

        if (btnAdd != null) btnAdd.set_enabled(false);
        if (btnModify != null) btnModify.set_enabled(false);
        if (btnPrint != null) btnPrint.set_enabled(false);
        if (btnLog != null) btnLog.set_enabled(false);
        if (btnFunction != null) btnFunction.set_enabled(false);
        if (btnDelete != null) btnDelete.set_enabled(false);
        if (btnGroups != null) btnGroups.set_enabled(false);
        if (btnMove != null) btnMove.set_enabled(false);
        if (btnClone != null) btnClone.set_enabled(false);
    }

    selectNode(nodeValue) {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let node: Telerik.Web.UI.RadTreeNode = treeView.findNodeByValue(nodeValue);
        node.select();
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Metodo di apertura finestra di modifica settore
     * @param name
     * @param operation
     */
    openEditWindow(name: string, operation: string): boolean {
        let parameters: string = "Action=".concat(operation.toString());

        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();

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
        let url: string = "../Tblt/TbltSettoreGes.aspx?Type=Comm&".concat(parameters);

        return this.openWindow(url, name, WindowHelper.WIDTH_EDIT_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
    }

    /**
     * Metodo di apertura finestra di visualizzazione log settore
     * @param name
     */
    openLogWindow(name: string): boolean {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let url: string = "../Tblt/TbltLog.aspx?Type=Comm&TableName=RoleGroup";
        if (selectedNode != null) {
            var attributes = selectedNode.get_attributes();
            url += "&entityUniqueId=".concat(attributes.getAttribute("UniqueId"));
        }

        return this.openWindow(url, name, WindowHelper.WIDTH_LOG_WINDOW, WindowHelper.HEIGHT_LOG_WINDOW);
    }

    /**
     * Metodo di apertura finestra di stampa settore
     * @param name
     */
    openPrintWindow(name: string): boolean {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let url: string = "../Comm/CommPrint.aspx?Type=Comm&PrintName=SingleRolePrint&IdRef=".concat(selectedNode.get_value());

        return this.openWindow(url, name, WindowHelper.WIDTH_PRINT_WINDOW, WindowHelper.HEIGHT_PRINT_WINDOW);
    }
    /**
     * Metodo di apertura finestra di gestione gruppi AD settore
     */
    openRolesWindow(): boolean {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let nodeType: string = selectedNode.get_attributes().getAttribute("NodeType");
        let url: string = "../UserControl/CommonSelSettori.aspx?Type=Comm&MultiSelect=False&RoleRestiction=None&Rights=&TenantEnabled=False&RightEnabled=false&RootSelectable=true&ConfirmSelection=true&isActive=".concat(this.showDisabled);
        return this.openWindow(url, "windowRoles", WindowHelper.WIDTH_GROUP_WINDOW, WindowHelper.HEIGHT_GROUP_WINDOW - 200);
    }
    /**
     * Metodo di apertura finestra di gestione gruppi AD settore
     */
    openGroupsWindow(): boolean {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let nodeType: string = selectedNode.get_attributes().getAttribute("NodeType");
        let url: string = "../Tblt/TbltSettoreGesGruppi.aspx?Type=Comm&IdRole=".concat(selectedNode.get_value());
        if (nodeType == "Group") {
            url = url.concat("&GroupName=", selectedNode.get_text());
        }

        return this.openWindow(url, "windowGroupRoles", WindowHelper.WIDTH_GROUP_WINDOW + 200, WindowHelper.HEIGHT_GROUP_WINDOW - 100);
    }

    /**
  * Apre la finestra di propagazione massiva
  * TODO: da rivedere
  * @param name
  */
    openPropagationWindow(name: string): boolean {
        let url: string = "../Tblt/TbltSettorePropagation.aspx?Type=ProtDB";
        return this.openWindow(url, name, WindowHelper.WIDTH_PRINT_WINDOW, WindowHelper.HEIGHT_PRINT_WINDOW);
    }

    /**
     * Metodo di apertura nuova finestra
     * @param url
     * @param name
     * @param width
     * @param height
     */
    openWindow(url: string, name: string, width: number, height: number): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerRolesId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.center();
        return false;
    }

    /**
     * Chiamata per la visualizzazione della lista utenti cofigurati nel disegno di funzione
     */
    viewFunctionUsers(): void {
        let ajaxManager: Telerik.Web.UI.RadAjaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        ajaxManager.ajaxRequest('showcollaboration');
    }

    /**
     * Metodo per settare la visibilità del pulsante di cancellazione
     * @param node
     * @param menu
     */
    checkDeleteItem(node: Telerik.Web.UI.RadTreeNode, menu: Telerik.Web.UI.RadMenu): boolean {
        var btnDelete = this._folderToolBar.findItemByValue(TbltSettore.DELETE_OPTION);
        node.get_nodes().forEach(function (item: Telerik.Web.UI.RadTreeNode) {
            let nodeType: string = item.get_attributes().getAttribute("NodeType").toUpperCase();
            if (nodeType != "GROUP") {
                let isNodeRecovery: boolean = !!JSON.parse(item.get_attributes().getAttribute("Recovery"));
                if (nodeType == "SUBROLE" && !isNodeRecovery) {
                    btnDelete.disable();
                    return false;
                }
            }
        });

        btnDelete.enable();
        return true;
    }

    /**
     * Metodo per allineare i pulsante al context menu
     * @param menu
     */
    alignButtons(menu): void {
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
    }


    /**
    * Chiamata Ajax per caricamento Settori figli del settore selezionato
    */
    loadChildrenRoles(): void {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        if (selectedNode != null && selectedNode.get_value() != "Root") {
            let nodeTypeId: string = selectedNode.get_value();
            let ajaxManager: Telerik.Web.UI.RadAjaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
            ajaxManager.ajaxRequest('loadChildrenRoles');
        }
    }

    /**
     * Chiamata Ajax per la modifica degli utenti di un settore
     * @param source
     */
    updateGroups(source?: any): void {
        let ajaxManager: Telerik.Web.UI.RadAjaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        if (source != null) {
            ajaxManager.ajaxRequest('Update' + '|' + source.Operation + '|' + source.ID);
        } else {
            ajaxManager.ajaxRequest('Update');
        }
    }

}

export = TbltSettore
