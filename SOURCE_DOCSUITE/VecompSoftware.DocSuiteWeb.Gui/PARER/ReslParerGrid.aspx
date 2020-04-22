<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReslParerGrid.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslParerGrid" %>

<%@ Register Src="~/UserControl/uscReslGrid.ascx" TagName="uscReslGrid" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script type="text/javascript">

            function OpenParerDetail(id) {
                var wnd = window.radopen("<%=ParerDetailUrl() %>?Type=Resl&Id=" + id, "parerDetailWindow");
                wnd.setSize(400, 300);
                wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.center();
                return false;
            }
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
             
        </script>
    </telerik:RadScriptBlock>
        
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="true">
        <usc:uscReslGrid runat="server" id="uscReslGrid" />
    </telerik:RadScriptBlock>
</asp:Content>