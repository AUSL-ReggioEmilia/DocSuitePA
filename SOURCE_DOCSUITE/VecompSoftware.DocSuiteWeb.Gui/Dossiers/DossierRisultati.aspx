<%@ Page Language="vb"  Title="Dossier - Risultati" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierRisultati.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierRisultati" %>

<%@ Register Src="~/UserControl/uscDossierGrid.ascx" TagName="uscDossierGrid" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">
<telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var dossierRisultati;
            require(["Dossiers/DossierRisultati"], function (DossierRisultati) {
                $(function () {
                    dossierRisultati = new DossierRisultati(tenantModelConfiguration.serviceConfiguration);
                    dossierRisultati.uscDossierGridId = "<%= uscDossierGrid.PageContentDiv.ClientID %>";
                    dossierRisultati.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierRisultati.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    dossierRisultati.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

     <telerik:RadPageLayout runat="server" ID="pageContent" Height="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow Height="100%" HtmlTag="Div">
                <Content>
                    <uc:uscDossierGrid runat="server" ID="uscDossierGrid" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>

       
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

</asp:Content>