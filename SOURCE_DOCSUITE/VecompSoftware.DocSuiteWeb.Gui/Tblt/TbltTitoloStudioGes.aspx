<%@ Page AutoEventWireup="false" CodeBehind="TbltTitoloStudioGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTitoloStudioGes" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock2" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                var oWindow = GetRadWindow();
                oWindow.close("update");
            }
        </script>

    </telerik:RadScriptBlock>

    <telerik:RadTreeView ID="RadTreeView1" runat="server" />
</asp:Content>


<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table id="tblAdd" runat="server" visible="False" class="dataform">
        <tr>
            <td class="label" width="20%">Codice:</td>
            <td width="80%">
                <asp:TextBox ID="txtCode" MaxLength="10" runat="server" Width="100px" /></td>
        </tr>
        <tr>
            <td class="label">Descrizione:</td>
            <td>
                <telerik:RadTextBox ID="txtObject" MaxLength="50" runat="server" Width="100%" /><br>
                <asp:RequiredFieldValidator ControlToValidate="txtObject" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="RequiredFieldValidator2" runat="server" />
            </td>
        </tr>
        <tr class="Spazio">
            <td></td>
        </tr>
    </table>
    <table id="tblRename" runat="server" visible="False" class="dataform">
        <tr>
            <td style="width: 155px" class="label">Codice:</td>
            <td>
                <asp:TextBox Enabled="False" ID="txtOldCode" runat="server" Width="100px" />
            </td>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Descrizione:</td>
            <td>
                <telerik:RadTextBox Enabled="False" ID="txtOldObject" runat="server" Width="100%" />
            </td>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Nuovo Codice:
            </td>
            <td>
                <asp:TextBox ID="txtNewCode" MaxLength="10" runat="server" Width="100px" />
            </td>
        </tr>
        <tr>
            <td style="width: 155px" class="label">Nuova Descrizione:
            </td>
            <td>
                <telerik:RadTextBox ID="txtNewObject" MaxLength="50" runat="server" Width="100%" />
                <asp:RequiredFieldValidator ControlToValidate="txtNewObject" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="Requiredfieldvalidator3" runat="server"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
</asp:Content>
