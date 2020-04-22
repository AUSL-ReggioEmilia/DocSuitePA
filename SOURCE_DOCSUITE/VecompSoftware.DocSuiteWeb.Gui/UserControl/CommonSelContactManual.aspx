<%@ Page AutoEventWireup="false" CodeBehind="CommonSelContactManual.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactManual" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Contatto Manuale" %>

<%@ Register Src="~/UserControl/uscContattiGes.ascx" TagName="ContattiGes" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
     <usc:ContattiGes id="uscContattiManualGes" runat="server" />
</asp:Content>