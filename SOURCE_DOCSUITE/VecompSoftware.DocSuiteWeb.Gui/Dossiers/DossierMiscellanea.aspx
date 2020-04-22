<%@ Page Title="Dossier - Inserti" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierMiscellanea.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierMiscellanea" %>

<%@ Register Src="~/UserControl/uscMiscellanea.ascx" TagName="uscMiscellanea" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var dossierMiscellanea;
            require(["Dossiers/DossierMiscellanea"], function (DossierMiscellanea) {
                $(function () {
                    dossierMiscellanea = new DossierMiscellanea(tenantModelConfiguration.serviceConfiguration);
                    dossierMiscellanea.currentDossierId = "<%= IdDossier %>";
                    dossierMiscellanea.locationId = "<%= ProtocolEnv.DossierMiscellaneaLocation%>";
                    dossierMiscellanea.ajaxManagerId = "<%= AjaxManager.ClientID %>";                    
                    dossierMiscellanea.currentPageId = "<%= Me.ClientID %>";
                    dossierMiscellanea.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    dossierMiscellanea.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierMiscellanea.pageContentId = "<%= pageContent.ClientID %>";
                    dossierMiscellanea.btnUploadDocumentId = "<%= btnUploadDocument.ClientID %>";                  
                    dossierMiscellanea.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";                    
                    dossierMiscellanea.uscMiscellaneaId = "<%= uscMiscellanea.PageContentDiv.ClientID %>";
                    dossierMiscellanea.radNotificationInfoId = "<%= radNotificationInfo.ClientID %>";                  
                    dossierMiscellanea.pnlButtonsId = "<%= pnlButtons.ClientID%>";
                    dossierMiscellanea.archiveName = "<%= ArchiveName%>";
                    dossierMiscellanea.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerCollegamenti" runat="server">
        <Windows>           
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadPageLayout runat="server" ID="pageContent" Height="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow Height="100%" HtmlTag="Div">
                <Content>
                    <uc:uscMiscellanea ID="uscMiscellanea" runat="server" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadNotification ID="radNotificationInfo" runat="server"
        VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="300" Height="150" Animation="FlyIn"
        EnableRoundedCorners="true" EnableShadow="true" ContentIcon="info" Title="Informazioni Miscellanea del Dossier" TitleIcon="none" AutoCloseDelay="4000" Position="Center" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel ID="pnlButtons" runat="server" Width="100%">        
        <asp:Panel runat="server">
            <telerik:RadButton ID="btnUploadDocument" runat="server" AutoPostBack="false" Width="150px" Text="Aggiungi"></telerik:RadButton>
        </asp:Panel>

    </asp:Panel>
</asp:Content>
