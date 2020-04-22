<%@ Page AutoEventWireup="false" CodeBehind="CommonSelContactOChart.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactOChart" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Contatto da Organigramma" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">

            function returnValuesJson(action, serializedContact, close) {
                var arg = new Object();
                arg.Action = action;
                arg.Contact = serializedContact;

                if (close == "true") {
                    closeWindow(arg);
                    return;
                }
                GetRadWindow().BrowserWindow.<%= QueryStringParentID%>_CloseDomain(arg);
            }

            function closeWindow(serializedContact) {
                GetRadWindow().close(serializedContact);
            }

        </script>
    </telerik:RadScriptBlock>

    <table style="border: 0; width: 100%;">
        <tr>
            <td class="SXScuro" style="text-align: right;">Organigramma:</td>
            <td class="DXChiaro">
                <asp:Panel ID="PanelProviders" runat="server" DefaultButton="ButtonReloadProviders"  Style="float: left;">
                    <asp:DropDownList ID="DropDownProviders" runat="server" AutoPostBack="true" Width="300px" />
                </asp:Panel>
                <asp:Button ID="ButtonReloadProviders" runat="server" Text="Aggiorna" Width="120px" />
            </td>
        </tr>
        <tr>
            <td class="SXScuro" style="text-align: right;">Ricerca:</td>
            <td class="DXChiaro">
                <asp:Panel DefaultButton="ButtonFilter" runat="server" Style="float: left;">
                    <asp:TextBox ID="TextFilter" runat="server" Width="300px" MaxLength="30" />
                </asp:Panel>
                <asp:Button ID="ButtonFilter" runat="server" Text="Ricerca" Width="120px" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Label ID="lblNoProviders" runat="server" Text="Nessun provider configurato." Visible="false" />
    <telerik:RadTreeView ID="TreeViewItems" runat="server" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button ID="ButtonConfirm" runat="server" Text="Conferma" Width="120px" />
</asp:Content>
