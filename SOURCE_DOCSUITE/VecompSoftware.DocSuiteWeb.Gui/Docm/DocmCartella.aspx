<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DocmCartella.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmCartella" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                var oWindow = GetRadWindow();
                oWindow.close('rimuovere');
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="FolderAdd" runat="server" Visible="False">
        <table id="TblAdd" class="datatable">
            <tr>
                <th colspan="2">Cartella
                </th>
            </tr>
            <tr>
                <td style="width: 155px; vertical-align: middle;" class="label">Nome Cartella:</td>
                <td>
                    <telerik:RadTextBox ID="txtFolderName" runat="server" Width="100%" MaxLength="50" />
                    <asp:RequiredFieldValidator ControlToValidate="txtFolderName" Display="Dynamic" ErrorMessage="Campo Nome Cartella Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 155px; vertical-align: middle;" class="label">Documenti Richiesti:</td>
                <td>
                    <asp:TextBox ID="txtDocNumber" runat="server" MaxLength="50" Width="50px" Text="0" />
                    <asp:RequiredFieldValidator ControlToValidate="txtDocNumber" Display="Dynamic" ErrorMessage="Campo Document Richiesti Obbligatorio" ID="Requiredfieldvalidator4" runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="txtDocNumber" Display="Dynamic" ErrorMessage="Il Campo deve essere numerico" ID="Regularexpressionvalidator2" runat="server" ValidationExpression="\d+" />
                </td>
            </tr>
            <tr>
                <td style="width: 155px; vertical-align: middle;" class="label">Data scadenza:</td>
                <td>
                    <telerik:RadDatePicker ID="rdpExpiryDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 155px; vertical-align: middle;" class="label">Descrizione:</td>
                <td>
                    <telerik:RadTextBox ID="txtDescription" runat="server" MaxLength="255" Width="100%" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="FolderRename" runat="server" Visible="False">
        <table id="TblRename" class="datatable">
            <tr>
                <th colspan="2">Cartella
                </th>
            </tr>
            <%--Vecchio--%>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Nome Cartella:</td>
                <td>
                    <telerik:RadTextBox ID="txtOldFolderName" runat="server" Width="100%" MaxLength="50" Enabled="False" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Documenti Richiesti:</td>
                <td>
                    <asp:TextBox ID="txtOldDocNumber" runat="server" MaxLength="50" Width="50px" Enabled="False" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Data scadenza:</td>
                <td>
                    <asp:TextBox runat="server" ID="txtOldExpiryDate" Width="75px" Enabled="false" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Descrizione:</td>
                <td>
                    <telerik:RadTextBox ID="txtOldDescription" runat="server" MaxLength="255" Width="100%" Enabled="false" />
                </td>
            </tr>
            <%--Nuovo--%>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Nuovo Nome Cartella:</td>
                <td>
                    <telerik:RadTextBox ID="txtNewFolderName" MaxLength="50" runat="server" Width="100%" />
                    <asp:RequiredFieldValidator ControlToValidate="txtNewFolderName" Display="Dynamic" ErrorMessage="Campo Nome Cartella Obbligatorio" ID="Requiredfieldvalidator2" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Nuovi Documenti Richiesti:</td>
                <td>
                    <asp:TextBox ID="txtNewDocNumber" MaxLength="50" runat="server" Width="50px" />
                    <asp:RequiredFieldValidator ControlToValidate="txtNewDocNumber" Display="Dynamic" ErrorMessage="Campo Document Richiesti Obbligatorio" ID="Requiredfieldvalidator3" runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="txtNewDocNumber" Display="Dynamic" ErrorMessage="Il Campo deve essere numerico" ID="Regularexpressionvalidator1" runat="server" ValidationExpression="\d+" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Nuova Data scadenza:</td>
                <td>
                    <telerik:RadDatePicker ID="rdpNewExpiryDate" runat="server" />
                </td>
            </tr>
            <tr>
                <td style="width: 175px; vertical-align: middle;" class="label">Nuova Descrizione:</td>
                <td>
                    <telerik:RadTextBox ID="txtNewDescription" runat="server" MaxLength="255" Width="100%" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
    <asp:Button ID="btnClearExpiryDate" runat="server" Style="" Text="Cancella Scadenza" />
    <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
</asp:Content>
