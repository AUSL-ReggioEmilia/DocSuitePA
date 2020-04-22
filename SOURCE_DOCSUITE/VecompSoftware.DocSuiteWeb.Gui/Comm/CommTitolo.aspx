<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommTitolo"
    Codebehind="CommTitolo.aspx.vb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html runat="server">
<head runat="server">
    <title>Comm-Titolo</title>
</head>
<body runat="server">
    <form method="post" runat="server">
        <table class="datatable">
            <tr>
                <th>
                    <%=request("Titolo")%>
                </th>
            </tr>
        </table>
    </form>
</body>
</html>
