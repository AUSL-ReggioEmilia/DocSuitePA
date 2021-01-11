<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommBiblosAttributi" Codebehind="CommBiblosAttributi.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Proprietà Documento" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
	<table class="datatable" style="height:99%;">
		<tr class="Chiaro">
			<td valign="top">
				<telerik:RadTreeView runat="server" ID="rtvAttributes"></telerik:RadTreeView>
			</td>
		</tr>
	</table>
</asp:Content>