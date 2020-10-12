<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UtltADProperties.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltADProperties" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <style type="text/css">
        code {font-size:large}
    </style>
    <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBarSearch" runat="server" Width="100%">
        <Items>
            <telerik:RadToolBarButton Value="search">
                <ItemTemplate>
                    <telerik:RadTextBox ID="txtAccount" EmptyMessage="Nome utente" runat="server" Width="170px"></telerik:RadTextBox>
                </ItemTemplate>
            </telerik:RadToolBarButton>
            <telerik:RadToolBarButton IsSeparator="true" />
            <telerik:RadToolBarButton Value="search" Text="Cerca" ImageUrl="~/App_Themes/DocSuite2008/images/search-transparent.png" />
        </Items>

    </telerik:RadToolBar>
    <asp:Literal ID="phProperty" runat="server"/>            
</asp:Content>