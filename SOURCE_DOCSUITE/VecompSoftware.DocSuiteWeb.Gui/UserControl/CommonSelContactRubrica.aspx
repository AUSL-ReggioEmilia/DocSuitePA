<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CommonSelContactRubrica.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactRubrica" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Selezione Contatti Rubrica" %>
<%@ Register Src="~/UserControl/uscContatti.ascx" TagName="uscContatti" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <uc1:uscContatti EditMode="false" id="UscContatti1" runat="server" SearchMode="true" ShowDetails="false" Type="Comm" />
</asp:Content>