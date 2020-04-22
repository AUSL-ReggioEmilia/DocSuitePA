<%@ Control AutoEventWireup="false" Codebehind="uscContainerExtGes.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContainerExtGes" Language="vb" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlockTRV">
    <script type="text/javascript">
        //Apre la finestra di editazione di un nodo (aggiunta,modifica)
        function OpenEditWindowContainerExtGes(action, idContainer, keyType) {
            var parameters = "action=" + action + "&Type=Docm&idContainer=" + idContainer + "&keyType=" + keyType;

            var treeView = $find("<%= rtvContainerExt.ClientID %>");
            var selectedNode = treeView.get_selectedNode();

            switch (action) {
            case "Add":
                parameters += "&incremental=" + selectedNode.get_value();
                break;
            case "Rename":
                parameters += "&incremental=" + selectedNode.get_value() + AppendRenameAttributes(selectedNode);
                break;
            }

            var wnd = window.radopen("../Tblt/TbltContainerExtNewEdit.aspx?" + parameters, null);
            wnd.set_title("Gestione Cartelle Standard");
            wnd.setActive(true);
            wnd.set_modal(true);
            wnd.setSize(600, 240);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Move + Telerik.Web.UI.WindowBehaviors.Close);
            wnd.set_visibleStatusbar(false);
            wnd.set_showContentDuringLoad(false);
            wnd.add_close(<%= Me.ID %>_CloseContainerExtGes);
            wnd.center();
            
            return false;
        }

        function AppendRenameAttributes(node) {
            var nodeText = node.get_attributes().getAttribute("KeyValue");
            var array = nodeText.split("|");
            return "&oldKey=" + ReplaceJS(array[0]) + "&oldNumber=" + ReplaceJS(array[1]);
        }

        function ReplaceJS(str) {
            str = str.replace("'", "%27");
            return str;
        }

        // funzione richiamata dopo la chiusura della finestra di editazione di un nodo
        function <%= Me.ID %>_CloseContainerExtGes(sender, args) {
            if (args.get_argument() !== null) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest("<%= Me.ClientID %>|" + args.get_argument().Action + "|" + args.get_argument().ContainerExt);
            }
        }

    </script>

</telerik:RadScriptBlock>
<table style="width: 100%; height: 100%;" class="docm">
    <tr style="height: 100%;">
        <td>
            <table style="height: 100%; width: 100%;" class="datatable">
                <tr>
                    <th>
                        <asp:Label runat="server" ID="lblCaption" Text="Gestione Cartelle Standard" />
                    </th>
                </tr>
                <tr>
                    <td>
                        <usc:Settori Caption="Settore di Default" ID="uscStandardRole" MultipleRoles="false" MultiSelect="false" Required="false" RoleRestictions="None" runat="server" Type="Docm" />
                    </td>
                </tr>
                <tr class="Spazio">
                    <td>
                    </td>
                </tr>
                <tr style="height: 100%">
                    <td style="height: 100%">
                        <table style="height: 100%; width: 100%;">
                            <tr class="Chiaro" style="height: 100%">
                                <td style="vertical-align: top; border: solid 1px black;">
                                    <telerik:RadTreeView BorderColor="Gray" BorderWidth="0px" ID="rtvContainerExt" OnNodeDrop="rtvContainerExt_NodeDrop" runat="server">
                                        <Nodes>
                                            <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Cartelle" Value="" />
                                        </Nodes>
                                    </telerik:RadTreeView>
                                </td>
                                <td style="height: 100%; width: 8%; vertical-align: middle;">
                                    <table style="height: 20%; border: 0;">
                                        <tr>
                                            <td>
                                                <asp:ImageButton BorderColor="black" BorderWidth="1px" ID="btnMoveUp" ImageUrl="../Comm/Images/ArrowUp.gif" runat="server" ToolTip="Muovi cartella sopra di una posizione" Visible="True" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:ImageButton BorderColor="black" BorderWidth="1px" ID="btnMoveDown" ImageUrl="../Comm/Images/ArrowDown.gif" runat="server" ToolTip="Muovi cartella sotto di una posizione" Visible="True" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <asp:Panel runat="server" id="panelButtons" Width="100%">
                <asp:Button ID="cmdAdd" runat="server" Text="Inserisci" Visible="True" />
                <asp:Button Enabled="false" ID="cmdRename" runat="server" Text="Rinomina" />
                <asp:Button Enabled="False" ID="cmdDelete" runat="server" Text="Cancella" />
                <asp:Button ID="cmdConfirm" runat="server" Text="Conferma" />
            </asp:Panel>
        </td>
    </tr>
</table>
