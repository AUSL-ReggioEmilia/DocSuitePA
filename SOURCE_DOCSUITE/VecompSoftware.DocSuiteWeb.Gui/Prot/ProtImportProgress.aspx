<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ProtImportProgress.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtImportProgress" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProgressBar.ascx" TagName="uscProgressBar" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
            
            function CloseWindow(returnValue)
            {
                setTimeout(function () {
                    var oWindow = GetRadWindow();
                    oWindow.close(returnValue);
                }, 500);                
            }       
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <usc:uscProgressBar ID="uscProgressBarImpot" runat="server" />
                </Content>                
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>    
</asp:Content>
