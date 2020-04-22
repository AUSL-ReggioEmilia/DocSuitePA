<%@ Page Language="vb" Title="Dossier - Log" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DossierLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DossierLog" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ID="Content" runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            var dossierLog;
            require(["Dossiers/DossierLog"], function (DossierLog) {
                $(function () {
                    dossierLog = new DossierLog(tenantModelConfiguration.serviceConfiguration);
                    dossierLog.dossierId = "<%=IdDossier%>";
                    dossierLog.dossierTitle = "<%= DossierTitle %>";
                    dossierLog.titleContainerId = "<%= MasterDocSuite.TitleContainer.ClientID  %>";
                    dossierLog.dossierLogGridId = "<%= dossierLogGrid.ClientID %>";
                    dossierLog.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    dossierLog.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID  %>";
                    dossierLog.initialize();
                });
            });

            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() == "Page") {
                    args.set_cancel(true);
                    dossierLog.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                dossierLog.onGridDataBound();
            }
        </script>
    </telerik:RadScriptBlock>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

    <telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="dossierGrid" ID="dossierLogGrid" Skin="Office2010Blue" PageSize="30" GridLines="None" Height="100%" AllowPaging="True" AllowMultiRowSelection="True" AllowFilteringByColumn="False">
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
