<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslRisultati" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscReslGrid.ascx" TagName="uscReslGrid" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscReslGridBar.ascx" TagName="uscReslGridBar" TagPrefix="usc" %>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="False">
        <script language="javascript" type="text/javascript">
            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    ExecuteAjaxRequest("InitialPageLoad");
                }
            }

            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
            	
            function CloseWindow(argument)
            {
                var oWindow = GetRadWindow();
                oWindow.close(argument);    
            }

            function ExecuteAjaxRequest(operationName) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(operationName);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="true">
        <div style="height:100%">
            <usc:uscReslGrid runat="server" id="uscReslGrid" />        
        </div>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <div style="text-align: right; width: 100%">
        <usc:uscReslGridBar runat="server" ID="uscReslGridBar" />
    </div>
</asp:Content>
