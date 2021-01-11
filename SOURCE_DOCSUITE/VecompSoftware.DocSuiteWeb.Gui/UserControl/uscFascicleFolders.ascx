<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascicleFolders.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascicleFolders" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscFascicleFolders;
        require(["UserControl/uscFascicleFolders"], function (UscFascicleFolders) {
            $(function () {
                uscFascicleFolders = new UscFascicleFolders(tenantModelConfiguration.serviceConfiguration);
                uscFascicleFolders.pageId = "<%= pageContent.ClientID%>";
                uscFascicleFolders.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                uscFascicleFolders.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscFascicleFolders.treeFascicleFoldersId = "<%= rtvFascicleFolders.ClientID%>";
                uscFascicleFolders.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscFascicleFolders.folderToolBarId = "<%= FolderToolBar.ClientID%>";
                uscFascicleFolders.managerWindowsId = "<%= manager.ClientID %>";
                uscFascicleFolders.managerCreateFolderId = "<%= managerCreateFolder.ClientID %>";
                uscFascicleFolders.managerModifyFolderId = "<%= managerModifyFolder.ClientID%>";
                uscFascicleFolders.managerMoveFolderId = "<%= managerMoveFolder.ClientID %>";
                uscFascicleFolders.managerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscFascicleFolders.btnExpandFascicleFoldersId = "<%= btnExpandFascicleFolders.ClientID %>";
                uscFascicleFolders.pnlFascicleFolderId = "<%= pnlFascicleFolder.ClientID %>";
                uscFascicleFolders.isVisible = JSON.parse("<%= IsVisibile %>".toLowerCase()); 
                uscFascicleFolders.foldersToDisabled = <%= FoldersToDisabledSerialized %>;
                uscFascicleFolders.viewOnlyFolders = <%= ViewOnlyFolders.ToString().ToLower() %>;
                uscFascicleFolders.doNotUpdateDatabase = "<%= DoNotUpdateDatabase %>";
                uscFascicleFolders.scannerLightRestEnabled = "<%= ScannerLightRestEnabled%>";
                uscFascicleFolders.initialize();

                new ResizeSensor($("#pnlMainFascicleFolder")[0], function () {
                     var currentWidth = $("#pnlMainFascicleFolder").width();
                     $("#<%= pnlTitle.ClientID %>").width(currentWidth -3);
                     $("#<%= pnlFolderToolbar.ClientID %>").width(currentWidth);
                });
            });
        });

        function treeView_ClientNodeClicked(sender, args) {
            uscFascicleFolders.treeView_ClientNodeClicked(sender, args);
        }

        function treeView_ClientNodeExpanding(sender, args) {
            uscFascicleFolders.treeView_ClientNodeExpanding(sender, args);
        }

        function treeView_ClientNodeClicking(sender, args) {
            uscFascicleFolders.treeView_ClientNodeClicking(sender, args);
        }

        function uscFascicleFolders_hideStickifyControls() {
            $("#<%= pnlTitle.ClientID %>").css("display", "none");
            $("#<%= pnlFolderToolbar.ClientID %>").css("display", "none");
        }

        function uscFascicleFolders_showStickifyControls() {
            $("#<%= pnlTitle.ClientID %>").css("display", "block");
            $("#<%= pnlFolderToolbar.ClientID %>").css("display", "block");
        }

        function uscFascicleFolders_refreshStickifyPosition(position) {
            $("#<%= pnlTitle.ClientID %>").css({ top: position.top });
            $("#<%= pnlFolderToolbar.ClientID %>").css({ top: position.top });
        }
    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="300" ID="managerCreateFolder" runat="server" Title="Crea nuova cartella" Width="150" />
        <telerik:RadWindow Height="300" ID="managerModifyFolder" runat="server" Title="Modifica cartella" Width="150" />
        <telerik:RadWindow Height="300" ID="managerMoveFolder" runat="server" Title="Sposta cartella" Width="150" />
        <telerik:RadWindow Height="300" ID="managerUploadFile" runat="server" Title="Caricare un file" Width="150" />
        <telerik:RadWindow Height="460" ID="managerScannerDocument" runat="server" Title="Scansione documento" Width="800" />
    </Windows>
</telerik:RadWindowManager>

<div class="dsw-panel fascicle-folders-panel" id="pnlMainFascicleFolder" style="overflow-y: auto;">
    <div class="dsw-panel-title" runat="server" id="pnlTitle">
        Cartelle del fascicolo
        <telerik:RadButton ID="btnExpandFascicleFolders" CssClass="dsw-vertical-middle" runat="server" Width="16px" Height="16px" Visible="true">
            <Image EnableImageButton="true" />
        </telerik:RadButton>
    </div>
    <div runat="server" id="pnlFolderToolbar">
        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%">
            <Items>
                <telerik:RadToolBarButton ToolTip="Crea cartella" CheckOnClick="false" Checked="false" Value="createFolder" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                <telerik:RadToolBarButton ToolTip="Modifica cartella" CheckOnClick="false" Checked="false" Value="modifyFolder" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                <telerik:RadToolBarButton ToolTip="Elimina cartella" CheckOnClick="false" Checked="false" Value="deleteFolder" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
                <telerik:RadToolBarButton ToolTip="Sposta cartella" CheckOnClick="false" Checked="false" Value="moveFolder" ImageUrl="~/App_Themes/DocSuite2008/imgset16/linked_folder.png" />
                <telerik:RadToolBarButton ToolTip="Ricaricare cartella" CheckOnClick="false" Checked="false" Value="refreshFolder" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Activity_16x.png" />
            
                <telerik:RadToolBarButton ToolTip="Carica documento da scanner" CheckOnClick="false" Checked="false" Value="scanner" ImageUrl="~/App_Themes/DocSuite2008/imgset16/scanner.png" style="display: none;" />
                <telerik:RadToolBarButton ToolTip="Caricare un file" CheckOnClick="false" Checked="false" Value="uploadFile" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" style="display: none;"/>
            
            </Items>
        </telerik:RadToolBar>
    </div>
    <div id="pnlFascicleFolder" runat="server" class="dsw-panel-content">
        <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
            <Rows>
                <telerik:LayoutRow HtmlTag="Div">
                    <Content>
                        <div class="treeViewWrapper">
                            <telerik:RadTreeView runat="server" 
                                ID="rtvFascicleFolders" 
                                OnClientNodeClicked="treeView_ClientNodeClicked"
                                OnClientNodeClicking="treeView_ClientNodeClicking"
                                OnClientNodeExpanding="treeView_ClientNodeExpanding">
                                <Nodes>
                                    <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Fascicolo" Value="" />
                                </Nodes>
                            </telerik:RadTreeView>
                        </div>
                    </Content>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
    </div>
</div>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
