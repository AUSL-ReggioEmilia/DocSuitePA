<%@ Page Title="Dossier - Visualizza" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierVisualizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierVisualizza" %>

<%@ Register Src="~/UserControl/uscDossier.ascx" TagName="uscDossier" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscDossierFolders.ascx" TagName="uscDossierFolders" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var dossierVisualizza;
            require(["Dossiers/DossierVisualizza"], function (DossierVisualizza) {
                $(function () {
                    dossierVisualizza = new DossierVisualizza(tenantModelConfiguration.serviceConfiguration);
                    dossierVisualizza.currentDossierId = "<%= IdDossier %>";
                    dossierVisualizza.currentDossierTitle = "<%= DossierTitle %>";
                    dossierVisualizza.splContentId = "<%= splContent.ClientID %>";
                    dossierVisualizza.currentUser = <%= CurrentUser %>;
                    dossierVisualizza.currentPageId = "<%= Me.ClientID %>";
                    dossierVisualizza.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    dossierVisualizza.uscDossierId = "<%= uscDossier.PageContentDiv.ClientID %>";
                    dossierVisualizza.uscDossierFoldersId = "<%= uscDossierFolders.PageContentDiv.ClientID %>";
                    dossierVisualizza.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    dossierVisualizza.btnDocumentiId = "<%=btnDocumenti.ClientID%>";
                    dossierVisualizza.btnModificaId = "<%=btnModifica.ClientID%>";
                    dossierVisualizza.btnSendToRolesId = "<%=btnSendToRoles.ClientID%>";
                    dossierVisualizza.btnSendToSecretariesId = "<%=btnSendToSecretaries.ClientID%>";
                    dossierVisualizza.btnCloseId = "<%=btnClose.ClientID%>";
                    dossierVisualizza.btnAutorizzaId = "<%=btnAutorizza.ClientID%>";
                    dossierVisualizza.btnAvviaWorkflowId = "<%=btnWorkflow.ClientID%>";
                    dossierVisualizza.windowCompleteWorkflowId = "<%= windowCompleteWorkflow.ClientID %>";
                    dossierVisualizza.windowStartWorkflowId = "<%= windowStartWorkflow.ClientID %>";
                    dossierVisualizza.btnLogId = "<%=btnLog.ClientID%>";
                    dossierVisualizza.btnInsertiId = "<%=btnInserti.ClientID%>";
                    dossierVisualizza.miscellaneaLocationEnabled = JSON.parse("<%= IsMiscellaneaLocationEnabled %>".toLowerCase());
                    dossierVisualizza.actionPage = "<%= Action %>";
                    dossierVisualizza.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierVisualizza.radWindowManagerCollegamentiId = "<%= RadWindowManagerCollegamenti.ClientID %>";
                    dossierVisualizza.fascPaneId = "<%= fascPane.ClientID%>";
                    dossierVisualizza.dossierPageId = "<%= dossierPane.ClientID %>";
                    dossierVisualizza.btnCompleteWorkflowId = "<%= btnCompleteWorkflow.ClientID%>";
                    dossierVisualizza.workflowActivityId = "<%= IdWorkflowActivity%>";
                    dossierVisualizza.workflowEnabled = "<%= WorkflowEnabled%>";
                    dossierVisualizza.sendToSecretariesEnabled = "<%= DossierSendToSecretariesEnabled%>";
                    dossierVisualizza.radNotificationId = "<%= radNotificationInfo.ClientID%>";
                    dossierVisualizza.currentFascicleId = "<%= IdFascicle %>";
                    dossierVisualizza.isWindowPopupEnable = <%= IsWindowPopupEnable.ToString().ToLower() %>;
                    dossierVisualizza.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerCollegamenti" runat="server">
        <Windows>
            <telerik:RadWindow Height="450" ID="windowStartWorkflow" runat="server" Title="Avvia attività" Width="600" />
            <telerik:RadWindow Height="450" ID="windowCompleteWorkflow" runat="server" Title="Stato attività" Width="600" />
        </Windows>
    </telerik:RadWindowManager>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadNotification ID="radNotificationInfo" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="300" Height="150" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni Dossier" TitleIcon="none" AutoCloseDelay="4000" Position="Center" />

    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">
            <telerik:RadPane runat="server" Collapsed="false" Width="380" MaxWidth="380">
                <telerik:RadPageLayout runat="server" ID="RadPageLayout1" HtmlTag="Div">
                    <Rows>
                        <telerik:LayoutRow Height="100%" HtmlTag="Div">
                            <Content>
                                <uc:uscDossierFolders ID="uscDossierFolders" runat="server" />
                            </Content>
                        </telerik:LayoutRow>
                    </Rows>
                </telerik:RadPageLayout>
            </telerik:RadPane>
            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
            <telerik:RadPane runat="server" ID="dossierPane" Collapsed="true">
                <telerik:RadPageLayout runat="server" ID="pageContent" Height="100%" HtmlTag="Div">
                    <Rows>
                        <telerik:LayoutRow Height="100%" HtmlTag="Div" Style="padding-bottom: 30px;">
                            <Content>
                                <uc:uscDossier ID="uscDossier" runat="server"/>                                
                            </Content>
                        </telerik:LayoutRow>  
                        <telerik:LayoutRow Height="30px" Style="bottom: 0; position: absolute;">
                            <Content>
                                <asp:Panel runat="server" ID="footer" CssClass=".footer-buttons-wrapper">
                                    <telerik:RadButton ID="btnDocumenti" runat="server" Width="150px" Text="Documenti" AutoPostBack="false" d></telerik:RadButton>
                                    <telerik:RadButton ID="btnModifica" runat="server" Width="150px" Text="Modifica" AutoPostBack="false"></telerik:RadButton>
                                    <telerik:RadButton ID="btnSendToRoles" runat="server" Width="150px" Text="Invia settori"></telerik:RadButton>
                                    <telerik:RadButton ID="btnSendToSecretaries" runat="server" Width="150px" Text="Invia segreterie"></telerik:RadButton>
                                    <telerik:RadButton ID="btnClose" runat="server" Width="150px" Text="Chiudi" Visible="false"></telerik:RadButton>
                                    <telerik:RadButton ID="btnAutorizza" runat="server" Width="150px" Text="Autorizza" AutoPostBack="false" />
                                    <telerik:RadButton ID="btnWorkflow" runat="server" Width="150px" Text="Avvia attività" AutoPostBack="false" Visible="false" />
                                    <telerik:RadButton ID="btnCompleteWorkflow" runat="server" Width="150" Text="Stato attività" Visible="false" />
                                    <telerik:RadButton ID="btnInserti" runat="server" Width="150px" Text="Inserti" AutoPostBack="false"></telerik:RadButton>
                                    <telerik:RadButton ID="btnLog" runat="server" Width="150px" Text="Log"></telerik:RadButton>
                                </asp:Panel>
                            </Content>
                        </telerik:LayoutRow>
                    </Rows>
                </telerik:RadPageLayout>
            </telerik:RadPane>
            <telerik:RadPane runat="server" ID="fascPane" Collapsed="true">
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>

