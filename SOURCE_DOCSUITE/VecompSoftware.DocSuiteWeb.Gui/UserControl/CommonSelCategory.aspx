<%@ Page AutoEventWireup="false" Codebehind="CommonSelCategory.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelCategory" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Classificazione" %>

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

            function ReturnValues() {
                var treeView = $find('<%= RadTreeCategory.ClientID %>');
                var selectedNode = treeView.get_selectedNode();
                if (selectedNode != null) {
                    // Gestione dei diritti dell'utente ad inserire il classificatore
                    if (selectedNode.get_attributes().getAttribute('Rights') == '1')
                        CloseWindow(selectedNode.get_value());
                    else
                        alert('Non si dispongono i permessi per questa voce del piano di fascicolazione.');
                }
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

            function ReturnValueOnClick(sender, args) {
                var node = args.get_node();
                if (node.get_attributes().getAttribute('TypeNode') == 'LeafNode') {
                    // Gestione dei diritti dell'utente ad inserire il classificatore
                    if (node.get_attributes().getAttribute('Rights') == '1')
                        CloseWindow(node.get_value());
                } else {
                    node.expand();
                }
            }
        </script>
    </telerik:RadScriptBlock>

    <table id="tblHeader" width="100%" class="dataform">
        <tr>
            <td class="label" width="20%">
                Descrizione:
            </td>
            <td width="80%">
                <asp:Panel DefaultButton="btnSearch" ID="pnlSearch" runat="server" Style="display: inline;">
                    <asp:TextBox ID="txtSearch" runat="server" Width="300px" />
                </asp:Panel>
                <asp:Button ID="btnSearch" runat="server" Text="Cerca" ToolTip="Ricerca per Descrizione" />
            </td>
        </tr>
        <tr>
            <td class="label" width="20%">
                Codice:
            </td>
            <td width="80%">
                <asp:Panel runat="server" ID="pnlSearchCode" DefaultButton="btnSearchCode" Style="display: inline;">
                    <asp:TextBox ID="txtSearchCode" MaxLength="20" runat="server" Width="150px" />
                </asp:Panel>
                <asp:Button ID="btnSearchCode" runat="server" Text="Cerca e Seleziona" ToolTip="Ricerca per Codice" />
                <asp:TextBox CssClass="hiddenField" ID="txtCategoryCode" runat="server" Width="20px" />
            </td>
        </tr>
        <tr id="rowSelectCategorySchema" runat="server">
            <td class="label" width="20%">
                Versione:
            </td>
            <td width="80%">
                <telerik:RadComboBox runat="server" ID="rcbCategorySchemas" AutoPostBack="true"></telerik:RadComboBox>
            </td>
        </tr>
        <tr id="rowOnlyFascicolable" runat="server">
            <td class="label" width="20%">Visualizza:</td>
            <td width="80%">
                <telerik:RadButton Text="Piano Fascicolazione" runat="server" ToggleType="CheckBox" Checked="false" ID="btnSearchOnlyFascicolable" OnCheckedChanged="btnSearchOnlyFascicolable_CheckedChanged" />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="RadTreeCategory" runat="server" ShowLineImages="true" Width="100%" Height="100%">
        <Nodes>
            <telerik:RadTreeNode Expanded="true" Font-Bold="true" runat="server" Selected="true" Text="Classificatore" />
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma Selezione"></asp:Button>
</asp:Content>
