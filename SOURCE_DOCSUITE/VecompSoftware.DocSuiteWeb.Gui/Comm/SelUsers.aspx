<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SelUsers.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelUsers" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
        	
            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
            
            function ClientClickNode(sender, eventArgs) {
                var node = eventArgs.get_node();
                if (node.get_value() !== "") {
                    CloseWindow(node.get_value());
                }
            }
        </script>
    </telerik:RadScriptBlock>
    
    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 30%;">Utente:
            </td>
            <td>
                <asp:Panel DefaultButton="btnSearch" runat="server" Style="display: inline;">
                    <asp:TextBox ID="txtFilter" runat="server" Width="200px" MaxLength="30" />
                </asp:Panel>
                <asp:DropDownList ID="lbMultiDomain" runat="server" Visible="false" />
                <asp:Button ID="btnSearch" runat="server" Text="Ricerca" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView EnableViewState="false" ID="RadTreeUsers" OnClientNodeClicked="ClientClickNode" runat="server" />
</asp:Content>

