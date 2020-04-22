<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltLog" Codebehind="TbltLog.aspx.vb"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Tabella - Log" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <docsuite:bindgrid id="RadGridLog" runat="server" allowfilteringbycolumn="True" allowpaging="True" allowsorting="True" autogeneratecolumns="False" gridlines="None" showgrouppanel="True" AllowAutofitTextBoxFilter="True" CloseFilterMenuOnClientClick="True">
            <MasterTableView NoMasterRecordsText="Nessun Log disponibile" AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True">
                <RowIndicatorColumn Visible="False">
                    <HeaderStyle Width="20px" />
                </RowIndicatorColumn>
                <ExpandCollapseColumn Visible="False" Resizable="False">
                    <HeaderStyle Width="20px" />
                </ExpandCollapseColumn>
                <Columns>
                    <telerik:GridDateTimeColumn DataField="RegistrationDate" HeaderText="Data" UniqueName="RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" SortExpression="RegistrationDate" DataType="System.DateTime">
                        <HeaderStyle Width="135px" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="SystemComputer" HeaderText="Computer" UniqueName="SystemComputer" CurrentFilterFunction="Contains" SortExpression="SystemComputer">
                        <HeaderStyle Width="10%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="RegistrationUser" HeaderText="Utente" UniqueName="RegistrationUser" CurrentFilterFunction="Contains" SortExpression="RegistrationUser">
                        <HeaderStyle Width="27%" />
                    </telerik:GridBoundColumn>
                    <%--<telerik:GridBoundColumn DataField="LogType" HeaderText="T" UniqueName="LogType" CurrentFilterFunction="Contains" SortExpression="LogType">
                        <HeaderStyle Width="5%" />
                    </telerik:GridBoundColumn>--%>
                    <telerik:GridBoundColumn DataField="LogTypeDescription" HeaderText="Tipo" UniqueName="LogTypeDescription" CurrentFilterFunction="Contains" SortExpression="LogType" AllowFiltering="False" >
                        <HeaderStyle Width="17%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LogDescription" HeaderText="Descrizione" UniqueName="LogDescription" CurrentFilterFunction="Contains" SortExpression="LogDescription">
                        <HeaderStyle Width="30%" />
                    </telerik:GridBoundColumn>
                </Columns>
                <EditFormSettings>
                    <PopUpSettings ScrollBars="None" />
                </EditFormSettings>
                <PagerStyle Position="Top" />
            </MasterTableView>
            <SelectedItemStyle BackColor="#008A8C" Font-Bold="True" />
            <ClientSettings AllowDragToGroup="True" />
            <SortingSettings SortToolTip="Ordina" SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Decrescente" />
            <ExportSettings FileName="Esportazione">
                <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
                <Excel Format="ExcelML" />
            </ExportSettings>
        </docsuite:bindgrid>
    </div>
</asp:Content>
