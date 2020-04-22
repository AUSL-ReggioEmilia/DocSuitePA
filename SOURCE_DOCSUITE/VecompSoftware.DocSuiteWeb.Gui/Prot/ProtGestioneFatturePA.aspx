<%@ Page Title="Gestione Fatture PA" Language="vb" ValidateRequest="false" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtGestioneFatturePA.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtGestioneFatturePA" %>

<%@ Register Src="../UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="grdProt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    LoadPageData();
                }
            }

            function LoadPageData() {
                InvoicePaPageRequest("loadData");
            }

            function InvoicePaPageRequest(action) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(action);
            }
        </script>
    </telerik:RadScriptBlock>
    <table id="filterTable" runat="server" class="dataform">
        <tr>
            <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                <b>Protocolli dal:</b>
            </td>
            <td style="vertical-align: middle; font-size: 8pt">
                <telerik:RadDatePicker ID="dateFrom" runat="server" />
                <span class="miniLabel">al:</span>
                <telerik:RadDatePicker ID="dateTo" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <grdProt:uscProtGrid ColumnRegistrationDateVisible="False" ID="uscProtocolGrid" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button runat="server" Width="150" ID="btnUpdateGrid" Text="Aggiorna" OnClientClick="return LoadPageData();" />
</asp:Content>
