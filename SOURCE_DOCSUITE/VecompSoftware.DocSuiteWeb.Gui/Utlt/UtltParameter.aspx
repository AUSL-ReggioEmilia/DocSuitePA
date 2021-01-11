<%@ Page AutoEventWireup="false" CodeBehind="UtltParameter.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltParameter" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Parametri" %>

<%@ Register Src="~/UserControl/uscParameter.ascx" TagName="UscParameter" TagPrefix="usc" %>
            
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:UscParameter id="uscParam" runat="server" ViewMode="ParameterEnv" />
</asp:Content>