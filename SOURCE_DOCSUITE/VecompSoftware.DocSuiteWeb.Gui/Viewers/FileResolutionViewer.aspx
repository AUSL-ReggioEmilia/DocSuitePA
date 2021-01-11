<%@ Page AutoEventWireup="false" CodeBehind="FileResolutionViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.FileResolutionViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register TagPrefix="uc1" TagName="uscViewerLight" Src="~/Viewers/ViewerLight.ascx" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <div runat="server" class="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="ResolutionDocumentHandler" ID="ViewerLight" runat="server" />
    </div>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="send" runat="server" Width="120px" Text="Invia Mail" PostBackUrl="~/MailSenders/GenericMailSender.aspx?Type=Resl"/>
</asp:Content>