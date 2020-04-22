<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master"
    CodeBehind="ProtConcorsiPrint.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtConcorsiPrint"
    Title="Protocollo - Stampa Elenco Concorsi" %>

<%@ Register Src="../UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="uc1" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphContent">
    <div style="position: absolute;">
        <asp:ImageButton ID="btnDefault" runat="server" Height="0px" ImageUrl="../Comm/Images/spacer.gif"
            Width="0px"></asp:ImageButton>
    </div>
    <table id="tbl" class="datatable">
        <tr>
            <td class="label labelPanel">
                Dal:
            </td>
            <td class="searchInput">
                <telerik:RadDatePicker ID="RadDatePicker1" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">
                Al:
            </td>
            <td class="searchInput">
                <telerik:RadDatePicker ID="RadDatePicker2" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel">
                Classificazione:
            </td>
            <td class="searchInput">
                <uc1:uscClassificatore ID="UscClassificatore1" EnableViewState="true" HeaderVisible="false" Required="true" runat="server"></uc1:uscClassificatore>
            </td>
        </tr>
        <tr id="trOrdinamento" runat="server">
            <td class="label labelPanel">
                Ordinamento:
            </td>
            <td class="searchInput">
                <asp:RadioButtonList ID="rblTipologia" runat="server" RepeatDirection="Horizontal" >
                    <asp:ListItem Value="P" Selected="True">N. Protocollo</asp:ListItem>
                    <asp:ListItem Value="A">Alfabetico</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnSearch" runat="server" Text="Ricerca" Visible="False"></asp:Button>
    <asp:Button ID="btnSearchGrid" runat="server" Text="Ricerca" PostBackUrl="ProtConcorsiViewGrid.aspx?Type=Prot" Visible="False"></asp:Button>
</asp:Content>
