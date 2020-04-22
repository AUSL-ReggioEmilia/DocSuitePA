<%@ Page AutoEventWireup="false" Codebehind="UtltContatori.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltContatori" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Contatori" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:PlaceHolder ID="phTable" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
        <asp:Button ID="btnDocmAggiorna" runat="server" Text="Aggiorna Pratiche" />
        <asp:Button ID="btnProtAggiorna" runat="server" Text="Aggiorna Protocollo" />
        <asp:Button ID="btnReslAggiorna" runat="server" Text="Aggiorna" />
</asp:Content>