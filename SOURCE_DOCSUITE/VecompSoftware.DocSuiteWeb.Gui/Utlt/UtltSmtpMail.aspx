<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltSmtpMail" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Test SmtpMail" Codebehind="UtltSmtpMail.aspx.vb" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <asp:Panel ID="pnlRicerca" runat="server">
        <table class="datatable">
            <tr>
                <td style="width:20%">
                </td>
                <td width="80%">
                </td>
            </tr>
            <tr>
                <td class="label">
                    MailSmtpServer:&nbsp;</td>
                <td>
                    <asp:TextBox ID="txtMailSmtpServer" runat="server" Width="300px" MaxLength="100"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" style="height: 28px">
                    UserErrorMailFrom:&nbsp;</td>
                <td style="height: 28px">
                    <asp:TextBox ID="txtUserErrorMailFrom" runat="server" Width="300px" MaxLength="200"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" style="height: 28px">
                    UserErrorMailTo:&nbsp;</td>
                <td style="height: 28px">
                    <asp:TextBox ID="txtUserErrorMailTo" runat="server" Width="300px" MaxLength="200"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br class="Spazio" />
        <asp:Button ID="btnInvia" runat="server" Text="Invia"></asp:Button>
    </asp:Panel>
</asp:Content>
