<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslVisualizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslVisualizza" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscResolution.ascx" TagName="uscResolution" TagPrefix="usc" %>
<%@ Register Src="../UserControl/uscResolutionBar.ascx" TagName="uscResolutionBar" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript" language="javascript">
            function OpenWindowElimina() {
                var manager = $find("<%=RadWindowManagerResl.ClientID %>");
                var wnd = manager.open("<%= GetWindowDeletePage() %>", "windowDelete");
                wnd.center();
                return false;
            }

            function OpenWindowAnnulla(url) {
                var manager = $find("<%=RadWindowManagerResl.ClientID %>");
                var wnd = manager.open(url, "windowDelete");
                wnd.center();
                return false;
            }

            function OpenWindowDuplica() {
                var manager = $find("<%=RadWindowManagerResl.ClientID %>");
                var wnd = manager.open("../Resl/ReslDuplica.aspx?Type=Resl&Titolo=Duplicazione Atto", "windowDuplica");
                wnd.center();
                return false;
            }

            function OpenWindowLastPage(url) {
                var manager = $find("<%=RadWindowManagerResl.ClientID %>");
                var wnd = manager.open(url, "windowLastPage");
                wnd.center();
                return false;
            }

            function OpenWindowSelDocument() {
                var manager = $find("<%= RadWindowManagerResl.ClientID %>");
                var wnd = manager.open("<%= GetWindowSelDocument() %>", "windowDocmSceltaPratica");
                wnd.setSize(600, 400);
                wnd.center();
                return false;
            }

            function CloseSelPratica(sender, args) {
                CallAjaxRequest('SELDOC', args.get_argument());
            }

            function CloseElimina(sender, args) {
                CallAjaxRequest(args.get_argument(), '');
            }

            function CloseDuplica(sender, args) {
                CallAjaxRequest('DUPLICATE', args.get_argument());
            }

            function CloseRequestStatement(sender, args) {
                CallAjaxRequest('REQUESTSTATEMENT', args.get_argument());
            }

            function CloseLastPage(sender, args) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest('LASTPAGE');
            }

            function CallAjaxRequest(type, argument) {
                if (argument !== null) {
                    var manager = $find("<%= AjaxManager.ClientID %>");
                    manager.ajaxRequest(type + "|" + argument);
                }
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var reslVisualizza;
            require(["Resl/ReslVisualizza"], function (ReslVisualizza) {
                $(function () {
                    reslVisualizza = new ReslVisualizza(tenantModelConfiguration.serviceConfiguration);                    
                    reslVisualizza.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerResl" runat="server">
        <Windows>
            <telerik:RadWindow Height="180" ID="windowDelete" OnClientClose="CloseElimina" runat="server" Width="500" />
            <telerik:RadWindow Height="300" ID="windowDuplica" OnClientClose="CloseDuplica" runat="server" Title="Duplicazione Atto" Width="500" />
            <telerik:RadWindow Height="300" ID="windowRequestStatement" OnClientClose="CloseRequestStatement" runat="server" Title="Richiesta di Attestazione" Width="500" />
            <telerik:RadWindow Height="300" ID="windowDocmSceltaPratica" OnClientClose="CloseSelPratica" runat="server" Title="Pratiche - Seleziona" Width="500" />
            <telerik:RadWindow Height="380" ID="windowLastPage" OnClientClose="CloseLastPage" runat="server" Width="650" />
        </Windows>
    </telerik:RadWindowManager>

    <div runat="server" id="PageDiv" style="height: 100%">
        <table height="100%" id="tblPrincipale" runat="server" width="100%">
            <tr>
                <td style="width: 100%; vertical-align: top;">
                    <usc:uscResolution ID="uscResolution" runat="server" />
                </td>
                <td class="center" style="width: 3%; height: 100%;">
                    <asp:Table BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" CellSpacing="3" CssClass="datatable" Height="100%" ID="TblButtons" runat="server">
                        <asp:TableRow Height="100%">
                            <asp:TableCell Height="100%" VerticalAlign="Top" />
                        </asp:TableRow>
                    </asp:Table>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <usc:uscResolutionBar ID="resolutionBottomBar" runat="server" />
</asp:Content>
