<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="RequestStatement.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.RequestStatement" %>

<%@ Register Src="~/UserControl/uscRequestStatement.ascx" TagPrefix="usc" TagName="uscRequestStatement" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
        <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow)
                    oWindow = window.radWindow;
                else if (window.frameElement && window.frameElement.radWindow)
                    oWindow = window.frameElement.radWindow;
                return oWindow;
            }
            function CloseModal(args) {
                // GetRadWindow().close();
                setTimeout(function () {
                    GetRadWindow().close(args);
                }, 0);
            }

        </script>
    </telerik:RadScriptBlock>
    <telerik:RadNotification ID="radNotification" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="delete" Title="Errore pagina" TitleIcon="none" AutoCloseDelay="0" Position="Center" />
    <telerik:RadNotification ID="radNotificationSuccess" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="ok" Title="Notifica" TitleIcon="none" AutoCloseDelay="0" Position="Center" />
    <telerik:RadPageLayout runat="server" ID="pnlRequestStatementCompliance" CssClass="dsw-panel" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow CssClass="dsw-panel-title" Style="margin-bottom: 2px;" ID="wfPanel" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="12" Style="margin-bottom: 10px;">
                        <b>Seleziona flusso di lavoro</b>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow HtmlTag="Div" Style="height: 100%;">
                <Content>
                    <asp:Label runat="server" ID="lblNoWorkflow"></asp:Label>
                    <telerik:RadDropDownList runat="server" ID="ddlWorkflows" Width="300px" Style="margin: 5px;" AutoPostBack="true" CausesValidation="false" />                                        
                    <usc:uscRequestStatement ID="uscRequestStatementId" runat="server" Required="true" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>    
</asp:Content>

<asp:Content ID="cphFooter" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" CausesValidation="false" Text="Conferma" />
</asp:Content>