<%@ Page Title="Recupero Errori" AutoEventWireup="false" CodeBehind="ProtRecupera.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtRecupera" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function ToggleSelection(p_checked) {
                var grid = document.getElementById("<%= uscProtocolGrid.Grid.ClientID %>");
                for (var i = 0; i < grid.getElementsByTagName("INPUT").length; i++) {
                    grid.getElementsByTagName("INPUT")[i].checked = p_checked;
                }
            }

            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }
        </script>
    </telerik:RadScriptBlock>
    <table class="datatable">
        <tr>
            <td style="width: 20%"></td>
            <td style="width: 80%; vertical-align: middle;">
                <asp:Panel runat="server" ID="pnlCerca" DefaultButton="btnSearch" Style="display: inline;">
                    <telerik:RadNumericTextBox Label="Anno:" LabelCssClass="label" ID="txtYear" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" runat="server" Width="156px" />
                    <asp:CheckBox ID="cbShowAll" runat="server" font-Bold="true" Text="Visualizza tutti" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="Aggiorna" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscProtGrid runat="server" ID="uscProtocolGrid" ColumnActionsVisible="True" ColumnViewDocumentsVisible="False" ColumnViewProtocolVisible="false" ColumnFullProtocolNumberVisible="true" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnAnnulla" runat="server">
        <table id="tblAnnullamento" class="datatable" runat="server">
            <tr>
                <th style="width: 885px;">Estremi del provvedimento di annullamento dei Protocolli
                </th>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkDisableUnlinkPec" runat="server" Text="Scollegare anche le PEC dal protocollo." Checked="false" Enabled="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadTextBox ID="txtAnnulla" runat="server" Width="100%" />
                </td>
            </tr>
        </table>
        <asp:Button ID="btnSelectAll" runat="server" Text="Seleziona tutti" Width="120px" CausesValidation="False" OnClientClick="ToggleSelection(true); return false;" />
        <asp:Button ID="btnDeselectAll" runat="server" Text="Deseleziona tutti" Width="120px" CausesValidation="False" OnClientClick="ToggleSelection(false); return false;" />
        <asp:Button ID="btnCancel" runat="server" Text="Annulla Protocolli" />
    </asp:Panel>
</asp:Content>
