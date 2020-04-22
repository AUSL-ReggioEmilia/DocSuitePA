<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslModifica.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslModifica" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscResolutionChange.ascx" TagName="UscResolutionChange" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:UscResolutionChange id="uscReslChange" runat="server" />
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConfirm" runat="server" text="Conferma modifica" />
</asp:Content>