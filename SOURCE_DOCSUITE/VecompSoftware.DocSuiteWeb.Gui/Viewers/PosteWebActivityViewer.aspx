<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PosteWebActivityViewer.aspx.vb" 
    Inherits="VecompSoftware.DocSuiteWeb.Gui.PosteWebActivityViewer" %>

<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscViewerLight Visible="true" CheckBoxes="true" DocumentSourcePage="GenericDocumentHandler" ID="uscDocument" runat="server" />
</asp:Content>
