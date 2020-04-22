<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DocmChiusuraApertura.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmChiusuraApertura" %>

<%@ Register Src="../UserControl/uscDocument.ascx" TagName="uscDocument" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscDocument ID="UscDocument1" runat="server" />
    <br />
    <table id="Table1" class="datatable">
        <tr>
            <th align="left" colspan="2">
                <asp:Label ID="lblAzione" runat="server"></asp:Label></th>
        </tr>
        <tr>
            <td class="label" style="width:20%;vertical-align:middle;">
                <b>Note:</b></td>
            <td width="80%">
                <telerik:RadTextBox ID="txtNote" runat="server" Width="100%" MaxLength="255"></telerik:RadTextBox></td>
        </tr>
        <asp:Panel ID="pnlChiusura" runat="server">
            <tr>
                <td class="label" style="width:20%;vertical-align:middle;">
                    <b>Data Chiusura:</b></td>
                <td width="80%">
                    <telerik:RadDatePicker ID="RadDatePicker1" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="RadDatePicker1" ErrorMessage="Data Chiusura Obbligatoria" ID="rfvClosedDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <asp:Button ID="btnConfermaChiusura" runat="server" Text="Conferma"></asp:Button></td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlRiapertura" runat="server">
            <tr>
                <td class="label" style="width:20%;vertical-align:middle;">
                    <b>Data Riapertura:</b></td>
                <td width="80%">
                    <telerik:RadDatePicker ID="RadDatePicker2" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="RadDatePicker2" ErrorMessage="Data Riapertura Obbligatoria" ID="rfvReOpenDate" runat="server" />
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <asp:Button ID="btnConfermaRiapertura" runat="server" Text="Conferma"></asp:Button></td>
            </tr>
        </asp:Panel>
    </table>
</asp:Content>