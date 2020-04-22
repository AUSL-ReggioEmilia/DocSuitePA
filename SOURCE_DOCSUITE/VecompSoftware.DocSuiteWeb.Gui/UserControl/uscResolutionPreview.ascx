<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscResolutionPreview.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionPreview" %>

<div class="Resl">
     <table class="datatable">
	    <tr>
		    <th colspan="4">
		        <asp:Label ID="lblType" runat="server" />
		    </th>
	    </tr>
         <tr runat="server" id="trNumber">
             <td class="label" style="width: 20%">
                 <asp:Label runat="server" ID="lblProvLabel" Font-Bold="true" />
             </td>
             <td style="width: 30%">
                 <asp:Label runat="server" ID="lblProvNumber" Font-Bold="true" />
             </td>
             <td class="label" style="width: 20%">
                 <asp:Label runat="server" ID="lblNumberLabel" Font-Bold="true" />
             </td>
             <td style="width: 30%">
                 <asp:Label runat="server" ID="lblNumberFull" Font-Bold="true" />
             </td>
         </tr>
	    <tr runat="server" id="trContainer">
		    <td class="label">Contenitore: </td>
		    <td colspan="3">
		        <asp:Label ID="lblContainer" runat="server" />
		    </td>
	    </tr>
	    <tr runat="server" id="trCategory">
		    <td class="label">Classificazione: </td>
		    <td colspan="3">
		        <asp:Label ID="lblCategoryDescription" runat="server" />
		    </td>
	    </tr>
	    <tr runat="server" id="trObject">
	        <td class="label">Oggetto: </td>
	        <td colspan="3">
	            <asp:Label ID="lblObject" runat="server" />
	        </td>
	    </tr>
    </table> 
</div>