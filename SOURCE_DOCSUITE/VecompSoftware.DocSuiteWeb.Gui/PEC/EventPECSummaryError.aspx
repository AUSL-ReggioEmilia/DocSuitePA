<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EventPECSummaryError.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.EventPECSummaryError" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="PEC in errore" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var eventPECSummaryError;
            require(["PEC/EventPECSummaryError"], function (EventPECSummaryError) {
                $(function () {
                    eventPECSummaryError = new EventPECSummaryError(tenantModelConfiguration.serviceConfiguration);
                    eventPECSummaryError.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    eventPECSummaryError.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    eventPECSummaryError.eventPECSummaryErrorGridId = "<%= eventPECSummaryErrorGrid.ClientID %>";
                    eventPECSummaryError.initialize();
                });
            });
            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    eventPECSummaryError.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                eventPECSummaryError.onGridDataBound();
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">

    <telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="eventPECSummaryErrorGrid" ID="eventPECSummaryErrorGrid"
            Skin="Office2010Blue" PageSize="10" GridLines="None" Height="100%" AllowPaging="True" AllowMultiRowSelection="false"
            AllowFilteringByColumn="False">

            <ClientSettings EnablePostBackOnRowClick="false">
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="false" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>

            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed"
                NoMasterRecordsText="Nessuna PEC in errore individuata con l'attuale configurazione." NoDetailRecordsText="Nessuna PEC in errore individuata con l'attuale configurazione."
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
                <Columns>
                    <telerik:GridBoundColumn DataField="$id" HeaderText="$id" Visible="false" />
                    <telerik:GridBoundColumn DataField="$type" HeaderText="$type" Visible="false"/>
                    <telerik:GridBoundColumn DataField="Body" HeaderText="Corpo della PEC" Visible="false" />
                    <telerik:GridBoundColumn DataField="CorrelatedId" HeaderText="CorrelationId" Visible="false" />
                    <telerik:GridBoundColumn DataField="Subject" HeaderText="Oggetto" />
                    <telerik:GridBoundColumn DataField="Priority" HeaderText="Priorità" />
                    <telerik:GridBoundColumn DataField="ProcessedErrorMessages" HeaderText="Anomalia riscontrata" />
                    <telerik:GridBoundColumn DataField="ReceivedDate" HeaderText="Data di ricezione" />
                    <telerik:GridBoundColumn DataField="Sender" HeaderText="Mittente" />
                    <telerik:GridBoundColumn DataField="Recipients" HeaderText="Destinatari" />
                </Columns>
            </MasterTableView>
            <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
            <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
        </telerik:RadGrid>
    </telerik:RadAjaxPanel>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>
