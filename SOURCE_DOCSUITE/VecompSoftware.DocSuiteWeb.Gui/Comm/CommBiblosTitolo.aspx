<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommBiblosTitolo"
    CodeBehind="CommBiblosTitolo.aspx.vb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head id="Head1" runat="server">
    <title>Comm-Biblos Document</title>
</head>
<body onload="parent.FrameTitolo=true; parent.Controlla()">
    <form id="Form1" method="post" runat="server">
        <telerik:RadScriptManager runat="server" ID="radScriptManager" EnablePartialRendering="true"></telerik:RadScriptManager>
        <telerik:RadAjaxManager runat="server" ID="radAjaxManager1"></telerik:RadAjaxManager>
        <telerik:RadScriptBlock runat="server" ID="radScriptBlock1">
            <script language="javascript" type="text/javascript">
                function OpenWindow(name, width, height) {
                    var URL = '<%= ResolveUrl("~/Comm/CommBiblosAttributi.aspx?Titolo=Proprieta") %>';
                    var oWnd = parent.parent.top.radopen(URL, name);
                    oWnd.setSize(width, height);
                    oWnd.center();
                    return false;
                }
            </script>
        </telerik:RadScriptBlock>
        <telerik:RadWindowManager EnableViewState="False" ID="rwmDialogManager" runat="server">
            <Windows>
                <telerik:RadWindow ID="wdAttributes" Modal="true" runat="server" Title="Proprietà" />
            </Windows>
        </telerik:RadWindowManager>
        <div class="<%=Type%>">
            <table id="tblTitle" width="100%">
                <tr>
                    <td>
                        <table width="100%">
                            <tr class="titolo">
                                <td style="text-align: left;">
                                    <%=Titolo%>
                                </td>
                                <td style="text-align: right;">
                                    <asp:ImageButton ID="Proprieta" runat="server" ImageUrl="Images/Proprieta16.gif"
                                        ToolTip="Proprietà" OnClientClick="return OpenWindow('wdAttributes',700,350);">
                                    </asp:ImageButton>&nbsp;
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
                <tr class="tabella">
                    <td>
                        <b id="TestoSegnatura">
                            <%=Segnatura%>
                        </b>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
