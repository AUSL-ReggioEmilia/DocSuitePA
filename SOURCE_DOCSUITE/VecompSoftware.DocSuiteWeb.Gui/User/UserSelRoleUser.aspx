<%@ Page AutoEventWireup="false" CodeBehind="UserSelRoleUser.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserSelRoleUser" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Dirigente" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlockModal">
        <script type="text/javascript" language="javascript">

            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }

        </script>
    </telerik:RadScriptBlock>
    <table class="dataform">
        <tr>
            <td class="label">Nome settore:&nbsp;
            </td>
            <td>
                <asp:TextBox ID="txtNameFilter" MaxLength="30" runat="server" runat="server" Width="200px" />
                &nbsp;
                <asp:Button ID="cmdFilter" runat="server" Text="Ricerca" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="cphContent">
    <telerik:RadTreeView ID="Tvw" runat="server" Width="100%" />
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
</asp:Content>
