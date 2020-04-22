<%@ Page Language="vb" AutoEventWireup="false" 
    Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslDuplica" Codebehind="ReslDuplica.aspx.vb" 
    MasterPageFile="~/MasterPages/DocSuite2008.Master" 
%>

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

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-COLLAPSE: collapse; BORDER-BOTTOM-STYLE: none"
		bordercolor="#999999" cellspacing="0" cellpadding="1" rules="cols" width="100%" height="100%" border="1">
        <tr>
            <td class="Chiaro" width="6%">
            </td>
            <td class="Chiaro" width="94%">
            </td>
        </tr>
        <tr>
            <td class="SXScuro">
            </td>
            <td class="DXChiaro">
                <asp:CheckBox ID="cbTipologia" runat="server" Text="Tipologia" Checked="True"></asp:CheckBox></td>
        </tr>
        <tr>
            <td class="SXScuro">
            </td>
            <td class="DXChiaro">
                <asp:CheckBox ID="cbContenitore" runat="server" Text="Contenitore" Checked="True"></asp:CheckBox></td>
        </tr>
        <tr>
            <td class="SXScuro">
            </td>
            <td class="DXChiaro">
                <asp:CheckBox ID="cbDestinatari" runat="server" Checked="True" Text="Destinatari"></asp:CheckBox></td>
        </tr>
        <tr>
            <td class="SXScuro">
            </td>
            <td class="DXChiaro">
                <asp:CheckBox ID="cbProponente" runat="server" Checked="True" Text="Proponente"></asp:CheckBox></td>
        </tr>
        <asp:Panel ID="pnlAssegnatario" runat="server">
            <tr>
                <td class="SXScuro" style="height: 21px">
                </td>
                <td class="DXChiaro" style="height: 21px">
                    <asp:CheckBox ID="cbAssegnatario" runat="server" Checked="True" Text="Assegnatario">
                    </asp:CheckBox></td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlResponsabile" runat="server">
            <tr>
                <td class="SXScuro" style="height: 21px">
                </td>
                <td class="DXChiaro" style="height: 21px">
                    <asp:CheckBox ID="cbResponsabile" runat="server" Checked="True" Text="Responsabile">
                    </asp:CheckBox></td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="SXScuro">
            </td>
            <td class="DXChiaro">
                <asp:CheckBox ID="cbOggetto" runat="server" Text="Oggetto" Checked="True"></asp:CheckBox></td>
        </tr>
        <tr>
            <td class="SXScuro">
            </td>
            <td class="DXChiaro">
                <asp:CheckBox ID="cbNote" runat="server" Text="Note" Checked="True"></asp:CheckBox></td>
        </tr>
        <asp:Panel ID="pnlCategory" runat="server">
            <tr>
                <td class="SXScuro" style="height: 21px">
                </td>
                <td class="DXChiaro" style="height: 21px">
                    <asp:CheckBox ID="cbClassificazione" runat="server" Checked="True" Text="Classificazione">
                    </asp:CheckBox></td>
            </tr>
        </asp:Panel>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:button id="btnConferma" runat="server" text="Conferma" />
</asp:Content>