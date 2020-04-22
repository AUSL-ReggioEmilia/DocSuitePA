<%@ Page AutoEventWireup="false" CodeBehind="BiblosViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.BiblosViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="" %>
<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <div runat="server" class="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" ID="ViewerLight" runat="server" />
    </div>
</asp:Content>
