<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserDesktop" CodeBehind="UserDesktop.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns='http://www.w3.org/1999/xhtml' runat="server">
<head runat="server">
    <title></title>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge, chrome=1" />
    <link href="../Content/style.css" rel="stylesheet" />
</head>
<body style="height: 100%; margin: 0px;">
    <form runat="server" style="height: 100%;">
        <telerik:RadScriptManager ID="ScriptManager1" runat="server"></telerik:RadScriptManager>
        <telerik:RadStyleSheetManager runat="server" EnableStyleSheetCombine="true" OutputCompression="AutoDetect">
        </telerik:RadStyleSheetManager>
        <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
            <script type="text/javascript" language="javascript">
                function SetFramePage(page) {
                    parent.$find('contentPane').set_contentUrl(page);
                    return false;
                }
            </script>
        </telerik:RadScriptBlock>
        <telerik:RadAjaxManager ID="tempAjaxManager" runat="server" UpdatePanelsRenderMode="Inline" />
        <telerik:RadSplitter BorderSize="0" PanesBorderSize="0" ID="userSplitter" runat="server" Height="100%" Width="100%">
            <%-- Menu --%>
            <telerik:RadPane ID="menuPane" runat="server" Width="255" CssClass="left">
                <telerik:RadAjaxPanel runat="server" ID="pnlMenu">
                    <telerik:RadTreeView runat="server" ID="RadTreeMenu" />
                </telerik:RadAjaxPanel>
            </telerik:RadPane>
            <telerik:RadSplitBar ID="middleSplitBar" runat="server" CollapseMode="Forward" />
            <%-- content --%>
            <telerik:RadPane ID="contentPane" runat="server"></telerik:RadPane>
        </telerik:RadSplitter>
    </form>
</body>
</html>
