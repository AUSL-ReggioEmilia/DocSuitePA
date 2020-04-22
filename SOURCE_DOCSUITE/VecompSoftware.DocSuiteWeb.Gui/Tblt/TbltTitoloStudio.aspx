<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltTitoloStudio.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTitoloStudio" Title="Titolo Studio" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
            function OpenEditWindow(name, operation) {
                var treeView = $find("<%= RadTreeView1.ClientID %>");
                var selectedNode = treeView.get_selectedNode();

                var url = "../Tblt/TbltTitoloStudioGes.aspx?Type=Comm&Action=" + operation;
                switch (operation) {
                    case "Rename":
                    case "Delete":
                    case "Recovery":
                        url += "&idObject=" + selectedNode.get_value();
                        break;
                }

                OpenWindow(url, name, WIDTH_EDIT_WINDOW, HEIGHT_EDIT_WINDOW);
            }

            //Apre una finestra generale
            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=RadWindowManager.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                wnd.add_close(CloseEdit);
                return false;
            }

            function CloseEdit(sender, args) {
                if (args.get_argument() !== null) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest('Update');
                }
            }

            //Handler del clik di apertura del ContextMenu
            function ContextMenuItemClicked(sender, args) {
                var menuItem = args.get_menuItem();
                OpenEditWindow('windowEdit', menuItem.get_value());
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
                    var isActive = node.get_attributes().getAttribute("isActive");

                    if (isActive == "1") {
                        //TITLE (enabled)
                        EnableItemByValue(menu, 'Add');
                        EnableItemByValue(menu, 'Rename');
                        EnableItemByValue(menu, 'Delete');
                        DisableItemByValue(menu, 'Recovery');
                    } else {
                        //TITLE (disabled)
                        EnableItemByValue(menu, 'Add');
                        EnableItemByValue(menu, 'Rename');
                        EnableItemByValue(menu, 'Recovery');
                        DisableItemByValue(menu, 'Delete');
                    }
                }
                AdjustButtons(menu);
            }

            //Allinea i pulsanti con il contextMenu
            function AdjustButtons(menu) {
                AdjustButtonWithItem('<%= btnAggiungi.ClientID %>', menu, 'Add');
                AdjustButtonWithItem('<%= btnRinomina.ClientID %>', menu, 'Rename');
                AdjustButtonWithItem('<%= btnElimina.ClientID %>', menu, 'Delete');
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowEdit" ReloadOnShow="false" runat="server" Title="Gestione Titolo di Studio" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="RadTreeView1" runat="server">
        <Nodes>
            <telerik:RadTreeNode runat="server" Text="Titoli Studio" Expanded="true" Value="Root" />
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="btnAggiungi" OnClientClick="OpenEditWindow('windowEdit','Add');return false;" runat="server" Text="Aggiungi" />
        <asp:Button ID="btnRinomina" OnClientClick="OpenEditWindow('windowEdit','Rename');return false;" runat="server" Text="Modifica" />
        <asp:Button ID="btnElimina" OnClientClick="OpenEditWindow('windowEdit','Delete');return false;" runat="server" Text="Elimina" />
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
    </asp:Panel>
</asp:Content>
