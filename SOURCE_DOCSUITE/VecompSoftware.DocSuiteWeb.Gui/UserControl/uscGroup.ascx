<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscGroup.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscGroup" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        function OpenSelGroupWindow() {
            var url = "../Comm/SelGruppi2.aspx?Type=Comm";
            var manager = $find("<%=RadWindowManagerSelGroup.ClientID %>");
            var wnd = manager.open(url, "windowSelGroup");
            wnd.setSize(600, 400);
            wnd.center();
            return false;
        }

        function OpenSelGroupFromRoleWindow() {
            var url = "../UserControl/CommonSelSettori.aspx?&DSWEnvironment=1&Type=Comm&MultiSelect=False&isActive=True";
            var wnd = GetRadWindow().GetWindowManager().open(url);
            wnd.add_close(CloseAddRoleGroup);
            wnd.setSize(700, 500);
            wnd.set_modal(true);
            wnd.center();
            return false;
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function CloseAddRoleGroup(sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                var ajaxModel = {};
                ajaxModel.Value = [];
                ajaxModel.Value.push(JSON.stringify(args.get_argument()));
                ajaxModel.ActionName = "AddRoleGroup";
                ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                ShowLoadingPanel();
            }
        }

        function CloseAddGroup(sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                var ajaxModel = {};
                ajaxModel.Value = [];
                ajaxModel.Value.push(args.get_argument());
                ajaxModel.ActionName = "AddGroup";
                ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                ShowLoadingPanel();
            }
        }

        function ShowLoadingPanel() {
            var ajaxLoadingPanel = $find("<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
            var loadingPanel = "<%= RadTreeViewGroups.ClientID%>";
            ajaxLoadingPanel.show(loadingPanel);
        }

        function HideLoadingPanel() {
            var ajaxLoadingPanel = $find("<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
            var loadingPanel = "<%= RadTreeViewGroups.ClientID%>";
            ajaxLoadingPanel.hide(loadingPanel);
        }

        function ScrollToSelectedNode() {
            var tree = $find("<%= RadTreeViewGroups.ClientID %>");
                var selectedNode = tree.get_selectedNode();
                if (selectedNode != null) {
                    window.setTimeout(function () { selectedNode.scrollIntoView(); }, 20);
                }
            }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerSelGroup" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelGroup" OnClientClose="CloseAddGroup" runat="server" />
    </Windows>
</telerik:RadWindowManager>

<telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <div class="dsw-panel">
                    <div class="dsw-panel-content">
                        <telerik:RadTreeView runat="server" ID="RadTreeViewSelectedRole" EnableViewState="false" />
                    </div>
                </div>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <telerik:RadTreeView runat="server" ID="RadTreeViewGroups" EnableViewState="true" Width="100%" Height="120px">
                    <Nodes>
                        <telerik:RadTreeNode runat="server" Text="Gruppi" Expanded="true" Value="Root" Selected="true">
                        </telerik:RadTreeNode>
                    </Nodes>
                </telerik:RadTreeView>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div">
            <Content>
                <telerik:RadAjaxPanel runat="server" ID="pnlButtons">
                    <asp:Button ID="btnAggiungiGruppo" OnClientClick="return OpenSelGroupWindow();" runat="server" Width="80px" Text="Aggiungi"></asp:Button>
                    <asp:Button ID="btnAddFromRole" OnClientClick="return OpenSelGroupFromRoleWindow();" runat="server" Width="120px" Text="Aggiungi (Settore)"></asp:Button>
                    <asp:Button ID="btnEliminaGruppo" runat="server" Text="Elimina" Enabled="False" Width="80px"></asp:Button>
                </telerik:RadAjaxPanel>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
