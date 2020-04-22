<%@ Page AutoEventWireup="false" Codebehind="UtltLog.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltLog" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Log " %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label" style="width: 15%;">Data:</td>
            <td style="width: 70%;">
                <span class="miniLabel">Da</span>
                <telerik:RadDatePicker ID="txtDate_From" runat="server" />
                <span class="miniLabel">A</span>
                <telerik:RadDatePicker id="txtDate_To" runat="server" />
            </td>
            <td align="center" width="15%">
                <strong>Reg. Totali</strong>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">Computer:</td>
            <td style="width: 70%;">
                <asp:TextBox ID="txtComputer" runat="server" Width="200px" />&nbsp;
                <asp:Panel ID="pnlTabella" runat="server" Style="display: inline">
                    <strong>Tabella:</strong>
                    <asp:TextBox ID="txtTableName" runat="server" Width="200px"></asp:TextBox>
                </asp:Panel>
            </td>
            <td align="center" width="15%">
                <asp:Label ID="txtTotal" runat="server" Font-Bold="True">0</asp:Label></td>
        </tr>
        <tr>
            <td class="label" style="width: 15%;">Utente:
            </td>
            <td style="width: 70%;">
                <asp:TextBox ID="txtUser" runat="server" Width="200px" />
                <span class="miniLabel">Tipo:</span>
                <asp:TextBox ID="txtType" runat="server" Width="50px" />
            </td>
            <td align="center" width="15%">
                <strong>Reg. Estratte</strong></td>
        </tr>
        <asp:Panel ID="pnlInputTblt" runat="server">
            <tr>
                <td class="label" style="width: 15%;"></td>
                <td class="col-dsw-7"></td>
                <td align="center" width="15%">
                    <asp:Label ID="txtTbltEstratto" runat="server" Font-Bold="True">0</asp:Label>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlInputProt" runat="server">
            <tr>
                <td class="label" style="width: 15%;">Anno:
                </td>
                <td class="col-dsw-7">
                    <asp:TextBox ID="txtProtYear" runat="server" Width="72px" />
                    <asp:RegularExpressionValidator ControlToValidate="txtProtYear" Display="Dynamic" ErrorMessage="Errore formato" ID="vYear" runat="server" ValidationExpression="\d{4}" />
                    <span class="miniLabel">Numero:</span>
                    <asp:TextBox ID="txtProtNumber" runat="server" Width="100px" />
                    <asp:RegularExpressionValidator ControlToValidate="txtProtNumber" ErrorMessage="Errore formato" ID="vNumber" runat="server" ValidationExpression="\d*" Width="200px" />
                </td>
                <td align="center" width="15%">
                        <asp:Label ID="txtProtEstratto" runat="server" Font-Bold="True">0</asp:Label>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlInputResl" runat="server">
            <tr>
                <td class="label" style="width: 15%;">Id:</td>
                <td align="left" width="70%">
                    <asp:TextBox ID="txtIdResolution" runat="server" Width="72px" />
                    <asp:RegularExpressionValidator ControlToValidate="txtIdResolution" Display="Dynamic" ErrorMessage="Errore formato" ID="Regularexpressionvalidator1" runat="server" ValidationExpression="\d+" />
                </td>
                <td align="center" width="15%">
                    <asp:Label ID="txtReslEstratto" runat="server" Font-Bold="True">0</asp:Label>
                </td>
            </tr>
        </asp:Panel>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="Cerca" />
    <asp:Button ID="btnStampa" runat="server" Text="Stampa" />
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
        <asp:Panel ID="pnlDgTblt" runat="server" CssClass="radGridWrapper">
            <DocSuite:BindGrid ID="dgLog" runat="server" BackColor="White" GridLines="Vertical"
                BorderWidth="1px" CellPadding="3" BorderColor="#999999" BorderStyle="None"
                AutoGenerateColumns="False" AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CloseFilterMenuOnClientClick="True" ShowGroupPanel="True">
                <MasterTableView CommandItemDisplay="Top" AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" TableLayout="Auto" NoMasterRecordsText="Nessun Log disponibile">
                    <RowIndicatorColumn
                        Visible="False">
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn
                        Visible="False" Resizable="False">
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </ExpandCollapseColumn>
                    <Columns>                        
                        <telerik:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" DataType="System.Int32" AllowFiltering="False"></telerik:GridBoundColumn>
                        <telerik:GridDateTimeColumn DataField="LogDate" HeaderText="Data" UniqueName="LogDate" CurrentFilterFunction="EqualTo" SortExpression="LogDate">
                            <HeaderStyle Width="135px"></HeaderStyle>
                            <ItemStyle Width="135px"></ItemStyle>                            
                        </telerik:GridDateTimeColumn>
                        <telerik:GridBoundColumn DataField="SystemComputer" HeaderText="Computer" CurrentFilterFunction="Contains" SortExpression="SystemComputer" UniqueName="SystemComputer"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="SystemUser" HeaderText="Utente" CurrentFilterFunction="Contains" SortExpression="SystemUser" UniqueName="SystemUser"></telerik:GridBoundColumn>
                    </Columns>
                    <EditFormSettings>
                        <PopUpSettings ScrollBars="None" />
                    </EditFormSettings>
                    <PagerStyle Position="Top" Visible="False" />
                </MasterTableView>
                <ExportSettings FileName="Esportazione">
                    <Pdf PageHeight="297mm" PageWidth="210mm" PaperSize="A4" />
                    <Excel Format="ExcelML" />
                </ExportSettings>
                <ClientSettings AllowDragToGroup="True" />
                <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Decrescente" SortToolTip="Ordina" />
            </DocSuite:BindGrid>
        </asp:Panel>
</asp:Content>
