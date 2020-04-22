<%@ Control Language="vb" AutoEventWireup="false" Codebehind="uscDocmToken.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocmToken" %>

<div style="overflow:hidden;width:100%;height:100%;">
    <DocSuite:BindGrid ID="dgTokens" AllowSorting="true" runat="server" AllowPaging="False"
    AutoGenerateColumns="False" GridLines="None" ShowGroupPanel="True"
    AllowFilteringByColumn="true" EnableViewState="true">
    <MasterTableView TableLayout="auto" AllowMultiColumnSorting="True"
        GridLines="Horizontal" NoMasterRecordsText="Nessun dettaglio trovato" AllowFilteringByColumn="True">
        <Columns>
            <telerik:GridTemplateColumn AllowFiltering="False" Groupable="False" HeaderImageUrl="../Docm/Images/Info16.gif" HeaderText="T" UniqueName="cUnread">
                <headerstyle horizontalalign="Center" width="3%" />
                <itemstyle horizontalalign="Center" />
                <itemtemplate>	    		    
	    		    <asp:image ID="imgInfo" Runat="server" />
		        </itemtemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn CurrentFilterFunction="EqualTo" DataField="StepDescription" DataType="System.Int16" HeaderText="Step" SortExpression="DocStep" UniqueName="DocStep">
                <headerstyle horizontalalign="Center" width="5%" />
                <itemstyle horizontalalign="Center" width="5%" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DocumentTabToken.id" HeaderText="Tip." CurrentFilterFunction="Contains" UniqueName="DocumentTabToken.Id" SortExpression="DocumentTabToken.Id">
                <headerstyle horizontalalign="Center" width="5%" />
                <itemstyle horizontalalign="Center" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn AllowFiltering="false" CurrentFilterFunction="Contains" DataField="DocumentTabToken.Description" HeaderText="Descrizione" AllowSorting="False" UniqueName="DocumentTabToken.Description">
                <headerstyle horizontalalign="Center" width="15%" />
                <itemstyle font-bold="True" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="SourceDestinationRoleDescription" HeaderText="Mitt./Dest."
                UniqueName="SourceDestinationRoleDescription" CurrentFilterFunction="NoFilter"
                AllowFiltering="false" AllowSorting="False" Groupable="False">
                <headerstyle horizontalalign="Left" width="10%" />
                <itemstyle horizontalalign="Left" wrap="False" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="OperationExpiryDateDescription" HeaderText="Oper./Scad."
                UniqueName="OperationExpiryDateDescription" CurrentFilterFunction="NoFilter"
                AllowFiltering="False" AllowSorting="False" Groupable="False">
                <headerstyle horizontalalign="Center" width="10%" />
                <itemstyle horizontalalign="Center" wrap="true" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="DocObject" HeaderText="Oggetto" CurrentFilterFunction="Contains"
                UniqueName="DocObject" SortExpression="DocObject">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Reason" HeaderText="Motivo" CurrentFilterFunction="Contains"
                UniqueName="Reason" SortExpression="Reason">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="Note" HeaderText="Note" CurrentFilterFunction="Contains"
                UniqueName="Note" SortExpression="Note">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="ReasonResponse" HeaderText="Ritorno" CurrentFilterFunction="Contains"
                UniqueName="ReasonResponse" SortExpression="ReasonResponse">
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="RegistrationUserDateDescription" HeaderText="Inserimento"
                UniqueName="RegistrationUser" SortExpression="RegistrationUser" CurrentFilterFunction="Contains">
                <headerstyle horizontalalign="Center" width="10%" />
                <itemstyle horizontalalign="Left" />
            </telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="LastChangedUserDateDescription" HeaderText="Modifica"
                UniqueName="LastChangedUser" SortExpression="LastChangedUser" CurrentFilterFunction="Contains">
                <headerstyle horizontalalign="Center" width="10%" />
                <itemstyle horizontalalign="Left" />
            </telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
    <ClientSettings AllowDragToGroup="True">
        <Selecting AllowRowSelect="True"></Selecting>
    </ClientSettings>
    <ExportSettings>
        <Pdf FontType="Subset" PaperSize="Letter" />
        <Excel Format="Html" />
        <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" />
    </ExportSettings>
    <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
</DocSuite:BindGrid>
</div>