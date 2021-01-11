<%@ Page Title="Avvio fascicolo periodico" Language="vb" AutoEventWireup="false" CodeBehind="FascPeriodInserimento.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascPeriodInserimento" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscFascicleInsert.ascx" TagName="uscFascicleInsert" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script type="text/javascript">
            var fascPeriodInserimento;
            require(["Fasc/FascPeriodInserimento"], function (FascPeriodInserimento) {
                $(function () {
                    fascPeriodInserimento = new FascPeriodInserimento(tenantModelConfiguration.serviceConfiguration);
                    fascPeriodInserimento.currentPageId = "<%= pageContent.ClientID%>";
                    fascPeriodInserimento.btnConfermaId = "<%= btnConferma.ClientID%>";
                    fascPeriodInserimento.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascPeriodInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascPeriodInserimento.uscFascInsertId = "<%= uscFascicleInsert.PageContentDiv.ClientID %>";
                    fascPeriodInserimento.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascPeriodInserimento.initialize();
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
            <telerik:LayoutRow HtmlTag="Div" runat="server" ID="uscInsert">
                <Content>
                    <usc:uscFascicleInsert runat="server" ID="uscFascicleInsert" Required="false"/>
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
                    <telerik:RadButton Text="Conferma inserimento" runat="server" ID="btnConferma" CausesValidation="true" AutoPostBack="false" />
                    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False"></asp:ValidationSummary>
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>