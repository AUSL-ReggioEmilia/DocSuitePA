<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserRoles.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserRoles" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Collaborazione - Modifica Autorizzazioni" %>

<%@ Register Src="~/UserControl/uscCollRoles.ascx" TagName="UscCollRoles" TagPrefix="usc" %>

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

            function Close() {
                var oWindow = GetRadWindow();
                oWindow.close('');
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="dataform">
        <tr>
            <td class="label"> Selezione Settore </td>
            <td><asp:dropdownlist id="ddlRoles" runat="server" AutoPostBack="True" /></td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:UscCollRoles runat="server" id="uscUserRoles" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button runat="server" ID="btnConfirm" Text="Conferma" />
</asp:Content>
