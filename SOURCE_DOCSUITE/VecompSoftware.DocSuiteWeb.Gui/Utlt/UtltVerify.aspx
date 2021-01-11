<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltVerify" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Verifica Tabelle" Codebehind="UtltVerify.aspx.vb" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="Table">
        <tr>
            <td colspan="2">
                <asp:Button ID="btnFullPathRuoli" runat="server" Text="Verifica Settori"></asp:Button>
                <asp:Button ID="btnFullPathCategoria" runat="server" Text="Verifica Classificazione"></asp:Button>
                <asp:Button ID="btnFullPathRubrica" runat="server" Text="Verifica Rubrica"></asp:Button>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadAjaxPanel ID="pnlRisultati" runat="server" Width="100%" Height="100%">
        <table width="100%">
            <tr class="Spazio">
                <td></td>
            </tr>
            <tr width="100%">
                <td width="30%">
                    <asp:Label ID="lblDes" runat="server" Font-Bold="True"></asp:Label></td>
                <td width="70%">
                    <asp:Label ID="lblReport" runat="server"></asp:Label></td>
            </tr>
            <tr width="100%">
                <td width="30%">
                    <asp:Label ID="lblDes1" runat="server" Font-Bold="True"></asp:Label></td>
                <td width="70%">
                    <asp:Label ID="lblReport1" runat="server"></asp:Label></td>
            </tr>
        </table>
    </telerik:RadAjaxPanel>
</asp:Content>
