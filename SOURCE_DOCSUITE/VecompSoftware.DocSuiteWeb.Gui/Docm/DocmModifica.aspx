<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmModifica"Codebehind="DocmModifica.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Modifica" %>
<%@ Register Src="~/UserControl/uscDocument.ascx" TagName="uscDocument" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscDocument ID="uscDocumentData" runat="server" EnableViewState="true" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" />
</asp:Content>