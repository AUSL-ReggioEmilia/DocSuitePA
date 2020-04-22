<%@ Page AutoEventWireup="false" CodeBehind="UtltCheckDBMapping.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltCheckDBMapping" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Verifica Mappatura" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="Table">
        <tr>
            <td colspan="2">
                <asp:Label ID="lbDatabase" runat="server" Text="Mappatura:" Font-Bold="True"></asp:Label>
                <asp:DropDownList ID="ddlMapping" runat="server">
                    <asp:ListItem>Tutti</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnEsegui" runat="server" Text="Esegui" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="rgMapping" runat="Server" Width="100%">
        <MasterTableView>
            <RowIndicatorColumn>
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="DBName" HeaderText="Database" UniqueName="DBName" />
                <telerik:GridBoundColumn DataField="ClassName" HeaderText="Classe" UniqueName="ClassName" />
                <telerik:GridBoundColumn DataField="FieldName" HeaderText="Campo" UniqueName="FieldName" />
                <telerik:GridBoundColumn DataField="ClassDataType" HeaderText="Tipo Classe" UniqueName="ClassDataType" />
                <telerik:GridBoundColumn DataField="TableName" HeaderText="Tabella" UniqueName="DBDataType" />
                <telerik:GridBoundColumn DataField="DBDataType" HeaderText="Tipo DB" UniqueName="DBDataType" />
                <telerik:GridBoundColumn DataField="Reason" HeaderText="Descrizione" UniqueName="Reason" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>
