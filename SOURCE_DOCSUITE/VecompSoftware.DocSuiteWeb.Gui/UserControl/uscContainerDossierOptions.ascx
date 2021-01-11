<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContainerDossierOptions.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContainerDossierOptions" %>

<%@ Register Src="~/UserControl/uscDossierFolders.ascx" TagName="uscDossierFolders" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMetadataRepositorySel.ascx" TagName="uscMetadataRepositorySel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscContainerDossierOptions;
        require(["UserControl/UscContainerDossierOptions"], function (UscContainerDossierOptions) {
            $(function () {
                uscContainerDossierOptions = new UscContainerDossierOptions(tenantModelConfiguration.serviceConfiguration);
                uscContainerDossierOptions.uscDossierFoldersId = "<%= uscDossierFolders.PageContentDiv.ClientID%>";
                uscContainerDossierOptions.btnConfirmDossierFoldersId = "<%= btnConfirmDossierFolders.ClientID %>";
                uscContainerDossierOptions.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientId %>";
                uscContainerDossierOptions.rtvMetadataId = "<%= rtvMetadata.ClientID %>";
                uscContainerDossierOptions.uscMetadataSelId = "<%= uscMetadataSel.PageContentDiv.ClientID %>";
                uscContainerDossierOptions.chkMetadataReadonlyId = "<%= chkMetadataReadonly.ClientID %>";
                uscContainerDossierOptions.tlbMetadataId = "<%= tlbMetadata.ClientID %>";
                uscContainerDossierOptions.splPageContentId = "<%= splPageContent.ClientID %>";
                uscContainerDossierOptions.initialize();
            });
        });

        function loadFolders(idContainer) {
            $(function () {
                setTimeout(function () {
                    var uscContainerDossierOptionsInstance = $("#<%= splPageContent.ClientID %>").data();
                    var uscDossierFoldersInstance = $("#<%= uscDossierFolders.PageContentDiv.ClientID %>").data();
                    if (jQuery.isEmptyObject(uscContainerDossierOptionsInstance) || jQuery.isEmptyObject(uscDossierFoldersInstance)) {
                        loadFolders(idContainer);
                        return;
                    }
                    uscContainerDossierOptions.loadFolders(idContainer);
                }, 200);                
            });
        }
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<telerik:RadSplitter runat="server" Orientation="Horizontal" ID="splPageContent">
    <telerik:RadPane Height="100%" runat="server">
        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="false" ExpandMode="MultipleExpandedItems" Width="100%">
            <Items>
                <telerik:RadPanelItem Text="Gestione alberatura dossier" Expanded="true" />
            </Items>
        </telerik:RadPanelBar>
        <usc:uscDossierFolders ID="uscDossierFolders" runat="server" PersistanceDisabled="True" HideFascicleAssociateButton="True" HideStatusToolbar="True" />
    </telerik:RadPane>
    <telerik:RadPane Height="250px" runat="server">
        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="false" ExpandMode="MultipleExpandedItems" Width="100%">
            <Items>
                <telerik:RadPanelItem Text="Gestione metadata" Expanded="true" />
            </Items>
        </telerik:RadPanelBar>
        <div style="padding: 2px;">
            <div>
                <b>Seleziona una tipologia di metadati:</b>
                <usc:uscMetadataRepositorySel runat="server" ID="uscMetadataSel" />
            </div>
            <div style="margin-top: 3px; margin-bottom: 10px;">
                <asp:CheckBox runat="server" ID="chkMetadataReadonly" Text="Forza selezione singolo metadata" />
            </div>
            <div>
                <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="tlbMetadata" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton ID="removeMetadata" ToolTip="Rimuovi metadata" Value="removeMetadata" CheckOnClick="false" ImageUrl="~/App_Themes/DocSuite2008/imgset16/cancel.png" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadTreeView runat="server" ID="rtvMetadata" Width="100%">
                    <Nodes>
                        <telerik:RadTreeNode Text="Metadati" Value="" Expanded="true" />
                    </Nodes>
                </telerik:RadTreeView>
            </div>
        </div>
    </telerik:RadPane>
    <telerik:RadSplitBar runat="server" EnableResize="false"></telerik:RadSplitBar>
    <telerik:RadPane Height="30px" runat="server">
        <asp:Button runat="server" ID="btnConfirmDossierFolders" Text="Conferma" OnClientClick="return uscContainerDossierOptions.saveDossierFolders();" Style="margin-left: 5px; margin-top: 5px;" />
    </telerik:RadPane>
</telerik:RadSplitter>