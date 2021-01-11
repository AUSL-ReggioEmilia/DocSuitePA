<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommFrameset" Codebehind="CommFrameset.aspx.vb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head runat="server">
<title>
<%=request("Titolo")%>
</title>
</head>
<frameset border=0 cols=1 frameborder=no framespacing = 0>
<frame name = Padre scrolling =auto noresize src="<%=request("Pagina")%>?<%=Genera()%>">
</frameset>
</html>
