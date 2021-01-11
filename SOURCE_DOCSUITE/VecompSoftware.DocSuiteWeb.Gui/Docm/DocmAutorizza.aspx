<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmAutorizza" CodeBehind="DocmAutorizza.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Autorizzazioni" %>

<%@ Register Src="~/UserControl/uscDocument.ascx" TagName="Document" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:Document ID="uscDocumentData" runat="server" EnableViewState="true" />
    <usc:Settori Caption="Autorizzazione" HeaderVisible="true" ID="uscSettori" MultipleRoles="true" MultiSelect="true" Required="true" RequiredMessage="Campo Settori Obbligatorio" RoleRestictions="None" runat="server" Type="Docm" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma modifica" />
</asp:Content>

