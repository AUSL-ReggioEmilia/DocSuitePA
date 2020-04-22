<%@ Page Title="Avvia attività" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="StartWorkflow.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.StartWorkflow" %>

<%@ Register Src="~/UserControl/uscStartWorkflow.ascx" TagPrefix="usc" TagName="uscStartWorkflow" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var startWorkflow;
            require(["Workflows/StartWorkflow"], function (StartWorkflow) {
                $(function () {
                    startWorkflow = new StartWorkflow(tenantModelConfiguration.serviceConfiguration);
                    startWorkflow.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    startWorkflow.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    startWorkflow.radNotificationId = "<%= radNotification.ClientID %>";
                    startWorkflow.radNotificationSuccessId = "<%=radNotificationSuccess.ClientID %>";
                    startWorkflow.uscContentId = "<%= uscWorkflowId.PageContent.ClientID %>";
                    startWorkflow.uscStartWorkflowId = "<%= uscWorkflowId.ClientID %>";                    
                    startWorkflow.initialize();
                });
            });

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadNotification ID="radNotification" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="delete" Title="Errore pagina" TitleIcon="none" AutoCloseDelay="0" Position="Center" />
    <telerik:RadNotification ID="radNotificationSuccess" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="ok" Title="Notifica" TitleIcon="none" AutoCloseDelay="0" Position="Center" />
    <telerik:RadPageLayout runat="server" ID="pnlWorkflow" HtmlTag="Div" Height="100%">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div" Style="height:100%;">
                <Content>
                    <usc:uscStartWorkflow ID="uscWorkflowId" runat="server" Required="true" />
                </Content>
            </telerik:LayoutRow>
        </Rows>

    </telerik:RadPageLayout>
</asp:Content>

