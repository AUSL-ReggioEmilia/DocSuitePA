<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ProtRolesLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtRolesLog" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <docsuite:bindgrid ID="GridProt"  runat="server" AllowPaging="True" AllowSorting="true" AllowFilteringByColumn="true" AutoGenerateColumns="False" GridLines="None"
            ShowGroupPanel="True" EnableViewState="true">
            <ExportSettings>
                <Pdf FontType="Subset" PaperSize="Letter" />
                <Excel Format="Html" />
                <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" />
            </ExportSettings>
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst"
                Dir="LTR" Frame="Border" TableLayout="Auto" Width="100%" NoMasterRecordsText="Nessun Log disponibile">
                <Columns>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="LogDate" DataType="System.DateTime" HeaderText="Data" SortExpression="LogDate" UniqueName="LogDate">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="10%" />
                        <ItemStyle horizontalalign="Center" verticalalign="Middle" width="10%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemComputer" HeaderText="Computer" SortExpression="SystemComputer" UniqueName="SystemComputer">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="8%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemUser" HeaderText="Utente" SortExpression="SystemUser" UniqueName="SystemUser">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="26%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="26%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Program" HeaderText="P" SortExpression="Program" UniqueName="Program">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                        <ItemStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogType" HeaderText="Tipo" SortExpression="LogType" UniqueName="LogType">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                        <ItemStyle horizontalalign="Center" verticalalign="Middle" width="6%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn AllowFiltering="False" Groupable="False" CurrentFilterFunction="Contains" DataField="LogTypeDescription" HeaderText="Descrizione Tipo" SortExpression="LogTypeDescription" UniqueName="LogTypeDescription">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="20%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogDescription" HeaderText="Descrizione" SortExpression="LogDescription" UniqueName="LogDescription">
                        <HeaderStyle horizontalalign="Center" verticalalign="Middle" width="44%" />
                        <ItemStyle horizontalalign="Left" verticalalign="Middle" width="44%" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
        </docsuite:bindgrid>  
    </div>
</asp:Content>


