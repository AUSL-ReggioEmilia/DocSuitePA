<%@ Page Title="Inserimento metadati dinamici" Language="vb" AutoEventWireup="false" CodeBehind="TbltMetadataRepositoryDesigner.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltMetadataRepositoryDesigner" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositoryDesigner.ascx" TagName="uscMetadataRepositoryDesigner" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var tbltMetadataRepositoryDesigner;
            require(["Tblt/TbltMetadataRepositoryDesigner"], function (TbltMetadataRepositoryDesigner) {
                $(function () {
                    tbltMetadataRepositoryDesigner = new TbltMetadataRepositoryDesigner(tenantModelConfiguration.serviceConfiguration);
                    tbltMetadataRepositoryDesigner.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    tbltMetadataRepositoryDesigner.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                    tbltMetadataRepositoryDesigner.uscMetadataRepositoryDesignerId = "<%= uscMetadataRepositoryDesigner.PageContentDiv.ClientID%>";
                    tbltMetadataRepositoryDesigner.pageContentId = "<%= pnlContent.ClientID%>";
                    tbltMetadataRepositoryDesigner.btnPublishId = "<%= btnPublish.ClientID%>";
                    tbltMetadataRepositoryDesigner.btnDraftId = "<%= btnDraft.ClientID%>";
                    tbltMetadataRepositoryDesigner.metadataRepositoryId = "<%= IdMetadataRepository%>";
                    tbltMetadataRepositoryDesigner.isEditPage = JSON.parse("<%= IsEditPage %>".toLowerCase());
                    tbltMetadataRepositoryDesigner.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadPageLayout runat="server" ID="pnlContent">
        <Rows>
            <telerik:LayoutRow>
            </telerik:LayoutRow>
            <telerik:LayoutRow>
                <Content>
                    <usc:uscMetadataRepositoryDesigner runat="server" ID="uscMetadataRepositoryDesigner" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>



<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnDraft" runat="server" Text="Bozza" CausesValidation="false" AutoPostBack="false" />
    <telerik:RadButton ID="btnPublish" runat="server" Text="Conferma" CausesValidation="false" AutoPostBack="false" />
</asp:Content>
