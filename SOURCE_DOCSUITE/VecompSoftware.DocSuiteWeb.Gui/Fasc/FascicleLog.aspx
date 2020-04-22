<%@ Page Language="vb" CodeBehind="FascicleLog.aspx.vb" Title="Fascicle - Log" MasterPageFile="~/MasterPages/DocSuite2008.Master"  Inherits="VecompSoftware.DocSuiteWeb.Gui.FascicleLog" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var fascInstanceLog;
            require(["Fasc/FascicleLog"], function (FascicleLog) {
                $(function () {
                    fascInstanceLog = new FascicleLog(tenantModelConfiguration.serviceConfiguration);
                    fascInstanceLog.fascicleId = "<%=IdFascicle%>";
                    fascInstanceLog.fascicleTitle = "<%=CurrentFascicle.Title%>";
                    fascInstanceLog.titleContainerId = "<%= MasterDocSuite.TitleContainer.ClientID  %>";
                    fascInstanceLog.fascicleLogGridId = "<%= fascicleLogGrid.ClientID %>";
                    fascInstanceLog.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    fascInstanceLog.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID  %>";
                    fascInstanceLog.initialize();
                });

            });
            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() == "Page") {
                    args.set_cancel(true);
                    fascInstanceLog.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                fascInstanceLog.onGridDataBound();
            }
        </script>
    </telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="dossierGrid" ID="fascicleLogGrid" Skin="Office2010Blue" PageSize="30" GridLines="None" Height="100%" AllowPaging="True" AllowMultiRowSelection="True" AllowFilteringByColumn="False">
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

</asp:Content>
