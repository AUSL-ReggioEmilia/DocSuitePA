<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscGroupDetails.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscGroupDetails" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">

        var UscGroupDetailsScripts = (function () {

            function UscGroupDetailsScripts() {
            }

            //Apre una finestra generale
            UscGroupDetailsScripts.prototype.OpenWindow_Users = function (url, name, width, height) {
                var manager = $find("<%= RadWindowManagerGroups.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();
                return false;
            }

            //Apre la finestra di editazione di un nodo (aggiunta,eliminazione,modifica)
            UscGroupDetailsScripts.prototype.OpenEditWindow_Users = function (name) {
                var url = "../Comm/SelUsers.aspx?Type=Comm&";
                return UscGroupDetailsScripts.prototype.OpenWindow_Users(url, name, 600, 400);
            }

            UscGroupDetailsScripts.prototype.OpenEditWindow_ImportFromGroup = function (name) {
                var url = "../Comm/SelGruppi2.aspx?Type=Comm&";
                return UscGroupDetailsScripts.prototype.OpenWindow_Users(url, name, 600, 400);
            }

            //funzione richiamata dopo la chiusura della finestra di editazione di un nodo
            UscGroupDetailsScripts.prototype.CloseEditUsers = function (sender, args) {
                if (args.get_argument() !== null) {
                    UscGroupDetailsScripts.prototype.UpdateUsers('users', args.get_argument());
                }
            }

            //funzione richiamata dopo la chiusura della finestra di editazione di un nodo
            UscGroupDetailsScripts.prototype.CloseImportFrom = function (sender, args) {
                if (args.get_argument() !== null) {
                    UscGroupDetailsScripts.prototype.UpdateUsers('importFrom', args.get_argument());
                }
            }

            UscGroupDetailsScripts.prototype.UpdateUsers = function (ajaxKey, value) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest(ajaxKey + '|' + value);
            }

            //Handler sul click di una voce del ContextMenu
            UscGroupDetailsScripts.prototype.ContextMenuItemClicked_Users = function (sender, args) {
                var menuItem = args.get_menuItem();
                switch (menuItem.get_value()) {
                    case "Add":
                        UscGroupDetailsScripts.prototype.OpenEditWindow_Users('windowAddUsers', 'Add');
                        break;
                    case "ImportFrom":
                        UscGroupDetailsScripts.prototype.OpenEditWindow_ImportFromGroup('windowImportFrom', 'ImportFrom');
                        break;
                    case "DeleteFromGroup":
                        var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                        var selectedNode = $find("<%=RadTreeViewUsers.ClientID%>").get_selectedNode().get_value();
                        ajaxManager.ajaxRequest('deleteFromGroup' + '|' + selectedNode);
                        break;
                }
            }

            UscGroupDetailsScripts.prototype.ContextMenuShowing_Users = function (sender, args) {
                var treeNode = args.get_node();
                treeNode.set_selected(true);
            }

            return UscGroupDetailsScripts;
        })();
        var currentUscGroupDetailsScripts = new UscGroupDetailsScripts();

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerGroups" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowAddUsers" OnClientClose="currentUscGroupDetailsScripts.CloseEditUsers" runat="server" Title="Aggiungi utente" />
        <telerik:RadWindow ID="windowImportFrom" OnClientClose="currentUscGroupDetailsScripts.CloseImportFrom" runat="server" Title="Aggiungi utenti dal gruppo" />
    </Windows>
</telerik:RadWindowManager>

<asp:Panel runat="server" ID="pnlDetail" Style="visibility: hidden">
    <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbGroups">
        <Items>
            <telerik:RadPanelItem Value="rpiUsers" Text="Utenti" Expanded="true" runat="server">
                <ContentTemplate>
                    <telerik:RadTreeView ID="RadTreeViewUsers" MultipleSelect="true" CheckBoxes="True" OnClientContextMenuItemClicked="currentUscGroupDetailsScripts.ContextMenuItemClicked_Users" OnClientContextMenuShowing="currentUscGroupDetailsScripts.ContextMenuShowing_Users" runat="server">
                        <ContextMenus>
                            <telerik:RadTreeViewContextMenu ID="RadTreeViewContextUsers" runat="server">
                                <Items>
                                    <telerik:RadMenuItem Width="200px" Value="Add" Text="Aggiungi" />
                                    <telerik:RadMenuItem Width="200px" Value="ImportFrom" Text="Importa da gruppo" />
                                    <telerik:RadMenuItem Value="DeleteFromGroup" Text="Elimina" />
                                </Items>
                            </telerik:RadTreeViewContextMenu>
                        </ContextMenus>
                    </telerik:RadTreeView>
                </ContentTemplate>
            </telerik:RadPanelItem>
            <telerik:RadPanelItem Text="Contenitori" Expanded="true">
                <ContentTemplate>
                    <telerik:RadGrid runat="server" ID="grdContainers" AllowMultiRowSelection="true" AutoGenerateColumns="False" Style="margin-top: 2px;" GridLines="none" ItemStyle-BackColor="LightGray" expa>
                        <MasterTableView Width="100%" DataKeyNames="id" NoMasterRecordsText="Nessun contenitore presente">
                            <DetailTables>
                                <telerik:GridTableView runat="server" Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="DocumentType" UniqueName="DocumentType" ItemStyle-Width="40"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Rights" UniqueName="Rights" ItemStyle-Width="60"></telerik:GridBoundColumn>
                                    </Columns>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="SelectContainer" />
                                <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                    <ItemTemplate>
                                        <asp:Image runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/box_open.png" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Contenitore" UniqueName="ContainerName" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings EnableRowHoverStyle="true" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
                    </telerik:RadGrid>
                </ContentTemplate>
            </telerik:RadPanelItem>

            <telerik:RadPanelItem Text="Settori" Expanded="true">
                <ContentTemplate>
                    <telerik:RadGrid runat="server" ID="grdRoles" AllowMultiRowSelection="true" ClientSettings-Selecting-AllowRowSelect="true" AutoGenerateColumns="False" Style="margin-top: 2px;" GridLines="none" ItemStyle-BackColor="LightGray" expa>
                        <MasterTableView Width="100%" DataKeyNames="id" NoMasterRecordsText="Nessun settore presente">
                            <DetailTables>
                                <telerik:GridTableView runat="server" Width="100%" ShowHeader="false" AlternatingItemStyle-BackColor="WhiteSmoke" ItemStyle-BackColor="Snow" ItemStyle-BorderWidth="2px">
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="DocumentType" UniqueName="DocumentType" ItemStyle-Width="40"></telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Rights" UniqueName="Rights" ItemStyle-Width="60"></telerik:GridBoundColumn>
                                    </Columns>
                                </telerik:GridTableView>
                            </DetailTables>
                            <Columns>
                                <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="SelectRole" />
                                <telerik:GridTemplateColumn HeaderStyle-Width="16px">
                                    <ItemTemplate>
                                        <asp:Image runat="server" ID="roleDisabled" ImageUrl="~/App_Themes/DocSuite2008/imgset16/brick.png" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Settore" UniqueName="RoleName" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings EnableRowHoverStyle="true" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
                    </telerik:RadGrid>
                </ContentTemplate>
            </telerik:RadPanelItem>
        </Items>
    </telerik:RadPanelBar>
</asp:Panel>
