<%@ Page AutoEventWireup="false" CodeBehind="DocumentSeriesItemViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.DocumentSeriesItemViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <div runat="server" class="viewerWrapper" id="pnlMainContent">
        <uc1:uscViewerLight CheckBoxes="true" ID="ViewerLight" runat="server" />
    </div>
</asp:Content>
