<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtJournal.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtJournal" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Registri Giornalieri Protocollo" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>
    <asp:Panel runat="server" ID="pnlSearchJournal" DefaultButton="btnSearch">
        <table class="Table">
            <tr>
                <td class="SXScuro" style="width: 68px">Dal giorno:
                </td>
                <td class="DXChiaro" width="80%">
                    <telerik:RadDatePicker ID="rdpDateFrom" runat="server" />
                    <asp:CompareValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Data non valida" ID="CompareValidatorFrom" Operator="DataTypeCheck" runat="server" Type="Date" />
                    <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Campo Dal Giorno Obbligatorio" ID="RequiredFieldValidator4" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="SXScuro" style="width: 68px">Al giorno:
                </td>
                <td class="DXChiaro" width="80%">
                    <telerik:RadDatePicker ID="rdpDateTo" runat="server" />
                    <asp:CompareValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Data non valida" ID="CompareValidator1" Operator="DataTypeCheck" runat="server" Type="Date" />
                    <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Campo Dal Giorno Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                </td>
            </tr>
            <tr class="Spazio">
                <td colspan="2"></td>
            </tr>
            <tr>
                <td style="width: 68px">
                    <asp:Button ID="btnSearch" runat="server" Text="Ricerca"></asp:Button>
                </td>
            </tr>
            <tr class="Spazio">
                <td colspan="2"></td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table style="width: 100%; height: 99%;">
        <tr class="tabella">
            <td>Registri Giornalieri</td>
        </tr>
        <tr style="height: 100%;">
            <td style="vertical-align: top">
                <div class="radGridWrapper">
                    <DocSuite:BindGrid AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CloseFilterMenuOnClientClick="True" GridLines="None" ID="gvJournalLog" runat="server" ShowGroupPanel="True">
                        <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True"
                            CommandItemDisplay="Top" NoMasterRecordsText="Nessun Registro Trovato" TableLayout="Fixed">
                            <PagerStyle Position="Top" Visible="False" />
                            <Columns>
                                <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" CurrentFilterFunction="EqualTo"
                                    SortExpression="Id">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn DataField="ProtocolJournalDate" HeaderText="Registro" SortExpression="ProtocolJournalDate" CurrentFilterFunction="EqualTo"
                                    UniqueName="ProtocolJournalDate">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lblProtocolJournalDate" runat="server" Text='<%#String.Format("{0:dd/MM/yyyy}", DataBinder.Eval(Container.DataItem, "ProtocolJournalDate"))%>' CommandName="ShowDoc" CommandArgument='<%# SetDocumentInfo(Container) %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>


                                <telerik:GridDateTimeColumn DataField="StartDate" HeaderText="Ora Inizio" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                                    CurrentFilterFunction="EqualTo" UniqueName="StartDate" SortExpression="StartDate">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="125px" />
                                    <ItemStyle HorizontalAlign="Center" Width="125px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridDateTimeColumn DataField="EndDate" HeaderText="Ora Fine" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}"
                                    CurrentFilterFunction="EqualTo" UniqueName="EndDate" SortExpression="EndDate">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="125px" />
                                    <ItemStyle HorizontalAlign="Center" Width="125px" />
                                </telerik:GridDateTimeColumn>
                                <telerik:GridBoundColumn DataField="ProtocolTotal" HeaderText="Totali" UniqueName="ProtocolTotal" CurrentFilterFunction="EqualTo"
                                    SortExpression="ProtocolTotal" DataType="System.Int32">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ProtocolRegister" HeaderText="Registrati" UniqueName="ProtocolRegister" CurrentFilterFunction="EqualTo"
                                    SortExpression="ProtocolRegister" DataType="System.Int32">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ProtocolError" HeaderText="Errori" UniqueName="ProtocolError" CurrentFilterFunction="EqualTo"
                                    SortExpression="ProtocolError" DataType="System.Int32">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ProtocolCancelled" HeaderText="Annullati" UniqueName="ProtocolCancelled" CurrentFilterFunction="EqualTo"
                                    SortExpression="ProtocolCancelled" DataType="System.Int32">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ProtocolActive" HeaderText="Attivi" UniqueName="ProtocolActive" CurrentFilterFunction="EqualTo"
                                    SortExpression="ProtocolActive" DataType="System.Int32">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="ProtocolOthers" HeaderText="Altri" UniqueName="ProtocolOthers" CurrentFilterFunction="EqualTo"
                                    SortExpression="ProtocolOthers" DataType="System.Int32">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="5%" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </telerik:GridBoundColumn>
                                <telerik:GridBoundColumn DataField="LogDescription" HeaderText="Descrizione" UniqueName="LogDescription" CurrentFilterFunction="Contains"
                                    SortExpression="LogDescription">
                                    <HeaderStyle HorizontalAlign="Center" Wrap="False" Width="35%" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </telerik:GridBoundColumn>
                            </Columns>
                            <RowIndicatorColumn>
                                <HeaderStyle Width="20px" />
                            </RowIndicatorColumn>
                            <ExpandCollapseColumn>
                                <HeaderStyle Width="20px" />
                            </ExpandCollapseColumn>
                        </MasterTableView>
                        <ClientSettings AllowDragToGroup="True">
                            <ClientMessages DragToResize="Ridimensiona" />
                            <Resizing AllowColumnResize="True" ClipCellContentOnResize="False" ResizeGridOnColumnResize="True" />
                        </ClientSettings>
                        <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Decrescente"
                            SortToolTip="Ordina" />
                        <ExportSettings FileName="Esportazione">
                            <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
                            <Excel Format="ExcelML" />
                        </ExportSettings>
                    </DocSuite:BindGrid>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
