<%@ Page AutoEventWireup="false" Codebehind="DocmToken.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmToken" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Pratica - Dettaglio WorkFlow" %>
<%@ Register Src="../UserControl/uscDocmToken.ascx" TagName="DocmToken" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">            
    <usc:DocmToken id="uscDocmToken" runat="server" />
</asp:Content>
