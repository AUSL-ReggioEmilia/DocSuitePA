<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltServiceCategory"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Codebehind="TbltServiceCategory.aspx.vb"
    Title="Categorie Servizio" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=rwmDialogManager.ClientID %>");
                var oWnd = manager.open(url, name);
                oWnd.setSize(width, height);
                oWnd.center();
                return oWnd;
            }

            //Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
            function OpenEditWindow(name, operation) {
                var parameters = "Action=" + operation;
                var treeView = $find("<%= RadTreeViewServiceCategories.ClientID %>");
                var selectedNode = treeView.get_selectedNode();
                switch (operation) {
                    case "Rename":
                        parameters += "&idServiceCategory=" + selectedNode.get_value();
                        break;
                    case "Delete":
                        parameters += "&idServiceCategory=" + selectedNode.get_value();
                        break;
                }
                parameters += "&Type=Comm&DBName=ProtDB";
                var url = '<%= ResolveUrl("~/Tblt/TbltServiceCategoryGes.aspx?") %>' + parameters;
                OpenWindow(url, name, WIDTH_EDIT_WINDOW, HEIGHT_EDIT_WINDOW);
            }

            function getRadioSelectedValue(radioList) {
                var options = radioList.getElementsByTagName('input');
                for (var i = 0; i < options.length; i++) {
                    var opt = options[i];
                    if (opt.checked) {
                        return opt.value;
                    }
                }
                return null;
            }

            function OnClientClose(sender, args) {
                if (args.get_argument() !== null) {
                    UpdateServiceCategories();
                }
            }

            //Chiamata Ajax per ricaricare la treeview
            function UpdateServiceCategories() {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('Update');
            }

            //Handler del clik di apertura del ContextMenu
            function ContextMenuItemClicked(sender, args) {
                var menuItem = args.get_menuItem();
                OpenEditWindow('wdEditServiceCategory', menuItem.get_value());
            }

            //Handler apertura ContextMenu
            function ContextMenuShowing(sender, args) {
                var node = args.get_node();
                var menu = args.get_menu();
                node.set_selected(true);

                if (node.get_value() == 'Root') {
                    //ROOT
                    EnableItemByValue(menu, 'Add');
                    DisableItemByValue(menu, 'Rename');
                    DisableItemByValue(menu, 'Delete');
                } else {
                    //OBJECTS
                    DisableItemByValue(menu, 'Add');
                    EnableItemByValue(menu, 'Rename');
                    EnableItemByValue(menu, 'Delete');
                }

                AdjustButtons(menu);
            }

            //Allinea i pulsanti con il contextMenu
            function AdjustButtons(menu) {
                AdjustButtonWithItem('<%= FolderToolBar.FindItemByValue("create").ClientID %>', menu, 'Add');
                AdjustButtonWithItem('<%= FolderToolBar.FindItemByValue("modify").ClientID %>', menu, 'Rename');
                AdjustButtonWithItem('<%= FolderToolBar.FindItemByValue("delete").ClientID %>', menu, 'Delete');
            }

        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="rwmDialogManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="wdEditServiceCategory" OnClientClose="OnClientClose" runat="server" Title="Gestione Categorie Servizio" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%">
        <Items>
            <telerik:RadToolBarButton ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
            <telerik:RadToolBarButton ToolTip="Modifica" Value="modify" Text="Modifica" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
            <telerik:RadToolBarButton runat="server" ToolTip="Elimina" Value="delete" Text="Elimina" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
        </Items>
    </telerik:RadToolBar>
    <telerik:RadTreeView ID="RadTreeViewServiceCategories" runat="server" Width="100%">
        <ContextMenus>
            <telerik:RadTreeViewContextMenu ID="ContextMenuRoles" runat="server" CollapseAnimation-Type="none">
                <Items>
                    <telerik:RadMenuItem Width="200px" Value="Add" Text="Aggiungi...">
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Width="200px" Value="Rename" Text="Modifica...">
                    </telerik:RadMenuItem>
                    <telerik:RadMenuItem Width="200px" Value="Delete" Text="Elimina...">
                    </telerik:RadMenuItem>
                </Items>
            </telerik:RadTreeViewContextMenu>
        </ContextMenus>
        <Nodes>
            <telerik:RadTreeNode runat="server" Text="Categorie Servizio" Expanded="true" Value="Root">
            </telerik:RadTreeNode>
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>
