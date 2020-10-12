<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.DocmTokenUser" 
     CodeBehind="DocmTokenUser.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" 
    Title="Pratica - Dettaglio Assegnazione Utenti" %>


<asp:Content ID="Content1" ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid ID="RadGrid1" runat="server" AutoGenerateColumns="False" GridLines="None" AllowFilteringByColumn="True" AllowSorting="True" ShowGroupPanel="True" AllowPaging="True">
            <MasterTableView CommandItemDisplay="None"  TableLayout="Auto" NoMasterRecordsText="Nessun dettaglio presente">
                <Columns>
                    <telerik:GridTemplateColumn HeaderImageUrl="../Comm/Images/User16.gif" AllowFiltering="false" HeaderText="T" CurrentFilterFunction="NoFilter" UniqueName="IsActive">
                        <ItemTemplate>
                            <asp:Image runat="server" ID="Image1" /> 
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="FormatStep" HeaderText="DocStep" UniqueName="DocStep" CurrentFilterFunction="EqualTo" SortExpression="Step" DataType="System.Int16"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Role.Name" HeaderText="Settore" UniqueName="Role.Name" CurrentFilterFunction="Contains" SortExpression="Role.Name"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="UserRole" HeaderText="Gruppo" ItemStyle-Font-Bold="true" UniqueName="UserRole" CurrentFilterFunction="Contains" SortExpression="UserRole"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="UserName" HeaderText="Utente" UniqueName="UserName" CurrentFilterFunction="Contains" SortExpression="UserName"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Account" HeaderText="Account" UniqueName="Account" CurrentFilterFunction="Contains" SortExpression="Account"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="note" HeaderText="Note" UniqueName="note" CurrentFilterFunction="Contains" SortExpression="note"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="UserDate" HeaderText="Inserimento" CurrentFilterFunction="Contains" UniqueName="RegistrationDate" SortExpression="RegistrationDate"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LastUserDate" HeaderText="Modifica" CurrentFilterFunction="Contains" UniqueName="LastChangedDate" SortExpression="LastChangedDate"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LastFormatStep" HeaderText="Step" CurrentFilterFunction="Contains" UniqueName="LastStep" SortExpression="LastStep" DataType="System.Int16"></telerik:GridBoundColumn>
               </Columns>
            </MasterTableView>
            <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom" ShowPagerText="False"  />
            <ClientSettings AllowDragToGroup="True">
            </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>