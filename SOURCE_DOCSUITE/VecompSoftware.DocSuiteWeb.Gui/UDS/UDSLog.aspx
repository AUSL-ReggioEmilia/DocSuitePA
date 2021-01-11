<%@ Page Language="vb" Title="Archivi - Log" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UDSLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UDSLog" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var UdsLog;
            require(["UDS/UDSLog"], function (UDSLog) {
                $(function () {
                    UdsLog = new UDSLog(tenantModelConfiguration.serviceConfiguration);
                    UdsLog.UDSId = "<%= CurrentIdUDS %>";
                    UdsLog.UDSIdRepository = "<%= CurrentIdUDSRepository %>";
                    UdsLog.titleContainerId = "<%= MasterDocSuite.TitleContainer.ClientID  %>";
                    UdsLog.UDSLogGridId = "<%= UDSLogGrid.ClientID %>";
                    UdsLog.HasAdminRight = JSON.parse("<%=HasAdminRight.ToString()%>".toLowerCase());
                    UdsLog.UDSLogShowOnlyCurrentIfNotAdmin = JSON.parse("<%=ProtocolEnv.UDSLogShowOnlyCurrentIfNotAdmin%>".toLowerCase());
                    UdsLog.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    UdsLog.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID  %>";
                    UdsLog.initialize();
                });
            });

            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() == "Page") {
                    args.set_cancel(true);
                    UdsLog.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                UdsLog.onGridDataBound();
            }
        </script>
        <style type="text/css">
            /*Controlli Telerik*/

            div.RadGrid .rgPager .rgAdvPart {
                display: none;
            }
        </style>
    </telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="dossierGrid" ID="UDSLogGrid" GridLines="None" Height="100%" AllowPaging="True" AllowSorting="true" AllowFilteringByColumn="False" PageSize="30">
            <ExportSettings ExportOnlyData="true" IgnorePaging="true"></ExportSettings>
            <ClientSettings>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="false" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed" NoMasterRecordsText="Nessun log trovato." PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                <Columns>
                    <telerik:GridBoundColumn DataField="LogDate" UniqueName="LogDate" HeaderText="Data" SortExpression="LogDate" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="10%" />
                    <telerik:GridBoundColumn DataField="Computer" UniqueName="Computer" HeaderText="Computer" SortExpression="Computer" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="8%" />
                    <telerik:GridBoundColumn DataField="LogUser" UniqueName="LogUser" HeaderText="Utente" SortExpression="LogUser" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="left" HeaderStyle-Width="20%" />
                    <telerik:GridBoundColumn DataField="TypeDescription" UniqueName="TypeDescription" HeaderText="Descrizione Tipo" SortExpression="TypeDescription" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="25%" />
                    <telerik:GridBoundColumn DataField="Description" UniqueName="Description" ItemStyle-HorizontalAlign="left" SortExpression="Description" HeaderStyle-HorizontalAlign="Center" HeaderText="Descrizione" />
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>
</asp:Content>
