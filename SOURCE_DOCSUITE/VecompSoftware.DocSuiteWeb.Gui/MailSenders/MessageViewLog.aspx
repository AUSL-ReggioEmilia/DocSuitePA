<%@ Page Title="Log Messaggi" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="MessageViewLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.MessageViewLog" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="datatable">
        <tr>
            <td class="label col-dsw-1">
                Log del Messaggio:
            </td>
            <td>
                <asp:label runat="server" ID="lblMessage" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="radGridWrapper">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both" ShowGroupPanel="True" ID="dgLog" PageSize="20" runat="server" Width="100%">

            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione" TableLayout="Auto" Width="100%">
                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />

                <Columns>
                    <telerik:GridDateTimeColumn AllowSorting="True" DataField="LogDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" DataType="System.DateTime" HeaderText="Data" ShowSortIcon="True" SortExpression="LogDate" UniqueName="LogDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </telerik:GridDateTimeColumn>

                    <DocSuite:SuggestFilteringColumn AllowSorting="False" AllowFiltering="False" DataField="Type" HeaderText="Evento" SortExpression="Type" UniqueName="Type">
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
