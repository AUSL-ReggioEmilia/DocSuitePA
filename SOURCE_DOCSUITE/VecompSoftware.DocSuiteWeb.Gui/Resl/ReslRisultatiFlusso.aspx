<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslRisultatiFlusso.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslRisultatiFlusso" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscReslGrid.ascx" TagName="uscReslGrid" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReslGridBar.ascx" TagName="uscReslGridBar" TagPrefix="usc" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="cphHeader">
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function <%= Me.ID %>_OpenWindow(name, url, width, height) {
                var manager = $find("<%= RadWindowManagerRicercaFlusso.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            //richiamata quando la finestra rubrica viene chiusa
            function RefreshSearch() {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('UPDATE');
            }

        </script>
    </telerik:RadScriptBlock>
    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerRicercaFlusso" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowReport" ReloadOnShow="false" runat="server" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock2">
        <usc:uscReslGrid runat="server" ID="uscReslGrid" IsWorkflow="true" />
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="cphFooter">
    <usc:uscReslGridBar runat="server" ID="uscReslGridBar" HasWorkflow="true"></usc:uscReslGridBar>
</asp:Content>

