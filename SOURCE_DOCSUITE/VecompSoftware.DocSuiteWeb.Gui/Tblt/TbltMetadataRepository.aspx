<%@ Page Title="Metadati" Language="vb" AutoEventWireup="false" CodeBehind="TbltMetadataRepository.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltMetadataRepository" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepository.ascx" TagName="uscMetadataRepository" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySummary.ascx" TagName="uscMetadataRepositorySummary" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var tbltMetadataRepository;
            require(["Tblt/TbltMetadataRepository"], function (TbltMetadataRepository) {
                $(function () {
                    tbltMetadataRepository = new TbltMetadataRepository(tenantModelConfiguration.serviceConfiguration);
                    tbltMetadataRepository.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                    tbltMetadataRepository.pnlRtvMetadataId = "<%= pnlRtvMetadata.ClientID%>";
                    tbltMetadataRepository.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                    tbltMetadataRepository.uscMetadataRepositoryId = "<%= uscMetadataRepository.PageContent.ClientID%>";
                    tbltMetadataRepository.uscMetadataRepositorySummaryId = "<%= uscMetadataRepositorySummary.PageContentDiv.ClientID%>";
                    tbltMetadataRepository.btnAggiungiId = "<%= btnAggiungi.ClientID%>"
                    tbltMetadataRepository.btnModificaId = "<%= btnModifica.ClientID%>"
                    tbltMetadataRepository.pageContentId = "<%= pageContent.ClientID%>"
                    tbltMetadataRepository.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="splitterWrapper" runat="server" id="pageContent">
        <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="false" Orientation="Vertical" ID="splPage">
            <telerik:RadPane runat="server" Height="100%" ID="pnlRtvMetadata" Width="25%">
                <usc:uscMetadataRepository runat="server" ID="uscMetadataRepository"></usc:uscMetadataRepository>
            </telerik:RadPane>
            <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
            <telerik:RadPane runat="server" ID="pnlDetaild" Width="75%">
                <usc:uscMetadataRepositorySummary runat="server" ID="uscMetadataRepositorySummary" />
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">

    <telerik:RadButton ID="btnAggiungi" runat="server" Text="Aggiungi" CausesValidation="false" AutoPostBack="false" />
    <telerik:RadButton ID="btnModifica" runat="server" Text="Modifica" Enabled="false" AutoPostBack="false" />
    <telerik:RadButton ID="btnElimina" runat="server" Text="Elimina" Enabled="false" AutoPostBack="false" Visible="false" />

</asp:Content>
