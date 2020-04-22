<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TaskHeaderViewer.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TaskHeaderViewer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">

            function openEditWindow() {
                var oWnd = $find("<%=OpenFile.ClientID%>");
                oWnd.show();
                return false;
            }

        </script>

    </telerik:RadCodeBlock>
    <telerik:RadWindow runat="server" ID="OpenFile" Title="Modifica documento" Width="480" Height="140" Behaviors="Close">
        <ContentTemplate>
            <asp:UpdatePanel ID="messagePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="warningAreaLow">
                        <p>
                            Percorso per modifica documento.<br />
                            Il seguente link pemette di modificare il documento direttamente sul server.
                        </p>
                    </div>
                    <div>
                        <asp:HyperLink runat="server" ID="linkDocumenti" Text="LINK"></asp:HyperLink>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphContent" runat="server">



    <table class="datatable" runat="server" id="tblHead">
        <tr>
            <th colspan="2">Dettagli del Task</th>
        </tr>
        <tr class="Chiaro">
            <td class="label" style="width: 200px">
                <b>Codice identificativo</b>
            </td>
            <td>
                <asp:Label ID="lblCode" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label">
                <b>Data registrazione</b>
            </td>
            <td>
                <asp:Label ID="lblRegistrationDate" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label">
                <b>Stato del Task</b>
            </td>
            <td>
                <asp:Label ID="lblStatus" runat="server"></asp:Label>
            </td>
        </tr>
    </table>


    <table class="datatable">
        <tr>
            <th>Elenco attività</th>
        </tr>
    </table>
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="True" AllowMultiRowSelection="false" AutoGenerateColumns="False" GridLines="Both"
                    ID="dgTaskDetails" PageSize="20" runat="server">

                    <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" NoMasterRecordsText="Nessun registrazione"
                        TableLayout="Auto">

                        <ItemStyle CssClass="Scuro" />
                        <AlternatingItemStyle CssClass="Chiaro" />

                        <Columns>

                            <telerik:GridDateTimeColumn DataField="RegistrationDate"
                                DataType="System.DateTime" HeaderText="Data registrazione" UniqueName="RegistrationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                                SortExpression="RegistrationDate" AllowSorting="True" ShowSortIcon="True">
                                <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="150" />
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                            </telerik:GridDateTimeColumn>

                            <DocSuite:SuggestFilteringColumn UniqueName="DetailType" HeaderText="Tipo" DataField="DetailType" CurrentFilterFunction="EqualTo"
                                SortExpression="DetailType" AllowSorting="False">
                                <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100" />
                                <ItemStyle HorizontalAlign="Center" />
                            </DocSuite:SuggestFilteringColumn>

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


                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="false" CellSelectionMode="None" UseClientSelectColumnOnly="false"
                            EnableDragToSelectRows="False" />
                    </ClientSettings>
                    <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente"
                        SortToolTip="Ordina" />

                    <GroupingSettings ShowUnGroupButton="true" UnGroupButtonTooltip="Rimuovi" />
                </DocSuite:BindGrid>
    </div>
           
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="pnlButtons">
        <asp:Button runat="server" ID="cmdCancel" Text="Cancella" Width="150" />
        <asp:Button runat="server" ID="cmdRetry" Text="Reset" Width="150"  />
        <asp:Button runat="server" ID="cmdEdit" Text="Documento" Width="150"  OnClientClick="return openEditWindow();" />
    </asp:Panel>
</asp:Content>
