<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtModificaOggettiImport.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtModificaOggettiImport" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Modifica Oggetti Automatica" %>

<%@ Register Src="~/UserControl/uscProtocolObjectFinder.ascx" TagName="uscProtocolObjectFinder" TagPrefix="usc" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphHeader">
    <usc:uscProtocolObjectFinder runat="server" id="uscObjectFinder" HideNumberPanel="true"></usc:uscProtocolObjectFinder>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table id="tblSearch" class="datatable" runat="server">
		<tr>
			<th colspan="2">Risultati Ricerca</th>
		</tr>
	</table>
	<asp:table id="tblRicerca" runat="server">
        <asp:TableRow>
		    <asp:TableCell Font-Bold="True">
	            <asp:label id="lblCounter" runat="server"></asp:label>&nbsp;&nbsp;
		    </asp:TableCell>
		    <asp:TableCell>
		        <asp:Button id="btnConferma" runat="server" Text="Conferma Importazione" Visible="false"></asp:Button>
		    </asp:TableCell>
        </asp:TableRow>
     </asp:table>
</asp:Content>