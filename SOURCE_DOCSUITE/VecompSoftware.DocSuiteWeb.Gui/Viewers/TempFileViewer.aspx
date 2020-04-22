<%@ Page AutoEventWireup="false" CodeBehind="TempFileViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.TempFileViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register TagPrefix="uc1" TagName="uscViewerLight" Src="~/Viewers/ViewerLight.ascx" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
        <uc1:uscViewerLight CheckBoxes="true" ID="ViewerLight" runat="server" />
</asp:Content>