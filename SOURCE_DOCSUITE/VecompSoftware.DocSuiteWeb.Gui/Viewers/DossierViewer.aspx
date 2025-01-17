﻿<%@ Page AutoEventWireup="false" CodeBehind="DossierViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.DossierViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= btnSend.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlMainContent" CssClass="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="DossierDocumentHandler" ID="ViewerLight" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnSend" OnClientClick="ShowLoadingPanel();" runat="server" Width="120px" Text="Invia Mail" PostBackUrl="~/MailSenders/DossierMailSender.aspx" Visible="false"/>
</asp:Content>