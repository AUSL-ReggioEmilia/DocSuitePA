<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContatti" enableViewState="True" smartNavigation="True" Codebehind="TbltContatti.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Rubrica - Contatti" %>

<%@ Register Src="~/UserControl/uscContatti.ascx" TagName="Contatti" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:Contatti EditMode="true" id="uscContatti" MultiSelect="False" runat="server" SearchinRubrica="true" ShowDetails="true" ExcludeRoleRoot="false" />
</asp:Content>