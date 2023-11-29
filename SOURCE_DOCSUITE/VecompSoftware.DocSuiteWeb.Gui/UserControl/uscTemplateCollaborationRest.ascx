<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscTemplateCollaborationRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscTemplateCollaborationRest" %>


<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>


<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_usc;

        require(["UserControl/uscTemplateCollaborationRest"], function (UscTemplateCollaborationRest) {
            $(function () {
                <%= Me.ClientID %>_usc = new UscTemplateCollaborationRest(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_usc.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                <%= Me.ClientID %>_usc.rtvTemplateCollaborationId = "<%= rtvTemplateCollaboration.ClientID %>";
                <%= Me.ClientID %>_usc.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                <%= Me.ClientID %>_usc.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<div>
    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <asp:Panel runat="server" ID="pnlMainContent">
        <telerik:RadTreeView ID="rtvTemplateCollaboration" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
            <Nodes>
                <telerik:RadTreeNode Expanded="true" NodeType="Root" runat="server" Selected="true" Text="Templates" Value="" />
            </Nodes>
        </telerik:RadTreeView>
    </asp:Panel>
</div>

