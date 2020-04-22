/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import WindowHelper = require('App/Helpers/WindowHelper');
import TbltSettoreOperation = require('Tblt/TbltSettoreOperation');

class TbltSettore {
    radTreeViewRolesId: string;
    radWindowManagerRolesId: string;
    ajaxManagerId: string;
    showDisabled: string;
    ajaxLoadingPanelId: string;
    pnlDetails: string;
    pnlInformations: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

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
        let wndManager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerRolesId);
        wndManager.getWindowByName('windowEditRoles').add_close(this.onCloseWindowEdit);
        wndManager.getWindowByName('windowRoles').add_close(this.onCloseWindowRoles);
        wndManager.getWindowByName('windowAddUsers').add_close(this.onCloseWindowAddUserInGroup);

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
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
            this.hideLoadingSpinner();
        }
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
        }

        if (isNodeRecovery) {
            return;
        }

        let nodeType: string = treeNode.get_attributes().getAttribute("NodeType");
        //let isNodeFather: boolean 
        switch (nodeType) {
            case "Role":
            case "SubRole":
                menu.findItemByValue('Clone').enable();

                for (let childNode of treeNode.get_allNodes()) {
                    if (childNode.get_attributes().getAttribute("NodeType") == "SubRole") {
                        break;
                    }
                }
                break;
            case "Group":
                break;
            case "Root":
                break;
            default:
                break;
        }
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
     * Metodo di apertura finestra di visualizzazione history settore
     */
    openHistoryWindow(): boolean {
        let treeView: Telerik.Web.UI.RadTreeView = <Telerik.Web.UI.RadTreeView>$find(this.radTreeViewRolesId);
        let selectedNode: Telerik.Web.UI.RadTreeNode = treeView.get_selectedNode();
        let nodeType: string = selectedNode.get_attributes().getAttribute("NodeType");
        let url: string = "../Tblt/TbltRoleHistory.aspx?Type=ProtDB&IdRole=".concat(selectedNode.get_value());
        return this.openWindow(url, "windowHistory", WindowHelper.WIDTH_GROUP_WINDOW, WindowHelper.HEIGHT_GROUP_WINDOW);
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
        node.get_nodes().forEach(function (item: Telerik.Web.UI.RadTreeNode) {
            let nodeType: string = item.get_attributes().getAttribute("NodeType").toUpperCase();
            if (nodeType != "GROUP") {
                let isNodeRecovery: boolean = !!JSON.parse(item.get_attributes().getAttribute("Recovery"));
                if (nodeType == "SUBROLE" && !isNodeRecovery) {
                    return false;
                }
            }
        });
        return true;
    }

    /**
     * Chiamata Ajax per la modifica degli utenti di un settore
     * @param source
     */
    updateGroups(source?: any): void {
        this._loadingPanel.show(this.pnlDetails);
        let ajaxManager: Telerik.Web.UI.RadAjaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        if (source != null) {
            ajaxManager.ajaxRequest('Update' + '|' + source.Operation + '|' + source.ID);
        } else {
            ajaxManager.ajaxRequest('Update');
        }
    }   

    public hideLoadingSpinner(): void {
        this._loadingPanel.hide(this.pnlDetails);
    }
}

export = TbltSettore
