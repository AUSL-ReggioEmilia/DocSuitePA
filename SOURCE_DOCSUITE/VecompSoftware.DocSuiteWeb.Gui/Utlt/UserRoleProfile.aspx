<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="UserRoleProfile.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserRoleProfile" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscUserProfile.ascx" TagName="UserProfile" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <usc:UserProfile runat="server" ID="uscUserProfile" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
</asp:Content>
