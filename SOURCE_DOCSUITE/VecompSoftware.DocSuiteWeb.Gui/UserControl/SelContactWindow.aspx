<%@ Page AutoEventWireup="false" CodeBehind="SelContactWindow.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelContactWindow" Language="vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" style="height: 100%">
<head>
    <title></title>
    <script language="javascript" type="text/javascript">
        //This code is used to provide a reference to the radwindow "wrapper"
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function OnClientNodeClickedHandler(sender, eventArgs) {
            var node = eventArgs.get_node();
            var oWindow = GetRadWindow();
            oWindow.Close(node);
        }
    </script>
</head>
<body>
    <form id="SelContact" method="post" runat="server">
        <telerik:RadScriptManager ID="RadScriptManager1" runat="server" />
        <table id="tblContent" width="100%" border="0">
            <tr class="Chiaro">
                <td colspan="2">&nbsp;
                    <telerik:RadTreeView ExpandAnimation-Type="Linear" ID="rtvContact" LoadingStatusPosition="BeforeNodeText" OnClientNodeClicked="OnClientNodeClickedHandler" OnNodeExpand="rtvContact_NodeExpand" PersistLoadOnDemandNodes="false" runat="server" Width="687px">
                        <Nodes>
                            <telerik:RadTreeNode Expanded="true" ExpandMode="ServerSidecallBack" runat="server" Text="Rubrica" />
                        </Nodes>
                        <CollapseAnimation Duration="100" Type="OutQuint" />
                        <ExpandAnimation Duration="100" Type="OutQuart" />
                    </telerik:RadTreeView>
                </td>
            </tr>
        </table>
        <asp:Button ID="btnConferma" runat="server" Text="Conferma Selezione" />
    </form>
</body>
</html>
