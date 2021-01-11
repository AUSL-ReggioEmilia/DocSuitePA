<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscCollRoles.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscCollRoles" %>

<telerik:RadScriptBlock runat="server" ID="rsbRoles" EnableViewState="false">
    <script type="text/javascript">

        function <%= ID %>_OpenWindow(url, onClientClose, width, height) {
            var wnd = window.DSWOpenGenericWindow(url, window.WindowTypeEnum.SLENDER);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            wnd.set_visibleStatusbar(false);
            if (onClientClose) {
                wnd.add_close(onClientClose);
            }
            wnd.set_modal(true);
            if (width !== undefined && height !== undefined) {
                wnd.setSize(width, height);
            }
            wnd.center();
            return false;
        }

        function <%= ID %>_closeUsers(sender, args) {
            var arg = args.get_argument();
            if (arg != '' && arg != null) {
                $find("<%= AjaxManager.ClientID %>").ajaxRequest("INSERT|" + arg);
            }
        }

    </script>
</telerik:RadScriptBlock>

<table class="page" style="min-height: 18cm">
    <tr>
        <td>
            <asp:Panel ID="pnlCollaborationRights" runat="server" Visible="false">
                <span class="miniLabel" style="vertical-align: middle;">TIpologia di documento:</span>
                <telerik:RadDropDownList ID="ddlDSWEnvironment" runat="server" Style="vertical-align: middle;" AutoPostBack="true" />
                <asp:CheckBox runat="server" ID="chkEnableLocation" OnCheckedChanged="chkEnableLocation_CheckedChanged" AutoPostBack="true" Text="Gestione separata" TextAlign="Right" />
            </asp:Panel>
        </td>
    </tr>
    <tr>
        <td class="content-wrapper" style="height: 100%">
            <div class="content">
                <telerik:RadTreeView BorderStyle="none" CheckBoxes="true" ID="rtvRoles" runat="server" Width="100%" />
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel runat="server" ID="pnlButtonsGes" Style="margin-left: 4px;">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnAggiungi" runat="server" Text="Aggiungi" />
                            <asp:Button ID="btnDelete" runat="server" Text="Elimina" />
                            <asp:Button ID="btnModifyContactRP" runat="server" Text="Modifica contatto" Visible="false" />
                            <asp:Button ID="btnControlla" runat="server" Text="Controlla utenti" />
                            <asp:CheckBox ID="chkCheckPropagate" runat="server" Text="Controlla e propaga" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnSetMainRole" runat="server" Text="Imposta settore principale" Visible="False" />
                            <asp:Button ID="btnRemoveMainRole" runat="server" Text="Rimuovi settore principale" Visible="false" />
                            <asp:Button ID="btnEditSignProfile" runat="server" Visible="false" Text="Modifica profilo firma" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </td>
    </tr>
</table>
