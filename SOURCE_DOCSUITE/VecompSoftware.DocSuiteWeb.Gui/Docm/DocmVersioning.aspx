<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmVersioning"
    Codebehind="DocmVersioning.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    Title="Pratica - Versioning" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="1" CloseFilterMenuOnClientClick="True" GridLines="Vertical" Height="10px" ID="RadGrid11" runat="server" ShowGroupPanel="True">
            <FilterItemStyle Width="3%" />
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="False" />
            <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" CommandItemDisplay="Top" GridLines="Horizontal" NoMasterRecordsText="Nessun documento presente" TableLayout="Fixed" Width="100%">
                <Columns>
                    <DocSuite:CompositeTemplateColumn DataField="Description" HeaderText="Documento" GroupByExpression="Group by description" CurrentFilterFunction="Contains" UniqueName="Documento">
                        <itemtemplate>
                            <asp:LinkButton Runat="server" text='<%# Eval("description")%>' CommandName="show"  ID="Linkbutton1" />
                        </itemtemplate>
                    </DocSuite:CompositeTemplateColumn>
                    <telerik:GridBoundColumn DataField="DocObject" HeaderText="Oggetto" UniqueName="DocObject"
                        CurrentFilterFunction="Contains" SortExpression="DocObject">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Reason" HeaderText="Motivo" UniqueName="Reason"
                        CurrentFilterFunction="Contains" SortExpression="Reason">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Note" HeaderText="Note" UniqueName="Note" CurrentFilterFunction="Contains"
                        SortExpression="Note">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="DocumentVersionings(0).CheckSystemComputer" AllowFiltering="False"
                        HeaderText="PC" UniqueName="CheckSystemComputer" CurrentFilterFunction="Contains"
                        SortExpression="DocumentVersionings(0).CheckSystemComputer">
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="DocumentVersionings(0).CheckOutUserDate" AllowFiltering="False"
                        HeaderText="CheckOut" UniqueName="CheckOutDate" DataType="System.DateTime" CurrentFilterFunction="EqualTo"
                        SortExpression="DocumentVersionings(0).CheckOutDate" DataFormatString="{0:dd/MM/yyyy}">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn DataField="DocumentVersionings(0).CheckInUserDate" AllowFiltering="False"
                        HeaderText="CheckIn" UniqueName="CheckInDate" CurrentFilterFunction="EqualTo"
                        SortExpression="DocumentVersionings(0).CheckInDate" DataType="System.DateTime"
                        DataFormatString="{0:dd/MM/yyyy}">
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="DocumentVersionings(0).CheckStatus" AllowFiltering="False"
                        HeaderText="Stato" UniqueName="CheckStatus" CurrentFilterFunction="Contains"
                        SortExpression="DocumentVersionings(0).CheckStatus">
                    </telerik:GridBoundColumn>
                </Columns>
                <ItemStyle Font-Names="Verdana" />
                <AlternatingItemStyle Font-Names="Verdana" />
                <PagerStyle Position="Top" Visible="False" />
                <RowIndicatorColumn>
                    <HeaderStyle Width="20px" />
                </RowIndicatorColumn>
                <ExpandCollapseColumn>
                    <HeaderStyle Width="20px" />
                </ExpandCollapseColumn>
            </MasterTableView>
            <ExportSettings FileName="Esportazione">
                <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
                <Excel Format="ExcelML" />
            </ExportSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Decrescente"
                SortToolTip="Ordina" />
            <ClientSettings AllowDragToGroup="True">
                <ClientMessages DragToResize="Ridimensiona" />
                <Resizing AllowColumnResize="True" ClipCellContentOnResize="False" ResizeGridOnColumnResize="True" />
            </ClientSettings>
        </DocSuite:BindGrid>
    </div>
</asp:Content>
