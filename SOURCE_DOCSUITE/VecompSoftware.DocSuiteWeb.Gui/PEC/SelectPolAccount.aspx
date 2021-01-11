<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SelectPolAccount.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelectPolAccount" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement && window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function closeDialog() {
            alert('Invio raccomandata avvenuto con successo');
            var wnd = GetRadWindow();
            wnd.close();
        }

        function closeDialogWithError(errStr) {
            alert(errStr);
            var wnd = GetRadWindow();
            wnd.close();
        }
</script>
</asp:Content>

    <asp:Content runat="server" ContentPlaceHolderID="cphContent">
   
        <asp:Panel ID="panelDatatable" runat="server" >
            <table id="datatable" class="datatable" runat="server">
                <tr>
                    <th>Account PosteWeb</th>
                </tr>
                <tr>
                    <td valign="top">
                        <asp:DropDownList AppendDataBoundItems="true" DataTextField="Name" DataValueField="Id" ID="ddlPolAccount" runat="server" Width="300px" />
                        <asp:RequiredFieldValidator ControlToValidate="ddlPolAccount" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvPolAccount" runat="server" />
                    </td>                            
                </tr>
            </table>
            <asp:Button Text="Invia" ID="btnSendRaccomandata" runat="server" />
        </asp:Panel>

</asp:Content>
