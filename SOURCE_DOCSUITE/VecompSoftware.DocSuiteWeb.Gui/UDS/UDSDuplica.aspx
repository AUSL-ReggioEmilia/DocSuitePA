<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSDuplica" CodeBehind="UDSDuplica.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Archivio - Selezione dati duplicazione" %>

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

            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table style="border-style: none"
        bordercolor="#999999" cellspacing="0" cellpadding="1" rules="cols" width="100%" height="100%" border="1">
        <asp:Panel runat="server" ID="UDSInfo">
            <asp:Panel ID="pnlRoles" runat="server" Visible="False">
                <tr>
                    <td class="SXScuro"></td>
                    <td class="DXChiaro">
                        <asp:CheckBox ID="cbRoles" runat="server" Checked="True" Text="Autorizzazioni"></asp:CheckBox></td>
                </tr>
            </asp:Panel>
            <asp:Panel ID="pnlContacts" runat="server" Visible="False">
                <tr>
                    <td class="SXScuro"></td>
                    <td class="DXChiaro">
                        <asp:CheckBox ID="cbContacts" runat="server" Checked="True" Text="Contatti"></asp:CheckBox></td>
                </tr>
            </asp:Panel>

            <tr>
                <td class="SXScuro"></td>
                <td class="DXChiaro">
                    <asp:CheckBox ID="cbOggetto" runat="server" Checked="True" Text="Oggetto"></asp:CheckBox></td>
            </tr>
            <tr>
                <td class="SXScuro"></td>
                <td class="DXChiaro">
                    <asp:CheckBox ID="cbClassificazione" runat="server" Checked="True" Text="Classificazione"></asp:CheckBox></td>
            </tr>
            <asp:Panel ID="metadatapnl" runat="server" Visible="False">
                <tr>
                    <td class="SXScuro"></td>
                    <td class="DXChiaro">
                        <asp:CheckBox ID="cbMetadata" runat="server" Checked="True" Text="Metadata"></asp:CheckBox></td>
                </tr>
            </asp:Panel>
        </asp:Panel>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButton">
        <br />
        <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
    </asp:Panel>
</asp:Content>
