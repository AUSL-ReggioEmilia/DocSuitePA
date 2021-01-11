<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscTemplateDocumentRepository.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscTemplateDocumentRepository" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscTemplateDocumentRepository;
        require(["UserControl/uscTemplateDocumentRepository"], function (UscTemplateDocumentRepository) {
            $(function () {
                uscTemplateDocumentRepository = new UscTemplateDocumentRepository(tenantModelConfiguration.serviceConfiguration);
                uscTemplateDocumentRepository.treeTemplateDocumentId = "<%= rtvTemplateDocument.ClientID %>";
                uscTemplateDocumentRepository.toolBarSearchId = "<%= ToolBarSearch.ClientID %>";
                uscTemplateDocumentRepository.toolBarTagsId = "<%= ToolBarTags.ClientID %>";
                uscTemplateDocumentRepository.racTagsDataSourceId = "<%= racTagsDataSource.ClientID %>";
                uscTemplateDocumentRepository.onlyPublishedTemplate = <%= OnlyPublishedTemplateSerializedValue %>;
                uscTemplateDocumentRepository.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscTemplateDocumentRepository.initialize();
            });
        });

        function treeView_ClientNodeClicked(sender, args) {
            uscTemplateDocumentRepository.treeView_ClientNodeClicked(sender, args);
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
    <Items>
        <telerik:RadToolBarButton Value="searchDescription">
            <ItemTemplate>
                <telerik:RadTextBox ID="txtTemplateName" EmptyMessage="Nome" runat="server" Width="150px" Style="margin-left: 3px;"></telerik:RadTextBox>
            </ItemTemplate>
        </telerik:RadToolBarButton>
        <telerik:RadToolBarButton IsSeparator="true" />
        <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
    </Items>
</telerik:RadToolBar>

<telerik:RadToolBar AutoPostBack="false" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%" RenderMode="Lightweight">
    <Items>
        <telerik:RadToolBarButton runat="server" ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
        <telerik:RadToolBarButton runat="server" ToolTip="Modifica" Value="modify" Text="Modifica" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
        <telerik:RadToolBarButton runat="server" ToolTip="Elimina" Value="delete" Text="Elimina" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
        <telerik:RadToolBarButton runat="server" ToolTip="Log" Value="log" Text="Log" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_log.png" />
        <telerik:RadToolBarButton runat="server" ToolTip="Visualizza" Value="view" Text="Visualizza" Enabled="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/watch.png" />
    </Items>
</telerik:RadToolBar>

<telerik:RadClientDataSource runat="server" ID="racTagsDataSource"></telerik:RadClientDataSource>
<telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="false" EnableShadows="false" ID="ToolBarTags" runat="server" Width="100%">
    <Items>
        <telerik:RadToolBarButton Value="searchTags">
            <ItemTemplate>
                <telerik:RadAutoCompleteBox ID="racTags" runat="server" Width="350" Style="margin-top: 3px; margin-left: 3px;"
                    EmptyMessage="Seleziona un Tag" AutoPostBack="false" ClientDataSourceID="racTagsDataSource"
                    AllowCustomEntry="False" DropDownHeight="250" DataTextField="value" DataValueField="id">
                </telerik:RadAutoCompleteBox>                
            </ItemTemplate>
        </telerik:RadToolBarButton>
    </Items>
</telerik:RadToolBar>

<telerik:RadTreeView ID="rtvTemplateDocument" OnClientNodeClicked="treeView_ClientNodeClicked" runat="server" Style="margin-top: 10px;">
    <Nodes>
        <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Deposito" Font-Bold="true" Value="" />
    </Nodes>
</telerik:RadTreeView>
      <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>