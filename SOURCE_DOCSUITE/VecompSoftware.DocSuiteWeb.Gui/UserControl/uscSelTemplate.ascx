<%@ Control Language="vb" AutoEventWireup="false" Codebehind="uscSelTemplate.ascx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSelTemplate" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript" language="javascript">
        //richiamata quando la finestra rubrica viene chiusa
        function <%= Me.ID %>_OnClose(sender, args) {
            <%= Me.ID %>_Update(args.get_argument());
        }

        //esegue l'aggiornamento della lista dei contatti
        function <%= Me.ID %>_Update(argument) {
            document.getElementById("<%= txtTemplate.ClientID %>").value = argument;
            document.getElementById("<%= btnAddTemplate.ClientID %>").click();
        }

        //richiamata quando non esiste nessun nodo figlio della root per svuotare la textbox di validazione
        function <%= Me.ID %>_ClearTextValidator() {
            document.getElementById("<%= txtTemplate.ClientID %>").value = "";
        }
    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerProtocol" runat="server">
    <windows>
        <telerik:RadWindow ID="windowSelTemplate" ReloadOnShow="false" runat="server" Title="Seleziona protocollo" />
    </windows>
</telerik:RadWindowManager>
<table class="datatable" style="min-width: 400px">
    <tr class="Chiaro">
        <td class="DXChiaro" style="width: auto">
            <telerik:RadTreeView ID="RadTreeTemplate" runat="server" Width="100%">
                <nodes>
                    <telerik:RadTreeNode runat="server" Text="Template" Value="Root" Expanded="true"
                        Font-Bold="true" />
                </nodes>
            </telerik:RadTreeView>
            <asp:RequiredFieldValidator ID="rfvTemplate" runat="server" ControlToValidate="txtTemplate"
                ErrorMessage="Documento Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
        </td>
        <td align="right" style="width: 120px">
            <asp:Panel runat="server" ID="panelButtons" HorizontalAlign="Right">
                <div style="float: right;">
                    <asp:ImageButton CausesValidation="False" ID="imgSelTemplate" ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" runat="server" ToolTip="Seleziona Lettera" />
                </div>
                <asp:Button CausesValidation="False" ID="btnAddTemplate" runat="server" Text="Add" />
            </asp:Panel>
            <asp:TextBox AutoPostBack="True" ID="txtTemplate" runat="server" Width="16px" />
        </td>
    </tr>
</table>
