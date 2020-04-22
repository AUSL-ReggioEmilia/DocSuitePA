<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltClassificatoreGesNew.aspx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltClassificatoreGesNew" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

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

            function CloseWindow(operator) {
                var oWindow = GetRadWindow();
                oWindow.close(operator);
            }

        </script>
    </telerik:RadScriptBlock>
    <telerik:RadTreeView runat="server" ID="RadTreeViewSelectedCategory" EnableViewState="false" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table class="dataform">
            <!-- OLD -->
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblOldCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldCode" runat="server" MaxLength="100" Width="40%" Enabled="False"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblOldName" Text="Nome:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldName" runat="server" MaxLength="100" Width="100%" Enabled="False"></telerik:RadTextBox>
                </td>
            </tr>
            <asp:Panel ID="pnlOldStartDate" runat="server">
                <tr>
                    <td class="label" width="25%">
                        <asp:Label runat="server" Text="Data di attivazione:" Font-Bold="true"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="rdpOldStartDate" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" runat="server" />         
                    </td>
                </tr>
            </asp:Panel>
            <!-- EDIT -->
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblNewCodice" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtNewCode" runat="server" MaxLength="100" Width="40%"></telerik:RadTextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic" ErrorMessage="Errore formato" ControlToValidate="txtNewCode" ValidationExpression="\d*" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Campo codice obbligatorio" ControlToValidate="txtNewCode" />
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblNewName" Text="Nome:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtNewName" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic"
                        ErrorMessage="Campo nome obbligatorio" ControlToValidate="txtName" />
                </td>
            </tr>
             <asp:Panel ID="pnlNewStartDate" runat="server">
                <tr>
                    <td class="label" align="right" width="25%">
                        <asp:Label runat="server" Text="Data di attivazione:" Font-Bold="true"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="rdpNewStartDate" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" runat="server" />         
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </asp:Panel>
    <!-- INSERT -->
    <asp:Panel ID="pnlInserimento" runat="server" Width="100%">
        <table class="dataform">
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtCode" runat="server" MaxLength="100" Width="40%"></telerik:RadTextBox>
                    <asp:RegularExpressionValidator ID="vNumber" runat="server" Display="Dynamic" ErrorMessage="Errore formato" ControlToValidate="txtCode" ValidationExpression="\d*" />
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Campo codice obbligatorio" ControlToValidate="txtCode" />
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblName" Text="Nome:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtName" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Campo nome obbligatorio" ControlToValidate="txtName" />
                </td>
            </tr>
           <asp:Panel ID="pnlStartDate" runat="server">
                <tr>
                    <td class="label" align="right" width="25%">
                        <asp:Label runat="server" ID="lblStartDate" Text="Data di attivazione:" Font-Bold="true"></asp:Label>
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="rdpStartDate" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="150" runat="server" />         
                        <asp:RequiredFieldValidator ID="rfvStartDate1" runat="server" Display="Dynamic" ErrorMessage="Campo Data di attivazione obbligatorio" ControlToValidate="rdpStartDate" />
                    </td>
                </tr>
            </asp:Panel>
        </table>
    </asp:Panel>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
</asp:Content>
