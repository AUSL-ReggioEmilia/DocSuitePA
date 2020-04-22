<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltUserError" Title="Errori - Lista" MasterPageFile="~/MasterPages/DocSuite2008.Master" Codebehind="UtltUserError.aspx.vb" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="datatable">
        <tr>
            <td class="label">Data:
            </td>
            <td>
                <telerik:RadDatePicker ID="rdpErrorDate" runat="server" />
            </td>
            <td class="DXChiaro" align="center">
                <b>Reg. Estratte</b></td>
        </tr>
        <tr>
            <td class="label">Server:
            </td>
            <td>
                <asp:TextBox ID="txtSystemServer" runat="server" Width="200px" MaxLength="30" />
                <span class="miniLabel">Utente:</span>
                <asp:TextBox ID="txtSystemUser" runat="server" Width="200px" MaxLength="30" />
            </td>
            <td class="DXChiaro" align="center">
                <asp:Label ID="lblCounter" runat="server" Font-Bold="True"></asp:Label></td>
        </tr>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="Cerca" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div class="radGridWrapper">
        <DocSuite:BindGrid AllowAutofitTextBoxFilter="True" AllowFilteringByColumn="True" AllowMultiRowSelection="True" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" CloseFilterMenuOnClientClick="True" GridLines="None" ID="gvUserError" runat="server" ShowGroupPanel="True">
            <MasterTableView TableLayout="Fixed" NoMasterRecordsText="Ricerca Nulla"
                GridLines="Both" AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True">
                <Columns>
                    <telerik:GridBoundColumn DataField="Id" HeaderText="Id" CurrentFilterFunction="EqualTo"
                        UniqueName="Id" SortExpression="Id">
                        <headerstyle width="5%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SystemUser" HeaderText="Utente" CurrentFilterFunction="Contains"
                        UniqueName="SystemUser" SortExpression="SystemUser">
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SystemServer" HeaderText="Server" CurrentFilterFunction="Contains"
                        UniqueName="SystemServer" SortExpression="SystemServer">
                        <headerstyle wrap="false" />
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="SystemComputer" HeaderText="Computer" CurrentFilterFunction="Contains"
                        UniqueName="SystemComputer" SortExpression="SystemComputer">
                        <headerstyle wrap="false"/>
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn DataField="ErrorDate" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy<BR>HH:mm:ss}"
                        CurrentFilterFunction="EqualTo" UniqueName="ErrorDate" SortExpression="ErrorDate">
                        <headerstyle horizontalalign="Center" width="12%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="ModuleName" HeaderText="Modulo" CurrentFilterFunction="Contains"
                        UniqueName="ModuleName" SortExpression="ModuleName">
                        <headerstyle horizontalalign="Left" width="16%" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ErroreDescriptionFormat" HeaderText="Descrizione" CurrentFilterFunction="Contains"
                        UniqueName="ErrorDescription" SortExpression="ErrorDescription">
                        <headerstyle horizontalalign="Left" width="55%" />
                        <itemstyle horizontalalign="Left" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </DocSuite:BindGrid>
    </div>
</asp:Content>
