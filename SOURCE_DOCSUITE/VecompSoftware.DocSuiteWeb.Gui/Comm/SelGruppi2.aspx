<%@ Page AutoEventWireup="false" CodeBehind="SelGruppi2.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelGruppi2" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Gruppo" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }
        	
            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

            function getTreeView() {
                return $find("<%= RadTreeGroups.ClientID %>");
            }
            
            function confirmClick() {
                this.showLoadingPanel();
                var tree = getTreeView();
                var checkedNodes = tree.get_checkedNodes();
                var toJson = [];
                if (checkedNodes.length > 0) {
                    for (var i = 0; i < checkedNodes.length; i++) {
                        var node = checkedNodes[i];
                        toJson.push(node.get_text());
                    }
                    CloseWindow(JSON.stringify(toJson));
                }
                this.hideLoadingPanel();
                return false;
            }

            function showLoadingPanel() {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var loadingPanel = "<%= btnConfirm.ClientID%>";
                ajaxLoadingPanel.show(loadingPanel);
            }

            function hideLoadingPanel() {
                var ajaxLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var loadingPanel = "<%= btnConfirm.ClientID%>";
                ajaxLoadingPanel.hide(loadingPanel);
            }

            function setAllNodesCheckStatus(check) {
                var tree = getTreeView();
                var rootNodes = tree.get_nodes();
                if (rootNodes.get_count() > 0) {
                    var rootNode = rootNodes.getNode(0);
                    for (var i = 0; i < rootNode.get_nodes().get_count(); i++) {
                        var node = rootNode.get_nodes().getNode(i);
                        node.set_checked(check);
                    }
                }
                return false;
            }
        </script>
    </telerik:RadScriptBlock>
    
    <asp:Panel runat="server" DefaultButton="btnCerca" CssClass="datatable">
        <asp:TextBox ID="txtCerca" runat="server" />
        <asp:Button ID="btnCerca" runat="server" Text="Cerca" />
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="RadTreeGroups" runat="server" CheckBoxes="true" EnableViewState="false" Height="100%" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConfirm" runat="server" Text="Conferma" OnClientClick="return confirmClick();" />
    <asp:Button ID="btnSelectAll" runat="server" Text="Seleziona tutti" OnClientClick="return setAllNodesCheckStatus(true);" />
    <asp:Button ID="btnUndoSelect" runat="server" Text="Deseleziona tutti" OnClientClick="return setAllNodesCheckStatus(false);" />
</asp:Content>
