<%@ Page Language="vb" Title="Monitoraggio pubblicazioni Settori di appartenenza" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="MonitoringSeriesRole.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MonitoringSeriesRole" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var monitoringSeriesRole;
            function onRequestStart(sender, args) {
                args.set_enableAjax(false);
            }

            function OnGridCommand(sender, args) {
                if (args.get_commandName() === "Page") {
                    args.set_cancel(true);
                    monitoringSeriesRole.onPageChanged();
                }
            }

            function OnGridDataBound(sender, args) {
                monitoringSeriesRole.onGridDataBound();
            }
        </script>
    </telerik:RadScriptBlock>

    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%;">Periodo:
            </td>
            <td style="width: 80%;">
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Dalla data" ID="dtpDateFrom" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="dtpDateFrom" Display="Dynamic" ErrorMessage="Il campo data è obbligatorio"></asp:RequiredFieldValidator>
                <telerik:RadDatePicker DateInput-LabelWidth="30%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="300" DateInput-Label="Alla data" ID="dtpDateTo" runat="server" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="dtpDateTo" Display="Dynamic" ErrorMessage="Il campo data è obbligatorio"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <div style="margin: 1px 1px 10px 1px;">
        <div>
            <telerik:RadButton ID="btnSearch" Text="Aggiorna visualizzazione" Width="150px" runat="server" TabIndex="1" AutoPostBack="true" OnClick="btnSearch_Click" />
            <telerik:RadButton ID="btnClean" Text="Azzera filtri" Width="150px" runat="server" TabIndex="1" AutoPostBack="true" OnClick="btnClean_Click" />
            <telerik:RadButton ID="cmdExportToExcel" runat="server" Text="Esporta" ToolTip="Esporta in Excel" CausesValidation="false" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">    
    <asp:Panel runat="server" CssClass="radGridWrapper" ID="pageContent">
        <telerik:RadGrid runat="server" CssClass="monitoringSeriesRoleGrid" ID="monitoringSeriesRoleGrid"
            Skin="Office2010Blue" GridLines="None" AllowPaging="False" Height="100%" AllowMultiRowSelection="false"
            AllowFilteringByColumn="False" OnDetailTableDataBind="monitoringSeriesRoleGrid_DetailTableDataBind">
            <ClientSettings EnablePostBackOnRowClick="false">
                <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
                <Resizing AllowColumnResize="true" AllowRowResize="true" />
                <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
            </ClientSettings>

            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed"
                NoMasterRecordsText="Nessun elemento trovato nel perido indicato." NoDetailRecordsText="Nessun elemento trovato nel perido indicato."
                PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}" DataKeyNames="Role" AllowMultiColumnSorting="True">
                <DetailTables>
                    <telerik:GridTableView Name="MonitoringSeriesRoleDetails" AutoGenerateColumns="false" Width="100%">
                        <Columns>
                            <telerik:GridBoundColumn DataField="IdDocumentSeries" HeaderText="IdSerie" Visible="false" />
                            <telerik:GridTemplateColumn DataField="DocumentSeries" HeaderText="Sezioni" UniqueName="DocumentSeries"  HeaderStyle-Width="42%" ItemStyle-Width="42%">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lbtViewMonitoringQuality" Text='<%# Eval("DocumentSeries") %>' CommandName="ViewMonitoringQuality" CommandArgument='<%# Eval("IdDocumentSeries").ToString()+"|"+Eval("Role") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <%--<telerik:GridBoundColumn DataField="ActivePublished" HeaderText="Attive" HeaderTooltip="Numero di pubblicazioni attive" />--%>
                            <telerik:GridBoundColumn DataField="Inserted" HeaderText="Inserite" HeaderTooltip="Numero di pubblicazioni inserite" />
                            <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicate" HeaderTooltip="Numero di pubblicazioni pubblicate" />
                            <telerik:GridBoundColumn DataField="Updated" HeaderText="Aggiornate" HeaderTooltip="Numero di pubblicazioni aggiornate" />
                            <telerik:GridBoundColumn DataField="Cancelled" HeaderText="Cancellate" HeaderTooltip="Numero di pubblicazioni cancellate" />
                            <telerik:GridBoundColumn DataField="Retired" HeaderText="Ritirate" HeaderTooltip="Numero di pubblicazioni ritirate nel periodo" />
                            <telerik:GridBoundColumn DataField="LastUpdated" HeaderText="Ultimo aggiornamento" HeaderTooltip="Data ultimo aggiornamento" DataFormatString="{0:dd/MM/yyyy}" />
                        </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn DataField="Role" HeaderText="Settore" HeaderStyle-Width="42%" ItemStyle-Width="42%" />
                    <%--<telerik:GridBoundColumn DataField="ActivePublished" HeaderText="Attive" HeaderTooltip="Numero di pubblicazioni attive" />--%>
                    <telerik:GridBoundColumn DataField="Inserted" HeaderText="Inserite" HeaderTooltip="Numero di pubblicazioni inserite" />
                    <telerik:GridBoundColumn DataField="Published" HeaderText="Pubblicate" HeaderTooltip="Numero di pubblicazioni pubblicate" />
                    <telerik:GridBoundColumn DataField="Updated" HeaderText="Aggiornate" HeaderTooltip="Numero di pubblicazioni aggiornate" />
                    <telerik:GridBoundColumn DataField="Cancelled" HeaderText="Cancellate" HeaderTooltip="Numero di pubblicazioni cancellate" />
                    <telerik:GridBoundColumn DataField="Retired" HeaderText="Ritirate" HeaderTooltip="Numero di pubblicazioni ritirate nel periodo" />
                    <telerik:GridBoundColumn DataField="LastUpdated" HeaderText="Ultimo aggiornamento" HeaderTooltip="Data ultimo aggiornamento" DataFormatString="{0:dd/MM/yyyy}" />
                </Columns>
            </MasterTableView>
            <GroupingSettings ShowUnGroupButton="true"></GroupingSettings>
        </telerik:RadGrid>
    </asp:Panel>

</asp:Content>
