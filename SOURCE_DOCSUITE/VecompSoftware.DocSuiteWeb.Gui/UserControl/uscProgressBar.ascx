<%@ Control AutoEventWireup="false" Codebehind="uscProgressBar.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProgressBar" Language="vb" %>

<script type="text/javascript">
    var timeID;

    function setProgressBarPercentage(percentage) {
        var bar = $find('<%= taskProgress.ClientID %>');
        if (bar != null) {
            bar.set_value(percentage);
        }
    }

    function startProgress() {
        timeID = window.setInterval(
            function ControlUpdate() {
                var ajaxManager = <%= AjaxManager.ClientID %>;
                ajaxManager.ajaxRequest('RefreshArgument');
            },
            1000);
    }

    function stopProgress() {
        if (timeID != null) {
            window.clearInterval(timeID);
        }
    }
</script>

<telerik:RadPageLayout runat="server">
    <Rows>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center">
            <Content>
               <asp:Label ID="lblTitle" runat="server" Font-Bold="true"></asp:label> 
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center" Style="margin-bottom: 5px;">
            <Content>
                <telerik:RadProgressBar runat="server" ID="taskProgress" ShowLabel="false"></telerik:RadProgressBar>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center">
            <Content>
                <asp:Label ID="lblOperation" runat="server" Font-Bold="true"></asp:Label>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center">
            <Content>
                <asp:Label ID="lblDescription" runat="server" Font-Bold="true"></asp:Label>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center">
            <Content>
                <asp:Label ID="lblTime" runat="server" Font-Bold="true"></asp:Label>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center">
            <Content>
                <asp:Label ID="lblNote" runat="server" Font-Bold="true"></asp:Label>
            </Content>
        </telerik:LayoutRow>
        <telerik:LayoutRow HtmlTag="Div" CssClass="dsw-text-center" Style="margin-top: 5px;">
            <Content>
                <telerik:RadButton ID="btnStop" Text="Interrompi" SingleClick="true" SingleClickText="Completamento ultima operazione in corso..." runat="server"></telerik:RadButton>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>
