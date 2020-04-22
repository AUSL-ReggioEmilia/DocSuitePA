<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.FrameSet" CodeBehind="FrameSet.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DocSuite</title>
    <meta http-equiv="Content-Type" content="text/html;charset=utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge, chrome=1" />
    <link href="" type="text/css" rel="stylesheet" id="stylesheet" />
    <asp:PlaceHolder runat="server">
        <%: Styles.Render("~/bundles/css")%>
        <%: Styles.Render("~/bundles/browserConditions")%>
    </asp:PlaceHolder>
    <link rel="stylesheet" href="Content/menu.css" />
</head>
<body>
    <form id="Form1" runat="server">
        <telerik:RadScriptManager runat="server" EnableEmbeddedjQuery="false">
            <Scripts>
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
                <telerik:RadScriptReference Path="~/js/CookieUtil.js" />
            </Scripts>
        </telerik:RadScriptManager>
        <telerik:RadStyleSheetManager runat="server" EnableStyleSheetCombine="true" OutputCompression="AutoDetect">
        </telerik:RadStyleSheetManager>
        <telerik:RadScriptBlock runat="server">
            <script type="text/javascript">
                window.$ = $telerik.$; // Aggiungo al namespace della pagina il simbolo di jQuery preso dalla versione telerik
                function fix() {
                    document.documentElement.focus();
                    Sys.Application.remove_load(fix);
                }
                Sys.Application.add_load(fix);

                //Controllo risoluzione minima del Client (1024x768)
                if ((screen.width < 1024) || (screen.height < 768)) {
                    alert('Per un corretto funzionamento è consigliato impostare una risoluzione minima di 1024x768.');
                }
            </script>
            <script type="text/javascript" src="Scripts/jquery.signalR-2.4.1.min.js"></script>
            <script type="text/javascript" src="signalr/hubs?version=<%= DSWVersion %>"></script>
            <script type="text/javascript" src="Scripts/dsw.signalR.js?version=<%= DSWVersion %>"></script>
            <script type="text/javascript" src="Scripts/dsw.frameset.js?version=<%= DSWVersion %>"></script>
            <script type="text/javascript">
                Sys.Application.add_load(function (e, args) {
                    if (!args.get_isPartialLoad()) {
                        framesetElement.mainElement = $find("<%= main.ClientID%>");
                        framesetElement.slidingZoneElement = $find("<%= SlidingZoneMenu.ClientID%>");
                        framesetElement.currentSignalRUrl = "<%= VecompSoftware.DocSuiteWeb.Data.DocSuiteContext.Current.CurrentTenant.DSWUrl %>";
                        framesetElement.btnProtocolNotReader = "<%= btnProtocolNotReaded.ClientID %>";
                        framesetElement.btnProtocolInvoiceNotReaded = "<%= btnProtocolInvoiceNotReaded.ClientID %>";
                        framesetElement.btnProtocolToAccept = "<%= btnProtocolToAccept.ClientID %>";
                        framesetElement.btnProtocolRefused = "<%= btnProtocolRefused.ClientID %>";
                        framesetElement.btnProtocolToDistribute = "<%= btnProtocolToDistribute.ClientID %>";
                        framesetElement.btnProtocolRejected = "<%= btnProtocolRejected.ClientID %>";
                        framesetElement.btnCollToProtocol = "<%= btnCollToProtocol.ClientID %>";
                        framesetElement.btnCollToVision = "<%= btnCollToVision.ClientID %>";
                        framesetElement.btnWorkflow = "<%= btnWorkflow.ClientID %>";
                        framesetElement.notificationInterval = <%= ProtocolEnv.NotificationTimer %>;
                        framesetElement.notificationEnabled = "<%= ProtocolEnv.NotificationEnabled %>";
                        framesetElement.btnHighlightProtocols = "<%= btnHighlightProtocols.ClientID %>";
                        framesetElement.btnPECNotReader = "<%= btnPECNotReaded.ClientID %>";
                        framesetElement.btnLastPagesToSign = "<%= btnLastPagesToSign.ClientID %>";
                        framesetElement.btnRequestStatement = "<%= btnRequestStatement.ClientID %>";

                        initializeSignalR();
                    }
                });

                function reloadSession() {
                    document.getElementById('RAD_SPLITTER_PANE_EXT_CONTENT_main').contentWindow.location.reload(true);
                }

            </script>
        </telerik:RadScriptBlock>
        <telerik:RadAjaxManager ID="tempAjaxManager" runat="server" UpdatePanelsRenderMode="Inline" />
        <telerik:RadAjaxLoadingPanel ID="defaultLoadingPanel" runat="server" />
        <telerik:RadSplitter BorderSize="0" Height="100%" ID="splitter" Orientation="Horizontal" PanesBorderSize="0" ResizeWithBrowserWindow="true" ResizeWithParentPane="true" runat="server" Width="100%">
            <%-- Header --%>
            <telerik:RadPane ID="header" runat="server" Width="100%" Height="31px" Scrolling="None">
                <table class="bodyFrameHeader">
                    <tr>
                        <td class="leftHead">
                            <asp:Label ID="applicationTitle" runat="server" />
                        </td>
                        <td class="rightHead">
                            <asp:Panel ID="notificationPanel" runat="server" CssClass="dsw-display-inline">
                                <telerik:RadButton runat="server" ID="btnProtocolNotReaded" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli da leggere" Visible="false">
                                    <Icon PrimaryIconUrl="Comm/Images/DocSuite/Protocollo16.gif" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnProtocolInvoiceNotReaded" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli di fattura da leggere" Visible="false">
                                    <Icon PrimaryIconUrl="~/App_Themes/DocSuite2008/imgset16/invoice-icon.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnProtocolToDistribute" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli da distribuire" Visible="false">
                                    <Icon PrimaryIconUrl="Comm/Images/DocSuite/Protocollo16.gif" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnProtocolRejected" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli rigettati" Visible="false">
                                    <Icon PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnProtocolToAccept" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli da accettare" Visible="false">
                                    <Icon PrimaryIconHeight="16px" PrimaryIconUrl="Comm/Images/DocSuite/Protocollo16.gif" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnProtocolRefused" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli respinti" Visible="false">
                                    <Icon PrimaryIconHeight="16px" PrimaryIconUrl="Comm/Images/DocSuite/Protocollo16.gif" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnHighlightProtocols" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Protocolli in evidenza" Visible="false">
                                    <Icon PrimaryIconUrl="Comm/Images/DocSuite/Protocollo16.gif" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnCollToProtocol" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Collaborazioni da protocollare/gestire" Visible="false">
                                    <Icon PrimaryIconUrl="~/App_Themes/DocSuite2008/imgset16/collaboration.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnCollToVision" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Collaborazioni da visionare/firmare" Visible="false">
                                    <Icon PrimaryIconUrl="~/App_Themes/DocSuite2008/imgset16/ToSign.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnWorkflow" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Attività assegnate" Visible="false">
                                    <Icon PrimaryIconUrl="Comm/Images/DocSuite/Workflow16.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnPECNotReaded" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="PEC da leggere" Visible="false">
                                    <Icon PrimaryIconUrl="~/App_Themes/DocSuite2008/imgset16/pec.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnLastPagesToSign" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Ultime pagine da firmare" Visible="false">
                                    <Icon PrimaryIconUrl="Comm/Images/DocSuite/Protocollo16.gif" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                                <telerik:RadButton runat="server" ID="btnRequestStatement" ButtonType="LinkButton" OnClientClicking="ToolBarClientClicking"
                                    Target="main" ToolTip="Richiesta di dematerializzazione" Visible="false">
                                    <Icon PrimaryIconUrl="Comm/Images/DocSuite/Workflow16.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                                </telerik:RadButton>
                            </asp:Panel>
                            <telerik:RadButton ID="btnScrivania" runat="server" Text="Scrivania" ToolTip="Vai a Scrivania" ButtonType="LinkButton">
                                <Icon PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                            </telerik:RadButton>
                            <telerik:RadButton ID="btnCompanyName" runat="server" Text="No Company" ToolTip="Configurazione utente" ButtonType="LinkButton" Visible="false">
                                <Icon PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                            </telerik:RadButton>
                            <telerik:RadButton ID="btnReloadSessionTenant" AutoPostBack="true" runat="server" Text="" ToolTip="Ricarica diritti AOO" ButtonType="LinkButton" Visible="false" CssClass="btnReloadSession">
                                <Icon PrimaryIconUrl="App_Themes/DocSuite2008/imgset16/Activity_16x.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                            </telerik:RadButton>
                            <telerik:RadButton ID="btnHelp" runat="server" ToolTip="Documentazione" ButtonType="LinkButton" Visible="true" NavigateUrl="User/ZenDeskHelp.aspx?IsButtonPressed=True">
                                <Icon PrimaryIconUrl="App_Themes/DocSuite2008/imgset16/help.png" PrimaryIconHeight="16px" PrimaryIconWidth="16px" />
                            </telerik:RadButton>
                        </td>
                    </tr>
                </table>
            </telerik:RadPane>
            <%-- Content --%>
            <telerik:RadPane ID="content" runat="server" Height="100%">
                <telerik:RadSplitter BorderSize="0" Height="100%" ID="RadSplitterContent" OnClientLoaded="SplitterLoaded" PanesBorderSize="0" ResizeWithParentPane="True" runat="server" Width="100%">
                    <telerik:RadPane ID="RadPaneMenu" runat="server" Width="28px" Height="100%">
                        <telerik:RadSlidingZone ID="SlidingZoneMenu" runat="server" Width="28px" Height="100%" ExpandedPaneId="SlidingPaneMenu" ClickToOpen="true" SlideDuration="0">
                            <telerik:RadSlidingPane ID="SlidingPaneMenu" TabView="ImageOnly" IconUrl="Comm/Images/Home/Menu.jpg" runat="server" Width="260" Overlay="true" Height="100%"
                                MaxWidth="260" MinWidth="180" PersistScrollPosition="true" OnClientDocked="SaveDockedCookie" OnClientUndocked="SaveUnDockedCookie"
                                OnClientExpanded="RadSlidingPane_OnClientExpanded" OnClientCollapsing="RadSlidingPane_OnClientCollapsing">
                                <telerik:RadPanelBar ID="RadPanelBarMenu" runat="server" Width="100%" Height="100%" ExpandMode="FullExpandedItem" OnClientItemClicked="OnClickedSaveCookie" Skin="Bootstrap" CssClass="PanelBarMenu">
                                    <ExpandAnimation Type="None" />
                                    <CollapseAnimation Type="None" />
                                    <Items>
                                        <telerik:RadPanelItem Expanded="true" ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" Value="UtenteItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/images/desk/Desk.png" Value="DeskItem" Visible="False" Width="100%" runat="server"></telerik:RadPanelItem>
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/collaboration.png" Value="CollaborazioneItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/Comm/Images/DocSuite/Pratica16.gif" Value="PraticheItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/Comm/Images/DocSuite/Protocollo16.gif" Value="ProtocolloItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem Value="SeriesItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/fascicle_open.png" Value="FascicoliItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/invoice-icon.png" Value="InvoiceItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/user_info.png" Value="ContrattiItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/Comm/Images/DocSuite/Atti16.gif" Value="AttiItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/pec.png" Value="PECItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/AdminTable.png" Value="TabelleItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/Comm/Images/DocSuite/Collegamento16.gif" Text="Utilità" Value="UtilitaItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/Admin.png" Value="AmministrazioneItem" Visible="false" Width="100%" />
                                        <telerik:RadPanelItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/GroupEmpty.png" Text="Conservazione sostitutiva" Value="ConservazioneItem" Visible="false" Width="100%" />
                                    </Items>
                                </telerik:RadPanelBar>
                            </telerik:RadSlidingPane>
                        </telerik:RadSlidingZone>
                    </telerik:RadPane>
                    <telerik:RadPane ID="main" runat="server" Width="100%" />
                </telerik:RadSplitter>
            </telerik:RadPane>
            <%-- Footer --%>
            <telerik:RadPane ID="footer" runat="server" Height="25px" CssClass="bodyFrameFooter" Scrolling="None">
                <div style="float: left; margin-left: 10px;">
                    <asp:Label ID="applicationName" runat="server" />
                </div>
                <div style="float: right; margin-right: 5px;">
                    Utente:&nbsp;<asp:Label ID="lblUtenteCollegato" runat="server" />
                </div>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </form>
</body>
</html>
