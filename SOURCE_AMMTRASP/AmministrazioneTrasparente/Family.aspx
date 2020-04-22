<%@ Page AutoEventWireup="true" CodeBehind="Family.aspx.cs" Inherits="AmministrazioneTrasparente.Family" Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" %>

<%@ MasterType VirtualPath="~/MasterPages/DocumentSeries.Master" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <div class="well">
        <h1>
            <asp:Label runat="server" ID="lblFamilyName"></asp:Label>
        </h1>

        <asp:Repeater runat="server" ID="SubMenuRepeater">
            <HeaderTemplate>
                <ul class="nav" id="submenu">
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href='<%# ResolveUrl("~/SubSectionSelection.aspx?idSeries=" + Eval("Id")) + (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : "") %>'>
                        <%# Eval("Name") %>
                    </a>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Repeater runat="server" ID="customFamilyLinksRepeater">
            <HeaderTemplate>
                <ul class="nav" id="submenu">
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href='<%# Eval("Url") + (MyMaster.HistoryEnable ? "&history=" + MyMaster.StoricoEnabled : "") %>'>
                        <%# Eval("Text") %>
                    </a>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
