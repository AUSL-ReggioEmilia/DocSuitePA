<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ReslLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslLog"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CloseFilterMenuOnClientClick="True" GridLines="Vertical" ID="Dgr" runat="server" ShowGroupPanel="True">
            <MasterTableView TableLayout="Auto" AllowCustomPaging="True"
                AllowCustomSorting="True" AllowMultiColumnSorting="True" NoMasterRecordsText="Nessun Log trovato">
                <Columns>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="LogDate" DataType="System.DateTime"
                        HeaderText="Data" UniqueName="LogDate" SortExpression="LogDate">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="10%" />
                        <itemstyle horizontalalign="Center" verticalalign="Middle" width="10%" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemComputer"
                        HeaderText="Computer" UniqueName="SystemComputer" SortExpression="SystemComputer">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="8%" />
                        <itemstyle horizontalalign="Left" verticalalign="Middle" width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemUser"
                        HeaderText="Utente" UniqueName="SystemUser" SortExpression="SystemUser">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="26%" />
                        <itemstyle horizontalalign="Left" verticalalign="Middle" width="26%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Program" HeaderText="P"
                        UniqueName="Program" SortExpression="Program">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="3%" />
                        <itemstyle horizontalalign="Center" verticalalign="Middle" width="3%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogType" HeaderText="T"
                        UniqueName="LogType" SortExpression="LogType">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="3%" />
                        <itemstyle horizontalalign="Center" verticalalign="Middle" width="3%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogTypeDescription"
                        HeaderText="Tipo" UniqueName="LogTypeDescription" SortExpression="LogTypeDescription" AllowFiltering="false" Groupable="false">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="20%" />
                        <itemstyle horizontalalign="Left" verticalalign="Middle" width="20%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="LogDescription"
                        HeaderText="Descrizione" UniqueName="LogDescription" SortExpression="LogDescription">
                        <headerstyle horizontalalign="Center" verticalalign="Middle" width="45%" />
                        <itemstyle horizontalalign="Left" verticalalign="Middle" width="45%" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente"
                SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>
