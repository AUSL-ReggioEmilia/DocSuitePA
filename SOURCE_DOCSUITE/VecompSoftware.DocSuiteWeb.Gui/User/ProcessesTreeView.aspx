<%@ Page Title="Vista archivistica" Language="vb" AutoEventWireup="false" CodeBehind="ProcessesTreeView.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProcessesTreeView" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock" EnableViewState="false">
        <script type="text/javascript">
            var processesTreeView;
            require(["User/ProcessesTreeView"], function (ProcessesTreeView) {
                processesTreeView = new ProcessesTreeView(tenantModelConfiguration.serviceConfiguration);
                processesTreeView.categoryTreeViewId = "<%= categoriesTreeView.ClientID %>";
                processesTreeView.detailsPaneId = "<%= detailsPane.ClientID %>";
                processesTreeView.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                processesTreeView.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                processesTreeView.showOnlyMyProcesses = "<%= ShowOnlyHasRight %>";
                processesTreeView.currentTenantAOOId = "<%= CurrentTenant.TenantAOO.UniqueId %>";
                processesTreeView.actionToolbarId = "<%= actionsToolbar.ClientID %>";
                processesTreeView.windowManagerId = "<%= RadWindowManager.ClientID %>";
                processesTreeView.windowMoveFascId = "<%= windowMoveFasc.ClientID %>";
                processesTreeView.splitterMainId = "<%= splitterMain.ClientID %>";
                processesTreeView.treeViewNodesPageSize = <%= ProtocolEnv.TreeViewNodesPageSize %>;
                processesTreeView.initialize();
            });

            function treeView_LoadNodeChildrenOnExpand(sender, args) {
                processesTreeView.treeView_LoadNodeChildrenOnExpand(sender, args);
            }

            function treeView_LoadNodeTypeDetailsOnClick(sender, args) {
                processesTreeView.treeView_LoadNodeTypeDetailsOnClick(sender, args);
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowMoveFasc" 
                               Height="600" 
                               Width="750" runat="server" 
                               Behaviors="Close, Maximize, Minimize" 
                               Title="Sposta fascicolo" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" ID="splitterMain" Width="100%" Height="100%">
            <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
                <telerik:RadSplitter runat="server" Width="100%" Height="100%" ResizeWithParentPane="False" ID="splContent">
                    <telerik:RadPane runat="server" Width="50%" ID="treeviewPane">
                        <telerik:RadToolBar AutoPostBack="false" 
                                            EnableRoundedCorners="False" 
                                            EnableShadows="False" ID="ActionsToolbar" runat="server" 
                                            Width="100%" RenderMode="Lightweight">
                            <Items>
                                <telerik:RadToolBarButton ToolTip="Sposta fascicolo"
                                    CommandName="MoveFascicle"
                                    CheckOnClick="false" Checked="false"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/move_to_folder.png" />
                                <telerik:RadToolBarButton ToolTip="Modifica serie"
                                    CommandName="EditProcess"
                                    CheckOnClick="false" Checked="false"
                                    ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="categoriesTreeView"
                            LoadingStatusPosition="BeforeNodeText"
                            PersistLoadOnDemandNodes="false"
                            OnClientNodeExpanding="treeView_LoadNodeChildrenOnExpand"
                            OnClientNodeClicked="treeView_LoadNodeTypeDetailsOnClick"
                            runat="server" Style="margin-top: 10px;" Width="100%"/>
                    </telerik:RadPane>
                    <telerik:RadSplitBar runat="server" CollapseMode="None"></telerik:RadSplitBar>
                    <telerik:RadPane runat="server" Width="50%" ID="detailsPane" ShowContentDuringLoad="false">
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>
