<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscPecHistory.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscPecHistory" %>

<asp:Repeater ID="historyRepeater" runat="server">
    <HeaderTemplate>
    <table class="datatable">
        <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td style="width: 10px">
                <asp:Image ID="imgMailState" runat="server" />
            </td>
            <td style="width: 200px">
                <asp:Label ID="lblMailType" runat="server" />
            </td>
            <td style="width: 200px">
                <asp:Label ID="lblMailDate" runat="server" />
            </td>
            <td></td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody>
    </table>
    </FooterTemplate>
</asp:Repeater>
<asp:Label runat="server" ID="lblMessage" Visible="False" Text="-" />