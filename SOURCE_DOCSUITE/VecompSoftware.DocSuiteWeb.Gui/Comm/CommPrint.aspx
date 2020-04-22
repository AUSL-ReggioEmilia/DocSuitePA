<%@ Page Language="vb" AutoEventWireup="false" Codebehind="CommPrint.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommPrint" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head id="Head1" runat="server">
    <style type="text/css">THEAD { DISPLAY: table-header-group }
	TFOOT { DISPLAY: table-footer-group }
	</style>
</head>
<body onload="window.focus();" style="overflow: auto;">
    <form id="Form1" method="post" runat="server">
        <telerik:RadScriptManager runat="server">
                <Scripts>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                    <telerik:RadScriptReference Path="~/js/CookieUtil.js" />
                </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadAjaxManager ID="ajaxManager" runat="server" UpdatePanelsRenderMode="Block">
            <ClientEvents></ClientEvents>
        </telerik:RadAjaxManager>
        <telerik:RadAjaxLoadingPanel ID="defaultLoadingPanel" runat="server" />
        <telerik:RadCodeBlock ID="radCodeBlock" runat="server">

            <script type="text/javascript">

                function pageLoad(sender, eventArgs) {
                    if (!eventArgs.get_isPartialLoad()) {
                        $find("<%= AjaxManager.ClientID %>").ajaxRequest("InitialPageLoad");
                    }
                }
               
                function doSaveAs() {
                    var fileName = "Stampa.htm";
                    var fileURL = location.href;

                    var element = $get("<%= pnlReport.ClientID%>")
                    // for IE            
                    if (document.execCommand) {
                        var oWin = window.open(fileURL, "_blank");
                        oWin.document.write(element.innerHTML);
                        oWin.document.close();
                        var success = oWin.document.execCommand('SaveAs', true, fileName);
                        oWin.close();
                        if (!success)
                            alert("Funzionalità disponibile solo in Internet Exlorer 4.0 e successivi.");
                    }
                    else {
                        alert('Funzionalità disponibile solo in Internet Exlorer 4.0 e successivi.');
                    }
                }
            </script> 
        </telerik:RadCodeBlock>


        <asp:Panel id="pnlReport" runat="server">
            <table id="TBLTITLE" cellspacing="0" cellpadding="1" width="100%" border="0">
                <asp:panel ID="pnlPrint" runat="server">
                    <tbody>
                        <tr class="titolo">
                            <td>
                                &nbsp;
                                <img title="Stampa Documento" onclick="document.all('TBLTITLE').style.display='none';window.print();document.all('TBLTITLE').style.display='inline';"
                                    alt="" src="Images/Printer16.gif">
                                &nbsp;
                                <img title="Salva Documento" onclick="doSaveAs();" alt="" src="Images/Saveas.gif">                                
                            </td>
                        </tr>
                </asp:panel>
                <tr class="Spazio">
                    <td>
                    </td>
                </tr>
            </table>
            <table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
                <thead>
                    <asp:panel ID="pnlPrintDate" runat="server">
                        <tr class="Prnt-Titolo">
                            <td align="center" width="10%">
                                <asp:Label ID="lblData" runat="server" Font-Bold="True"></asp:Label></td>
                            <td align="center" width="80%">
                                <asp:Label ID="lblAziendaT" runat="server" Font-Bold="True"></asp:Label><br>
                                <asp:Label ID="lblTitoloT" runat="server" Font-Bold="True"></asp:Label></td>
                            <td align="center" width="10%">
                                <asp:Label ID="lblOra" runat="server" Font-Bold="True"></asp:Label></td>
                        </tr>
                    </asp:panel>
                    <asp:panel ID="pnlNoPrintDate" runat="server">
                        <tr class="Prnt-Titolo">
                            <td align="center" width="100%" colspan="3">
                                <asp:Label ID="lblAziendaF" runat="server" Font-Bold="True"></asp:Label><br>
                                <asp:Label ID="lblTitoloF" runat="server" Font-Bold="True"></asp:Label></td>
                        </tr>
                    </asp:panel>
                </thead>
                <tr>
                    <td runat="server" id="tblCellPrint">
                    </td>
                </tr>
            </table>
            <asp:table ID="tblStampa" runat="server" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="100%" Font-Bold="True" Text="Stampa Nulla"></asp:TableCell>
                </asp:TableRow>
            </asp:table>
            <asp:panel ID="pRptViewer" runat="server">
            </asp:panel>        
            <br />
            <asp:placeholder runat="server" ID="phNoData"></asp:placeholder>
            <br />
        </asp:Panel>
    </form>
</body>
</html>
