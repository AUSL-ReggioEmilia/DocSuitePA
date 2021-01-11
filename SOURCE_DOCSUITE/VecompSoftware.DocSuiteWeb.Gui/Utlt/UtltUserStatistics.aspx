<%@ Page AutoEventWireup="false" Codebehind="UtltUserStatistics.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltUserStatistics" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Statistiche Log" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label" width="15%">Data:
            </td>
            <td >
                <span class="miniLabel">Da</span>
                <telerik:RadDatePicker ID="txtDate_From" runat="server" />
                <span class="miniLabel">A</span>
                <telerik:RadDatePicker id="txtDate_To" runat="server" />
           </td>
            <td align="center" width="15%">
                <b>Utenti Totali</b>
            </td> 
        </tr>
        <tr>
            <td class="label">Utente:
            </td>
            <td align="left" width="70%">
                <asp:TextBox ID="txtUser" MaxLength="30" runat="server" Width="200px" />
            </td>
            <td align="center" width="15%">
                <asp:Label ID="txtTotal" runat="server" Font-Bold="True">0</asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label">Operazioni minime:
            </td>
            <td align="left" width="70%">
                <asp:TextBox Columns="10" ID="txtOperations" MaxLength="10" runat="server" />
                <asp:CompareValidator ControlToValidate="txtOperations" Display="Dynamic" ErrorMessage="Errore formato" ID="Comparevalidator2" Operator="DataTypeCheck" runat="server" Type="Integer" />
            </td>
            <td align="center" width="15%">
                <b>Utenti Selezionati</b></td>
        </tr>
        <tr>
            <td class="label">
            </td>
            <td>
            </td>
            <td align="center" width="15%">
                <asp:Label ID="lblSelected" runat="server" Font-Bold="True">0</asp:Label>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="Cerca" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server"> 
    <telerik:RadAjaxPanel ID="pnlRisultati" runat="server" Width="100%" Height="100%">
        <asp:Panel ID="pnlProt" runat="server">
            <telerik:RadGrid ID="dgLogStatistics" runat="server" Width="100%" BackColor="White" AutoGenerateColumns="False" GridLines="Vertical" BorderWidth="1px">
                <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst"
                    Dir="LTR" Frame="Border" TableLayout=Fixed>
                    <RowIndicatorColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                        Visible="False">
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </RowIndicatorColumn>
                    <ExpandCollapseColumn CurrentFilterFunction="NoFilter" FilterListOptions="VaryByDataType"
                        Visible="False" Resizable="False">
                        <HeaderStyle Width="20px"></HeaderStyle>
                    </ExpandCollapseColumn>
                    <Columns>
                        <telerik:GridBoundColumn DataField="SystemUser" HeaderText="Utente">
                            <HeaderStyle HorizontalAlign="Center" Width="23%"></HeaderStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="TotalOperationsCount" HeaderText="Operazioni Totali">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle Font-Bold="True" HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ICount" HeaderText="Inserimento">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="SCount" HeaderText="Sommario">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DCount" HeaderText="Documento">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ZCount" HeaderText="Autorizzazione" UniqueName="ZCount">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="MCount" HeaderText="Modifica" UniqueName="MCount">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="OTCount" HeaderText="Altri">
                            <HeaderStyle HorizontalAlign="Center" Width="11%"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </asp:Panel>
    </telerik:RadAjaxPanel>
</asp:Content>