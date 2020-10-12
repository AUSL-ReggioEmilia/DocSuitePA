<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FascInserimento.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.FascInserimento" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Fascicolo - Inserimento" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscFascicleInsert.ascx" TagName="uscFascicleInsert" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var fascInserimento;
            require(["Fasc/FascInserimento"], function (FascInserimento) {
                $(function () {
                    fascInserimento = new FascInserimento(tenantModelConfiguration.serviceConfiguration);
                    fascInserimento.btnInserimentoId = "<%= btnInserimento.ClientID %>";
                    fascInserimento.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    fascInserimento.fasciclePageContentId = "<%= fasciclePageContent.ClientID %>";
                    fascInserimento.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascInserimento.radWindowManagerId = "<%= MasterDocSuite.DefaultWindowManager.ClientID %>";
                    fascInserimento.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    fascInserimento.currentUDId = "<%= IdDocumentUnit %>";
                    fascInserimento.environment = "<%= Environment %>";
                    fascInserimento.activityFascicleEnabled= JSON.parse("<%=ProtocolEnv.ActivityFascicleEnabled%>".toLowerCase());
                    fascInserimento.fasciclesPanelVisibilities = <%=FasciclesPanelVisibilities%>;
                    fascInserimento.currentIdUDSRepository = "<%= CurrentIdUDSRepository %>";
                    fascInserimento.uscFascInsertId = "<%= uscFascicleInsert.PageContentDiv.ClientID %>";   
                    fascInserimento.fascicleTypeRowId = "<%= fascicleTypeRow.ClientID %>";   
                    fascInserimento.initialize();
                });
            });

            function GetRadWindow() {
                return fascInserimento.getRadWindow();
            }

            function CloseWindow(value) {
                fascInserimento.closeWindow();
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadPageLayout runat="server" ID="fasciclePageContent" Width="100%" Height="100%" HtmlTag="Div">
        <Rows>
           <telerik:LayoutRow runat="server" id="fascicleTypeRow" HtmlTag="Div" CssClass="ts-initialize">
                <Content>
                    <usc:uscFascicleInsert runat="server" ID="uscFascicleInsert" Required="false"/>
                </Content>
            </telerik:LayoutRow>     
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnInserimento" runat="server" CausesValidation="true" Width="150px" Text="Conferma inserimento" />
    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False"></asp:ValidationSummary>
</asp:Content>
