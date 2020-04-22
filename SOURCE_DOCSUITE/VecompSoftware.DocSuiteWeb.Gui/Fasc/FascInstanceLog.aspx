<%@ Page Language="vb" Title="Log attività" AutoEventWireup="false" CodeBehind="FascInstanceLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascInstanceLog" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscWorkflowInstanceLog.ascx" TagPrefix="usc" TagName="uscWorkflowInstanceLog" %>


<asp:Content ID="Content" runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var fascInstanceLog;
            require(["Fasc/FascInstanceLog"], function (FascInstanceLog) {
                $(function () {
                    fascInstanceLog = new FascInstanceLog(tenantModelConfiguration.serviceConfiguration);
                    fascInstanceLog.IdFascicle = "<%= IdFascicle%>";
                    fascInstanceLog.uscWorkflowInstanceLogsId = "<%= uscWorkflowInstanceLogs.PageContentPane.ClientID%>";
                    fascInstanceLog.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="pageContent" runat="server" ContentPlaceHolderID="cphContent">
    <usc:uscWorkflowInstanceLog runat="server" ID="uscWorkflowInstanceLogs" />
</asp:Content>
