<%@ Page AutoEventWireup="false" CodeBehind="PECView.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECView" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" ValidateRequest="false" %>
<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            function Handle(take) {
                var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest(take ? "Handle" : "Unhandle");
            }

            function ShowLoadingPanel() {
                var ajaxDefaultLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var viewerLight = "<%= viewerLight.ClientID%>";
                //show the loading panel over the updated control
                ajaxDefaultLoadingPanel.show(viewerLight);


                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                //show the loading panel over the updated control
                ajaxFlatLoadingPanel.show(pnlButtons);
            }


        </script>
    </telerik:RadCodeBlock>

    <asp:Panel runat="server" ID="WarningPanel" CssClass="hiddenField">
        <asp:Label ID="WarningLabel" runat="server" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDestination" CssClass="hiddenField">
        <asp:Label ID="lblDestination" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel runat="server" ID="pnlMainContent" style="width:100%!important;height:100%!important;overflow:hidden;">
        <uc1:uscViewerLight runat="server" DocumentSourcePage="PecDocumentHandler" ID="viewerLight" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <div>
            <asp:Button ID="btnDestinate" PostBackUrl="~/PEC/PECDestination.aspx" runat="server" Text="Destina" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnPECToDocumentUnit" OnClientClick="ShowLoadingPanel();" PostBackUrl="~/Pec/PECToDocumentUnit.aspx?isInWindow=true" runat="server" Text="Protocolla" Width="150" />
            <asp:Button ID="btnAttachPec" PostBackUrl="~/PEC/PECAttachToDocumentUnit.aspx" runat="server" Text="Allega" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnLinkToProtocol" PostBackUrl="~/Pec/PECLinkProtocol.aspx" runat="server" Text="Collega" Width="150" Visible="False" OnClientClick="ShowLoadingPanel();"/>
            <asp:Button ID="btnReply" PostBackUrl="~/PEC/PECInsert.aspx" runat="server" Width="150" OnClientClick="ShowLoadingPanel();" Visible="False" Enabled="true" />
            <asp:Button ID="btnMovePEC" PostBackUrl="~/PEC/PECMoveMails.aspx" runat="server" Text="Sposta" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnMail" PostBackUrl="~/MailSenders/PecMailSender.aspx" runat="server" Text="Invia" Width="150" OnClientClick="ShowLoadingPanel();" />
        </div>

        <div>
            <asp:Button ID="cmdPECView" runat="server" Text="Sommario" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnUnhandle" runat="server" Text="Rilascia" Visible="False" Width="150" CommandArgument="False" Enabled="true" />
            <asp:Button ID="btnHandle" runat="server" Text="Prendi in carico" Visible="False" Width="150" CommandArgument="True" Enabled="true" />
            <asp:Button ID="btnDelete" PostBackUrl="~/PEC/PECDelete.aspx?Type=PEC" runat="server" Text="Elimina" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnRestore" runat="server" Text="Ripristina" Width="150" />
            <asp:Button ID="btnForward" PostBackUrl="~/PEC/PECInsert.aspx" runat="server" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="btnReceipt" runat="server" Text="Ricevuta" Visible="False" Width="150" />
            <asp:Button ID="btnViewLog" PostBackUrl="PecViewLog.aspx" runat="server" Text="LOG" Width="150" OnClientClick="ShowLoadingPanel();" />
        </div>
    </asp:Panel>
</asp:Content>
