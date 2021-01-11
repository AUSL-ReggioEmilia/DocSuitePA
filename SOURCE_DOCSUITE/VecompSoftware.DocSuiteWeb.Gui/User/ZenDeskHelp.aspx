<%@ Page Language="vb" Title="Documentazione" AutoEventWireup="false" CodeBehind="ZenDeskHelp.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.ZenDeskHelp" %>

<%@ Register Src="~/UserControl/uscZenDeskHelp.ascx" TagName="uscZenDeskHelp" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:uscZenDeskHelp runat="server" ID="uscZenDeskHelp" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
