<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSelTemplateDocumentRepository.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelTemplateDocumentRepository" %>

<%@ Register Src="~/UserControl/uscTemplateDocumentRepository.ascx" TagName="TemplateDocumentRepository" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var commonSelTemplateDocumentRepository;
            require(["UserControl/CommonSelTemplateDocumentRepository"], function (CommonSelTemplateDocumentRepository) {
                $(function () {
                    commonSelTemplateDocumentRepository = new CommonSelTemplateDocumentRepository();
                    commonSelTemplateDocumentRepository.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    commonSelTemplateDocumentRepository.uscTemplateDocumentRepositoryId = "<%= uscTemplateDocumentRepository.TreeTemplateDocumentRepository.ClientID %>";
                    commonSelTemplateDocumentRepository.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    commonSelTemplateDocumentRepository.pnlPageId = "<%= pnlPage.ClientID %>";
                    commonSelTemplateDocumentRepository.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    commonSelTemplateDocumentRepository.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
     <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <asp:Panel runat="server" ID="pnlPage" Height="100%">
        <usc:TemplateDocumentRepository runat="server" ID="uscTemplateDocumentRepository" OnlyPublishedTemplate="True"></usc:TemplateDocumentRepository>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnConfirm" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>
