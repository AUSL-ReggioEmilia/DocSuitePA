<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFasciclePreview.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFasciclePreview" %>

<div class="prot">
     <table id="tblAnteprimaFascicolo" class="datatable" runat="server">
	    <tr>
		    <th colspan="2" >Fasicolo Selezionato</th>
	    </tr>
	    <tr>
		    <td class="label" style="width:15%;">Fascicolo</td>
		    <td style="width:85%;">
		        <asp:Label ID="lblId" runat="server" Font-Bold="true"/>
		    </td>
	    </tr>
	    <tr>
		    <td class="label">Oggetto: </td>
		    <td>
		        <asp:Label ID="lblObject" runat="server" />
		    </td>
	    </tr><tr>
		    <td class="label">Classificazione: </td>
		    <td>
		        <asp:Label ID="lblCategory" runat="server" />
		    </td>
	    </tr>
    </table> 
</div>
