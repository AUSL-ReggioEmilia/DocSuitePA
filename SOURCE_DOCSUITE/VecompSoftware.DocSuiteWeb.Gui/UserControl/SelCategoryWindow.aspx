<%@ Page AutoEventWireup="false" CodeBehind="SelCategoryWindow.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SelCategoryWindow" Language="vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height:100%">
<head>
    <title></title>
</head>
<body>
    <form id="SelCategory" method="post" runat="server">
        <table id="tblContent" width="100%" border="0" >
	        <tr>
		        <td class="SXScuro" width="20%">Descrizione:&nbsp;</td>
			    <td class="DXChiaro" width="80%">
			        <asp:textbox id="txtCerca" runat="server" Width="300px"></asp:textbox>
			        <asp:button id="btnCerca" runat="server" ToolTip="Ricerca per Descrizione" Text="Cerca"></asp:button>
			    </td>
		    </tr>
		    <tr>
			    <td class="SXScuro" width="20%">Codice:&nbsp;</td>
			    <td class="DXChiaro" width="80%">
			        <asp:textbox id="txtCategoryCode" runat="server" Width="150px" MaxLength="20"></asp:textbox>
			        <asp:button id="btnCercaCodice" runat="server" ToolTip="Ricerca per Codice" Text="Cerca e Seleziona"></asp:button>
			        <asp:textbox id="txtCode" runat="server" Width="16px" MaxLength="10"></asp:textbox>
			    </td>
		    </tr>
		    <tr class="Chiaro">
		        <td colSpan="2">
                    <telerik:RadTreeView ID="RadTreeCategory" runat="server" Width="100%" RetainScrollPosition="False">

                    </telerik:RadTreeView>
	            </td>
		    </tr>
	    </table>
        <asp:Button id="btnConferma" runat="server" Text="Conferma Selezione"></asp:Button>	
    </form>
</body>
</html>