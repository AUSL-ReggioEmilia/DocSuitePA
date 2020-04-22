
<%@ Page AutoEventWireup="false" CodeBehind="PECViewLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PecViewLog" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="PEC - Log" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both" ShowGroupPanel="True" ID="dgLog" PageSize="20" runat="server">
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione" TableLayout="Auto">

                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />

                <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <GroupByFields>
                            <telerik:GridGroupByField FieldName="MailId" HeaderText="PEC" />
                        </GroupByFields>
                        <SelectFields>
                            <telerik:GridGroupByField FieldName="MailSubject" FieldAlias="Oggetto" Aggregate="First" />
                            <telerik:GridGroupByField FieldName="Id" FieldAlias="Registrazioni" Aggregate="Count" />
                        </SelectFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>

                <Columns>
                    <telerik:GridDateTimeColumn DataField="Date"
                        DataType="System.DateTime" HeaderText="Date" UniqueName="Date" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                        SortExpression="Date" AllowSorting="True" ShowSortIcon="True">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                    </telerik:GridDateTimeColumn>

                    <DocSuite:SuggestFilteringColumn UniqueName="Type" HeaderText="Evento" DataField="Type" CurrentFilterFunction="EqualTo"
                        SortExpression="Type" AllowSorting="False" >
                        <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100" />
                        <ItemStyle HorizontalAlign="Center" />
                    </DocSuite:SuggestFilteringColumn>

                    <telerik:GridBoundColumn DataField="Description"
                        HeaderText="Descrizione" UniqueName="Description" Groupable="false" CurrentFilterFunction="Contains" HtmlEncode="true">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="SystemUser"
                        HeaderText="Operatore" UniqueName="SystemUser" Groupable="false" CurrentFilterFunction="Contains" HtmlEncode="true">
                        <HeaderStyle HorizontalAlign="Left" Width="120" />
                        <ItemStyle HorizontalAlign="Left" />
                    </telerik:GridBoundColumn>

                    <telerik:GridBoundColumn DataField="SystemComputer"
                        HeaderText="Postazione" UniqueName="SystemComputer" Groupable="True" CurrentFilterFunction="Contains" HtmlEncode="true">
                        <HeaderStyle HorizontalAlign="Left" Width="100" />
                        <ItemStyle HorizontalAlign="Left" />
                    </telerik:GridBoundColumn>


                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="false" CellSelectionMode="None" UseClientSelectColumnOnly="false" EnableDragToSelectRows="False" />
            </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
            <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>
