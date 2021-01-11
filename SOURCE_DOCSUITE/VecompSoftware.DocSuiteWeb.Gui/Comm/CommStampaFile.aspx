<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommStampaFile" Codebehind="CommStampaFile.aspx.vb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
	<HEAD>
		<title>CommStampa</title>
		<style type="text/css">THEAD { DISPLAY: table-header-group }
	TFOOT { DISPLAY: table-footer-group }
		</style>
		<script language="JavaScript">
			function doSaveAs() {
				if (document.execCommand){
					document.execCommand("SaveAs","1", "Stampa.htm");
				}else{
					alert('Funzionalità disponibile solo in Internet Exlorer 4.0 e successivi.');
				}
			}
		</script>
	</HEAD>
	<body onLoad="window.focus();">
		<form id="CommStampa" method="post" runat="server">
			<TABLE id="TBLTITLE" cellSpacing="0" cellPadding="1" width="100%" border="0">
				<asp:Panel id="pnlPrint" runat="server">
					<TBODY>
						<TR class="<%=SetType()%>-Titolo">
							<TD>&nbsp; <IMG title="Stampa Documento" onclick="document.all('TBLTITLE').style.display='none';window.print();document.all('TBLTITLE').style.display='inline'"
									alt="" src="Images/Printer16.gif"> &nbsp; <IMG title="Salva Documento" onclick="doSaveAs();" alt="" src="Images/Saveas.gif">
							</TD>
						</TR>
				</asp:Panel>
				<TR class="Spazio">
					<TD></TD>
				</TR>
			</TABLE>
			
			<iframe ID="pnlStampa" Runat="server" height="90%" width="100%" frameborder="0" scrolling="yes"></iframe>
			
			<asp:table id="tblStampa" runat="server" Width="100%">
				<asp:TableRow>
					<asp:TableCell Width="100%" Font-Bold="True" Text="Stampa Nulla"></asp:TableCell>
				</asp:TableRow>
			</asp:table>
		</form>
	</body>
</HTML>
