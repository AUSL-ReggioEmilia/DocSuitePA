<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltServiceLog"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Log dei Servizi"
    Codebehind="UtltServiceLog.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphHeader" runat="server">
    <table class="Table">
        <tr>
            <td class="SXScuro" width="15%" style="text-align:right;vertical-align:middle;">
                Da Data:&nbsp;</td>
            <td width="70%">
                <telerik:RadDatePicker ID="rdpDate_From" runat="server" />
                &nbsp;<b style="height: 100%; vertical-align: middle;">a:</b>&nbsp;
                <telerik:RadDatePicker ID="rdpDate_To" runat="server" />
            </td>
            <td align="center" width="15%">
                <strong>Reg. Totali</strong></td>
        </tr>
        <tr>
            <td class="SXScuro" width="15%" style="text-align:right;vertical-align:middle;">
                Sessione:&nbsp;</td>
            <td align="left" width="70%">
                <asp:TextBox ID="txtSession" runat="server" Width="200px"></asp:TextBox></td>
            <td align="center" width="15%">
                <asp:Label ID="txtTotal" runat="server" Font-Bold="True">0</asp:Label></td>
        </tr>
        <tr>
            <td class="SXScuro" width="15%" style="text-align:right;vertical-align:middle;">
                Descrizione:&nbsp;</td>
            <td align="left" width="70%">
                <asp:TextBox ID="txtText" runat="server" Width="200px"></asp:TextBox></td>
            <td align="center" width="15%">
                <strong>Reg. Estratte</strong></td>
        </tr>
        <tr>
            <td class="SXScuro" width="15%" style="text-align:right;vertical-align:middle;">
                Livello:&nbsp;</td>
            <td align="left" width="70%">
                <asp:DropDownList ID="ddlLevel" runat="server">
                    <asp:ListItem></asp:ListItem>
                    <asp:ListItem Value="0">0 - Informazioni</asp:ListItem>
                    <asp:ListItem Value="1">1 - Avviso</asp:ListItem>
                    <asp:ListItem Value="2">2 - Errore</asp:ListItem>
                </asp:DropDownList></td>
            <td align="center" width="15%">
                <strong>
                    <asp:Label ID="txtTbltEstratto" runat="server" Font-Bold="True">0</asp:Label></strong></td>
        </tr>
    </table>
    <br class="Spazio">
    <table>
        <tr>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="Cerca"></asp:Button>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server" >
    <div class="radGridWrapper">
        <DocSuite:BindGrid ID="gvServiceLog" runat="server"
            AutoGenerateColumns="False" AllowMultiRowSelection="True" AllowAutofitTextBoxFilter="True"
            AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" CloseFilterMenuOnClientClick="True"
            ShowGroupPanel="True" GridLines="None">
            <MasterTableView TableLayout="Fixed" NoMasterRecordsText="Nessun Log trovato"
                GridLines="Both" AllowCustomPaging="True" AllowCustomSorting="True" AllowMultiColumnSorting="True">
                <Columns>
                    <telerik:GridDateTimeColumn DataField="DateTime" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}"
                        CurrentFilterFunction="EqualTo" UniqueName="DateTime" SortExpression="DateTime">
                        <headerstyle width="135px" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridBoundColumn DataField="Server" HeaderText="Server" CurrentFilterFunction="Contains"
                        UniqueName="Server" SortExpression="Server">
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Client" HeaderText="Client" CurrentFilterFunction="Contains"
                        UniqueName="Client" SortExpression="Client">
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Application" HeaderText="Applicazione" CurrentFilterFunction="Contains"
                        UniqueName="Application" SortExpression="Application">
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Session" HeaderText="Sessione" CurrentFilterFunction="Contains"
                        UniqueName="Session" SortExpression="Session">
                        <headerstyle width="8%" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Level" HeaderText="Livello" CurrentFilterFunction="EqualTo"
                        UniqueName="Level" SortExpression="Level" DataType="System.Int16">
                        <headerstyle horizontalalign="Center" width="5%" />
                        <itemstyle horizontalalign="Center" />
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Text" HeaderText="Descrizione" CurrentFilterFunction="Contains"
                        UniqueName="Text" SortExpression="Text">
                        <headerstyle width="57%" />
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </DocSuite:BindGrid>
    </div>
</asp:Content>
