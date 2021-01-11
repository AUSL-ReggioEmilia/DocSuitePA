<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmTokenRichiestaPresa" Codebehind="DocmTokenRichiestaPresa.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Richiesta Presa in Carico" %>

<%@ Register src="~/UserControl/uscDocumentToken.ascx" TagPrefix="uc" TagName="UscDocumentToken" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc:UscDocumentToken runat="server" ID="uscDocumentToken" StepVisible="false" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma"></asp:Button>
</asp:Content>    

