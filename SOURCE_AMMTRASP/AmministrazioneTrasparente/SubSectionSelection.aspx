<%@ Page AutoEventWireup="true" CodeBehind="SubSectionSelection.aspx.cs" Inherits="AmministrazioneTrasparente.SubSectionSelection" Language="C#" MasterPageFile="~/MasterPages/DocumentSeries.Master" %>

<asp:Content ContentPlaceHolderID="MainPlaceHolder" runat="server">
    <div class="well">
        <h1>
            <asp:Label runat="server" ID="lblSeriesName"></asp:Label>
        </h1>

        <p>
            <asp:Label runat="server" ID="lblHeader" />
        </p>

        <asp:Repeater runat="server" ID="SubSectionRepeater">
            <HeaderTemplate>
                <ul class="nav" id="submenu">
            </HeaderTemplate>
            <ItemTemplate>
                <li>
                    <a href='<%# ResolveUrl("~/Series.aspx?idSeries=" + Request["IdSeries"] + "&IdSubSection=" + Eval("Id")) %>'>
                        <%# Eval("Description") %>
                    </a>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
