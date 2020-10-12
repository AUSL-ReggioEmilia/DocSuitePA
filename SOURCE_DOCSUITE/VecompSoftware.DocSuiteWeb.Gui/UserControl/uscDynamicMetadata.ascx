<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDynamicMetadata.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDynamicMetadata" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var uscDynamicMetadata;
        require(["UserControl/uscDynamicMetadata"], function (UscDynamicMetadata) {
            $(function () {
                uscDynamicMetadata = new UscDynamicMetadata(tenantModelConfiguration.serviceConfiguration);
                uscDynamicMetadata.dynamicPageContentId = "<%= dynamicPageContent.ClientID %>";
                uscDynamicMetadata.pnlDynamicContentId = "<%= pnlDynamicContent.ClientID %>";
                uscDynamicMetadata.uscNotificationId = "<%= uscNotification.ClientID %>";
                uscDynamicMetadata.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                uscDynamicMetadata.managerId = "<%= manager.ClientID%>";
                uscDynamicMetadata.fascicleInsertCommonIdEvent = "<%= FascicleInsertCommonIdEvent %>";
                uscDynamicMetadata.initialize();
            });
        });

        function OpenCommentsWindow(label) {
            uscDynamicMetadata.openCommentsWindow(label);
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="400" ID="managerViewComments" runat="server" Title="Metadati - Commenti" Width="650" />
    </Windows>
</telerik:RadWindowManager>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<asp:Panel runat="server" ID="pnlDynamicContent">
</asp:Panel>
<asp:PlaceHolder runat="server" ID="dynamicPageContent" />


