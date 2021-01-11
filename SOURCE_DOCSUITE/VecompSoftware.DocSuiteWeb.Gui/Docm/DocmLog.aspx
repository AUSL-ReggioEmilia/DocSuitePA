<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="DocmLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmLog" %>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="cphHeader">
<table class="datatable">
    <tr>
        <td class="label labelPanel" style="width: 25%;">
            Pratica:
        </td>
        <td>
            <asp:Label ID="lblId" runat="server"></asp:Label>
        </td>
    </tr>
    <asp:Panel ID="pnlUser" runat="server">
    <tr>
        <td class="label labelPanel" style="width: 25%;">
            Utente:
        </td>
        <td>
            <asp:Label ID="lblUser" runat="server"></asp:Label>
        </td>
    </tr>
    </asp:Panel> 
</table>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
<DocSuite:BindGrid AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CloseFilterMenuOnClientClick="True" GridLines="None" ID="RadGrid1" runat="server" ShowGroupPanel="True" Width="100%">
    <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" NoMasterRecordsText="Nessun Log trovato" TableLayout="Auto" Width="100%">
        <Columns>
            <telerik:GridDateTimeColumn DataField="LogDate" HeaderText="Data" UniqueName="LogDate" CurrentFilterFunction="EqualTo" SortExpression="LogDate">
            <HeaderStyle HorizontalAlign="Left" Width="130px"></HeaderStyle>
            <ItemStyle HorizontalAlign="Left" Width="130px"></ItemStyle>
            </telerik:GridDateTimeColumn>
            <telerik:GridBoundColumn DataField="SystemComputer" HeaderText="Computer" UniqueName="SystemComputer" CurrentFilterFunction="Contains" SortExpression="SystemComputer">
            <HeaderStyle HorizontalAlign="Left" Width="60px"></HeaderStyle>
            <ItemStyle HorizontalAlign="Left" Width="60px"></ItemStyle>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SystemUser" HeaderText="Utente" UniqueName="SystemUser" CurrentFilterFunction="Contains" SortExpression="SystemUser">
            <HeaderStyle HorizontalAlign="Left" Width="190px"></HeaderStyle>
            <ItemStyle HorizontalAlign="Left" Width="190px"></ItemStyle>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Program" HeaderText="P" UniqueName="Program" CurrentFilterFunction="Contains" SortExpression="Program">
            <HeaderStyle HorizontalAlign="Left" Width="30px"></HeaderStyle>
            <ItemStyle HorizontalAlign="Left" Width="30px"></ItemStyle>
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="LogType" HeaderText="T" UniqueName="LogType" CurrentFilterFunction="Contains" SortExpression="LogType">
            <HeaderStyle HorizontalAlign="Left" Width="30px"></HeaderStyle>
            <ItemStyle HorizontalAlign="Left" Width="30px"></ItemStyle>
            </telerik:GridBoundColumn>
            <DocSuite:SuggestFilteringColumn DataField="LogTypeDescription" HeaderText="Tipo" UniqueName="LogTypeDescription" CurrentFilterFunction="EqualTo" AllowSorting="false" SortExpression="LogTypeDescription" PropertyBind="LogType">
            <HeaderStyle HorizontalAlign="Left"></HeaderStyle>
		    </DocSuite:SuggestFilteringColumn>
            <telerik:GridBoundColumn DataField="LogDescription" HeaderText="Descrizione" UniqueName="LogDescription" CurrentFilterFunction="Contains" SortExpression="LogDescription" ></telerik:GridBoundColumn>            
        </Columns>
        <EditFormSettings>
            <PopUpSettings ScrollBars="None" />
        </EditFormSettings>
        <PagerStyle Position="Top" />
    </MasterTableView>
    <ExportSettings FileName="Esportazione">
        <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
        <Excel Format="ExcelML" />
    </ExportSettings>
    <ClientSettings AllowDragToGroup="True">
    </ClientSettings>
    <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine Decrescente"
        SortToolTip="Ordina" />
</DocSuite:BindGrid>        
</asp:Content>

