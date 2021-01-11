<%@ Page Title="Elenco attività" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TaskHeaderGrid.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TaskHeaderGrid" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
     <telerik:RadScriptBlock runat="server" ID="rsb">
        
         <script type="text/javascript">
            function onRowDeselected(sender, args) {
                var grid = sender;
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                if (selectedRows.length <= 0) {
                    $(id_cmdViewProtocols).attr('disabled', 'disabled');
                    $(id_cmdViewPECMails).attr('disabled', 'disabled');
                    $(id_cmdViewPOLRequests).attr('disabled', 'disabled');
                }
            }

            function onRowSelected(sender, args) {
                var grid = sender;
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                if (selectedRows.length > 0) {
                    $(id_cmdViewProtocols).removeAttr('disabled');
                    $(id_cmdViewPECMails).removeAttr('disabled');
                    $(id_cmdViewPOLRequests).removeAttr('disabled');
                }
            }
            $(document).ready(function () {
                $(id_cmdViewProtocols).attr('disabled', 'disabled');
                $(id_cmdViewPECMails).attr('disabled', 'disabled');
                $(id_cmdViewPOLRequests).attr('disabled', 'disabled');
            })
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both"
        ID="dgTaskHeaders" PageSize="20" runat="server">

        <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione"
            TableLayout="Auto" DataKeyNames="Id">

            <ItemStyle CssClass="Scuro" />
            <AlternatingItemStyle CssClass="Chiaro" />
            
            <Columns>

                <telerik:GridClientSelectColumn HeaderStyle-Width="16px" UniqueName="Select" />
                
                <telerik:GridTemplateColumn UniqueName="ViewSummary" AllowFiltering="false" Groupable="false"
                    HeaderText="Visualizza sommario" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/information.png" >
                    <HeaderStyle HorizontalAlign="Left" Width="1%" />
                    <ItemStyle HorizontalAlign="Left" Width="1%" />
                    <ItemTemplate>
                        <asp:ImageButton AlternateText="Visualizza sommario" runat="server" ID="cmdViewSummary" CommandName="ViewSummary"
                             ImageUrl="../App_Themes/DocSuite2008/imgset16/information.png" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
               
                <telerik:GridDateTimeColumn DataField="RegistrationDate"
                    DataType="System.DateTime" HeaderText="Data registrazione" UniqueName="RegistrationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                    SortExpression="RegistrationDate" AllowSorting="True" ShowSortIcon="True">
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                </telerik:GridDateTimeColumn>
                                
                <telerik:GridBoundColumn DataField="Code"
                    HeaderText="Codice" UniqueName="Code" Groupable="false" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                
                <telerik:GridBoundColumn DataField="Title"
                    HeaderText="Titolo" UniqueName="Title" Groupable="false" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>

                <telerik:GridBoundColumn DataField="Description"
                    HeaderText="Descrizione" UniqueName="Description" Groupable="false" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <DocSuite:SuggestFilteringColumn UniqueName="Status" HeaderText="Stato" DataField="Status" CurrentFilterFunction="EqualTo"
                    SortExpression="Status" AllowSorting="False" >
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </DocSuite:SuggestFilteringColumn>
                <DocSuite:SuggestFilteringColumn UniqueName="SendingProcessStatus" HeaderText="Stato attività" DataField="SendingProcessStatus" CurrentFilterFunction="EqualTo"
                    SortExpression="SendingProcessStatus" AllowSorting="False" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </DocSuite:SuggestFilteringColumn>
                <DocSuite:SuggestFilteringColumn UniqueName="SendedStatus" HeaderText="Stato invio" DataField="SendedStatus" CurrentFilterFunction="EqualTo"
                    SortExpression="SendedStatus" AllowSorting="False" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </DocSuite:SuggestFilteringColumn>

                    <telerik:GridTemplateColumn  HeaderText="Ripristino" UniqueName="ExcelRecovery"  >
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                              <asp:HyperLink ID="BtnExcelRecovery" runat="server" Enabled="false" Visible="true" Text="Scarica file errori" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="false" CellSelectionMode="None" UseClientSelectColumnOnly="false"
                 EnableDragToSelectRows="False" />
            <ClientEvents OnRowDeselected="onRowDeselected" OnRowSelected="onRowSelected" />
        </ClientSettings>

        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente"
             SortToolTip="Ordina" />

        <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />
    </DocSuite:BindGrid>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button ID="cmdNew" runat="server" Text="Inserisci" Width="150px" Visible="false" />
        <asp:Button ID="cmdViewProtocols" Text="Visualizza Protocolli" runat="server" Width="150px" Visible="false" />
        <asp:Button ID="cmdViewPECMails" Text="Visualizza PEC" runat="server" Width="150px" Visible="false" />
        <asp:Button ID="cmdViewPOLRequests" Text="Visualizza Poste Web" runat="server" Width="150px" Visible="false" />
        <asp:Button ID="cmdReset" Text="Reset" runat="server" Width="150px" Visible="false" />
    </asp:Panel>
</asp:Content>
