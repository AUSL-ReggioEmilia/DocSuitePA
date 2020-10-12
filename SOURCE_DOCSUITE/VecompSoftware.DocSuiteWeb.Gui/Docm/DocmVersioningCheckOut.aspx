<%@ Page Language="vb" AutoEventWireup="false" SmartNavigation="True" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmVersioningCheckOut"
    Codebehind="DocmVersioningCheckOut.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Pratica - Versioning CheckOut" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <DocSuite:BindGrid AllowCustomPaging="True" AllowFilteringByColumn="True" AllowMultiRowSelection="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" GridLines="None" Height="65px" ID="gvDocuments" PageSize="20" runat="server" ShowGroupPanel="True" Width="100%">
        <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom"
            ShowPagerText="False" />
        <MasterTableView AllowMultiColumnSorting="True" CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" Dir="LTR" Frame="Border" NoMasterRecordsText="Nessun Documento in checkout" TableLayout="Fixed" Width="100%">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Cartella" Groupable="false" UniqueName="TemplateColumn"
                    CurrentFilterFunction="NoFilter" AllowFiltering="False">
                    <itemtemplate>
                            <%--<asp:Label runat="server" Text='<%# SetupFolder(databinder.eval(container.dataitem,"Incremental"))%>' ID="Label1" />--%>
                            <asp:Label runat="server" Text='<%# SetupFolder(eval("DocumentObject.IncrementalFolder"))%>' ID="Label1" />
                        </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="DocumentObject.Description" HeaderText="Documento"
                    UniqueName="DocumentObject.Description" SortExpression="DocumentObject.Description"
                    CurrentFilterFunction="Contains">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CheckOutUser" HeaderText="Utente" UniqueName="CheckOutUser"
                    SortExpression="CheckOutUser" CurrentFilterFunction="Contains">
                </telerik:GridBoundColumn>
                <telerik:GridDateTimeColumn DataField="CheckOutDate" HeaderText="Data" UniqueName="CheckOutDate"
                    SortExpression="CheckOutDate" CurrentFilterFunction="EqualTo">
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="DocumentObject.DocObject" HeaderText="Oggetto"
                    UniqueName="DocumentObject.DocObject" SortExpression="DocumentObject.DocObject"
                    CurrentFilterFunction="Contains" AllowFiltering="false" >
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DocumentObject.Reason" HeaderText="Motivo" UniqueName="DocumentObject.Reason"
                    SortExpression="DocumentObject.Reason" CurrentFilterFunction="Contains" AllowFiltering="false" >
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DocumentObject.Note" HeaderText="Note" UniqueName="DocumentObject.Note"
                    SortExpression="DocumentObject.Note" CurrentFilterFunction="Contains" AllowFiltering="false" >
                </telerik:GridBoundColumn>
            </Columns>
            <RowIndicatorColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                Visible="False">
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                Resizable="False" Visible="False">
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <EditFormSettings>
                <EditColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType">
                </EditColumn>
            </EditFormSettings>
        </MasterTableView>
        <ClientSettings AllowDragToGroup="True">
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
        <ExportSettings>
            <Pdf FontType="Subset" PaperSize="Letter" />
            <Excel Format="Html" />
            <Csv ColumnDelimiter="Comma" RowDelimiter="NewLine" />
        </ExportSettings>
        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente"
            SortToolTip="Ordina" />
    </DocSuite:BindGrid>
</asp:Content>
