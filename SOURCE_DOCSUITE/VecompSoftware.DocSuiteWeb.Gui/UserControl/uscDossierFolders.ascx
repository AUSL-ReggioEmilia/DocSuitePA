<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDossierFolders.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDossierFolders" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscDossierFolders;
        require(["UserControl/uscDossierFolders"], function (UscDossierFolders) {
            $(function () {
                uscDossierFolders = new UscDossierFolders(tenantModelConfiguration.serviceConfiguration);
                uscDossierFolders.pageId = "<%= pageContent.ClientID%>";
                uscDossierFolders.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                uscDossierFolders.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscDossierFolders.treeDossierFoldersId = "<%= rtvDossierFolders.ClientID%>";
                uscDossierFolders.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscDossierFolders.statusToolBarId = "<%= StatusToolBar.ClientID%>";
                uscDossierFolders.folderToolBarId = "<%= FolderToolBar.ClientID%>";
                uscDossierFolders.managerWindowsId = "<%= manager.ClientID %>";
                uscDossierFolders.managerCreateFolderId = "<%= managerCreateFolder.ClientID %>";
                uscDossierFolders.managerFascicleLinkId = "<%= managerFascicleLink.ClientID %>";
                uscDossierFolders.managerModifyFolderId = "<%= managerModifyFolder.ClientID%>";
                uscDossierFolders.managerModifyFascicle = "<%=managerModifyFascicle.ClientID%>";
                uscDossierFolders.managerCreateFascicleFolderId = "<%=managerCreateFascicleFolder.ClientID%>";
                uscDossierFolders.managerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscDossierFolders.persistanceDisabled = <%= PersistanceDisabled.ToString().ToLower() %>;
                uscDossierFolders.hideFascicleAssociateButton = <%= HideFascicleAssociateButton.ToString().ToLower() %>;
                uscDossierFolders.hideStatusToolbar = <%= HideStatusToolbar.ToString().ToLower() %>;
                uscDossierFolders.processEnabled = <%= ProtocolEnv.ProcessEnabled.ToString().ToLower() %>;
                uscDossierFolders.isWindowPopupEnable = <%= IsWindowPopupEnable.ToString().ToLower() %>;
                uscDossierFolders.btnSelectDossierFolderId = "<%= btnSelectDossierFolder.ClientID %>";
                uscDossierFolders.fascicleModifyButtonEnable = <%= FascicleModifyButtonEnable.ToString().ToLower() %>;
                uscDossierFolders.initialize();
            });
        });

        function treeView_ClientNodeClicked(sender, args) {
            uscDossierFolders.treeView_ClientNodeClicked(sender, args);
        }

        function treeView_ClientNodeExpanding(sender, args) {
            uscDossierFolders.treeView_ClientNodeExpanding(sender, args);
        }

    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="600" ID="managerCreateFolder" runat="server" Title="Crea nuova cartella" Width="750" />
        <telerik:RadWindow Height="600" ID="managerCreateFascicleFolder" runat="server" Title="Crea nuova cartella con fascicolo" Width="750" />
        <telerik:RadWindow Height="600" ID="managerFascicleLink" runat="server" Title="Aggiungi fascicolo" Width="750" />
        <telerik:RadWindow Height="600" ID="managerModifyFolder" runat="server" Title="Modifica cartella" Width="750" />
        <telerik:RadWindow Height="600" ID="managerModifyFascicle" runat="server" Title="Modifica fascicolo" Width="750" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton ToolTip="Crea cartella" style="display:none;" CheckOnClick="false" Checked="false" Value="createFolder" ImageUrl="../App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                        <telerik:RadToolBarButton ToolTip="Modifica cartella" style="display:none;" CheckOnClick="false" Checked="false" Value="modifyFolder" ImageUrl="../App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                        <telerik:RadToolBarButton ToolTip="Elimina cartella" style="display:none;" CheckOnClick="false" Checked="false" Value="deleteFolder" ImageUrl="../App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
                        <telerik:RadToolBarButton ToolTip="Crea Fascicolo" style="display:none;" CheckOnClick="false" Checked="false" Value="createFascicle" ImageUrl="../App_Themes/DocSuite2008/imgset16/fascicle_open.png" />
                        <telerik:RadToolBarButton ToolTip="Aggiungi fascicolo" style="display:none;" CheckOnClick="false" Checked="false" Value="addFascicle" ImageUrl="../App_Themes/DocSuite2008/imgset16/add_fascicle.png" />
                        <telerik:RadToolBarButton ToolTip="Modifica fascicolo" style="display:none;" CheckOnClick="false" Checked="false" Value="modifyFascicle" ImageUrl="../App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                        <telerik:RadToolBarButton ToolTip="Rimuovi fascicolo" style="display:none;" CheckOnClick="false" Checked="false" Value="removeFascicle" ImageUrl="../App_Themes/DocSuite2008/imgset16/remove_fascicle.png" />
                        <telerik:RadToolBarButton ToolTip="Ricaricare cartella" CheckOnClick="false" Checked="false" Value="refreshDossierFolders" ImageUrl="../App_Themes/DocSuite2008/imgset16/Activity_16x.png" />
                        <telerik:RadToolBarButton ToolTip="Crea fascicolo da template" style="display:none;" CheckOnClick="false" Checked="false" Value="createFascicleFromTemplate" ImageUrl="../App_Themes/DocSuite2008/imgset16/clone.png" />
                    </Items>
                </telerik:RadToolBar>
                <telerik:RadToolBar CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="StatusToolBar" runat="server" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton>
                            <ItemTemplate>
                                <label>Cartella:</label>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton Text="Da gestire" CheckOnClick="true" Group="manage" Checked="false" Value="InProgress" AllowSelfUnCheck="true" />
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton Text="Fascicoli" CheckOnClick="true" Group="fascicle" Checked="false" Value="Fascicle" AllowSelfUnCheck="true" />
                        <telerik:RadToolBarButton IsSeparator="true" />
                        <telerik:RadToolBarButton Text="Fascicoli chiusi" CheckOnClick="true" Group="folder" Checked="false" Value="FascicleClose" AllowSelfUnCheck="true" />
                    </Items>
                </telerik:RadToolBar>
            </Content>
        </telerik:LayoutRow>
    </Rows>
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <div class="treeViewWrapper">
                    <telerik:RadTreeView runat="server" ID="rtvDossierFolders" OnClientNodeClicked="treeView_ClientNodeClicked" OnClientNodeExpanding="treeView_ClientNodeExpanding">
                        <Nodes>
                            <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Dossier" Value="" />
                        </Nodes>
                    </telerik:RadTreeView>
                </div>
            </Content>
        </telerik:LayoutRow>
    </Rows>
    <Rows>
        <telerik:LayoutRow HtmlTag="Div" style="position: fixed; bottom: 1em;">
            <Content>
                <telerik:RadButton runat="server" ID="btnSelectDossierFolder" AutoPostBack="false" Text="Seleziona cartella" Visible="false" />
            </Content>
        </telerik:LayoutRow>
    </Rows>

</telerik:RadPageLayout>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
