<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMetadataRepository.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMetadataRepository" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscMetadataRepository;
        require(["UserControl/uscMetadataRepository"], function (UscMetadataRepository) {
            $(function () {
                uscMetadataRepository = new UscMetadataRepository(tenantModelConfiguration.serviceConfiguration);
                uscMetadataRepository.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                uscMetadataRepository.rtvMetadataRepositoryId = "<%= rtvMetadataRepository.ClientID%>";
                uscMetadataRepository.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscMetadataRepository.toolBarSearchId = "<%= ToolBarSearch.ClientID%>";
                uscMetadataRepository.pageId = "<%= pageContentDiv.ClientID%>";
                uscMetadataRepository.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
    <Items>
        <telerik:RadToolBarButton Value="searchDescription" runat="server">
            <ItemTemplate>
                <telerik:RadTextBox ID="txtRepositoryName" EmptyMessage="Nome" runat="server" Width="150px" Style="margin-left: 3px;"></telerik:RadTextBox>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton IsSeparator="true" />
        <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
    </Items>
</telerik:RadToolBar>
<telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%">
    <Items>
        <telerik:RadToolBarButton ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
        <telerik:RadToolBarButton ToolTip="Modifica" Value="modify" Text="Modifica" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
        <telerik:RadToolBarButton runat="server" ToolTip="Elimina" Value="delete" Text="Elimina" Enabled="false" Visible="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
    </Items>
</telerik:RadToolBar>

<telerik:RadPageLayout runat="server" ID="pageContentDiv" Width="100%" HtmlTag="Div">
    <Rows>
        <telerik:LayoutRow>
            <Content>
                <telerik:RadTreeView ID="rtvMetadataRepository" runat="server">
                    <Nodes>
                        <telerik:RadTreeNode Expanded="true" runat="server" NodeType="Root" Selected="true" Text="Tipologie di metadati" Value="" />
                    </Nodes>
                </telerik:RadTreeView>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
