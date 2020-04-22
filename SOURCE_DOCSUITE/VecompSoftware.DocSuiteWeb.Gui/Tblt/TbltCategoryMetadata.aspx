<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltCategoryMetadata.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltCategoryMetadata" %>

<%@ Register Src="~/UserControl/uscMetadataRepository.ascx" TagName="uscMetadataRepository" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            var tbltCategoryMetadata;
            require(["Tblt/TbltCategoryMetadata"], function (TbltCategoryMetadata) {
                $(function () {
                    tbltCategoryMetadata = new TbltCategoryMetadata(tenantModelConfiguration.serviceConfiguration);
                    tbltCategoryMetadata.btnSubmitId = "<%= btnSubmit.ClientID %>";
                    tbltCategoryMetadata.btnRemoveId = "<%= btnRemove.ClientID %>";
                    tbltCategoryMetadata.uscMetadataRepositoryId = "<%= uscMetadataRepository.PageContent.ClientID %>";
                    tbltCategoryMetadata.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    tbltCategoryMetadata.categoryId = "<%= CategoryId %>";
                    tbltCategoryMetadata.metadataRepositoryId = "<%= MetadataRepositoryId %>";
                    tbltCategoryMetadata.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltCategoryMetadata.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    tbltCategoryMetadata.initialize();
                });
        });
        </script>
    </telerik:RadScriptBlock>

    <div class="radGridWrapper">
        <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
        <usc:uscMetadataRepository runat="server" ID="uscMetadataRepository" />
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <div class="footer-buttons-wrapper">
        <telerik:RadButton Width="150" runat="server" ID="btnSubmit" Text="Conferma" />    
        <telerik:RadButton Width="150" runat="server" ID="btnRemove" Text="Rimuovi associazione" />       
    </div>
</asp:Content>
