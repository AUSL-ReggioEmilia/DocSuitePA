<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscAmmTraspMonitorLogGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscAmmTraspMonitorLogGrid" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscAmmTraspMonitorLogGrid;
        require(["UserControl/uscAmmTraspMonitorLogGrid"], function (UscAmmTraspMonitorLogGrid) {
            $(function () {
                uscAmmTraspMonitorLogGrid = new UscAmmTraspMonitorLogGrid(tenantModelConfiguration.serviceConfiguration);
                uscAmmTraspMonitorLogGrid.uscAmmTraspMonitorLogId = "<%= ammTraspMonitorLogGrid.ClientID %>";
                uscAmmTraspMonitorLogGrid.pageId = "<%= pageContent.ClientID %>";
                uscAmmTraspMonitorLogGrid.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function OnGridCommand(sender, args) {
            if (args.get_commandName() === "Page") {
                args.set_cancel(true);
                uscAmmTraspMonitorLogGrid.onPageChanged();
            }
        }

        function OnGridDataBound(sender, args) {
            uscAmmTraspMonitorLogGrid.onGridDataBound();
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
    <telerik:RadGrid runat="server" CssClass="ammTraspMonitorLogGrid" ID="ammTraspMonitorLogGrid" Skin="Office2010Blue" PageSize="30" GridLines="None" Height="100%"
        AllowPaging="False" AllowMultiRowSelection="True" AllowFilteringByColumn="False" ExportSettings-ExportOnlyData="true">
        <ClientSettings>
            <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
            <Resizing AllowColumnResize="true" />
            <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
        </ClientSettings>
        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed" NoMasterRecordsText="Nessun registro monitoraggio trasparenza trovato."
            PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
            <Columns>
                <telerik:GridBoundColumn DataField="RoleName" HeaderText="Settore" HeaderStyle-Width="15%" ItemStyle-Width="15%" />
                <telerik:GridBoundColumn DataField="DocumentUnitName" HeaderText="Registrazione" HeaderStyle-Width="15%" ItemStyle-Width="15%" />
                <telerik:GridBoundColumn DataField="DocumentUnitSummaryUrl" HeaderText="Numero" HeaderStyle-Width="15%" ItemStyle-Width="15%" />
                <telerik:GridBoundColumn DataField="Date" HeaderText="Data monitoraggio" HeaderStyle-Width="10%" ItemStyle-Width="10%" />
                <telerik:GridBoundColumn DataField="RegistrationUser" HeaderText="Autore" HeaderStyle-Width="15%" ItemStyle-Width="15%" />
                <telerik:GridBoundColumn DataField="Note" HeaderText="Note" HeaderStyle-Width="20%" ItemStyle-Width="20%" />
                <telerik:GridBoundColumn DataField="Rating" HeaderText="Valutazione" HeaderStyle-Width="10%" ItemStyle-Width="10%" />
            </Columns>
        </MasterTableView>
        <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>

