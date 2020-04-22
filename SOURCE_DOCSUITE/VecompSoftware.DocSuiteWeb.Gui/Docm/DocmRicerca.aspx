<%@ Page  AutoEventWireup="false" Codebehind="DocmRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmRicerca" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratiche - Ricerca" %>

<%@ Register Src="~/UserControl/uscDocumentFinder.ascx" TagName="UscDocumentFinder" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <style type="text/css" media="all">
        option.disabled { color: Gray; }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphcontent">
    <uc1:UscDocumentFinder ID="uscDocumentFinder" runat="server" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
        <asp:Button ID="btnSearch" Text="Ricerca" Width="100px" runat="server" />
        <asp:button id=btnNuovo runat="server" Visible="False" text="Inserimento"></asp:button>
        <asp:textbox id=txtProtYear runat="server" Width="16px" AutoPostBack="True" Visible="false"></asp:textbox>
        <asp:textbox id=txtProtNumber runat="server" Width="16px" AutoPostBack="True" Visible="false"></asp:textbox>
        <asp:textbox id=txtReslId runat="server" Width="16px" AutoPostBack="True" Visible="false"></asp:textbox>
</asp:Content>
