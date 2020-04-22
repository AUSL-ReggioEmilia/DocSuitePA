<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltUserLog" Title="Utenti - Lista Accessi" CodeBehind="UtltUserLog.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label">Data Accesso:
            </td>
            <td class="DXChiaro">
                <span class="miniLabel">Da</span>
                <telerik:RadDatePicker ID="rdpDateFrom" runat="server" />
                <span class="miniLabel">A</span>
                <telerik:RadDatePicker ID="rdpDateTo" runat="server" />
            </td>
            <td class="DXChiaro" align="center">
                <b>Reg. Estratte</b></td>
        </tr>
        <tr>
            <td class="label">Server:
            </td>
            <td>
                <asp:TextBox ID="txtSystemServer" runat="server" Width="200px" MaxLength="30" />
            </td>
            <td class="DXChiaro" align="center">
                <asp:Label ID="lblCounter" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label">Utente:
            </td>
            <td>
                <asp:TextBox ID="txtSystemUser" runat="server" Width="200px" MaxLength="30" />
            </td>
            <td class="DXChiaro" align="center">&nbsp;</td>
        </tr>
    </table>
    <asp:Button CausesValidation="true" ID="btnSearch" runat="server" Text="Ricerca" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="radGridWrapper">
        <DocSuite:BindGrid AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowMultiRowSelection="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CloseFilterMenuOnClientClick="True" GridLines="None" ID="gvUserLog" runat="server" ShowGroupPanel="True">
            <MasterTableView AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True" GridLines="Both" NoMasterRecordsText="Ricerca Nulla" TableLayout="Auto">
                <Columns>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Id" HeaderText="Utente" SortExpression="Id" UniqueName="Id">
                        <HeaderStyle Width="9%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemServer" HeaderText="Server" SortExpression="SystemServer" UniqueName="SystemServer">
                        <HeaderStyle Width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SystemComputer" HeaderText="Computer" SortExpression="SystemComputer" UniqueName="SystemComputer">
                        <HeaderStyle Width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="LastOperationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderText="Accesso" SortExpression="LastOperationDate" UniqueName="LastOperationDate">
                        <HeaderStyle HorizontalAlign="Center" Width="12%" />
                        <ItemStyle HorizontalAlign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="EqualTo" DataField="AccessNumber" DataType="System.Int32" HeaderText="N." SortExpression="AccessNumber" UniqueName="AccessNumber">
                        <HeaderStyle HorizontalAlign="Center" Width="5%" />
                        <ItemStyle HorizontalAlign="Center" />
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="PrevOperationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" HeaderText="Accesso Prec." SortExpression="PrevOperationDate" UniqueName="PrevOperationDate">
                        <HeaderStyle HorizontalAlign="Center" Width="12%" />
                        <ItemStyle HorizontalAlign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="SessionId" HeaderText="Sessione" SortExpression="SessionId" UniqueName="SessionId">
                        <HeaderStyle Width="46%" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </DocSuite:BindGrid>
    </div>
</asp:Content>
