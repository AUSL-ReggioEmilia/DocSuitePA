<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscTemplateCollaborationSelRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscTemplateCollaborationSelRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_usc;

        require(["UserControl/uscTemplateCollaborationSelRest"], function (UscTemplateCollaborationSelRest) {
            $(function () {
                <%= Me.ClientID %>_usc = new UscTemplateCollaborationSelRest(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_usc.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                <%= Me.ClientID %>_usc.ddtDocumentTypeId = "<%= ddtDocumentType.ClientID %>";
                <%= Me.ClientID %>_usc.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                <%= Me.ClientID %>_usc.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxFlatLoadingPanel.ClientID %>";
                <%= Me.ClientID %>_usc.treeViewInitializationEnabled = <%= TreeViewInitializationEnabled.ToString().ToLower() %>;
                <%= Me.ClientID %>_usc.SetFilterStatus(<%= FilterStatus %>);
                <%= Me.ClientID %>_usc.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<div>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <asp:Panel runat="server" ID="pnlMainContent">
        <telerik:RadDropDownTree  AutoPostBack="False" CausesValidation="False" DropDownSettings-CloseDropDownOnSelection="true"
            ID="ddtDocumentType" Width="350px" runat="server" />
    </asp:Panel>
</div>
