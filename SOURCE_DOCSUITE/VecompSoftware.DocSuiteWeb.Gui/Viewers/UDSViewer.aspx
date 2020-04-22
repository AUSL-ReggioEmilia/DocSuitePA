<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSViewer" %>
<%@ Register Src="ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= pnlViewer.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);

                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= btnSend.ClientID%>";
                 ajaxFlatLoadingPanel.show(pnlButtons);

            
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlViewer" CssClass="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="UDSDocumentHandler" ID="ViewerLight" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
     <asp:Button ID="btnSend" OnClientClick="ShowLoadingPanel();" runat="server" Width="120px" Text="Invia Mail"/>
</asp:Content>
