<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonSelOChart.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelOChart" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Organigramma" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">

            // restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function ReturnValueOnClick(sender, args) {
                var values = new Array();
                values.push(args.get_node().get_value());
                if (WantToMove()) {
                    CloseWindow(values);
                }
            }

            function WantToMove() {
                return confirm("Sei sicuro di voler spostare il nodo di organigramma selezionato?");
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }


        </script>

    </telerik:RadScriptBlock>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div class="splitterWrapper">
        <telerik:RadSplitter runat="server" Height="100%" Width="100%">
            <telerik:RadPane runat="server" Scrolling="None">
                <telerik:RadSplitter runat="server" ID="treeSplitter" Width="100%" Height="100%" Orientation="Horizontal" ResizeMode="Proportional">
                    <telerik:RadPane runat="server" ID="treeToolbarPane" BorderStyle="None" Scrolling="None" Height="17" MinHeight="53" MaxHeight="53">
                        <asp:DropDownList Width="100%" AutoPostBack="True" DataTextField="Title" DataValueField="Id" ID="ddlOCharts" runat="server" />
                    </telerik:RadPane>
                    <telerik:RadPane runat="server" ID="treeDetailPane" BorderStyle="None" Scrolling="None" MinHeight="69" Height="89">
                        <table class="datatable">
                            <tr>
                                <td class="label" style="width: 20%">Nome
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadName"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Descrizione
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadDescription"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Stato
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadStatus"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Data Inizio
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadDateFrom"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="label">Data Fine
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblHeadDateTo"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </telerik:RadPane>
                    <telerik:RadPane runat="server" ID="treeInnerPane" BorderStyle="None" Height="80%">
                        <telerik:RadTreeView ID="OChartTree" runat="server" />
                    </telerik:RadPane>
                </telerik:RadSplitter>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </div>
</asp:Content>
