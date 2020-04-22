<%@ Page AutoEventWireup="false" CodeBehind="UtltProtAutorizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltProtAutorizza1" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Autorizzazioni" %>

<%@ Register Src="../UserControl/uscSettori.ascx" TagName="uscSettori" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }

            function OnClick(args) {
                GetRadWindow().BrowserWindow.Esegui(args);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscSettori ID="UscSettori1" runat="server" HeaderVisible="False" MultipleRoles="True" multiselect="True" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" />
</asp:Content>
