<%@ Page Language="vb" Title="Monitoraggio pubblicazioni per sezione" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="MonitoringSeriesSection.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MonitoringSeriesSection" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            <%--var monitoringSeriesSection;
            require(["Monitors/MonitoringSeriesSection"], function (MonitoringSeriesSection) {
                $(function () {
                    monitoringSeriesSection = new MonitoringSeriesSection(tenantModelConfiguration.serviceConfiguration);
                    monitoringSeriesSection.ajaxLoadingPanelId = "<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                    monitoringSeriesSection.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    monitoringSeriesSection.btnSearchId = "<%= btnSearch.ClientID%>";
                    monitoringSeriesSection.btnCleanId = "<%= btnClean.ClientID %>";
                    monitoringSeriesSection.dpStartDateFromId = "<%= dtpDateFrom.ClientID%>";
                    monitoringSeriesSection.dpEndDateFromId = "<%= dtpDateTo.ClientID%>";
                    monitoringSeriesSection.monitoringSeriesSectionGridId = "<%= monitoringSeriesSectionGrid.ClientID %>";
                    monitoringSeriesSection.pageId = "<%= pageContent.ClientID %>";
                    monitoringSeriesSection.initialize();
                });
            });--%>

            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    monitoringSeriesSection.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                monitoringSeriesSection.onGridDataBound();
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%;">Periodo:
            </td>
            <td style="width: 80%;">
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Dalla data" ID="dtpDateFrom" runat="server" />
                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="dtpDateFrom" ErrorMessage="Il campo data è obbligatorio"></asp:RequiredFieldValidator>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Alla data" ID="dtpDateTo" runat="server" />
                <asp:RequiredFieldValidator runat="server" Display="Dynamic" ControlToValidate="dtpDateTo" ErrorMessage="Il campo data è obbligatorio"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <div style="margin: 1px 1px 10px 1px;">
        <div>
            <telerik:RadButton ID="btnSearch" Text="Aggiorna visualizzazione" Width="150px" runat="server" OnClick="btnSearch_Click" />
            <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" OnClick="btnClean_Click" CausesValidation="false" />
            <telerik:RadButton ID="cmdExportToExcel" runat="server" Text="Esporta" ToolTip="Esporta in Excel" CausesValidation="false" />
        </div>
    </div>
    <br />
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">

    <asp:Panel runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="monitoringSeriesSectionGrid" ID="monitoringSeriesSectionGrid"
            Skin="Office2010Blue" GridLines="None" Height="100%" AllowPaging="False" AllowMultiRowSelection="True"
            AllowFilteringByColumn="False" ShowGroupPanel="true" MasterTableView-GroupLoadMode="Client" OnDetailTableDataBind="monitoringSeriesSectionGrid_DetailTableDataBind">
            <ClientSettings>
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="true" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>

            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed"
                NoMasterRecordsText="Nessun elemento trovato nel perido indicato." NoDetailRecordsText="Nessun elemento trovato nel perido indicato."
                DataKeyNames="Family" PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}" EnableGroupsExpandAll="true" GroupLoadMode="Client">

                <DetailTables>
                    <telerik:GridTableView DataKeyNames="Family,Series" Name="MonitoringSection" AutoGenerateColumns="false" Width="100%">
                        <DetailTables>
                            <telerik:GridTableView Name="MonitoringSectionDetails" AutoGenerateColumns="false" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="SubSection" HeaderText="Sottosezioni" HeaderStyle-Width="42%" ItemStyle-Width="42%" />
                                    <%--<telerik:GridBoundColumn DataField="ActivePublished" HeaderText="Attive" HeaderTooltip="Numero di pubblicazioni attive" />--%>
                                    <telerik:GridBoundColumn DataField="Inserted" HeaderText="Inserite" HeaderTooltip="Numero di pubblicazioni inserite" />
                                    <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicate" HeaderTooltip="Numero di pubblicazioni pubblicate" />
                                    <telerik:GridBoundColumn DataField="Updated" HeaderText="Aggiornate" HeaderTooltip="Numero di pubblicazioni aggiornate" />
                                    <telerik:GridBoundColumn DataField="Canceled" HeaderText="Cancellate" HeaderTooltip="Numero di pubblicazioni cancellate" />
                                    <telerik:GridBoundColumn DataField="Retired" HeaderText="Ritirate" HeaderTooltip="Numero di pubblicazioni ritirate nel periodo" />
                                    <telerik:GridBoundColumn DataField="LastUpdated" HeaderText="Ultimo aggiornamento" HeaderTooltip="Data di massimo ultimo aggiornamento" DataFormatString="{0:dd/MM/yyyy}" />
                                </Columns>
                            </telerik:GridTableView>
                        </DetailTables>
                        <Columns>
                            <telerik:GridTemplateColumn DataField="Series" HeaderText="Sezioni" UniqueName="DocumentSeries"  HeaderStyle-Width="42%" ItemStyle-Width="42%">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lbtViewMonitoringQuality" Text='<%# Eval("Series") %>' CommandName="ViewMonitoringQuality" CommandArgument='<%# Eval("Series") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                           <%-- <telerik:GridBoundColumn DataField="ActivePublished" HeaderText="Attive" HeaderTooltip="Numero di pubblicazioni attive" />--%>
                            <telerik:GridBoundColumn DataField="Inserted" HeaderText="Inserite" HeaderTooltip="Numero di pubblicazioni inserite" />
                            <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicate" HeaderTooltip="Numero di pubblicazioni pubblicate" />
                            <telerik:GridBoundColumn DataField="Updated" HeaderText="Aggiornate" HeaderTooltip="Numero di pubblicazioni aggiornate" />
                            <telerik:GridBoundColumn DataField="Canceled" HeaderText="Cancellate" HeaderTooltip="Numero di pubblicazioni cancellate" />
                            <telerik:GridBoundColumn DataField="Retired" HeaderText="Ritirate" HeaderTooltip="Numero di pubblicazioni ritirate nel periodo" />
                            <telerik:GridBoundColumn DataField="LastUpdated" HeaderText="Ultimo aggiornamento" HeaderTooltip="Data di massimo ultimo aggiornamento" DataFormatString="{0:dd/MM/yyyy}" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn DataField="Family" HeaderText="Serie" HeaderStyle-Width="42%" ItemStyle-Width="42%" />
                    <%--<telerik:GridBoundColumn DataField="ActivePublished" HeaderText="Attive" HeaderTooltip="Numero di pubblicazioni attive" />--%>
                    <telerik:GridBoundColumn DataField="Inserted" HeaderText="Inserite" HeaderTooltip="Numero di pubblicazioni inserite" />
                    <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicate" HeaderTooltip="Numero di pubblicazioni pubblicate" />
                    <telerik:GridBoundColumn DataField="Updated" HeaderText="Aggiornate" HeaderTooltip="Numero di pubblicazioni aggiornate" />
                    <telerik:GridBoundColumn DataField="Canceled" HeaderText="Cancellate" HeaderTooltip="Numero di pubblicazioni cancellate" />
                    <telerik:GridBoundColumn DataField="Retired" HeaderText="Ritirate" HeaderTooltip="Numero di pubblicazioni ritirate nel periodo" />
                    <telerik:GridBoundColumn DataField="LastUpdated" HeaderText="Ultimo aggiornamento" HeaderTooltip="Data di massimo ultimo aggiornamento" DataFormatString="{0:dd/MM/yyyy}" />
                </Columns>
            </MasterTableView>
            <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
        </telerik:RadGrid>
    </asp:Panel>

    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
</asp:Content>
