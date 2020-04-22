<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UtltADProperties.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltADProperties" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <asp:TextBox ID="txtUserToSearch" runat="server" Width="128px"></asp:TextBox>
    <asp:Button ID="btnSearch" runat="server" Text="Ricerca" />
    <asp:PlaceHolder ID="phProperty" runat="server"></asp:PlaceHolder>
</asp:Content>

