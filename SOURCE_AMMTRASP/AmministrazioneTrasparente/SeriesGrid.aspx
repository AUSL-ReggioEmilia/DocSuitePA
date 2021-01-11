<%@ Page Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" AutoEventWireup="true" CodeBehind="SeriesGrid.aspx.cs" Inherits="AmministrazioneTrasparente.SeriesGrid" %>

<%@ PreviousPageType TypeName="AmministrazioneTrasparente.Series" %>
<%@ Register Src="UserControls/uscSeriesGrid.ascx" TagPrefix="usc" TagName="uscSeriesGrid" %>
<%@ Register Src="UserControls/uscSeriesGridConstraints.ascx" TagPrefix="usc" TagName="uscSeriesGridConstraints" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">>
 
        <script type="text/javascript">
            $telerik.$(function () {
                var new_width = $telerik.$('#grid-content').width();
                $telerik.$('.list-inline').width(new_width);

                var headerHeight = $telerik.$('#header').height();
                var headerMarginTop = parseInt($telerik.$('#header').css('margin-top').replace('px', ''));
                $telerik.$("#header-commands").sticky({ topSpacing: headerHeight + headerMarginTop });
            });

            $telerik.$(window).resize(function () {
                var headerHeight = $telerik.$('#header').height();
                var headerMarginTop = parseInt($telerik.$('#header').css('margin-top').replace('px', ''));
                $telerik.$("#header-commands").unstick();
                $telerik.$("#header-commands").sticky({ topSpacing: headerHeight + headerMarginTop });
            });
        </script>
    </telerik:RadScriptBlock>

    <div class="well well-sm" id="header-commands">
        <ul class="list-inline" style="margin-bottom: 0;">
            <li>
                <h2>
                    <asp:Label runat="server" ID="lblSeriesName"></asp:Label>
                </h2>
            </li>
            <li>
                <a href="./index.aspx<%= (MyMaster.HistoryEnable ? "?history=" + MyMaster.StoricoEnabled : "") %>">
                    <img alt="Amministrazione Trasparente" title="clicca qui per tornare all'inizio" src="./img/Amministrazione_Trasparente_32_px.png" />
                </a>
            </li>
            <li>
                <a href="./Series.aspx?idSeries=<%= IdSeries %><%= (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : "") %>">
                    <img alt="Ricerca" title="clicca qui per cercare" src="./img/search.png" style="height: 32px;" />
                </a>
            </li>
        </ul>
        <h5 style="text-align: left">
            <asp:Label runat="server" ID="lblAnalyticsCounter" />
        </h5>

    </div>

    <div class="well" id="grid-content" style="margin-top: 10px;">
        <usc:uscSeriesGrid runat="server" ID="uscSeriesGrid"></usc:uscSeriesGrid>
        <usc:uscSeriesGridConstraints runat="server" ID="uscSeriesGridConstraints"></usc:uscSeriesGridConstraints>
    </div>
</asp:Content>
