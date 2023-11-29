<%@ Page AutoEventWireup="false" CodeBehind="TbltSettore.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltSettore" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Settori" %>

<%@ Register Src="~/UserControl/uscCollRoles.ascx" TagName="UscCollRoles" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock EnableViewState="false" ID="RadScriptBlock1" runat="server">
        <style type="text/css">
            div.RadGrid_Office2007 input {
                background-color: white;
            }
        </style>

        <script type="text/javascript">
            var tbltSettore;
            require(["Tblt/TbltSettore"], function (TbltSettore) {
                tbltSettore = new TbltSettore();
                Initialize();
            });

            function Initialize() {
                tbltSettore.radTreeViewRolesId = "<%= RadTreeViewRoles.ClientID %>";
                tbltSettore.radWindowManagerRolesId = "<%= RadWindowManagerRoles.ClientID %>";
                tbltSettore.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                tbltSettore.folderToolBarId = "<%= FolderToolBar.ClientID%>";
                tbltSettore.showDisabled = "<%= Not ProtocolEnv.ManageDisableItemsEnabled %>";
                tbltSettore.initialize();
            }

            function OpenEditWindow(name, operation) {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.openEditWindow(name, operation);
                });

            }

            function OpenRolesWindow() {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.openRolesWindow();
                });
            }

            function OpenPrintWindow(name) {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.openPrintWindow(name);
                });
            }

            function OpenGroupsWindow() {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.openGroupsWindow();
                });
            }

            function LoadChildrenRoles() {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.loadChildrenRoles();
                });
            }

            function OpenLogWindow(name) {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.openLogWindow(name);
                });
            }

            function OpenPropagationWindow(name) {
                require(["Tblt/TbltSettore"], function (TbltSettore) {
                    tbltSettore = new TbltSettore();
                    Initialize();
                    tbltSettore.openPropagationWindow(name);
                });
            }

            function OnContextMenuItemClicked(sender, args) {
                tbltSettore.onContextMenuItemClicked(sender, args);
            }

            function OnContextMenuShowing(sender, args) {
                tbltSettore.onContextMenuShowing(sender, args);
            }

            function UpdateGroups() {
                tbltSettore.updateGroups();
            }

            
   //Apre una finestra generale
            function OpenWindow(url, name, width, height) {
                var manager = $find("<%=RadWindowManagerRoles.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }
            //Apre una finestra generale
            function OpenEditWindowUsers(name, groupName) {
                var url = "../Comm/SelUsers.aspx?Type=Comm&GroupName=" + groupName;
                return OpenWindow(url, name, WIDTH_LOG_WINDOW, HEIGHT_LOG_WINDOW);
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">


    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerRoles" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowEditRoles" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Gestione Settore" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowLogRoles" Modal="true" runat="server" ShowContentDuringLoad="false" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowRoles" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Seleziona Settore" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowGroupRoles" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Gestione Gruppi Settore" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowPrintRoles" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Stampa Settore" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowPropagation" Modal="true" runat="server" ShowContentDuringLoad="false" Title="Propagazione gerarchia" VisibleStatusbar="false" />
            <telerik:RadWindow ID="windowAddUsers" Modal="true" Title="Aggiungi utente" runat="server" VisibleStatusbar="false" />
        </Windows>
    </telerik:RadWindowManager>

    <div class="splitterWrapper">
        <telerik:RadSplitter ID="RadSplitter1" runat="server" Width="100%" Height="100%">
            <telerik:RadPane runat="server" ID="treePane" Width="50%" Scrolling="None">
                <telerik:RadSplitter runat="server" Height="100%" Width="100%">
                    <telerik:RadPane runat="server" Height="100%" Width="100%">
                        <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" OnButtonClick="ToolBarSearch_ButtonClick" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
                            <Items>
                                <telerik:RadToolBarButton Value="searchDescription">
                                    <ItemTemplate>
                                        <telerik:RadTextBox ID="txtRoleDescription" EmptyMessage="Descrizione" runat="server" Width="170px"></telerik:RadTextBox>
                                    </ItemTemplate>
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Disattivi" CheckOnClick="true" Group="Disabled" Checked="false" Value="searchDisabled" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Attivi" CheckOnClick="true" Checked="true" Group="Active" Value="searchActive" PostBack="true"
                                    AllowSelfUnCheck="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true" />
                                <telerik:RadToolBarButton Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadToolBar AutoPostBack="true" EnableRoundedCorners="False" EnableShadows="False" ID="FolderToolBar" runat="server" Width="100%" RenderMode="Lightweight">
                            <Items>
                                <telerik:RadToolBarButton runat="server" ToolTip="Aggiungi" Value="create" Text="Aggiungi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/Add_Folder.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Modifica" Value="modify" Text="Modifica" ImageUrl="~/App_Themes/DocSuite2008/imgset16/modify_folder.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Elimina" Value="delete" Text="Elimina" ImageUrl="~/App_Themes/DocSuite2008/imgset16/DeleteFolder.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Gruppi" Value="groups" Text="Gruppi" ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Disegno di funzione" Value="function" Text="Funzione" ImageUrl="~/App_Themes/DocSuite2008/imgset16/function.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Sposta" Value="move" Text="Sposta" ImageUrl="~/App_Themes/DocSuite2008/imgset16/move_to_folder.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Clona" Value="clone" Text="Clona" ImageUrl="~/App_Themes/DocSuite2008/imgset16/clone.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Stampa" Value="print" Text="Stampa" ImageUrl="~/App_Themes/DocSuite2008/imgset16/printer.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Propagazione massiva" Value="propagation" Text="Propagazione" ImageUrl="~/App_Themes/DocSuite2008/imgset16/propagation.png" />
                                <telerik:RadToolBarButton runat="server" ToolTip="Log" Value="log" Text="Log" ImageUrl="~/App_Themes/DocSuite2008/imgset16/file_extension_log.png" />
                            </Items>
                        </telerik:RadToolBar>
                        <telerik:RadTreeView ID="RadTreeViewRoles" OnClientNodeClicked="OnClientNodeClickedExpand" runat="server" Style="margin-top: 10px;" Width="100%" Height="91%">
                            <Nodes>
                                <telerik:RadTreeNode Expanded="true" NodeType="Root" Selected="true" runat="server" Text="Settori" Value="" />
                            </Nodes>
                        </telerik:RadTreeView>
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
            <telerik:RadSplitBar ID="Bar1" runat="server" CollapseMode="None" />

            <telerik:RadPane runat="server" Width="50%" Height="100%">
                <asp:Panel runat="server" ID="pnlDetail" CssClass="dsw-panel" Style="margin-top: 5px; min-height: 20cm">
                    <div class="dsw-panel-content">
                        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="pnlRightDetails">
                            <Items>
                                <telerik:RadPanelItem Text="Informazioni" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server" ID="pnlInformations">
                                            <div class="col-dsw-10">
                                                <b>Settore:</b>
                                                <asp:Label runat="server" ID="lblName"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Indirizzo e-mail:</b>
                                                <asp:Label runat="server" ID="lblMailAddress"></asp:Label>
                                            </div>
                                            <div class="col-dsw-10">
                                                <b>Indirizzi PEC:</b>
                                                <asp:Label runat="server" ID="lblPecMailAddress"></asp:Label>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                                <telerik:RadPanelItem Text="Gruppi" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server">
                                            <telerik:RadTextBox runat="server" ID="txtSearchUser" EmptyMessage="Cerca utente" Width="250px" Style="margin-left: 2px;"></telerik:RadTextBox>
                                            <telerik:RadButton runat="server" ID="btnSearchUser" Text="Cerca" Style="margin-top: 2px;"></telerik:RadButton>
                                            <telerik:RadGrid runat="server" ID="grdGroups" AutoGenerateColumns="False" Style="margin-top: 2px;" GridLines="none" ItemStyle-BackColor="LightGray">
                                                <MasterTableView Width="100%" DataKeyNames="GroupName" NoMasterRecordsText="Nessun gruppo presente">
                                                    <DetailTables>
                                                        <telerik:GridTableView Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px" Name="UsersGrid" NoDetailRecordsText="Nessun utente presente">
                                                            <Columns>
                                                                <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                                    <ItemTemplate>
                                                                        <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" ID="imgUser" />
                                                                        <asp:ImageButton runat="server" ID="btnAddUser" ImageUrl="../App_Themes/DocSuite2008/imgset16/add.png"  />
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridBoundColumn HeaderText="Utente" DataField="Name" />
                                                            </Columns>
                                                        </telerik:GridTableView>
                                                    </DetailTables>
                                                    <GroupByExpressions>
                                                        <telerik:GridGroupByExpression>
                                                            <SelectFields>
                                                                <telerik:GridGroupByField FieldName="Location" FieldAlias="Tipologia" />
                                                            </SelectFields>
                                                            <GroupByFields>
                                                                <telerik:GridGroupByField FieldName="Location" />
                                                            </GroupByFields>
                                                        </telerik:GridGroupByExpression>
                                                    </GroupByExpressions>
                                                    <Columns>
                                                        <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                                            <ItemTemplate>
                                                                <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupMembers.png" />
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn HeaderText="NomeGruppo" DataField="GroupName" UniqueName="GroupName" />
                                                        <telerik:GridBoundColumn HeaderText="Autorizzazioni" DataField="Authorization" UniqueName="Authorization" />
                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                                <telerik:RadPanelItem Text="Disegno Di Funzione" Expanded="true">
                                    <ContentTemplate>
                                        <asp:Panel runat="server">
                                            <div class="col-dsw-10">
                                                <usc:UscCollRoles runat="server" style="height: 200%; width: 100%; border: none 0;" ID="uscRoleUsers" EditMode="false" />
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </telerik:RadPanelItem>
                            </Items>
                        </telerik:RadPanelBar>
                    </div>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlCollaboration" CssClass="dsw-panel" Style="margin-top: 5px;">
                    <div class="dsw-panel-content">
                        <usc:UscCollRoles runat="server" style="height: 100%; width: 100%; border: none 0;" ID="UscCollRoles1" EditMode="true" />
                    </div>
                </asp:Panel>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>
