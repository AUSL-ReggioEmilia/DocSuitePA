<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltOggettoGes"
    CodeBehind="TbltOggettoGes.aspx.vb"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(objOperation, objDescription, objCode, objId) {
                var oObject = new Object();
                oObject.Operation = objOperation;
                oObject.Name = objDescription;
                oObject.Code = objCode;
                oObject.ID = objId;

                var oWindow = GetRadWindow();
                oWindow.close(oObject);
            }
        </script>

    </telerik:RadScriptBlock>
    <telerik:RadTreeView EnableViewState="false" ID="RadTreeViewSelectedObject" runat="server" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table class="datatable">
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblOldCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldCode" runat="server" MaxLength="100" Width="100px"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblOldObject" Text="Descrizione:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtOldObject" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblNewCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtNewCode" runat="server" MaxLength="100" Width="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblNewObject" Text="Descrizione:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtNewObject" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ControlToValidate="txtObject" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlInserimento" runat="server" Width="100%">
        <table class="datatable">
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCode" runat="server" MaxLength="100" Width="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="25%">
                    <asp:Label runat="server" ID="lblObject" Text="Descrizione:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <telerik:RadTextBox ID="txtObject" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ControlToValidate="txtObject" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="rfvObject" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
</asp:Content>
