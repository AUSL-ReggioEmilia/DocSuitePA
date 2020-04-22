<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommStampa" MasterPageFile="~/MasterPages/DocSuite2008.Master" Codebehind="CommStampa.aspx.vb" %>

<asp:Content ID="Content1" runat="server"  ContentPlaceHolderID="cphContent">
    
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
      	function doSaveAs() {
				if (document.execCommand){
					document.execCommand("SaveAs","1", "Stampa.htm");
				}else{
					alert('Funzionalità disponibile solo in Internet Exlorer 4.0 e successivi.');
				}
			}
        </script>			
    </telerik:RadScriptBlock>
        
        
        
        <table id="TBLTITLE" cellspacing="0" cellpadding="1" width="100%" border="0" style="overflow:auto">
            <asp:Panel ID="pnlPrint" runat="server">
                <tbody>
                    <tr class="titolo">
                        <td>
                            &nbsp;
                            <img title="Stampa Documento" onclick="document.all('TBLTITLE').style.display='none';window.print();document.all('TBLTITLE').style.display='inline'"
                                alt="" src="Images/Printer16.gif">
                            &nbsp;
                            <img title="Salva Documento" onclick="doSaveAs();" alt="" src="Images/Saveas.gif">
                        </td>
                    </tr>
            </asp:Panel>
            <tr class="Spazio">
                <td>
                </td>
            </tr>
        </table>
        <table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
            <thead>
                <asp:Panel ID="pnlPrintDate" runat="server">
                    <tr class="Prnt-Titolo">
                        <td align="center" width="10%">
                            <asp:Label ID="lblData" runat="server" Font-Bold="True"></asp:Label></td>
                        <td align="center" width="80%">
                            <asp:Label ID="lblAziendaT" runat="server" Font-Bold="True"></asp:Label><br>
                            <asp:Label ID="lblTitoloT" runat="server" Font-Bold="True"></asp:Label></td>
                        <td align="center" width="10%">
                            <asp:Label ID="lblOra" runat="server" Font-Bold="True"></asp:Label></td>
                    </tr>
                </asp:Panel>
                <asp:Panel ID="pnlNoPrintDate" runat="server">
                    <tr class="Prnt-Titolo">
                        <td align="center" width="100%" colspan="3">
                            <asp:Label ID="lblAziendaF" runat="server" Font-Bold="True"></asp:Label><br>
                            <asp:Label ID="lblTitoloF" runat="server" Font-Bold="True"></asp:Label></td>
                    </tr>
                </asp:Panel>
            </thead>
            <tr>
                <td colspan="3">
                    <asp:Table ID="tbl" runat="server" GridLines="Both" BorderWidth="1px" BorderStyle="Solid"
                        Width="100%" CellSpacing="0" CellPadding="1">
                    </asp:Table>
                </td>
            </tr>
        </table>
        <asp:Table ID="tblStampa" runat="server" Width="100%">
            <asp:TableRow>
                <asp:TableCell Width="100%" Font-Bold="True" Text="Stampa Nulla"></asp:TableCell>
            </asp:TableRow>
        </asp:Table>

</asp:Content>
