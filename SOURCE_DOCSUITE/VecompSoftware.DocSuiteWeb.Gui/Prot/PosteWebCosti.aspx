<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="PosteWebCosti.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Prot.PosteWeb.Costi" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="datatable">
        <tr style="vertical-align: middle;">
            <td class="label labelPanel col-dsw-2">Spedizioni:
            </td>
            <td class="col-dsw-8">
                <telerik:RadDatePicker ID="dtpRegistrationDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                <telerik:RadDatePicker ID="dtpRegistrationDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
            </td>
        </tr>
    </table>
    <telerik:radbutton id="btnRefresh" runat="server" Text="Aggiorna" />
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="pnlDgPoste" runat="server" Visible="True" Width="100%">
        <telerik:RadGrid AutoGenerateColumns="False" ShowGroupPanel="True" GroupingEnabled="True" GridLines="Vertical" ID="dgPosteRequestContact" runat="server" >
            <ExportSettings ExportOnlyData="True" IgnorePaging="True" OpenInNewWindow="True" />
            <HeaderStyle CssClass="tabella" />
            <MasterTableView AllowNaturalSort="true" AllowSorting="true" ShowGroupFooter="true">
                <GroupFooterTemplate>
                    Somma:
                    <asp:Label ID="Label6" runat="server" Text='<%#Eval("CostoTotale")%>' />
                </GroupFooterTemplate>
                <Columns>
                    <telerik:GridBoundColumn Groupable="true" DataField="Settore" HeaderText="Settore" SortExpression="Settore" UniqueName="Settore" />
                    <telerik:GridBoundColumn DataField="Tipo" HeaderText="Tipo" SortExpression="Tipo" UniqueName="Tipo" />
                    <telerik:GridBoundColumn Groupable="False" Aggregate="Sum" DataField="CostoTotale" HeaderText="Costo" SortExpression="CostoTotale" UniqueName="CostoTotale" />
                </Columns>
            </MasterTableView>
            <ClientSettings AllowDragToGroup="true" />
        </telerik:RadGrid>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:radbutton id="btnExcel" runat="server" Text="Esporta" />
</asp:Content>
