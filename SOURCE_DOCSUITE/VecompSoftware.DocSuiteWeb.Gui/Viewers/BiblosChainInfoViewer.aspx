<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="BiblosChainInfoViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.BiblosChainInfoViewer" %>

<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <div runat="server" class="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="BiblosDocumentHandler" ID="ViewerLight" runat="server" />
    </div>
</asp:Content>
