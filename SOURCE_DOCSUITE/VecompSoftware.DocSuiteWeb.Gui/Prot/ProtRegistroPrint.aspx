<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ProtRegistroPrint.aspx.vb"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtRegistroPrint"
    Title="Protocollo - Registro Giornaliero" %>

<asp:Content runat="server"  ContentPlaceHolderID="cphHeader">
    <table class="datatable">
        <tr>
            <td class="label labelPanel" style="width: 20%">
                <asp:Label ID="lblDa" runat="server" Font-Bold="True" Width="99px">Dal giorno:</asp:Label>
            </td>
            <td>
                <telerik:RadDatePicker ID="RadDatePicker1" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="RadDatePicker1" ErrorMessage="Campo dal giorno obbligatorio" ID="RequiredFieldValidator2" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label labelPanel"style="width: 20%">
                <asp:Label ID="lblAl" runat="server" Font-Bold="True" Width="99px">Al giorno:</asp:Label>
            </td>
            <td style="padding-bottom: 5px;">
                <telerik:RadDatePicker ID="RadDatePicker2" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="RadDatePicker2" display="Dynamic" ErrorMessage="Campo al giorno obbligatorio" ID="RequiredFieldValidator1" runat="server" />
            </td>
            <asp:CompareValidator ID="dateCompareValidator" runat="server" ControlToValidate="Raddatepicker2"
                ControlToCompare="RadDatePicker1" Operator="GreaterThanEqual" Type="Date" ErrorMessage="La Seconda data deve essere Superiore alla prima" Display="Dynamic"></asp:CompareValidator>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table style="height: 100%; width: 100%;">
        <%-- Contenitore --%>
        <tr style="height: 13px">
            <td>
                <asp:Button ID="btnSelectAll" runat="server" Width="120px" CausesValidation="False" Text="Seleziona tutti"></asp:Button>
                <asp:Button ID="btnDeselectAll" runat="server" Width="120px" Text="Annulla selezione" CausesValidation="False"></asp:Button>
            </td>
            <td style="vertical-align: top; width: 50%; border-left: 1px solid black;">&nbsp;</td>
        </tr>
        <tr>
            <td style="vertical-align: top; width: 50%;">
                <telerik:RadTreeView ID="RadTreeView1" CheckBoxes="True" runat="server">
                    <CollapseAnimation Duration="100" Type="OutQuint" />
                    <ExpandAnimation Duration="100" Type="OutQuart" />
                </telerik:RadTreeView>
            </td>
            <td style="vertical-align: top; width: 50%; border-left: 1px solid black;">
                <asp:RadioButtonList ID="rblTipologia" runat="server" Width="100%" Font-Bold="True">
                    <asp:ListItem Value="Tutti" Selected="True">Tutti</asp:ListItem>
                    <asp:ListItem Value="Attivi">Attivi</asp:ListItem>
                    <asp:ListItem Value="Annullati">Annullati</asp:ListItem>
                    <asp:ListItem Value="Errati">Errati</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="cmdStampa" runat="server" Text="Stampa" />
</asp:Content>
