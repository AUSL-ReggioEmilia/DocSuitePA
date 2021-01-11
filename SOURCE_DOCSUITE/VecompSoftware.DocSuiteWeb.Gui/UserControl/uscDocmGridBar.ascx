<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDocmGridBar.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocmGridBar" %>

<asp:Panel runat="server" ID="pnlGridBar">
    <asp:button id="btnStampa" runat="server" text="Stampa selezione" visible="False" width="120px" />
    <asp:Button ID="btnDocuments" runat="server" Text="Visualizza documenti" Visible="False" Width="130px" />
    <asp:button id="btnSelectAll" runat="server" text="Seleziona tutti" visible="False" width="120px" />
    <asp:button id="btnDeselectAll" runat="server" text="Annulla selezione" visible="False" width="120px" />
    <asp:button id="btnSetRead" runat="server" text="Segna come letti" visible="False" width="120px" />	
</asp:Panel>