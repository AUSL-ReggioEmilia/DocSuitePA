<%@ Control AutoEventWireup="false" Codebehind="uscProtocolSelTree.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocolSelTree" Language="vb" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript" language="javascript">

        function getWindowManager() {
            return $find("<%=RadWindowManagerProtocol.ClientID%>");
        }

        function <%= Me.ID %>_OpenWindow(url, name) {
            var wnd = getWindowManager().open(url, name);
            wnd.setSize(600, 400);
            wnd.add_close(<%= Me.ID %>_OnClose);
            wnd.center();
            return false;
        }
        
        //richiamata quando la finestra rubrica viene chiusa
        function <%= Me.ID %>_OnClose(sender, args) {
            sender.remove_close(<%= Me.ID %>_OnClose);
            if (args.get_argument() !== null) {
                document.getElementById("<%= txtProtocollo.ClientID %>").value = args.get_argument();
                document.getElementById("<%= btnAddProtocollo.ClientID %>").click();
            }
        }

        //richiamata quando non esiste nessun nodo figlio della root per svuotare la textbox di validazione
        function <%= Me.ID %>_ClearTextValidator() {
            document.getElementById("<%= txtProtocollo.ClientID %>").value = "";
        }
    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerProtocol" runat="server">
    <Windows>
        <telerik:RadWindow ID="windowSelProtocollo" Width="600" Height="450" runat="server" Title="Seleziona protocollo" />
        <telerik:RadWindow ID="windowSearchProtocollo" runat="server" />
    </Windows>
</telerik:RadWindowManager>
<table class="datatable" >
    <tr class="Chiaro">
        <td class="DXChiaro" style="width: auto">
            <telerik:RadTreeView ID="RadTreeProtocollo" runat="server" Width="100%">
                <Nodes>
                    <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Text="Protocolli" Value="Root" />
                </Nodes>
            </telerik:RadTreeView>
            <asp:RequiredFieldValidator ControlToValidate="txtProtocollo" Display="Dynamic" ErrorMessage="Campo Protocolli Obbligatorio" ID="rfvProtocol" runat="server" />
        </td>
        <td align="right" style="width: 120px">
            <asp:Panel runat="server" ID="panelButtons" HorizontalAlign="Right">
                <div style="float: right;">
                    <asp:ImageButton CausesValidation="False" ID="btnRemoveProtocollo" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" runat="server" ToolTip="Elimina Riferimento" />
                </div>
                <div style="">
                    <asp:ImageButton CausesValidation="False" ID="imgSelProtocollo" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" ToolTip="Seleziona Riferimento" />
                </div>
                <asp:Button ID="btnAddProtocollo" runat="server" CausesValidation="false" />
            </asp:Panel>
            <asp:TextBox ID="txtProtocollo" runat="server" CssClass="hiddenField" />
        </td>
    </tr>
</table>
