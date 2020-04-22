<%@ Page AutoEventWireup="false" CodeBehind="TbltOggetto.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltOggetto" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Oggetti" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">

        <script type="text/javascript">
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
                var treeView = $find("<%= RadTreeViewObjects.ClientID %>");
                var selectedNode = treeView.get_selectedNode();
                var parameters = "Action=" + operation + "&Type=Comm&DBName=" + getSelectedRadioButton();
                if (operation != 'Add') {
                    parameters += "&idObject=" + selectedNode.get_value();
                }                 
                var url = '<%= ResolveUrl("~/Tblt/TbltOggettoGes.aspx?") %>' + parameters;
                OpenWindow(url, name, WIDTH_EDIT_WINDOW, HEIGHT_EDIT_WINDOW);
            }

            function OnClientClose(sender, args) {
                if (args.get_argument() !== null) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest('Update');
                }
            }

            //Handler del clik di apertura del ContextMenu
            function ContextMenuItemClicked(sender, args) {
                var menuItem = args.get_menuItem();
                OpenEditWindow('wdEditObject', menuItem.get_value());
            }

            function getSelectedRadioButton() {
                return $("#<%= rblDocType.ClientID %> input:checked").val();
            }

            function hasSelectRadioButton() {
                return $("#<%= rblDocType.ClientID %> input").is(":checked");
            }

            //Handler apertura ContextMenu
            function ContextMenuShowing(sender, args) {
                var node = args.get_node();
                var menu = args.get_menu();
                node.set_selected(true);

                DisableItemByValue(menu, 'Add');
                DisableItemByValue(menu, 'Rename');
                DisableItemByValue(menu, 'Delete');

                if (hasSelectRadioButton()) {
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

    <telerik:RadWindowManager ID="rwmDialogManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="wdEditObject" OnClientClose="OnClientClose" runat="server" Title="Gestione Oggetto" />
        </Windows>
    </telerik:RadWindowManager>
    <table class="datatable">
        <tr>
            <td class="label col-dsw-2">Modulo:</td>
            <td>
                <asp:RadioButtonList AutoPostBack="True" Font-Size="11px" ID="rblDocType" RepeatDirection="Horizontal" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadTreeView ID="RadTreeViewObjects" runat="server" Height="100%">
        <ContextMenus>
            <telerik:RadTreeViewContextMenu CollapseAnimation-Type="none" ID="ContextMenuRoles" runat="server">
                <Items>
                    <telerik:RadMenuItem Width="200px" Value="Add" Text="Aggiungi..." />
                    <telerik:RadMenuItem Width="200px" Value="Rename" Text="Modifica..." />
                    <telerik:RadMenuItem Width="200px" Value="Delete" Text="Elimina..." />
                </Items>
            </telerik:RadTreeViewContextMenu>
        </ContextMenus>
        <Nodes>
            <telerik:RadTreeNode Expanded="true" runat="server" Text="Oggetti" Value="Root" />
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="btnAggiungi" Width="100px" OnClientClick="OpenEditWindow('wdEditObject','Add');" runat="server" Text="Aggiungi" UseSubmitBehavior="false" />
        <asp:Button ID="btnRinomina" Width="100px" OnClientClick="OpenEditWindow('wdEditObject','Rename');" runat="server" Text="Modifica" UseSubmitBehavior="false" />
        <asp:Button ID="btnElimina" Width="100px" OnClientClick="OpenEditWindow('wdEditObject','Delete');" runat="server" Text="Elimina" UseSubmitBehavior="false" />
    </asp:Panel>
</asp:Content>
