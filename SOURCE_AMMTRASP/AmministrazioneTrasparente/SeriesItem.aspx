<%@ Page Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" AutoEventWireup="true" CodeBehind="SeriesItem.aspx.cs" Inherits="AmministrazioneTrasparente.SeriesItem" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="AmministrazioneTrasparente.Tools" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock">
        <script type="text/javascript">
            $telerik.$(function () {
                var new_width = $telerik.$('#details-content').outerWidth();
                $telerik.$('.affix').outerWidth(new_width);

                var headerHeight = $telerik.$('#header').height();
                var headerMarginTop = parseInt($telerik.$('#header').css('margin-top').replace('px', ''));
                $telerik.$("#header-commands").sticky({ topSpacing: headerHeight + headerMarginTop, zIndex: 100000 });
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
                    <asp:Label runat="server" ID="lblHeader"></asp:Label><br />
                </h2>
            </li>
            <li>
                <a href="./index.aspx<%= (MyMaster.HistoryEnable ? "?history=" + MyMaster.StoricoEnabled : "") %>">
                    <img alt="Amministrazione Trasparente" title="clicca qui per tornare all'inizio" src="./img/Amministrazione_Trasparente_32_px.png" />
                </a>
            </li>
            <li>
                <a href="./Series.aspx?idSeries=<%= IdParentSeries %><%= (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : "") %>">
                    <img alt="Ricerca" title="clicca qui per cercare" src="./img/search.png" style="height: 32px;" />
                </a>
            </li>
        </ul>
        <h5 style="text-align: left">
            <asp:Label runat="server" ID="lblAnalyticsCounter" />
        </h5>
    </div>

    <div class="well" id="details-content" style="margin-top: 10px;">
        <table class="table table-striped table-condensed dettaglio">
            <tr style="">
                <td class="colonna-etichetta">Anno</td>
                <td>
                    <asp:Label runat="server" ID="lblYear"></asp:Label></td>
            </tr>
            <tr>
                <td class="colonna-etichetta ">Oggetto</td>
                <td runat="server" class="colonna-grassetto" id="tdSubject"></td>
            </tr>
            <tr>
                <td class="colonna-etichetta">Data di pubblicazione</td>
                <td>
                    <asp:Label runat="server" ID="lblPublishingDate"></asp:Label></td>
            </tr>
            <tr>
                <td class="colonna-etichetta">Data ultima modifica</td>
                <td>
                    <asp:Label runat="server" ID="lblLastChangedDate"></asp:Label></td>
            </tr>
            <tr>
                <td class="colonna-etichetta">Data di ritiro</td>
                <td>
                    <asp:Label runat="server" ID="lblRetireDate"></asp:Label></td>
            </tr>
            <asp:Repeater runat="server" ID="DynamicRows">
                <ItemTemplate>
                    <tr>
                        <td class="colonna-etichetta"><%# GetAttributeDescription(Eval("Key") as string) %></td>
                        <td><%# ((string)Eval("Value")).UrlToAnchor() %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <asp:Panel runat="server" ID="DocumentsPanel">
            <div class="list-group">
                <h2>
                    <asp:Label runat="server" ID="lblMainDocuments"></asp:Label></h2>
                <asp:Repeater runat="server" ID="DocumentsRepeater">
                    <ItemTemplate>
                        <a class="list-group-item" href='<%# ResolveUrl("~/Document?idSeriesItem=" + IdSeriesItem + "&idDoc=" + Eval("Id") + "&idSeries=" + IdParentSeries + "&ext=" + Path.GetExtension(Eval("Name").ToString())) %>'>
                            <img src="./img/download.png" alt="Scarica <%# Eval("Name") %>" />&nbsp;<%# (ViewDocumentFullName) ? Eval("Name") : "Documento Principale" + (((IList)DocumentsRepeater.DataSource).Count > 1 ? Convert.ToString(Container.ItemIndex
                                                                                                 + 1) : string.Empty) %>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel runat="server" ID="pnlHeaderAnnexed">
                    <h2>
                        <asp:Label runat="server" ID="lblAnnexed"></asp:Label></h2>
                </asp:Panel>
                <asp:Repeater runat="server" ID="AnnexedRepeater">
                    <ItemTemplate>
                        <a class="list-group-item" href='<%# ResolveUrl("~/Annexed?idSeriesItem=" + IdSeriesItem + "&idDoc=" + Eval("Id") + "&idSeries=" + IdParentSeries + "&ext=" + Path.GetExtension(Eval("Name").ToString())) %>'>
                            <img src="./img/download.png" alt="Scarica <%# Eval("Name") %>" />&nbsp;<%# Eval("Name") %>
                        </a>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
