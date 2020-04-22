<%@ Page Language="vb" Title="Monitoraggio digitalizzazione flusso trasparenza" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="MonitoringQuality.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MonitoringQuality" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var monitoringQuality;
            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    monitoringQuality.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                monitoringQuality.onGridDataBound();
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
            <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" CausesValidation="false" OnClick="btnClean_Click" />
            <telerik:RadButton ID="cmdExportToExcel" runat="server" Text="Esporta" ToolTip="Esporta in Excel" CausesValidation="false" />
        </div>
    </div>
    <br />
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="monitoringQualityGrid" ID="monitoringQualityGrid"
            Skin="Office2010Blue" GridLines="None" Height="100%" AllowPaging="False" AllowMultiRowSelection="false"
            AllowFilteringByColumn="False" OnDetailTableDataBind="monitoringQualityGrid_DetailTableDataBind">
            <ClientSettings EnablePostBackOnRowClick="false">
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="true" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>

            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed"
                NoMasterRecordsText="Nessun elemento trovato nel perido indicato." NoDetailRecordsText="Nessun elemento trovato nel perido indicato."
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}" DataKeyNames="IdDocumentSeries" AllowMultiColumnSorting="True">
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="IdRole,IdDocumentSeries" Name="MonitoringQualityRole" AutoGenerateColumns="false" Width="100%">
                        <DetailTables>
                            <telerik:GridTableView Name="MonitoringQualityDetails" AutoGenerateColumns="false" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn DataField="IdDocumentSeriesItem" HeaderText="IdSettore" Visible="false" />
                                    <telerik:GridTemplateColumn DataField="Identifier" HeaderText="Registrazione" UniqueName="Identifier" HeaderStyle-Width="42%" ItemStyle-Width="42%">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="lbtViewDocumentSeriesSummary" Text='<%# Eval("Identifier") %>' CommandName="ViewDocumentSeriesSummary" CommandArgument='<%# Eval("IdDocumentSeriesItem") %>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicazioni" HeaderTooltip="Numero di pubblicazioni" />
                                    <telerik:GridBoundColumn DataField="FromResolution" HeaderText="Da atti" HeaderTooltip="Numero di pubblicazioni inserite da atti" />
                                    <telerik:GridBoundColumn DataField="FromProtocol" HeaderText="Da protocolli" HeaderTooltip="Numero di pubblicazioni inserite da protocollo" />
                                    <telerik:GridBoundColumn DataField="WithoutLink" HeaderText="Manuale" HeaderTooltip="Numero di pubblicazioni inserite manualmente" />
                                    <telerik:GridBoundColumn DataField="WithoutDocument" HeaderText="Senza documenti" HeaderTooltip="Numero di pubblicazioni senza documenti" />
                                </Columns>
                            </telerik:GridTableView>
                        </DetailTables>
                        <Columns>
                            <telerik:GridBoundColumn DataField="IdRole" HeaderText="IdSettore" Visible="false" />
                            <telerik:GridBoundColumn DataField="IdDocumentSeries" HeaderText="IdSerie" Visible="false" />
                            <telerik:GridBoundColumn DataField="Role" HeaderText="Settore" HeaderStyle-Width="42%" ItemStyle-Width="42%" />
                            <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicazioni" HeaderTooltip="Numero di pubblicazioni" />
                            <telerik:GridBoundColumn DataField="FromResolution" HeaderText="Da atti" HeaderTooltip="Numero di pubblicazioni inserite da atti" />
                            <telerik:GridBoundColumn DataField="FromProtocol" HeaderText="Da protocolli" HeaderTooltip="Numero di pubblicazioni inserite da protocollo" />
                            <telerik:GridBoundColumn DataField="WithoutLink" HeaderText="Manuale" HeaderTooltip="Numero di pubblicazioni inserite manualmente" />
                            <telerik:GridBoundColumn DataField="WithoutDocument" HeaderText="Senza documenti" HeaderTooltip="Numero di pubblicazioni senza documenti" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn DataField="IdDocumentSeries" HeaderText="IdSerie" Visible="false" />
                    <telerik:GridBoundColumn DataField="DocumentSeries" HeaderText="Sezioni" HeaderStyle-Width="42%" ItemStyle-Width="42%" />
                    <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicazioni" HeaderTooltip="Numero di pubblicazioni" />
                    <telerik:GridBoundColumn DataField="FromResolution" HeaderText="Da atti" HeaderTooltip="Numero di pubblicazioni inserite da atti" />
                    <telerik:GridBoundColumn DataField="FromProtocol" HeaderText="Da protocolli" HeaderTooltip="Numero di pubblicazioni inserite da protocollo" />
                    <telerik:GridBoundColumn DataField="WithoutLink" HeaderText="Manuale" HeaderTooltip="Numero di pubblicazioni inserite manualmente" />
                    <telerik:GridBoundColumn DataField="WithoutDocument" HeaderText="Senza documenti" HeaderTooltip="Numero di pubblicazioni senza documenti" />
                </Columns>
            </MasterTableView>
            <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>
