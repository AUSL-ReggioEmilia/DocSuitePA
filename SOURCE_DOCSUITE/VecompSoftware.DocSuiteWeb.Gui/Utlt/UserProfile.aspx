<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UserProfile.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserProfile" %>

<%@ Register Src="~/UserControl/uscUserProfile.ascx" TagName="UserProfile" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:UserProfile runat="server" ID="uscUserProfile" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>