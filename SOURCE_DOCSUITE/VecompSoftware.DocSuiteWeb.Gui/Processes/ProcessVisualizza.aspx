<%@ Page Title="Processo - Visualizzazione" Language="vb" AutoEventWireup="false" CodeBehind="ProcessVisualizza.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProcessVisualizza" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProcessDetails.ascx" TagName="uscProcessDetails" TagPrefix="usc" %>

<asp:Content ID="cn" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var processVisualizza;
            require(["Processes/ProcessVisualizza"], function (ProcessVisualizza) {
                $(function () {
                    processVisualizza = new ProcessVisualizza();
                    processVisualizza.processId = "<%= ProcessId %>";
                    processVisualizza.uscProcessDetailsId = "<%= uscProcessDetails.PanelDetails.ClientID %>";
                    processVisualizza.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server" ID="pageContent" Height="100%" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow Height="100%" HtmlTag="Div">
                <Content>
                    <usc:uscProcessDetails runat="server" ID="uscProcessDetails" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>
