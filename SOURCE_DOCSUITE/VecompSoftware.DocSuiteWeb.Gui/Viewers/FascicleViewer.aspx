<%@ Page AutoEventWireup="false" CodeBehind="FascicleViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Viewers.FascicleViewer" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
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

            function StartWorkflow(sender, args) {
                SetWorkflowSessionStorage();
                var url = "../Workflows/StartWorkflow.aspx?Type=Fasc&DSWEnvironment=Fascicle&Callback=$FascicleViewer.aspx";
                OpenWindow("<%=ViewerLight.StartWorkflow_Window%>", url, true);
                document.getElementById("ctl00_cphContent_ViewerLight_PDFPane").style.display="none";
            }

            function OpenWindow(name, url, closeWorkflow) {
                var wnd = $find(name);
                wnd.setUrl(url);
                if (closeWorkflow && closeWorkflow === true) {
                    wnd.add_close(CloseWorkflow);
                }
                wnd.set_modal(true);
                wnd.center();
                wnd.show();
                return false;
            }

            function CloseWorkflow(sender, args) {
                sender.remove_close(CloseWorkflow);
                var result = args.get_argument();
                if (result) {
                    if (result.ActionName === "redirect" && result.Value && result.Value.length > 0) {
                        ShowLoadingPanel();
                        window.location.href = result.Value[0];
                        return;
                    }
                }
            }

            function getWfVisibilityValue() {
                var buttonVisibility = JSON.parse(sessionStorage.getItem("IsButtonWorkflowVisible"));
                if (buttonVisibility) {
                    $find("<%=btnWorkflow.ClientID%>").set_visible(buttonVisibility);
                } else {
                    $find("<%=btnWorkflow.ClientID%>").set_visible(false);
                }

            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pnlMainContent" CssClass="viewerWrapper">
        <uc1:uscViewerLight CheckBoxes="true" DocumentSourcePage="FascicleDocumentHandler" ID="ViewerLight" runat="server" FromFascicle="true" />
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnSend" OnClientClick="ShowLoadingPanel();" runat="server" Width="120px" Text="Invia Mail" PostBackUrl="~/MailSenders/FascicleMailSender.aspx"/>
    <telerik:RadButton ID="btnWorkflow" OnClientClicked="StartWorkflow" runat="server" Width="120px" Text="Avvia attività" Enabled="false" AutoPostBack="false"/>
</asp:Content>