<%@ Page AutoEventWireup="false" CodeBehind="MessageEmailView.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MessageEmailView" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" ValidateRequest="false" %>

<%@ Register Src="~/Viewers/ViewerLight.ascx" TagName="uscViewerLight" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <uc1:uscViewerLight runat="server" ID="viewerLight" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
    </asp:Panel>
</asp:Content>
