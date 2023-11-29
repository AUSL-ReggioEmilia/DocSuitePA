<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscSearchADUser.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSearchADUser"%>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">

        function onClientMouseOver(sender, args) {
            var nodeElem = args.get_node();
            if (nodeElem.get_level() != 0) {
                var node = nodeElem.get_textElement();

                var tooltipManager = $find("<%= RadToolTipManager.ClientID%>");
                if (!tooltipManager) return;

                var tooltip = tooltipManager.getToolTipByElement(node);
                if (!tooltip) {
                    tooltip = tooltipManager.createToolTip(node);
                    tooltip.set_value(nodeElem.get_attributes()._data["TooltipText"]);
                    setTimeout(function () {
                        tooltip.show();
                    }, 10);
                }
            }
        }
    </script>
</telerik:RadScriptBlock>

<div style="width: 100%;">
    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 30%;">Utente:
            </td>
            <td>
                <asp:Panel DefaultButton="btnSearch" runat="server" Style="display: inline;">
                    <asp:TextBox ID="txtFilter" runat="server" Width="200px" MaxLength="30" />
                </asp:Panel>
                <asp:DropDownList ID="lbMultiDomain" runat="server" Visible="false" />
                <telerik:RadButton ID="btnSearch" runat="server" Text="Ricerca" />
            </td>
        </tr>
    </table>
</div>

<div style="width: 100%;">
    <telerik:RadToolTipManager RenderMode="Lightweight" RelativeTo="Element" ID="RadToolTipManager"
                                                   runat="server" OffsetX="15" Skin="Telerik" Position="MiddleRight" EnableShadow="true"
                                                   OnAjaxUpdate="RadToolTipmanager_AjaxUpdate">
                        </telerik:RadToolTipManager>
    <telerik:RadTreeView ID="tvwContactDomain" runat="server" OnClientMouseOver="onClientMouseOver" />
</div>





