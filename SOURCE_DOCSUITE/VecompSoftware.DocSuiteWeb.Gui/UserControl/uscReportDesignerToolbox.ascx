<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscReportDesignerToolbox.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscReportDesignerToolbox" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscReportDesignerToolbox;
        require(["UserControl/uscReportDesignerToolbox"], function (UscReportDesignerToolbox) {
            $(function () {
                uscReportDesignerToolbox = new UscReportDesignerToolbox();
                uscReportDesignerToolbox.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscReportDesignerToolbox.pnlContentId = "<%= pnlContent.ClientID %>";
                uscReportDesignerToolbox.rtvToolBoxId = "<%= rtvToolBox.ClientID %>";
                uscReportDesignerToolbox.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<asp:Panel runat="server" ID="pnlContent" Height="100%">
    <telerik:RadPanelBar runat="server" Width="100%" Height="100%" ExpandMode="FullExpandedItem">
        <Items>
            <telerik:RadPanelItem Text="Elementi" Expanded="true">
                <ContentTemplate>
                    <telerik:RadTreeView runat="server" ID="rtvToolBox" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="false"></telerik:RadTreeView>
                </ContentTemplate>
            </telerik:RadPanelItem>            
        </Items>
    </telerik:RadPanelBar>
</asp:Panel>
