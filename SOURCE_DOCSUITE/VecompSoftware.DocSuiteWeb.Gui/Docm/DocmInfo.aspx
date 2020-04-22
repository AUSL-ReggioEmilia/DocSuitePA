<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmInfo" MasterPageFile="~/MasterPages/DocSuite2008.Master" Codebehind="DocmInfo.aspx.vb" Title="Pratica - Informazioni" %>

<%@ Register Src="~/UserControl/uscDocument.ascx" TagName="Document" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:Document ID="uscDocument" runat="server" />
</asp:Content>
