<%@ Control AutoEventWireup="false" Codebehind="uscProtGridBar.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtGridBar" Language="vb" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script language="javascript" type="text/javascript">
        function <%= Me.ID %>_OpenWindow(name, _url, parameters) {
            var url = _url;
            url += ("?" + parameters);

            var manager = $find("<%= RadWindowManager.ClientID %>");
            var wnd = manager.open(url, name);
            wnd.center();

            return false;
        }

        function onTaskCompleted(sender, args) {
            var splitted = args.get_argument().split("|");
            if (splitted[0].toString().toLowerCase() == "true") {
                if (splitted[1] * 1 > 0) {
                    var isok = confirm("Si sono verificati " + splitted[1] + " errori durante la fase di esportazione.");
                    if (isok) {
                        var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                        ajaxManager.ajaxRequest("E");
                    }
                } else {
                    alert("Esportazione eseguita correttamente.");
                }
            } else {
                alert("Si sono verificati degli errori durante il processo d\'importazione.");
            }
        }

    </script>

</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
    <Windows>
        <telerik:RadWindow Height="400" ID="windowExportError" ReloadOnShow="false" runat="server" Width="650" />
        <telerik:RadWindow Behaviors="None" Height="550" ID="wndProgress" runat="server" Width="700" />
    </Windows>
</telerik:RadWindowManager>
<asp:Panel runat="server" ID="pnlGridBar">
    <asp:Button ID="btnExport" runat="server" Text="Esporta Documenti" Visible="False" Width="120px" />
    <asp:Button ID="btnDocuments" runat="server" Text="Visualizza documenti" Visible="False" Width="130px" />
    <asp:Button ID="btnStampa" runat="server" Text="Stampa selezione" Visible="False" Width="120px" />
    <asp:Button ID="btnSelectAll" runat="server" Text="Seleziona tutti" Visible="False" Width="120px" />
    <asp:Button ID="btnDeselectAll" runat="server" Text="Annulla selezione" Visible="False" Width="120px" />
    <asp:Button ID="btnSetRead" runat="server" Text="Segna come letti" Visible="False" Width="120px" />
    <asp:Button ID="btnAssign" runat="server" Text="Assegna" Visible="False" Width="120px" />
</asp:Panel>
