<%@ Page Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" AutoEventWireup="true" CodeBehind="Statics.aspx.cs" Inherits="AmministrazioneTrasparente.Statics" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">

    <div class="well">
        <h2>Statistiche di accesso</h2>
    </div>
    <div>
        <telerik:RadGrid RenderMode="Lightweight" runat="server" ID="StatisticsGrid" AllowPaging="False" AllowSorting="true">
            <HeaderStyle Font-Bold="true" Font-Size="Medium" ForeColor="Black" />
        </telerik:RadGrid>
        <br/>
    </div>

    <div class="well">
        <h2>Riepilogo serie documentali</h2>
    </div>
    <div>
        <telerik:RadGrid RenderMode="Lightweight" runat="server" ID="SeriesStatisticsGrid" AllowPaging="False" AllowSorting="true">
            <HeaderStyle Font-Bold="true" Font-Size="Medium" ForeColor="Black" />
        </telerik:RadGrid>
        <br/>
    </div>
</asp:Content>
