<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="TbltContenitoriGesProprieta.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContenitoriGesProprieta" MasterPageFile="~/MasterPages/DocSuite2008.Master"%>

<%@ Register Src="~/UserControl/uscParameter.ascx" TagName="UscParameter" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <usc:UscParameter id="uscParam" runat="server" ViewMode="ContainerProperties" />
</asp:Content>