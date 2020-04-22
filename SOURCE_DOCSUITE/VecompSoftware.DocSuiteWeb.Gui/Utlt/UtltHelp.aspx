<%@ Page AutoEventWireup="false" CodeBehind="UtltHelp.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltHelp" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Documentazione" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript" language="javascript">

            function NodeClicked(sender, eventArgs) {
                var node = eventArgs.get_node();
                if (node.get_nodes().get_count() > 0) {
                    if (node.get_expanded()) {
                        node.set_expanded(false);
                    } else
                        node.set_expanded(true);
                }
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function SetFramePage(page) {
                parent.$find('contentPane').set_contentUrl(page);
                return false;
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadSplitter BorderSize="0" PanesBorderSize="0" ID="userSplitter" runat="server" Height="100%" Width="100%">
        <%-- Menu --%>
        <telerik:RadPane ID="menuPane" runat="server" Width="255" CssClass="left">
            <telerik:RadAjaxPanel runat="server" ID="pnlMenu">
                <telerik:RadTreeView ID="trvHelp" runat="server" />
            </telerik:RadAjaxPanel>
        </telerik:RadPane>
        <telerik:RadSplitBar ID="middleSplitBar" runat="server" CollapseMode="Forward" />
        <%-- content --%>
        <telerik:RadPane ID="contentPane" runat="server"></telerik:RadPane>
    </telerik:RadSplitter>

</asp:Content>
