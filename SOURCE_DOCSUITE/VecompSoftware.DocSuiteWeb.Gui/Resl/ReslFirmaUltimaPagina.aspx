<%@ Page Language="vb" AutoEventWireup="false" Title="Firma ultima pagina" CodeBehind="ReslFirmaUltimaPagina.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslFirmaUltimaPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbFDQMultiple" EnableViewState="false">
        <script type="text/javascript">
            var currentLoadingPanel = null;
            var currentUpdatedControl = null;
            function RequestStart(sender, args) {
                if (currentLoadingPanel != null) {
                    return false;
                }
                currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= pnlPageContainer.ClientID%>";
                //show the loading panel over the updated control
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function ResponseEnd() {
                //hide the loading panel and clean up the global variables
                if (currentLoadingPanel != null) {
                    currentLoadingPanel.hide(currentUpdatedControl);
                }
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    ExecuteAjaxRequest("INITIALPAGELOAD");
                }
            }

            function SaveDocuments() {
                RequestStart();
                ExecuteAjaxRequest("SAVEDOCUMENTS");
            }

            function ExecuteAjaxRequest(operationName) {
                $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>").ajaxRequest(operationName);
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="dataform">
        <asp:Panel ID="pnlType" runat="server">
            <tr id="trType" runat="server">
            <td class="label" style="width: 30%;">Tipologia:
            </td>
            <td class="DXChiaro">
                <asp:RadioButtonList ID="rblAtto" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" Visible="true"></asp:RadioButtonList>
            </td>
        </tr>
        </asp:Panel>        
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblAdoptionDateFrom" Text="Adottate dal"></asp:Label>
            </td>
            <td style="width: 80%">
                <telerik:RadDatePicker ID="rdpAdoptionDateFrom" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblAdoptionDateTo" Text="Adottate al"></asp:Label>
            </td>
            <td style="width: 80%">
                <telerik:RadDatePicker ID="rdpAdoptionDateTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblDateFrom" Text="Pubblicate dal"></asp:Label>
            </td>
            <td style="width: 80%">
                <telerik:RadDatePicker ID="rdpDateFrom" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblDateTo" Text="Pubblicate al"></asp:Label>
            </td>
            <td style="width: 80%">
                <telerik:RadDatePicker ID="rdpDateTo" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label runat="server" ID="lblContainer" Text="Filtra per contenitore:"></asp:Label>
            </td>
            <td style="width: 80%">
                <telerik:RadDropDownList AutoPostBack="True" ID="ddlReslContainer" runat="server" Width="400px" EnableVirtualScrolling="true" DropDownHeight="200px"/>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">&nbsp;      
            </td>
            <td style="width: 80%">
                <asp:Button ID="btnSearch" runat="server" Width="100px" Text="Ricerca"></asp:Button>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlPageContainer" Height="100%">
        <asp:Table class="datatable" ID="tblResolutionsToSign" runat="server"></asp:Table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnSign" style="margin-left: 3px;" OnClientClick="RequestStart();" runat="server" Text="Firma documenti"></asp:Button>
    <asp:ValidationSummary runat="server" ID="validSummary" DisplayMode="List" ShowMessageBox="true" ShowSummary="false" />
</asp:Content>
