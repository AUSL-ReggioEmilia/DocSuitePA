<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltGruppi.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltGruppi" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gruppi" %>

<%@ Register Src="~/UserControl/uscGroupDetails.ascx" TagPrefix="usc" TagName="GroupDetails" %>

<asp:Content ID="ContentHeader" runat="server" ContentPlaceHolderID="cphHeader">


    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerGroups" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowEditGroups" OnClientClose="CloseEditGroups" runat="server" />
            <telerik:RadWindow ID="windowLogGroups" runat="server" />
        </Windows>
    </telerik:RadWindowManager>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadSplitter runat="server" ID="Splitter1" Width="100%" Height="100%">
        <telerik:RadPane runat="server" ID="Pane1" Width="100%" Height="100%">
            <telerik:RadSplitter runat="server" Height="100%" Width="100%">
                <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="None">
                    <telerik:RadToolBar AutoPostBack="false" 
                                        CssClass="ToolBarContainer" 
                                        RenderMode="Lightweight" 
                                        EnableRoundedCorners="False" 9
                                        EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                        <Items>
                            <telerik:RadToolBarButton Value="searchGroup">
                                <ItemTemplate>
                                    <telerik:RadTextBox ID="txtGroup" EmptyMessage="Nome gruppo contiene" runat="server" Width="170px"></telerik:RadTextBox>
                                </ItemTemplate>
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton IsSeparator="true" />
                            <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                        </Items>
                    </telerik:RadToolBar>
                    <telerik:RadToolBar AutoPostBack="false"
                        CssClass="ToolBarContainer"
                        RenderMode="Lightweight"
                        EnableRoundedCorners="False"
                        EnableShadows="False"
                        OnClientButtonClicked="toolbarbtn_Clicked"
                        ID="ActionsToolbar" runat="server" Width="100%">
                        <Items>
                            <telerik:RadToolBarButton ID="btnAddGroup"
                                runat="server"
                                ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png"
                                AutoPostBack="false"
                                CommandName="Add"
                                ToolTip="Aggiungi gruppo"
                                Text="Aggiungi"/>
                            <telerik:RadToolBarButton ID="btnRenameGroup"
                                runat="server"
                                ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png"
                                AutoPostBack="false"
                                CommandName="Rename"
                                ToolTip="Rinomina gruppo"
                                Text="Modifica"/>
                            <telerik:RadToolBarButton ID="btnDeleteGroup"
                                runat="server"
                                ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png"
                                AutoPostBack="false"
                                CommandName="Delete"
                                ToolTip="Elimina gruppo"
                                Text="Elimina"/>
                            <telerik:RadToolBarButton ID="btnLogGroup"
                                runat="server"
                                ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_log.png"
                                AutoPostBack="false"
                                CommandName="Log"
                                ToolTip="Log"
                                Text="Log"/>
                        </Items>
                    </telerik:RadToolBar>
                    <telerik:RadTreeView ID="RadTreeViewGroups" 
                                         OnClientContextMenuItemClicked="ContextMenuItemClicked_Groups" 
                                         Width="100%" OnClientContextMenuShowing="ContextMenuShowing_Groups" runat="server" >
                        <ContextMenus>
                            <telerik:RadTreeViewContextMenu ID="ContextMenuGroups" runat="server">
                                <Items>
                                    <telerik:RadMenuItem Width="200px" Value="Add" Text="Aggiungi" />
                                    <telerik:RadMenuItem Value="Rename" Text="Rinomina" />
                                    <telerik:RadMenuItem Value="Delete" Text="Elimina" />
                                    <telerik:RadMenuItem Value="Log" Text="Log" />
                                </Items>
                            </telerik:RadTreeViewContextMenu>
                        </ContextMenus>
                        <Nodes>
                            <telerik:RadTreeNode Expanded="true" runat="server" Selected="true" Text="Gruppi" />
                        </Nodes>
                    </telerik:RadTreeView>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </telerik:RadPane>

        <telerik:RadSplitBar runat="server" ID="Bar1" />

        <telerik:RadPane runat="server" ID="Pane2" Height="100%" Width="100%">
            <asp:Panel runat="server" ID="pnlGroupDetails" Height="100%" Visible="true">
                <usc:GroupDetails runat="server" ID="groupDetails" visible="true"/>
            </asp:Panel>
        </telerik:RadPane>
    </telerik:RadSplitter>

        <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">

            function toolbarbtn_Clicked(sender, args) {
                var btnCommandName = args.get_item().get_commandName();

                executeActionForOperation(btnCommandName);
            }

            function executeActionForOperation(operation) {
                switch (operation) {
                    case "Add":
                        OpenEditWindow('windowEditGroups', 'Add');
                        break;
                    case "Rename":
                        OpenEditWindow('windowEditGroups', 'Rename');
                        break;
                    case "Delete":
                        OpenEditWindow('windowEditGroups', 'Delete');
                        break;
                    case "Log":
                        OpenLogWindow('windowLogGroups');
                        break;
                    case "AddUser":
                        OpenEditWindowUsers('windowAddUsers');
                        break;
                    case "CopyFromUser":
                        OpenEditWindowImportFromGroup('windowImportFrom');
                        break;
                    case "DeleteUser":
                        var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                        ajaxManager.ajaxRequest("deleteuser");
                        break;
                }
            }

            function getTreeClickedNode() {
                var treeView = $find("<%= RadTreeViewGroups.ClientID %>");
   
                return treeView.get_selectedNode();
            }

            //Apre una finestra generale
            function OpenWindow(url, name, width, height) {
                var manager = $find("<%= RadWindowManagerGroups.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            //Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
            function OpenEditWindow(name, operation) {
                var selectedNode = getTreeClickedNode();

                if (!selectedNode || (!selectedNode.get_value() && operation !== "Add")) {
                    alert("Nessun nodo selezionato.");
                    return;
                }

                var parameters = "Action=" + operation;
                if (selectedNode.get_value()) {
                    parameters += "&IdGroup=" + selectedNode.get_value();
                }
                var url = "../Tblt/TbltGruppiGes.aspx?Type=Comm&" + parameters;
                return OpenWindow(url, name, WIDTH_EDIT_WINDOW, HEIGHT_EDIT_WINDOW);
            }

            //Apre la finestra con i Log per il nodo selezionato
            function OpenLogWindow(name) {
                var url = "../Tblt/TbltLog.aspx?Type=Comm&TableName=SecurityGroups"
                var selectedNode = getTreeClickedNode();
                if (selectedNode != null)
                {
                    var attributes = selectedNode.get_attributes();
                    url += "&entityUniqueId=" + attributes.getAttribute("UniqueId");
                }

                return OpenWindow(url, name, 800, 550);
            }

            //funzione richiamata dopo la chiusura della finestra di editazione di un nodo
            function CloseEditGroups(sender, args) {
                if (args.get_argument() !== null) {
                    UpdateGroups(args.get_argument());
                }
            }

            function UpdateGroups(value) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("groups|" + value);
            }

            //Handler sul click di una voce del ContextMenu
            function ContextMenuItemClicked_Groups(sender, args) {
                var menuItem = args.get_menuItem();

                executeActionForOperation(menuItem.get_value());
            }

            //Handler sull'apertura del Context Menu
            function ContextMenuShowing_Groups(sender, args) {
                var treeNode = args.get_node();
                treeNode.set_selected(true);
                var menu = args.get_menu();
                switch (treeNode.get_value()) {
                    case "":
                        //ROOT
                        EnableItemByValue(menu, 'Add');
                        DisableItemByValue(menu, 'Rename');
                        DisableItemByValue(menu, 'Delete');
                        DisableItemByValue(menu, 'Log');
                        break;
                    default:
                        //GRUPPO
                        EnableItemByValue(menu, 'Add');
                        EnableItemByValue(menu, 'Rename');
                        EnableItemByValue(menu, 'Delete');
                        EnableItemByValue(menu, 'Log');
                        break;
                }
                AdjustButtons_Groups(menu);
            }

            //Allinea i pulsanti con il contextMenu
            function AdjustButtons_Groups(menu) {
                AdjustButtonWithItem('<%= btnAddGroup.ClientID %>', menu, 'Add');
                AdjustButtonWithItem('<%= btnRenameGroup.ClientID %>', menu, 'Rename');
                AdjustButtonWithItem('<%= btnDeleteGroup.ClientID %>', menu, 'Delete');
                AdjustButtonWithItem('<%= btnLogGroup.ClientID %>', menu, 'Log');
            }            

            function OpenEditWindowUsers(name) {
                var uscGroupDetScripts = new UscGroupDetailsScripts();
                return uscGroupDetScripts.OpenEditWindow_Users(name);               
            }

            function OpenEditWindowImportFromGroup(name) {
                var uscGroupDetScripts = new UscGroupDetailsScripts();
                return uscGroupDetScripts.OpenEditWindow_ImportFromGroup(name);                
            }

        </script>
            <style>
            #ctl00_cphContent_RadTreeViewGroups {
                height: 90%;
            }
        </style>
    </telerik:RadScriptBlock>
</asp:Content>
