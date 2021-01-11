<%@ Page AutoEventWireup="false" CodeBehind="CommonSelLocation.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelLocation" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function ReturnValueOnClick(sender, args) {
                var node = args.get_node();
                var location = new Object()
                location.ID = node.get_value();
                location.Description = node.get_attributes().getAttribute("Description");
                CloseWindow(location);
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView runat="server" ID="RadTreeViewLocation" OnClientNodeClicked="ReturnValueOnClick">
        <Nodes>
            <telerik:RadTreeNode runat="server" Text="Locazioni" Value="Root" Expanded="true" />
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>
