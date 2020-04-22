<%@ Page AutoEventWireup="false" CodeBehind="FascAutorizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascAutorizza" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Fascicolo - Autorizzazioni" %>

<%@ Register Src="../UserControl/uscFascicolo.ascx" TagName="uscFascicolo" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var fascAutorizza;
            require(["Fasc/FascAutorizza"], function (FascAutorizza) {
                $(function () {
                    fascAutorizza = new FascAutorizza(tenantModelConfiguration.serviceConfiguration);
                    fascAutorizza.currentFascicleId = "<%= IdFascicle %>";
                    fascAutorizza.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascAutorizza.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascAutorizza.currentPageId = "<%= Me.ClientID %>";
                    fascAutorizza.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascAutorizza.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascAutorizza.pageContentId = "<%= pageContent.ClientID %>";
                    fascAutorizza.actionPage = "<%= Action %>";
                    fascAutorizza.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascAutorizza.uscFascicoloId = "<%= uscFascicolo.PageContentDiv.ClientID %>";
                    fascAutorizza.btnConfirmId = "<%= btnConfirm.ClientID%>"
                    fascAutorizza.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Content>
                    <uc:uscFascicolo ID="uscFascicolo" runat="server" IsAuthorizePage="true" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" Text="Conferma modifica" CausesValidation="true" Width="150px" />
</asp:Content>

