<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtRisultati"
     MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscProtGridBar.ascx" TagName="uscProtGridBar" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    ExecuteAjaxRequest("InitialPageLoad");
                }
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

            function ExecuteAjaxRequest(operationName) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(operationName);
            }
        </script>			
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <div style="height:100%">
        <uc1:uscProtGrid runat="server" id="uscProtocolGrid" ColumnRegistrationDateVisible="true"/>
    </div>
</asp:Content>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="cphFooter">
    <div style="width:100%;" align="right">
        <uc1:uscProtGridBar runat="server" id="uscProtocolGridBar"></uc1:uscProtGridBar> 
    </div>
    <div>
        <asp:PlaceHolder runat="server" ID="ReportButtons"></asp:PlaceHolder>
        <asp:CheckBox runat="server" ID="ReportCurrentPage" Text="Solo pagina corrente" Checked="True" Visible="False"/>
    </div>
</asp:Content>


