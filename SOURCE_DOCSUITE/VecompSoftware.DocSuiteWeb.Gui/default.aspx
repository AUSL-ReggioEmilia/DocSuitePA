<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui._default" CodeBehind="default.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html>
<head runat="server">
    <title id="myTitle" runat="server"></title>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge, chrome=1" /> 
    <link rel="shortcut icon" href="favicon.ico" />
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundles/css")%>
        <%: Styles.Render("~/bundles/browserConditions")%>
    </asp:PlaceHolder>
</head>
<body>
    <form runat="server">
        <telerik:RadScriptManager EnableHistory="true" EnablePartialRendering="true" EnableSecureHistoryState="false" ID="masterScript" runat="server" ScriptMode="Release" SupportsPartialRendering="true">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadStyleSheetManager runat="server" EnableStyleSheetCombine="true" OutputCompression="AutoDetect">
            
        </telerik:RadStyleSheetManager>
        <telerik:RadSplitter BorderSize="0" CssClass="noBorderSplitter" Height="100%" LiveResize="True" PanesBorderSize="0" ResizeMode="Proportional" ResizeWithBrowserWindow="True" ResizeWithParentPane="True" runat="server" Width="100%">
            <telerik:RadPane BorderStyle="None" Height="100%" ID="defaultFrame" runat="server" Scrolling="None" Width="100%" />
        </telerik:RadSplitter>
    </form>
</body>
</html>
