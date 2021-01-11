<%@ Page AutoEventWireup="false" CodeBehind="CommonContactGes.aspx.vb" EnableEventValidation="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonContactGes" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscContattiGes.ascx" TagName="ContattiGes" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphcontent">
    <usc:ContattiGes id="uscContattiGes" runat="server" />
</asp:Content>