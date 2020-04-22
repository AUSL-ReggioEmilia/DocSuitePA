<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSearchADUser.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSearchADUser"%>

<div style="width: 100%;">
    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 30%;">Utente:
            </td>
            <td>
                <asp:Panel DefaultButton="btnSearch" runat="server" Style="display: inline;">
                    <asp:TextBox ID="txtFilter" runat="server" Width="200px" MaxLength="30" />
                </asp:Panel>
                <asp:DropDownList ID="lbMultiDomain" runat="server" Visible="false" />
                <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" />
            </td>
        </tr>
    </table>
</div>

<div style="width: 100%;">
    <telerik:RadTreeView ID="tvwContactDomain" runat="server" />
</div>





