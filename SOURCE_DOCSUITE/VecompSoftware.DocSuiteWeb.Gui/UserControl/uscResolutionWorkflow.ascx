<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionWorkflow.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserControl.uscResWorkflow" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        function <%= Me.ID %>_OpenWindow(name, url) {
            var manager = $find("<%=RadWindowManagerWorkflow.ClientID %>");
            var wnd = manager.open(url, name);
            wnd.setSize(850, 520);
            wnd.center();
            return false;
        }

        function onClose(sender, args) {
            var data = args.get_argument();
            if (data) {
                var imageButton = document.getElementById("forwardButton");
                if (imageButton) {
                    imageButton.style.visibility = "hidden";
                }
                document.getElementById('<%= btnRefresh.ClientID %>').click();
                return true;
            }

        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerWorkflow" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowWorkflow" OnClientClose="onClose" ReloadOnShow="true" runat="server" Title="" />
    </Windows>
</telerik:RadWindowManager>

<asp:PlaceHolder ID="phWorkflow" runat="server"></asp:PlaceHolder>


<asp:Button runat="server" ID="btnRefresh" Style="display: none" />
