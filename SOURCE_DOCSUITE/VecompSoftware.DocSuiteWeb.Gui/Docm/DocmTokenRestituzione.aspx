<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmTokenRestituzione" Codebehind="DocmTokenRestituzione.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Restituzione" %>

<%@ Register Src="~/UserControl/uscDocumentToken.ascx" TagName="UscDocumentToken" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <table width="100%">
        <tr>
            <td>
                <uc:UscDocumentToken runat="server" ID="uscDocumentToken" />
            </td>
        </tr>
    </table>
    <table class="datatable">
        <tr>
            <th colspan="2">
                Restituzione</th>
        </tr>
    </table>
    <table class="dataform">
        <tr>
            <td class="label" width="20%">
                Restituzione:</td>
            <td width="80%">
                <telerik:RadTextBox ID="txtResponse" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox></td>
        </tr>
    </table>
    <br />
    <br />
    <asp:Button ID="btnRestituzione" runat="server" Text="Restituzione"></asp:Button>
    <asp:TextBox ID="txtProgrIncremental" runat="server" Width="16px" AutoPostBack="True" CssClass="hiddenField"></asp:TextBox>
</asp:Content>

