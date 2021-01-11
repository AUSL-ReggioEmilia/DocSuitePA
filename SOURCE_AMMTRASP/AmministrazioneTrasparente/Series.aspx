<%@ Page Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" AutoEventWireup="true" CodeBehind="Series.aspx.cs" Inherits="AmministrazioneTrasparente.Series" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <div class="well">
        <div>
            <h2>
                <asp:Label runat="server" ID="lblSeriesName" />
            </h2>
            <h5 style="text-align:left">
                <asp:Label runat="server" ID="lblAnalyticsCounter" />
            </h5>
        </div>

        <p>
            <asp:Label runat="server" ID="lblHeader" />
        </p>

        <asp:Panel runat="server" ID="pnlPreviewSeries">
            <h1>
                <asp:Label runat="server" ID="lblPriority" Style="margin-right: 50px;"></asp:Label>
                <asp:LinkButton runat="server" ID="hlPriority" NavigateUrl="#" Style="margin-right: 50px;" OnClick="hlPriority_Click"></asp:LinkButton>
                <asp:Label runat="server" ID="lblLastModifiedSeries" Font-Italic="true"></asp:Label>
                <asp:LinkButton runat="server" ID="hlLastModifiedSeries" NavigateUrl="#" OnClick="hlLastModifiedSeries_Click"></asp:LinkButton>
            </h1>
            <asp:PlaceHolder runat="server" ID="GridPlaceHolder"></asp:PlaceHolder>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlSearch">
            <table class="table-search">
                <tr style="display: none;">
                    <td class="td-label">Anno di Registrazione</td>
                    <td class="td-field">
                        <telerik:RadNumericTextBox CssClass="form-control input-sm" ID="inYear" MaxValue="2050" MinValue="2000" RenderMode="Lightweight" runat="server" Type="Number" />
                    </td>
                </tr>
                <tr>
                    <td class="td-label">Oggetto</td>
                    <td class="td-field">
                        <telerik:RadTextBox runat="server" ID="inSubject" CssClass="form-control input-sm" />
                    </td>
                </tr>
                <tr>
                    <td class="td-label">Data Pubblicazione Da</td>
                    <td class="td-field">
                        <telerik:RadDatePicker runat="server" ID="inDateFrom" CssClass=" input-sm" />
                    </td>
                </tr>
                <tr>
                    <td class="td-label">Data Pubblicazione A</td>
                    <td class="td-field">
                        <telerik:RadDatePicker runat="server" ID="inDateTo" CssClass="input-sm" />
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="dynamicDataPlaceHolder"></asp:PlaceHolder>
                <tr>
                    <td class="td-label">
                        <asp:Button runat="server" CssClass="btn btn-sm btn-info btn-cerca" Text="Visualizza dati pubblicati" ID="btnAllPublishedSeries" />
                    </td>
                    <td class="td-field">
                        <asp:Button runat="server" PostBackUrl="SeriesGrid.aspx" CssClass="btn btn-sm btn-info btn-cerca" Text="Cerca" ID="btnSearch" />
                        <asp:Button runat="server" CssClass="btn btn-sm btn-default btn-cerca" Text="" ID="btnSearchType" OnClick="btnSearchType_Click" Visible="false" />
                        <asp:HyperLink runat="server" ImageUrl="./img/excel.png" ID="csvLink"></asp:HyperLink>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
</asp:Content>
