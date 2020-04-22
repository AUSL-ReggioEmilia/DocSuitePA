<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtDuplica" Codebehind="ProtDuplica.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Selezione dati duplicazione" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
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
	<table style="BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-COLLAPSE: collapse; BORDER-BOTTOM-STYLE: none"
		bordercolor="#999999" cellspacing="0" cellpadding="1" rules="cols" width="100%" height="100%" border="1">
		<tr>
			<td class="SXScuro" width="6%"></td>
			<td class="DXChiaro" width="94%"><asp:checkbox id="cbContenitore" runat="server" checked="True" text="Contenitore"></asp:checkbox></td>
		</tr>
		<asp:panel id="pnlInterop" runat="server">
			<tr>
				<td class="SXScuro"></td>
				<td class="DXChiaro">
					<asp:checkbox id="cbMittente" runat="server" checked="True" text="Mittenti"></asp:checkbox></td>
			</tr>
			<tr>
				<td class="SXScuro"></td>
				<td class="DXChiaro">
					<asp:checkbox id="cbDestinatario" runat="server" checked="True" text="Destinatari"></asp:checkbox></td>
			</tr>
		</asp:panel>
		<asp:panel id="pnlRoles" runat="server" visible="False">
			<tr>
				<td class="SXScuro"></td>
				<td class="DXChiaro">
					<asp:checkbox id="cbRoles" runat="server" checked="True" text="Autorizzazioni"></asp:checkbox></td>
			</tr>
		</asp:panel>

		<tr>
			<td class="SXScuro"></td>
			<td class="DXChiaro"><asp:checkbox id="cbOggetto" runat="server" checked="True" text="Oggetto"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="SXScuro" ></td>
			<td class="DXChiaro" ><asp:checkbox id="cbClassificazione" runat="server" checked="True" text="Classificazione"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="SXScuro"></td>
			<td class="DXChiaro"><asp:checkbox id="cbAltre" runat="server" checked="True" text="Altre (Note, Assegnatario/Proponente, Categoria di servizio)"></asp:checkbox></td>
		</tr>
		<asp:panel id="pnlDocType" runat="server" visible="False">
			<tr>
				<td class="SXScuro"></td>
				<td class="DXChiaro">
					<asp:checkbox id="cbDocType" runat="server" checked="True" text="Tipologia spedizione"></asp:checkbox></td>
			</tr>
		</asp:panel>
	</table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <br />
    <asp:button id="btnConferma" runat="server" text="Conferma"></asp:button>
</asp:Content>			
