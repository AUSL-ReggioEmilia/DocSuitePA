<%@ Page AutoEventWireup="false" CodeBehind="ProtIncompleti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtIncompleti" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocolli Incompleti" %>

<%@ Register Src="../UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="dataform">
        <tr>
            <td class="label" style="width: 30%">Anno:</td>
            <td>
                <asp:Panel runat="server" ID="pnlCerca" DefaultButton="btnSearch" Style="display: inline;">
                    <telerik:RadNumericTextBox ID="txtYear" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="56px" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="Aggiorna" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscProtGrid ColumnClientSelectVisible="false" ColumnFullProtocolNumberVisible="True" ColumnPartialActionsVisible="True" ColumnViewDocumentsVisible="false" ColumnViewProtocolVisible="False" ID="uscProtocolGrid" runat="server" />
</asp:Content>
