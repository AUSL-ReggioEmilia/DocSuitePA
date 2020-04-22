<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtPackageGes" Codebehind="ProtPackageGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow() {
                var oWindow = GetRadWindow();
                oWindow.close('');
            }
        </script>
   </telerik:RadScriptBlock>
    <table id="tbl" class="dataform">
        <tr>
            <td class="label">
               Utente:
            </td>
            <td width="30%">
                <asp:TextBox ID="txtAccount" runat="server" MaxLength="30" />
            </td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="txtAccount" ErrorMessage="Campo Obbligatorio" ID="rfvAccount" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label">
               Tipo:
            </td>
            <td>
                <asp:TextBox ID="txtOrigin" runat="server" MaxLength="1" />
            </td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="txtOrigin" ErrorMessage="Campo Obbligatorio" ID="rfvOrigin" runat="server" />
            </td>
        </tr>
       
            <tr id="rowPackage" runat="server">
                <td class="label">
                    Scatolone:
                </td>
                <td>
                    <asp:TextBox ID="txtPackage" runat="server" />
                </td>
                <td>&nbsp;</td>
            </tr>
        <tr>
            <td class="label">
                Documenti Consentiti:
            </td>
            <td>
                <asp:TextBox ID="txtMaxDocuments" runat="server" Text="0" />
            </td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="txtMaxDocuments" ErrorMessage="Campo Obbligatorio" ID="rfvMaxDocuments" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label">
                Stato:
            </td>
            <td>
                <asp:DropDownList ID="cmbState" runat="server" AutoPostBack="False" />
            </td>
            <td>
                <asp:RequiredFieldValidator ControlToValidate="cmbState" ErrorMessage="Campo Obbligatorio" ID="rfvState" runat="server" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConferma" runat="server" Text="Conferma" />
</asp:Content>