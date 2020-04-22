<%@ Page Title="Collega fascicolo" Language="vb" AutoEventWireup="false" CodeBehind="FascAddUDLink.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascAddUDLink" %>
<%@ Register Src="../UserControl/uscFascicleLink.ascx" TagName="uscFascicleLink" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
        <script type="text/javascript">
            var fascAddUDLink;
            require(["Fasc/FascAddUDLink"], function (FascAddUDLink) {
                $(function () {
                    fascAddUDLink = new FascAddUDLink(tenantModelConfiguration.serviceConfiguration);                   
                    fascAddUDLink.currentPageId = "<%= pageContent.ClientID%>";
                    fascAddUDLink.btnConfermaId = "<%= btnConferma.ClientID%>";
                    fascAddUDLink.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                    fascAddUDLink.managerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascAddUDLink.uscFascLinkId = "<%= uscFascicleLink.PageContentDiv.ClientID %>";
                    fascAddUDLink.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascAddUDLink.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascAddUDLink.initialize();
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
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <usc:uscFascicleLink runat="server" ID="uscFascicleLink" Caption="Classificatore" HeaderVisible="true" Multiple="false" Required="false" ViewOnlyFascicolable="true"/>
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