<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ItemLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ItemLog" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <docsuite:bindgrid ID="GridLog"  runat="server" AllowPaging="True" AllowSorting="true" AllowFilteringByColumn="true" AutoGenerateColumns="False" GridLines="None"
            ShowGroupPanel="True" EnableViewState="true">
            <ExportSettings>
                <Pdf FontType="Subset" PaperSize="Letter" />
                <Excel Format="Html" />
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" />
            </ExportSettings>
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst"
                Dir="LTR" Frame="Border" TableLayout="Auto" Width="100%" NoMasterRecordsText="Nessun Log disponibile">
                <Columns>
                    <telerik:GridDateTimeColumn  CurrentFilterFunction="EqualTo" DataField="LogDate" DataType="System.DateTime"
                        HeaderText="Data" UniqueName="LogDate" SortExpression="LogDate">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="10%" />
                        <ItemStyle horizontalalign="Center" verticalalign="Middle" width="10%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemComputer"
                        HeaderText="Computer" UniqueName="SystemComputer" SortExpression="SystemComputer">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="8%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemUser"
                        HeaderText="Utente" UniqueName="SystemUser" SortExpression="SystemUser">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="8%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Program" HeaderText="P" UniqueName="Program" SortExpression="Program">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                        <ItemStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogType" HeaderText="T" UniqueName="LogType" SortExpression="LogType">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                        <ItemStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogTypeDescription"
                        HeaderText="Tipo" UniqueName="LogTypeDescription" SortExpression="LogTypeDescription">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="20%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogDescription"
                        HeaderText="Descrizione" UniqueName="LogDescription" SortExpression="LogDescription">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="44%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="44%" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
        </docsuite:bindgrid>  
    </div>
</asp:Content>