<%@ Page Title="Procedimento - Aggiungi fascicolo" Language="vb" AutoEventWireup="false" CodeBehind="DossierFolderLinkFascicle.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierFolderLinkFascicle" %>

<%@ Register Src="../UserControl/uscFascicleLink.ascx" TagName="uscFascicleLink" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
        <script type="text/javascript">
            var dossierFolderLinkFascicle;
            require(["Dossiers/DossierFolderLinkFascicle"], function (DossierFolderLinkFascicle) {
                $(function () {
                    dossierFolderLinkFascicle = new DossierFolderLinkFascicle(tenantModelConfiguration.serviceConfiguration);
                    dossierFolderLinkFascicle.currentDossierId = "<%= IdDossier%>";
                    dossierFolderLinkFascicle.currentPageId = "<%= pageContent.ClientID%>";
                    dossierFolderLinkFascicle.lblNameId = "<%= lblName.ClientID%>";
                    dossierFolderLinkFascicle.btnConfermaId = "<%= btnConferma.ClientID%>";
                    dossierFolderLinkFascicle.btnConfermaUniqueId = "<%= btnConferma.UniqueID%>";
                    dossierFolderLinkFascicle.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                    dossierFolderLinkFascicle.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    dossierFolderLinkFascicle.uscFascLinkId = "<%= uscFascicleLink.PageContentDiv.ClientID %>";
                    dossierFolderLinkFascicle.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierFolderLinkFascicle.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierFolderLinkFascicle.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="cphContent">
       <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
        <telerik:RadFormDecorator RenderMode="Lightweight" ID="frmDecorator"  EnableRoundedCorners="false" runat="server" DecoratedControls="Fieldset"></telerik:RadFormDecorator>
    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%" Height="95%">
        <Rows>
            <telerik:LayoutRow CssClass="windowTitle">
                <Columns>
                    <telerik:LayoutColumn Span="2" Height="35px">
                        <b>Nome: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="10" CssClass="t-col-left-padding t-col-right-padding" Height="35px">
                       <asp:Label ID="lblName" runat="server"/>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <usc:uscFascicleLink runat="server" ID="uscFascicleLink" Caption="Classificatore" HeaderVisible="true" Multiple="false" Required="false" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadPageLayout runat="server" ID="RadPageLayout1" HtmlTag="Div" Width="100%" Height="100%">
        <Rows>
            <telerik:LayoutRow>
                <Content>
                    <telerik:RadButton Text="Conferma" runat="server" ID="btnConferma" AutoPostBack="false"/>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>