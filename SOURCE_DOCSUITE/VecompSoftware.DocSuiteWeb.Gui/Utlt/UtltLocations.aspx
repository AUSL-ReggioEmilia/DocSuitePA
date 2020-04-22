<%@ Page Title="Locations" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UtltLocations.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltLocations" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <table class="dataform">
        <tr>
            <td class="label" style="width: 20%">
                <strong>Ambiente</strong>
            </td>
            <td style="width: 80%">
                <asp:RadioButtonList runat="server" ID="environmentsRadioButtonList" OnSelectedIndexChanged="environmentSelection" AutoPostBack="True" RepeatDirection="Horizontal"></asp:RadioButtonList>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadGrid AutoGenerateColumns="False" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" ID="dgLocation" runat="server" Width="100%">
        <HeaderStyle ForeColor="White" CssClass="tabella" />
        <MasterTableView>
            <HeaderStyle Width="20px" />
            <Columns>
                <telerik:GridBoundColumn DataField="Id" HeaderStyle-Width="20%" HeaderText="Locazione" UniqueName="Id">
                    <ItemStyle Font-Bold="True" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Name"  HeaderText="Nome" UniqueName="Name" />
                <telerik:GridBoundColumn DataField="DocumentServer" HeaderText="DS Server" UniqueName="DocumentServer" />
                <telerik:GridTemplateColumn HeaderText="DS Archivio">
                    <ItemTemplate>
                        <%=DsArchive()%>'
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>

    </telerik:RadGrid>
</asp:Content>