<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonSelSettori.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelSettori" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Settori" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            
            // restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function ReturnValues() {
                $get('<%= txtIdRole.ClientID %>').value = "";
                
                var treeView = $find('<%= RadTreeSettori.ClientID %>');
                GetCheckedNodes(treeView.get_nodes().getNode(0));

                var roles = $get('<%= txtIdRole.ClientID %>').value;
                var values = roles.slice(0, -1).split(';');
                var tenant = $get('<%= lbMultiDomain.ClientID %>')
                var userName = $get('<%= lblSearchUser.ClientID %>')
                if (tenant != null) {
                    var tenantId = tenant.value;
                    values = values + "|tenantId=" + tenantId;
                }
                if (userName != null) {
                    var account = userName.innerText;
                    values = values + "|userName=" + account;
                }
                CloseWindow(values);
            }

            function GetCheckedNodes(node) {
                if (node.get_nodes().get_count() == 0) {
                    return true;
                }
                for (var i = 0; i < node.get_nodes().get_count(); i++) {
                    var myNode = node.get_nodes().getNode(i);
                    if (myNode.get_checked()) {
                        $get('<%= txtIdRole.ClientID %>').value += myNode.get_value() + ";";
                    }
                    GetCheckedNodes(myNode);
                }
            }

            function <%= Me.ID%>_OpenWindowOLD(url, name, closeFunction) {
                var wnd;
                if (name === "windowSelContactManualSimpleMode" ) {
                    wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.SMALL);
                } else if (name === "windowImportContact") {
                    wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.DOCUMENTS);
                } else {
                    wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.NORMAL);
                }

                wnd.add_close(closeFunction);

                return false;
            }

            function ReturnValueOnClick(sender, args) {
                var rootSelectable = (<%= Convert.ToInt16(RootSelectable)%> == 1);
                var confirmSelection = (<%= Convert.ToInt16(ConfirmSelection)%> == 1);
                var values = new Array();
                values[0] = args.get_node().get_value();
                var selectable = args.get_node().get_attributes().getAttribute("Selectable");
                if (( (rootSelectable && values[0] == "Root" ) || values[0] != "Root") && selectable == "TRUE") {
                    if ((confirmSelection && confirm("Vuoi confermare?")) || ! confirmSelection) {
                        CloseWindow(values);
                    }
                }
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
            
            //richiamata quando la finestra contatti AD viene Chiusa
            function  <%= Me.ID %>_CloseDomain(sender, args) {
                sender.remove_close(<%= Me.ID %>_CloseDomain);
                if (args.get_argument() !== null) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("<%= Me.ClientID %>" + "|" + args.get_argument());
                }
            }

            function OpenUsersWindow() {
                var treeView = $find('<%= RadTreeSettori.ClientID %>');
                var node = treeView.get_selectedNode();
                if (node != null) {
                    var url = "../Comm/CommUtenti.aspx?Type=Comm&IDRole=" + node.get_value() + "&TenantId=" + node.get_attributes().getAttribute("TenantId");
                    var manager = $find("<%=RadWindowManagerSettoriUtenti.ClientID %>");
                    var wnd = manager.open(url, 'windowUsers');
                    wnd.setSize(400, 350);
                    wnd.center();
                }
                return false;
            }

            function CheckAllNodes(p_value, p_nodes) {
                var nodes;
                if (!p_nodes) {
                    var treeView = $find('<%= RadTreeSettori.ClientID %>');
                    nodes = treeView.get_nodes();
                } else {
                    nodes = p_nodes;
                }
                var count = nodes.get_count();
                for (var i = 0; i < count; i++) {
                    var node = nodes.getNode(i);
                    if (p_value) {
                        node.check();
                    } else {
                        node.uncheck();
                    }

                    var hasChildren = node.get_nodes().get_count() > 0;
                    if (hasChildren) CheckAllNodes(p_value, node.get_nodes());
                }
            }
        </script>

    </telerik:RadScriptBlock>
    
    <table class="dataform">
        <tr>
            <td class="label" style="width: 20%">
                Descrizione:
            </td>
            <td>
                <asp:Panel runat="server" ID="pnlCerca" DefaultButton="btnSearch" Style="display: inline;">
                    <asp:TextBox ID="txtFiltraSettori" runat="server" Width="250px" />
                    <asp:DropDownList ID="lbMultiDomain" runat="server" Visible="false" Width="80px"  />
                </asp:Panel>
                <asp:Button ID="btnSearch" runat="server" Text="Cerca" ToolTip="Ricerca per Descrizione" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                Codice Ricerca:&nbsp;
            </td>
            <td>
                <asp:Panel runat="server" ID="pnlSearchCode" DefaultButton="btnSearchCode" Style="display: inline;">
                    <asp:TextBox ID="txtSearchCode" MaxLength="10" runat="server" Width="280px" />
                </asp:Panel>
                <asp:Button ID="btnSearchCode" runat="server" Text="Cerca e Seleziona" ToolTip="Ricerca per Codice e Seleziona" />
            </td>
        </tr>
        <tr id="tr_SearchByUser" runat="server" visible="false">
            <td class="label" style="width: 20%">
                Ricerca per utente:&nbsp;
            </td>
            <td>
                <asp:Panel runat="server" ID="pnlSearchAccount" DefaultButton="btnSearchCode" Style="display: inline;">
                    <asp:Label runat="server" ID="lblSearchUser" text="" />
                    <asp:ImageButton CausesValidation="False" ID="btnSelContactDomain" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" runat="server" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerSettoriUtenti" runat="server">
        <Windows>
            <telerik:RadWindow Height="400" ID="windowUsers" ReloadOnShow="false" runat="server" Title="Utenti del gruppo" Width="600" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <input type="button" id="btnCheckTutti" value="Seleziona tutti" onclick="CheckAllNodes(true);" />
    &nbsp;
    <input type="button" id="btnUncheckTutti" value="Deseleziona tutti" onclick="CheckAllNodes(false);" />
    <br/>
    <%-- Treeview Contatti --%>
    <telerik:RadTreeView EnableViewState="false" ID="RadTreeSettori" LoadingMessage="Caricamento..." LoadingStatusPosition="BelowNodeText" runat="server" Width="100%">
        <Nodes>
            <telerik:RadTreeNode Checkable="false" EnableViewState="false" Expanded="false" ExpandMode="ServerSideCallBack" Font-Bold="true" runat="server" Text="Settori" Value="Root" />
        </Nodes>
    </telerik:RadTreeView>
 </asp:Content>
 
 <asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <table style="width: 100%">
        <tr>
            <td>
                <asp:Button ID="btnConferma" runat="server" Text="Conferma Selezione" />

                <asp:TextBox runat="server" ID="txtIdRole" CssClass="hiddenField" />
            </td>
            <td style="text-align:right">
                <asp:Button ID="btnUtenti" runat="server" Text="Utenti" />
            </td>
        </tr>
    </table>
 </asp:Content>
 