<%@ Page AutoEventWireup="false" CodeBehind="ProtocolViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.ProtocolViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlMainContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);


                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";                
                ajaxFlatLoadingPanel.show(pnlButtons);
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlMainContent" CssClass="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="ProtocolDocumentHandler" ID="ViewerLight" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
    <asp:Button ID="btnSend" OnClientClick="ShowLoadingPanel();" runat="server" Width="120px" Text="Invia Mail" PostBackUrl="../MailSenders/ProtocolMailSender.aspx"/>
        <asp:Button ID="btnMailProtocol" OnClientClick="ShowLoadingPanel();" runat="server" Width="180px" Text="Invia Mail con protocollo" Visible="false"/>
        <asp:Button ID="btnPEC" OnClientClick="ShowLoadingPanel();" runat="server" Width="120px" Text="Invia PEC" Visible="false"/>
        <asp:Button ID="btnPECProtocol" OnClientClick="ShowLoadingPanel();" runat="server" Width="180px" Text="Invia PEC con protocollo" Visible="false"/>
        <asp:Button ID="btnAssegna" runat="server" OnClientClick="ShowLoadingPanel();" Width="120px" Text="Assegna" visible="false"/>
    </asp:Panel>
</asp:Content>