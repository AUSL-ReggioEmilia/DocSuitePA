<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscWorkflowInstanceLog.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscWorkflowInstanceLog" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscWorkflowInstanceLog;
        require(["UserControl/uscWorkflowInstanceLog"], function (UscWorkflowInstanceLog) {
            $(function () {
                uscWorkflowInstanceLog = new UscWorkflowInstanceLog(tenantModelConfiguration.serviceConfiguration);
                uscWorkflowInstanceLog.workflowInstanceLogGridId = "<%= workflowInstanceLogGrid.ClientID %>";
                uscWorkflowInstanceLog.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                uscWorkflowInstanceLog.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID  %>";
                uscWorkflowInstanceLog.pageContentId = "<%= pageContent.ClientID%>";
                uscWorkflowInstanceLog.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function OnGridCommand(sender, args) {
            if (args.get_commandName() == "Page") {
                args.set_cancel(true);
                uscWorkflowInstanceLog.onPageChanged();
            }
        }

        function OnGridDataBound(sender, args) {
            uscWorkflowInstanceLog.onGridDataBound();
        }
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>


<telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
    <telerik:RadGrid runat="server" CssClass="Grid" ID="workflowInstanceLogGrid" Skin="Office2010Blue" PageSize="30" GridLines="None" Height="100%" AllowPaging="True" AllowMultiRowSelection="True" AllowFilteringByColumn="False">
        <ClientSettings>
            <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
            <Resizing AllowColumnResize="false" />
            <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
        </ClientSettings>
        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed" NoMasterRecordsText="Nessun log trovato." PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
            <Columns>
                <telerik:GridBoundColumn DataField="LogDate" UniqueName="LogDate" HeaderText="Data" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
                <telerik:GridBoundColumn DataField="Computer" UniqueName="Computer" HeaderText="Computer" HeaderStyle-Width="8%" />
                <telerik:GridBoundColumn DataField="LogUser" UniqueName="LogUser" HeaderText="Utente" HeaderStyle-Width="20%" />
                <telerik:GridBoundColumn DataField="TypeDescription" UniqueName="TypeDescription" HeaderText="Descrizione Tipo" HeaderStyle-Width="25%" />
                <telerik:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="Descrizione" />
            </Columns>
        </MasterTableView>
        <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>
