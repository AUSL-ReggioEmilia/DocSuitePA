<%@ Page AutoEventWireup="false" Codebehind="ReslDisposizioniPrint.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslDisposizioniPrint" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <table class="dataform">
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label ID="lblDa" runat="server" Font-Bold="True" Width="99px">Adozione dal giorno:</asp:Label>
            </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="RadDatePicker1" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="RadDatePicker1" ErrorMessage="Campo dal giorno obbligatorio" ID="RequiredFieldValidator2" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 20%">
                <asp:Label ID="lblAl" runat="server" Font-Bold="True" Width="99px">Al giorno:</asp:Label>
            </td>
            <td align="left" style="width: 80%">
                <telerik:RadDatePicker ID="RadDatePicker2" runat="server" />
                &nbsp;
                <asp:RequiredFieldValidator ControlToValidate="RadDatePicker2" Display="Dynamic" ErrorMessage="Campo al giorno obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                <asp:CompareValidator ID="dateCompareValidator" runat="server" ControlToValidate="Raddatepicker2"
                    ControlToCompare="RadDatePicker1" Operator="GreaterThanEqual" Type="Date" ErrorMessage="La Seconda data deve essere Superiore alla prima"
                    Display="Dynamic" />
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphContent">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="cmdStampa" runat="server" Text="Stampa"></asp:Button>
</asp:Content>
